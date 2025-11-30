import React, { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";

interface UserActivity {
  activityId: number;
  title: string;
  isRecurring: boolean;
}

interface ShiftedPlacementFormProps {
  onResults: (results: any[], activityId: number, isRecurring: boolean) => void;
}

const ShiftedPlacementForm: React.FC<ShiftedPlacementFormProps> = ({ onResults }) => {
  const { user } = useAuth();

  const [activities, setActivities] = useState<UserActivity[]>([]);
  const [activityId, setActivityId] = useState<number | null>(null);
  const [isRecurring, setIsRecurring] = useState<boolean>(false);

  const [plannedDuration, setPlannedDuration] = useState<number | null>(null);
  const [preferredStart, setPreferredStart] = useState("");
  const [preferredEnd, setPreferredEnd] = useState("");
  const [preferredDays, setPreferredDays] = useState<number[]>([]);

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!user) return;

    fetch("/api/Activity/user/get-activities", {
      headers: { Authorization: `Bearer ${user.token}` }
    })
      .then(r => r.json())
      .then(setActivities);
  }, []);

  const toggleDay = (day: number) => {
    setPreferredDays(prev =>
      prev.includes(day) ? prev.filter(d => d !== day) : [...prev, day]
    );
  };

  const fetchShifted = async () => {
    if (!user) return;
    if (!activityId || !plannedDuration) {
      alert("Select activity and duration.");
      return;
    }

    const selected = activities.find(a => a.activityId === activityId);

    const body: any = {
      activityId,
      plannedDuration
    };

    if (preferredStart) body.preferredStart = preferredStart;
    if (preferredEnd) body.preferredEnd = preferredEnd;
    if (preferredDays.length) body.preferredDays = preferredDays;

    setLoading(true);

    const res = await fetch("/api/ActivitySuggestion/shifted", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${user.token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify(body)
    });

    setLoading(false);

    if (!res.ok) {
      console.error(await res.text());
      alert("Error fetching shifted placement");
      return;
    }

    const data = await res.json();
    onResults(data, activityId, selected?.isRecurring ?? false);
  };

  return (
    <div className="flex flex-col gap-4">

      {/* ACTIVITY SELECT */}
      <label className="text-sm font-medium">
        Activity:
        <select
          className="w-full p-2 mt-1 rounded bg-black/20 border border-white/10"
          value={activityId ?? ""}
          onChange={(e) => {
            const id = e.target.value ? Number(e.target.value) : null;
            setActivityId(id);

            const selected = activities.find(a => a.activityId === id);
            setIsRecurring(selected?.isRecurring ?? false);
          }}
        >
          <option value="">Select...</option>
          {activities.map(a => (
            <option key={a.activityId} value={a.activityId}>
              {a.title}
            </option>
          ))}
        </select>
      </label>

      {/* Duration */}
      <input
        type="number"
        placeholder="Planned Duration (minutes)"
        className="p-2 rounded bg-black/20 border border-white/10"
        onChange={e => setPlannedDuration(Number(e.target.value))}
      />

      {/* Start time */}
      <input
        type="time"
        className="p-2 rounded bg-black/20 border border-white/10"
        onChange={e => setPreferredStart(e.target.value)}
      />

      {/* End time */}
      <input
        type="time"
        className="p-2 rounded bg-black/20 border border-white/10"
        onChange={e => setPreferredEnd(e.target.value)}
      />

      <button
        onClick={fetchShifted}
        className="px-4 py-2 rounded bg-accent text-black font-semibold"
        disabled={loading}
      >
        {loading ? "Loading…" : "Find Shifted Placement"}
      </button>
    </div>
  );
};

export default ShiftedPlacementForm;
