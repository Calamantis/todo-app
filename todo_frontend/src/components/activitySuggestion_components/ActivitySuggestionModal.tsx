import React, { useState } from "react";
import { X, Check, ChevronLeft, ChevronRight } from "lucide-react";
import { useAuth } from "../AuthContext";

interface SuggestionResult {
  activityId: number;
  title: string;
  categoryName: string | null;
  suggestedDurationMinutes: number;
  score: number;
}

interface ActivitySuggestionModalProps {
  suggestions: SuggestionResult[];
  onClose: () => void;

  // z formularza:
  startTime: string;
  endTime: string;
}

const ActivitySuggestionModal: React.FC<ActivitySuggestionModalProps> = ({
  suggestions,
  onClose,
  startTime,
  endTime,
}) => {
  const { user } = useAuth();

  const [index, setIndex] = useState(0);
  const [date, setDate] = useState("");
  const [isException, setIsException] = useState(false);

  const current = suggestions[index];

  const createInstance = async () => {
    if (!user) return;

    if (!date) {
      alert("Please select a date.");
      return;
    }

    const payload = {
      occurrenceDate: date,
      startTime: startTime,
      endTime: endTime,
      durationMinutes: current.suggestedDurationMinutes,
      isActive: true,
      didOccur: true,
      isException: isException,
      activityId: current.activityId,
    };

    console.log("INSTANCE PAYLOAD:", payload);

    const res = await fetch(`/api/ActivityInstance/create-instance`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${user.token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    });

    if (res.ok) {
      alert("Activity instance created!");
      onClose();
    } else {
      alert("Failed to create instance.");
    }
  };

  if (!current) return null;

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-[var(--card-bg)] text-[var(--text-color)] p-6 rounded-xl max-w-md w-full border border-white/10 shadow-xl">

        {/* HEADER */}
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">Suggested Activity</h2>
          <button onClick={onClose} className="opacity-70 hover:opacity-100">
            <X size={22} />
          </button>
        </div>

        {/* INFO */}
        <div className="flex flex-col gap-3">
          <div className="font-bold text-lg">{current.title}</div>
          <div>Category: {current.categoryName ?? "—"}</div>
          <div>Suggested Duration: {current.suggestedDurationMinutes} minutes</div>
          <div>Score: {(current.score * 100).toFixed(1)}%</div>

          {/* DATE FIELD */}
          <label className="mt-3">
            Select date:
            <input
              type="date"
              value={date}
              onChange={(e) => setDate(e.target.value)}
              className="w-full p-2 mt-1 rounded bg-black/20 border border-white/10"
            />
          </label>

          {/* CHECKBOX */}
          <label className="flex items-center gap-2 mt-2">
            <input
              type="checkbox"
              checked={isException}
              onChange={() => setIsException(!isException)}
            />
            Mark as exception?
          </label>

          <div className="text-xs opacity-60 mt-2">
            Time: <b>{startTime}</b> → <b>{endTime}</b>
          </div>
        </div>

        {/* NAVIGATION */}
        <div className="flex items-center justify-between mt-6">

          <button
            onClick={() => setIndex((i) => Math.max(0, i - 1))}
            disabled={index === 0}
            className="opacity-70 hover:opacity-100"
          >
            <ChevronLeft size={26} />
          </button>

          <button
            onClick={createInstance}
            className="px-4 py-2 bg-accent text-black rounded font-semibold flex items-center gap-2"
          >
            <Check size={18} /> Choose
          </button>

          <button
            onClick={() => setIndex((i) => Math.min(suggestions.length - 1, i + 1))}
            disabled={index === suggestions.length - 1}
            className="opacity-70 hover:opacity-100"
          >
            <ChevronRight size={26} />
          </button>

        </div>
      </div>
    </div>
  );
};

export default ActivitySuggestionModal;
