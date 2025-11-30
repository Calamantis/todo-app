import React, { useState } from "react";
import { X, Check, ChevronLeft, ChevronRight } from "lucide-react";
import { useAuth } from "../AuthContext";

interface OverlapActivity {
  activityId: number;
  title: string;
  startTime: string;
  endTime: string;
}

interface ShiftedResult {
  date: string;
  suggestedStart: string;
  suggestedEnd: string;
  activityTime: number;
  gapTime: number;
  overlappingActivities: OverlapActivity[];
}

interface ModalProps {
  activityId: number;
  results: ShiftedResult[];
  isRecurring: boolean;
  onClose: () => void;
}

const ShiftedPlacementModal: React.FC<ModalProps> = ({
  activityId,
  results,
  isRecurring,
  onClose
}) => {
  const { user } = useAuth();
  const [index, setIndex] = useState(0);

  const [shortenPrev, setShortenPrev] = useState(0);
  const [shortenCurrent, setShortenCurrent] = useState(0);
  const [shortenNext, setShortenNext] = useState(0);

  const current = results[index];
  if (!current) return null;

  const applyShift = async () => {
    if (!user) return;

    const payload = {
      activityId,
      date: current.date,
      suggestedStart: current.suggestedStart,
      suggestedEnd: current.suggestedEnd,
      shortenPrevious: shortenPrev,
      shortenCurrent: shortenCurrent,
      shortenNext: shortenNext
    };

    const res = await fetch("/api/ActivitySuggestion/apply-shifted", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${user.token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    });

    if (res.ok) {
      alert("Shift applied successfully!");
      onClose();
    } else {
      alert("Failed to apply shift.");
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-[var(--card-bg)] text-[var(--text-color)] p-6 rounded-xl max-w-lg w-full border border-white/10 shadow-xl">

        {/* HEADER */}
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">Shifted Placement</h2>
          <button onClick={onClose}>
            <X size={22} />
          </button>
        </div>

        <div className="flex flex-col gap-3">
          <div>
            <b>Date:</b> {current.date.split("T")[0]}
          </div>

          <div>
            <b>Suggested:</b> {current.suggestedStart} → {current.suggestedEnd}
          </div>

          <div>
            <b>Overlapping activities:</b>
          </div>

          <ul className="ml-4 list-disc">
            {current.overlappingActivities.map(a => (
              <li key={a.activityId}>
                {a.title} ({a.startTime} - {a.endTime})
              </li>
            ))}
          </ul>

          <div className="mt-3 flex flex-col gap-2">
            <label>
              Shorten previous:
              <input type="number" className="w-full p-1 bg-black/20 border border-white/10 rounded"
                value={shortenPrev}
                onChange={e => setShortenPrev(Number(e.target.value))}
              />
            </label>

            <label>
              Shorten current activity:
              <input type="number" className="w-full p-1 bg-black/20 border border-white/10 rounded"
                value={shortenCurrent}
                onChange={e => setShortenCurrent(Number(e.target.value))}
              />
            </label>

            <label>
              Shorten next:
              <input type="number" className="w-full p-1 bg-black/20 border border-white/10 rounded"
                value={shortenNext}
                onChange={e => setShortenNext(Number(e.target.value))}
              />
            </label>
          </div>
        </div>

        {/* NAVIGATION + APPLY */}
        <div className="flex items-center justify-between mt-6">

          <button
            onClick={() => setIndex(i => Math.max(0, i - 1))}
            disabled={index === 0}
          >
            <ChevronLeft size={26} />
          </button>

          <button
            onClick={applyShift}
            className="px-4 py-2 bg-accent text-black rounded font-semibold flex items-center gap-2"
          >
            <Check size={18} /> Apply
          </button>

          <button
            onClick={() => setIndex(i => Math.min(results.length - 1, i + 1))}
            disabled={index === results.length - 1}
          >
            <ChevronRight size={26} />
          </button>

        </div>
      </div>
    </div>
  );
};

export default ShiftedPlacementModal;
