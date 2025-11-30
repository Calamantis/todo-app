import React, { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";

interface UserActivity {
  activityId: number;
  title: string;
  description: string;
  isRecurring: boolean;
  categoryId: number | null;
  categoryName: string | null;
  colorHex: string | null;
  joinCode: string | null;
  isFriendsOnly?: boolean;
}

interface ActivityPlacementFormProps {
  onResults: (results: any[], activityId: number, isRecurring: boolean) => void;
}

const ActivityPlacementForm: React.FC<ActivityPlacementFormProps> = ({
  onResults,
}) => {
  const { user } = useAuth();

  const [activities, setActivities] = useState<UserActivity[]>([]);
  const [activityId, setActivityId] = useState<number | null>(null);
  const [isRecurring, setIsRecurring] = useState<boolean>(false);

  const [plannedDuration, setPlannedDuration] = useState<number | null>(null);
  const [preferredStart, setPreferredStart] = useState<string>("");
  const [preferredEnd, setPreferredEnd] = useState<string>("");
  const [preferredDays, setPreferredDays] = useState<number[]>([]);

  const [loading, setLoading] = useState(false);

  // ===== GET USER ACTIVITIES =====
  const fetchActivities = async () => {
    if (!user) return;

    const res = await fetch("/api/Activity/user/get-activities", {
      method: "GET",
      headers: { Authorization: `Bearer ${user.token}` },
    });

    if (res.ok) {
      setActivities(await res.json());
    }
  };

  useEffect(() => {
    fetchActivities();
  }, []);

  const toggleDay = (day: number) => {
    setPreferredDays((prev) =>
      prev.includes(day) ? prev.filter((d) => d !== day) : [...prev, day]
    );
  };

  // ===== POST placement =====
  const fetchPlacement = async () => {
    if (!user) return;

    // required
    if (!activityId) {
      alert("Select an activity.");
      return;
    }
    if (!plannedDuration) {
      alert("Enter planned duration.");
      return;
    }

    // BUILD PAYLOAD
    const body: any = {
      activityId: activityId,
      plannedDuration: plannedDuration,
      isRecurring: isRecurring,
    };

    if (preferredStart) body.preferredStart = preferredStart;
    if (preferredEnd) body.preferredEnd = preferredEnd;
    if (preferredDays.length > 0) body.preferredDays = preferredDays;

    console.log("PLACEMENT PAYLOAD:", body);

    setLoading(true);

    const res = await fetch("/api/ActivitySuggestion/placement", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${user.token}`,
      },
      body: JSON.stringify(body),
    });

    setLoading(false);

    if (!res.ok) {
      console.error(await res.text());
      alert("Error fetching placement!");
      return;
    }

    const data = await res.json();

    const selected = activities.find(a => a.activityId === activityId);
    // przekazujemy results + activityId do modala
    onResults(data, activityId, selected?.isRecurring ?? false);
  };

  return (
    <div className="flex flex-col gap-4">

      {/* ACTIVITY SELECT */}
      <label className="text-sm font-medium">
        Activity (required):
        <select
          className="w-full p-2 mt-1 rounded bg-black/20 border border-white/10"
          value={activityId ?? ""}
         onChange={(e) => {
            const id = e.target.value ? Number(e.target.value) : null;
            setActivityId(id);

            if (id !== null) {
                const selected = activities.find(a => a.activityId === id);
                setIsRecurring(selected?.isRecurring ?? false);   // ← KLUCZ
            } else {
                setIsRecurring(false);
            }
            }}
        >
          <option value="">Select activity...</option>
          {activities.map((a) => (
            <option key={a.activityId} value={a.activityId}>
              {a.title}
            </option>
          ))}
        </select>
      </label>

      {/* DURATION */}
      <input
        type="number"
        placeholder="Planned Duration (minutes)"
        className="p-2 rounded bg-black/20 border border-white/10"
        onChange={(e) =>
          setPlannedDuration(e.target.value ? Number(e.target.value) : null)
        }
      />

      {/* START TIME */}
      <input
        type="time"
        className="p-2 rounded bg-black/20 border border-white/10"
        onChange={(e) => setPreferredStart(e.target.value)}
      />

      {/* END TIME */}
      <input
        type="time"
        className="p-2 rounded bg-black/20 border border-white/10"
        onChange={(e) => setPreferredEnd(e.target.value)}
      />

      {/* DAYS PICKER */}
      <div className="flex gap-2 flex-wrap">
        {[0, 1, 2, 3, 4, 5, 6].map((day) => (
          <button
            key={day}
            type="button"
            onClick={() => toggleDay(day)}
            className={`px-3 py-1 rounded ${
              preferredDays.includes(day) ? "bg-accent" : "bg-black/30"
            }`}
          >
            {["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"][day]}
          </button>
        ))}
      </div>

      {/* SUBMIT BUTTON */}
      <button
        onClick={fetchPlacement}
        disabled={loading}
        className="px-4 py-2 rounded bg-accent text-black font-semibold"
      >
        {loading ? "Loading…" : "Get Placement Suggestions"}
      </button>

    </div>
  );
};

export default ActivityPlacementForm;
