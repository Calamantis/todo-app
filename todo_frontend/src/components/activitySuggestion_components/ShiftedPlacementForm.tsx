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

    {/* ACTIVITY */}
    <div>
      <label className="block text-sm font-medium mb-1">
        Activity
      </label>
      <select
        className="w-full p-2 rounded bg-surface-2 border border-white/10"
        value={activityId ?? ""}
        onChange={(e) => {
          const id = e.target.value ? Number(e.target.value) : null;
          setActivityId(id);

          const selected = activities.find(a => a.activityId === id);
          setIsRecurring(selected?.isRecurring ?? false);
        }}
      >
        <option value="">Select activity…</option>
        {activities.map(a => (
          <option key={a.activityId} value={a.activityId}>
            {a.title}
          </option>
        ))}
      </select>
    </div>

    {/* DURATION */}
    <div>
      <label className="block text-sm font-medium mb-1">
        Planned duration (minutes)
      </label>
      <input
        type="number"
        min={5}
        step={5}
        className="w-full p-2 rounded bg-surface-2 border border-white/10"
        value={plannedDuration ?? ""}
        onChange={e =>
          setPlannedDuration(e.target.value ? Number(e.target.value) : null)
        }
      />
    </div>

    {/* START TIME */}
    <div>
      <label className="block text-sm font-medium mb-1">
        Preferred start time
      </label>
      <input
        type="time"
        className="w-full p-2 rounded bg-surface-2 border border-white/10"
        value={preferredStart}
        onChange={e => setPreferredStart(e.target.value)}
      />
    </div>

    {/* END TIME */}
    <div>
      <label className="block text-sm font-medium mb-1">
        Preferred end time
      </label>
      <input
        type="time"
        className="w-full p-2 rounded bg-surface-2 border border-white/10"
        value={preferredEnd}
        onChange={e => setPreferredEnd(e.target.value)}
      />
    </div>

    {/* DAYS (opcjonalne, jeśli backend obsługuje) */}
    {preferredDays.length >= 0 && (
      <div>
        <label className="block text-sm font-medium mb-2">
          Preferred days
        </label>

        <div className="grid grid-cols-3 sm:grid-cols-4 gap-2">
          {[0, 1, 2, 3, 4, 5, 6].map(day => (
            <button
              key={day}
              type="button"
              onClick={() => toggleDay(day)}
              className={`px-3 py-1 rounded text-sm transition ${
                preferredDays.includes(day)
                  ? "bg-accent-0 text-text-0"
                  : "bg-surface-2 hover:bg-surface-3"
              }`}
            >
              {["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"][day]}
            </button>
          ))}
        </div>
      </div>
    )}

    {/* SUBMIT */}
    <button
      onClick={fetchShifted}
      disabled={loading}
      className="mt-2 px-4 py-2 rounded bg-surface-2 hover:bg-accent-0 transition font-medium"
    >
      {loading ? "Loading…" : "Find Shifted Placement"}
    </button>

  </div>
);
};

export default ShiftedPlacementForm;
