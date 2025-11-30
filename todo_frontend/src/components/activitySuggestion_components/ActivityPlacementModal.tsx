import React, { useState } from "react";
import { X, Check, ChevronLeft, ChevronRight } from "lucide-react";
import { useAuth } from "../AuthContext";

interface PlacementResult {
  dateLocal: string;          // "2025-11-30T00:00:00Z"
  totalFreeMinutes: number;
  suggestedStart: string;     // "09:15:00"
  suggestedEnd: string;       // "10:15:00"
}

interface PlacementSuggestionModalProps {
  activityId: number;
  results: PlacementResult[];
  isRecurring: boolean;
  onClose: () => void;
}

const PlacementSuggestionModal: React.FC<PlacementSuggestionModalProps> = ({
  activityId,
  results,
  isRecurring,
  onClose,
}) => {
  const { user } = useAuth();

  const [index, setIndex] = useState(0);

  const current = results[index];

  if (!current) return null;

    const calculateDuration = (start: string, end: string): number => {
    const [sh, sm] = start.split(":").map(Number);
    const [eh, em] = end.split(":").map(Number);
    return (eh * 60 + em) - (sh * 60 + sm);
    };

const createInstance = async () => {
  if (!user) return;

  const chosen = results[index];

  if (!chosen) return alert("No result selected.");

  const payload = {
    activityId: activityId,
    occurrenceDate: chosen.dateLocal,  // dokładna data
    startTime: chosen.suggestedStart,
    endTime: chosen.suggestedEnd,
    durationMinutes: calculateDuration(chosen.suggestedStart, chosen.suggestedEnd),
    isActive: true,
    didOccur: true,
    isException: isRecurring ? true : false,
  };

  console.log("INSTANCE PAYLOAD:", payload);

  const res = await fetch("/api/ActivityInstance/create-instance", {
    method: "POST",
    headers: {
      Authorization: `Bearer ${user.token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (res.ok) {
    alert("Instance created!");
    onClose();
  } else {
    alert("Failed to create instance.");
  }
};


  // Format dateLocal → YYYY-MM-DD
  const displayDate = current.dateLocal.split("T")[0];

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-[var(--card-bg)] text-[var(--text-color)] p-6 rounded-xl max-w-md w-full border border-white/10 shadow-xl">

        {/* HEADER */}
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">Available Time Slot</h2>
          <button onClick={onClose} className="opacity-70 hover:opacity-100">
            <X size={22} />
          </button>
        </div>

        {/* INFO */}
        <div className="flex flex-col gap-2">
          <div><b>Date:</b> {displayDate}</div>
          <div>
            <b>Suggested time:</b> {current.suggestedStart} → {current.suggestedEnd}
          </div>
          <div>
            <b>Total free minutes that day:</b> {current.totalFreeMinutes}
          </div>
        </div>

        {/* NAV */}
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
            onClick={() => setIndex((i) => Math.min(results.length - 1, i + 1))}
            disabled={index === results.length - 1}
            className="opacity-70 hover:opacity-100"
          >
            <ChevronRight size={26} />
          </button>

        </div>
      </div>
    </div>
  );
};

export default PlacementSuggestionModal;
