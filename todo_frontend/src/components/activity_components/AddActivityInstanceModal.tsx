import React, { useState } from "react";
import { useAuth } from "../AuthContext";

interface AddActivityInstanceModalProps {
  activityId: number;
  recurrenceRuleId?: number;
  categoryName?: string;
  categoryColorHex?: string;
  onClose: () => void;
  onCreateInstance: (instance: any) => void;
}

const AddActivityInstanceModal: React.FC<AddActivityInstanceModalProps> = ({
  activityId,
  recurrenceRuleId,
  categoryName,
  categoryColorHex,
  onClose,
  onCreateInstance,
}) => {
  const { user } = useAuth();
  const [occurrenceDate, setOccurrenceDate] = useState(new Date().toISOString().split("T")[0]);
  const [startTime, setStartTime] = useState("08:00");
  const [endTime, setEndTime] = useState("09:00");
  const [durationMinutes, setDurationMinutes] = useState(60);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      if (!user) return;

      const body = {
        occurrenceDate: new Date(occurrenceDate).toISOString(),
        startTime,
        endTime,
        durationMinutes,
        isActive: true,
        didOccur: false,
        isException: false,
        categoryName: categoryName || null,
        categoryColorHex: categoryColorHex || null,
        activityId,
        recurrenceRuleId: recurrenceRuleId || null,
        userId: 0,
      };

      const res = await fetch("/api/ActivityInstance/create-instance", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(body),
      });

      if (!res.ok) throw new Error("Failed to create activity instance");

      const created = await res.json();
      onCreateInstance(created);
      onClose();
    } catch (err: unknown) {
      if (err instanceof Error) alert(err.message);
      else alert("Unknown error");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black/50 z-50">
      <div className= "bg-surface-1 p-6 rounded-lg w-full max-w-md max-h-[90vh] overflow-y-auto text-text-0">
        <h2 className="text-xl font-semibold mb-4">Add Activity Instance</h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Occurrence Date */}
          <div>
            <label className="block text-sm font-medium mb-1">Occurrence Date</label>
            <input
              type="date"
              required
              value={occurrenceDate}
              onChange={(e) => setOccurrenceDate(e.target.value)}
              className="w-full px-3 py-2 rounded-lg bg-surface-2"
            />
          </div>

          {/* Start Time */}
          <div>
            <label className="block text-sm font-medium mb-1">Start Time</label>
            <input
              type="time"
              required
              value={startTime}
              onChange={(e) => setStartTime(e.target.value)}
              className="w-full px-3 py-2 rounded-lg bg-surface-2"
            />
          </div>

          {/* End Time */}
          <div>
            <label className="block text-sm font-medium mb-1">End Time</label>
            <input
              type="time"
              required
              value={endTime}
              onChange={(e) => setEndTime(e.target.value)}
              className="w-full px-3 py-2 rounded-lg bg-surface-2"
            />
          </div>

          {/* Duration */}
          <div>
            <label className="block text-sm font-medium mb-1">Duration (minutes)</label>
            <input
              type="number"
              min="0"
              value={durationMinutes}
              onChange={(e) => setDurationMinutes(parseInt(e.target.value) || 0)}
              className="w-full px-3 py-2 rounded-lg bg-surface-2"
            />
          </div>

          {/* Buttons */}
          <div className="flex justify-end gap-4 pt-4">
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              className="px-4 py-2 bg-red-600 hover:bg-red-500 text-text-0 rounded"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 bg-surface-3 hover:bg-accent-0 text-text-0 rounded"
            >
              {loading ? "Creating..." : "Create"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddActivityInstanceModal;
