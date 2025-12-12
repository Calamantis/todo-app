import React, { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";

interface Category {
  categoryId: number;
  name: string;
  colorHex: string;
}

interface ActivitySuggestionFormProps {
  onResults: (results: any[]) => void;
  // przekażemy do modala start i end
  onTimeChange?: (start: string, end: string) => void;
}

const ActivitySuggestionForm: React.FC<ActivitySuggestionFormProps> = ({
  onResults,
  onTimeChange,
}) => {
  const { user } = useAuth();

  const [categories, setCategories] = useState<Category[]>([]);

  const [categoryId, setCategoryId] = useState<number | null>(null);
  const [plannedDuration, setPlannedDuration] = useState<number | null>(null);
  const [preferredStart, setPreferredStart] = useState<string>("");
  const [preferredEnd, setPreferredEnd] = useState<string>("");
  const [preferredDays, setPreferredDays] = useState<number[]>([]);

  const [loading, setLoading] = useState(false);

  // ===== GET kategorii =====
  const fetchCategories = async () => {
    if (!user) return;

    const res = await fetch("/api/Category/browse-categories", {
      method: "GET",
      headers: { Authorization: `Bearer ${user.token}` },
    });

    if (res.ok) {
      const data = await res.json();
      setCategories(data);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const toggleDay = (day: number) => {
    setPreferredDays((prev) =>
      prev.includes(day) ? prev.filter((d) => d !== day) : [...prev, day]
    );
  };

  const fetchSuggestions = async () => {
    if (!user) return;

    // wymagane pola
    if (!plannedDuration) {
      alert("Enter planned duration (minutes).");
      return;
    }
    if (!preferredStart) {
      alert("Enter preferred start time.");
      return;
    }
    if (!preferredEnd) {
      alert("Enter preferred end time.");
      return;
    }

    setLoading(true);

    const body: any = {};

    if (categoryId !== null) body.categoryId = categoryId;
    if (plannedDuration) body.plannedDurationMinutes = plannedDuration;
    if (preferredStart) body.preferredStart = preferredStart;
    if (preferredEnd) body.preferredEnd = preferredEnd;
    if (preferredDays.length > 0) body.preferredDays = preferredDays;

    console.log("FINAL PAYLOAD:", body);

    const res = await fetch("/api/ActivitySuggestion/personal", {
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
      alert("Error fetching suggestions");
      return;
    }

    const data = await res.json();
    onResults(data);
  };


   const fetchSurpriseSuggestions = async () => {
    if (!user) return;

    // wymagane pola
    if (!plannedDuration) {
      alert("Enter planned duration (minutes).");
      return;
    }
    if (!preferredStart) {
      alert("Enter preferred start time.");
      return;
    }
    if (!preferredEnd) {
      alert("Enter preferred end time.");
      return;
    }

    setLoading(true);

    const body: any = {};

    if (categoryId !== null) body.categoryId = categoryId;
    if (plannedDuration) body.plannedDurationMinutes = plannedDuration;
    if (preferredStart) body.preferredStart = preferredStart;
    if (preferredEnd) body.preferredEnd = preferredEnd;
    if (preferredDays.length > 0) body.preferredDays = preferredDays;

    console.log("FINAL PAYLOAD:", body);

    const res = await fetch("/api/ActivitySuggestion/surprise-me", {
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
      alert("Error fetching suggestions");
      return;
    }

    const data = await res.json();
    onResults(data);
  };

  return (
    <div className="flex flex-col gap-4">

      {/* CATEGORY SELECT (GET z backendu) */}
      <label className="text-sm font-medium">
        Category (optional):
        <select
          className="w-full p-2 mt-1 rounded bg-surface-2"
          value={categoryId ?? ""}
          onChange={(e) => {
            const value = e.target.value === "" ? null : Number(e.target.value);
            setCategoryId(value);
          }}
        >
          <option value="">None (null)</option>
          {categories.map((cat) => (
            <option key={cat.categoryId} value={cat.categoryId}>
              {cat.name}
            </option>
          ))}
        </select>
      </label>

      {/* DURATION */}
      <input
        type="number"
        placeholder="Planned Duration (minutes)"
        className="p-2 rounded bg-surface-2"
        onChange={(e) =>
          setPlannedDuration(e.target.value ? Number(e.target.value) : null)
        }
      />

      {/* START TIME */}
      <input
        type="time"
        className="p-2 rounded bg-surface-2"
        onChange={(e) => {
          setPreferredStart(e.target.value);
          onTimeChange?.(e.target.value, preferredEnd);
        }}
      />

      {/* END TIME */}
      <input
        type="time"
        className="p-2 rounded bg-surface-2"
        onChange={(e) => {
          setPreferredEnd(e.target.value);
          onTimeChange?.(preferredStart, e.target.value);
        }}
      />

      {/* DAYS PICKER */}
      <div className="grid grid-cols-3 sm:grid-cols-4 gap-2 mb-2">
        {[0, 1, 2, 3, 4, 5, 6].map((day) => (
          <button
            key={day}
            type="button"
            onClick={() => toggleDay(day)}
            className={`px-3 py-1 rounded w-full ${
              preferredDays.includes(day) ? "bg-accent-0" : "bg-surface-2"
            }`}
          >
            {["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"][day]}
          </button>
        ))}
      </div>
<div className="flex gap-3">

  <button
    onClick={fetchSuggestions}
    disabled={loading}
    className="flex-1 px-4 py-2 rounded bg-surface-2 text-text-0 hover:bg-accent-0"
  >
    {loading ? "Loading…" : "Get Suggestions"}
  </button>

  <button
    onClick={fetchSurpriseSuggestions}
    disabled={loading}
    className="flex-1 px-4 py-2 rounded bg-surface-2 text-text-0 hover:bg-accent-0"
  >
    {loading ? "…" : "Surprise Me"}
  </button>

</div>


    </div>
  );
};

export default ActivitySuggestionForm;
