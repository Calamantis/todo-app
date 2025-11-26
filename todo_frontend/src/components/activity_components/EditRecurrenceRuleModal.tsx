import React, { useState } from "react";
import { useAuth } from "../AuthContext";

interface RecurrenceRule {
  recurrenceRuleId: number;
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

interface EditRecurrenceRuleModalProps {
  rule: RecurrenceRule;
  onClose: () => void;
  onEditRule: (updatedRule: RecurrenceRule) => void;
}

const EditRecurrenceRuleModal: React.FC<EditRecurrenceRuleModalProps> = ({
  rule,
  onClose,
  onEditRule,
}) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [type, setType] = useState(rule.type);
  const [interval, setInterval] = useState(rule.interval || 1);
  const [daysOfWeek, setDaysOfWeek] = useState(rule.daysOfWeek || "");
  const [daysOfMonth, setDaysOfMonth] = useState(rule.daysOfMonth || "");
  const [dayOfYear, setDayOfYear] = useState(rule.dayOfYear || "");
  const [startTime, setStartTime] = useState(rule.startTime);
  const [endTime, setEndTime] = useState(rule.endTime);
  const [dateRangeStart, setDateRangeStart] = useState(
    new Date(rule.dateRangeStart).toISOString().split("T")[0]
  );
  const [dateRangeEnd, setDateRangeEnd] = useState(
    new Date(rule.dateRangeEnd).toISOString().split("T")[0]
  );
  const [durationMinutes, setDurationMinutes] = useState(rule.durationMinutes);
  const [isActive, setIsActive] = useState(rule.isActive);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      if (!user) return;

      const response = await fetch(
        `/api/ActivityRecurrenceRule/edit-recurrence-rule?recurrenceRuleId=${rule.recurrenceRuleId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify({
            activityId: rule.activityId,
            type,
            daysOfWeek: daysOfWeek || undefined,
            daysOfMonth: daysOfMonth || undefined,
            dayOfYear: dayOfYear || undefined,
            interval,
            startTime,
            endTime,
            dateRangeStart: new Date(dateRangeStart).toISOString(),
            dateRangeEnd: new Date(dateRangeEnd).toISOString(),
            durationMinutes,
            isActive,
          }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to update recurrence rule");
      }

      const updatedRule = {
        ...rule,
        type,
        interval,
        daysOfWeek,
        daysOfMonth,
        dayOfYear,
        startTime,
        endTime,
        dateRangeStart: new Date(dateRangeStart).toISOString(),
        dateRangeEnd: new Date(dateRangeEnd).toISOString(),
        durationMinutes,
        isActive,
      };

      onEditRule(updatedRule);
      onClose();
    } catch (error: unknown) {
      if (error instanceof Error) {
        alert(error.message);
      } else {
        alert("An unknown error occurred");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black/50 z-50">
      <div className="bg-white dark:bg-slate-800 p-6 rounded-lg w-full max-w-md max-h-[90vh] overflow-y-auto">
        <h2 className="text-xl font-semibold mb-4">Edit Recurrence Rule</h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Type */}
          <div>
            <label className="block text-sm font-medium mb-1">Type</label>
            <select
              value={type}
              onChange={(e) => setType(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
            >
              <option value="DAY">Daily</option>
              <option value="WEEK">Weekly</option>
              <option value="MONTH">Monthly</option>
              <option value="YEAR">Yearly</option>
            </select>
          </div>

          {/* Interval */}
          <div>
            <label className="block text-sm font-medium mb-1">Interval</label>
            <input
              type="number"
              min="1"
              value={interval}
              onChange={(e) => setInterval(parseInt(e.target.value) || 1)}
              className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
            />
          </div>

          {/* Days of Week (for WEEK type) */}
          {type === "WEEK" && (
            <div>
              <label className="block text-sm font-medium mb-1">Days of Week (comma-separated, 0-6)</label>
              <input
                type="text"
                placeholder="e.g., 1,3,5"
                value={daysOfWeek}
                onChange={(e) => setDaysOfWeek(e.target.value)}
                className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
              />
            </div>
          )}

          {/* Days of Month (for MONTH type) */}
          {type === "MONTH" && (
            <div>
              <label className="block text-sm font-medium mb-1">Days of Month (comma-separated)</label>
              <input
                type="text"
                placeholder="e.g., 1,15,LAST"
                value={daysOfMonth}
                onChange={(e) => setDaysOfMonth(e.target.value)}
                className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
              />
            </div>
          )}

          {/* Day of Year (for YEAR type) */}
          {type === "YEAR" && (
            <div>
              <label className="block text-sm font-medium mb-1">Day of Year</label>
              <input
                type="date"
                value={dayOfYear ? new Date(dayOfYear).toISOString().split("T")[0] : ""}
                onChange={(e) => setDayOfYear(e.target.value)}
                className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
              />
            </div>
          )}

          {/* Start Time */}
          <div>
            <label className="block text-sm font-medium mb-1">Start Time</label>
            <input
              type="time"
              value={startTime}
              onChange={(e) => setStartTime(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
            />
          </div>

          {/* End Time */}
          <div>
            <label className="block text-sm font-medium mb-1">End Time</label>
            <input
              type="time"
              value={endTime}
              onChange={(e) => setEndTime(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
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
              className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
            />
          </div>

          {/* Date Range Start */}
          <div>
            <label className="block text-sm font-medium mb-1">Date Range Start</label>
            <input
              type="date"
              value={dateRangeStart}
              onChange={(e) => setDateRangeStart(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
            />
          </div>

          {/* Date Range End */}
          <div>
            <label className="block text-sm font-medium mb-1">Date Range End</label>
            <input
              type="date"
              value={dateRangeEnd}
              onChange={(e) => setDateRangeEnd(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600"
            />
          </div>

          {/* Active Status */}
          <div className="flex items-center">
            <input
              type="checkbox"
              checked={isActive}
              onChange={(e) => setIsActive(e.target.checked)}
              className="rounded"
            />
            <label className="ml-2 text-sm font-medium">Active</label>
          </div>

          {/* Buttons */}
          <div className="flex justify-end gap-4 pt-4">
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              className="px-4 py-2 bg-gray-400 text-white rounded hover:bg-gray-500 disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
            >
              {loading ? "Saving..." : "Save"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditRecurrenceRuleModal;
