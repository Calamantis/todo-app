import React, { useState } from "react";
import { X } from "lucide-react";
import { useAuth } from "../AuthContext";

interface ActivityInstance {
  instanceId: number;
  occurrenceDate: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
  isActive: boolean;
  didOccur: boolean;
  isException: boolean;
  categoryName?: string;
  categoryColorHex?: string;
  activityId: number;
  recurrenceRuleId?: number;
  userId: number;
}

interface EditActivityInstanceModalProps {
  instance: ActivityInstance;
  onClose: () => void;
  onEditInstance: () => void;
}

const EditActivityInstanceModal: React.FC<EditActivityInstanceModalProps> = ({
  instance,
  onClose,
  onEditInstance,
}) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [formData, setFormData] = useState({
    occurrenceDate: instance.occurrenceDate.split("T")[0],
    startTime: instance.startTime,
    endTime: instance.endTime,
    isActive: instance.isActive,
    didOccur: instance.didOccur,
    isException: instance.isException,
  });

  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) return;

    setLoading(true);
    setError("");

    try {
      // Calculate duration in minutes from start and end times
      const [startHours, startMinutes] = formData.startTime.split(":").map(Number);
      const [endHours, endMinutes] = formData.endTime.split(":").map(Number);
      const startTotalMinutes = startHours * 60 + startMinutes;
      const endTotalMinutes = endHours * 60 + endMinutes;
      const calculatedDuration = Math.max(0, endTotalMinutes - startTotalMinutes);

      const payload = {
        instanceId: instance.instanceId,
        occurrenceDate: formData.occurrenceDate,
        startTime: formData.startTime,
        endTime: formData.endTime,
        durationMinutes: calculatedDuration,
        isActive: formData.isActive,
        didOccur: formData.didOccur,
        isException: formData.isException,
        categoryName: instance.categoryName,
        categoryColorHex: instance.categoryColorHex,
        activityId: instance.activityId,
        recurrenceRuleId: instance.recurrenceRuleId,
        userId: instance.userId,
      };

      const response = await fetch(
        `/api/ActivityInstance/edit-instance?instanceId=${instance.instanceId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify(payload),
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Failed to edit instance");
      }

      onEditInstance();
      onClose();
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-6 w-96 max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-gray-900 dark:text-white">
            Edit Activity Instance
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
          >
            <X size={24} />
          </button>
        </div>

        {error && <div className="text-red-500 mb-4">{error}</div>}

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Occurrence Date */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Occurrence Date
            </label>
            <input
              type="date"
              name="occurrenceDate"
              value={formData.occurrenceDate}
              onChange={handleInputChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md dark:bg-gray-700 dark:text-white dark:border-gray-600"
            />
          </div>

          {/* Start Time */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Start Time
            </label>
            <input
              type="time"
              name="startTime"
              value={formData.startTime}
              onChange={handleInputChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md dark:bg-gray-700 dark:text-white dark:border-gray-600"
            />
          </div>

          {/* End Time */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              End Time
            </label>
            <input
              type="time"
              name="endTime"
              value={formData.endTime}
              onChange={handleInputChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md dark:bg-gray-700 dark:text-white dark:border-gray-600"
            />
          </div>

          {/* Checkboxes */}
          <div className="space-y-2">
            <label className="flex items-center text-sm">
              <input
                type="checkbox"
                name="isException"
                checked={formData.isException}
                onChange={handleInputChange}
                className="w-4 h-4 mr-2"
              />
              <span className="text-gray-700 dark:text-gray-300">Is Exception</span>
            </label>
          </div>

          {/* Buttons */}
          <div className="flex gap-2 pt-4">
            <button
              type="submit"
              disabled={loading}
              className="flex-1 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white font-medium py-2 rounded-md transition"
            >
              {loading ? "Saving..." : "Save"}
            </button>
            <button
              type="button"
              onClick={onClose}
              className="flex-1 bg-gray-300 hover:bg-gray-400 dark:bg-gray-600 dark:hover:bg-gray-700 text-gray-900 dark:text-white font-medium py-2 rounded-md transition"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditActivityInstanceModal;
