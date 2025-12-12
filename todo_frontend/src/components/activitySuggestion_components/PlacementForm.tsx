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

          if (id !== null) {
            const selected = activities.find(a => a.activityId === id);
            setIsRecurring(selected?.isRecurring ?? false);
          } else {
            setIsRecurring(false);
          }
        }}
      >
        <option value="">Select activity…</option>
        {activities.map((a) => (
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
        onChange={(e) =>
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
        onChange={(e) => setPreferredStart(e.target.value)}
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
        onChange={(e) => setPreferredEnd(e.target.value)}
      />
    </div>

    {/* DAYS */}
    <div>
      <label className="block text-sm font-medium mb-2">
        Preferred days
      </label>

      <div className="grid grid-cols-3 sm:grid-cols-4 gap-2">
        {[0, 1, 2, 3, 4, 5, 6].map((day) => (
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

    {/* SUBMIT */}
    <button
      onClick={fetchPlacement}
      disabled={loading}
      className="mt-2 px-4 py-2 rounded bg-surface-2 hover:bg-accent-0 transition font-medium"
    >
      {loading ? "Loading…" : "Get Placement Suggestions"}
    </button>

  </div>
);

};

export default ActivityPlacementForm;
