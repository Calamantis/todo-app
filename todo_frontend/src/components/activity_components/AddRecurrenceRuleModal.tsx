import React, { useState } from "react";
import { useAuth } from "../AuthContext";

interface NewRecurrenceRule {
  activityId: number;
  type: string;
  daysOfWeek?: string;
  daysOfMonth?: string;
  dayOfYear?: string;
  interval?: number;
  startTime: string;
  endTime: string;
  dateRangeStart: string;
  dateRangeEnd: string;
  durationMinutes: number;
  isActive: boolean;
}

interface AddRecurrenceRuleModalProps {
  activityId: number;
  onClose: () => void;
  onCreateRule: (createdRule: any) => void;
}

const AddRecurrenceRuleModal: React.FC<AddRecurrenceRuleModalProps> = ({ activityId, onClose, onCreateRule }) => {
  const { user } = useAuth();
  const [type, setType] = useState("DAY");
  const [interval, setInterval] = useState(1);
  const [daysOfWeek, setDaysOfWeek] = useState("");
  const [daysOfMonth, setDaysOfMonth] = useState("");
  const [dayOfYear, setDayOfYear] = useState("");
  const [startTime, setStartTime] = useState("08:00");
  const [endTime, setEndTime] = useState("09:00");
  const [dateRangeStart, setDateRangeStart] = useState(new Date().toISOString().split("T")[0]);
  const [dateRangeEnd, setDateRangeEnd] = useState(new Date().toISOString().split("T")[0]);
  const [durationMinutes, setDurationMinutes] = useState(60);
  const [isActive, setIsActive] = useState(true);
  const [loading, setLoading] = useState(false);
  const [selectedDays, setSelectedDays] = useState<number[]>([]);


  const WEEK_DAYS = [
    { value: 1, label: "Monday" },
    { value: 2, label: "Tuesday" },
    { value: 3, label: "Wednesday" },
    { value: 4, label: "Thursday" },
    { value: 5, label: "Friday" },
    { value: 6, label: "Saturday" },
    { value: 0, label: "Sunday" }
  ];


  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      if (!user) return;
      const body: NewRecurrenceRule = {
        activityId,
        type,
        daysOfWeek:type === "WEEK" && selectedDays.length > 0 ? selectedDays.join(",") : undefined,
        daysOfMonth: daysOfMonth || undefined,
        dayOfYear: dayOfYear || undefined,
        interval,
        startTime,
        endTime,
        dateRangeStart: new Date(dateRangeStart).toISOString(),
        dateRangeEnd: new Date(dateRangeEnd).toISOString(),
        durationMinutes,
        isActive,
      };

      const res = await fetch("/api/ActivityRecurrenceRule", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(body),
      });

      if (!res.ok) throw new Error("Failed to create recurrence rule");

      const created = await res.json();
      // Pass created rule back to parent (assume API returns the created object)
      onCreateRule(created);
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
      <div className="bg-surface-1 text-text-0 p-6 rounded-lg w-full max-w-md max-h-[95vh] overflow-y-auto">
        <h2 className="text-xl font-semibold mb-4">Add Recurrence Rule</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Type</label>
            <select value={type} onChange={(e) => setType(e.target.value)} className="w-full px-3 py-2 rounded-lg bg-surface-2">
              <option value="DAY">Daily</option>
              <option value="WEEK">Weekly</option>
              <option value="MONTH">Monthly</option>
              <option value="YEAR">Yearly</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Interval</label>
            <input type="number" min={1} value={interval} onChange={(e) => setInterval(parseInt(e.target.value) || 1)} className="w-full px-3 py-2 bg-surface-2 rounded-lg " />
          </div>

          {type === "WEEK" && (
            <div>
              <label className="block text-sm font-medium mb-2">
                Days of Week
              </label>

              <div className="grid grid-cols-2 gap-2">
                {WEEK_DAYS.map((d) => (
                  <label
                    key={d.value}
                    className="flex items-center gap-2 text-sm"
                  >
                    <input
                      type="checkbox"
                      checked={selectedDays.includes(d.value)}
                      onChange={(e) => {
                        setSelectedDays((prev) =>
                          e.target.checked
                            ? [...prev, d.value]
                            : prev.filter((x) => x !== d.value)
                        );
                      }}
                      className="accent-accent-0"
                    />
                    {d.label}
                  </label>
                ))}
              </div>
            </div>
          )}

          {type === "MONTH" && (
            <div>
              <label className="block text-sm font-medium mb-1">Days of Month (comma-separated, LAST for last day)</label>
              <input type="text" value={daysOfMonth} onChange={(e) => setDaysOfMonth(e.target.value)} placeholder="e.g. 1,15,LAST" className="w-full px-3 py-2 bg-surface-2 rounded-lg" />
            </div>
          )}

          {type === "YEAR" && (
            <div>
              <label className="block text-sm font-medium mb-1">Day of Year</label>
              <input type="date" value={dayOfYear ? new Date(dayOfYear).toISOString().split("T")[0] : ""} onChange={(e) => setDayOfYear(e.target.value)} className="w-full px-3 py-2 bg-surface-2 rounded-lg " />
            </div>
          )}

          <div>
            <label className="block text-sm font-medium mb-1">Start Time</label>
            <input type="time" value={startTime} onChange={(e) => setStartTime(e.target.value)} className="w-full px-3 py-2 bg-surface-2 rounded-lg " />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">End Time</label>
            <input type="time" value={endTime} onChange={(e) => setEndTime(e.target.value)} className="w-full px-3 py-2 bg-surface-2 rounded-lg" />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Duration (minutes)</label>
            <input type="number" min={0} value={durationMinutes} onChange={(e) => setDurationMinutes(parseInt(e.target.value) || 0)} className="w-full px-3 py-2 bg-surface-2 rounded-lg " />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Date Range Start</label>
            <input type="date" value={dateRangeStart} onChange={(e) => setDateRangeStart(e.target.value)} className="w-full px-3 py-2 bg-surface-2 rounded-lg " />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1">Date Range End</label>
            <input type="date" value={dateRangeEnd} onChange={(e) => setDateRangeEnd(e.target.value)} className="w-full px-3 py-2 bg-surface-2 rounded-lg " />
          </div>

          <div className="flex items-center">
            <input type="checkbox" checked={isActive} onChange={(e) => setIsActive(e.target.checked)} className="rounded accent-accent-0" />
            <label className="ml-2 text-sm font-medium">Active</label>
          </div>

          <div className="flex justify-end gap-4 pt-4">
            <button type="button" onClick={onClose} disabled={loading} className="px-4 py-2 bg-red-600 hover:bg-red-500 text-text-0 rounded">Cancel</button>
            <button type="submit" disabled={loading} className="px-4 py-2 bg-surface-3 hover:bg-accent-0 text-text-0 rounded">{loading ? "Creating..." : "Create"}</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddRecurrenceRuleModal;
