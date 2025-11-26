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

interface DeleteActivityInstanceModalProps {
  instance: ActivityInstance;
  onClose: () => void;
  onDelete: (instanceId: number) => void;
}

const DeleteActivityInstanceModal: React.FC<DeleteActivityInstanceModalProps> = ({
  instance,
  onClose,
  onDelete,
}) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleConfirmDelete = async () => {
    if (!user) return;

    setLoading(true);
    setError("");

    try {
      const response = await fetch(
        `/api/ActivityInstance/delete-instance?instanceId=${instance.instanceId}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Failed to delete instance");
      }

      onDelete(instance.instanceId);
      onClose();
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const date = new Date(instance.occurrenceDate).toLocaleDateString("pl-PL");

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-6 w-96">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-gray-900 dark:text-white">
            Delete Activity Instance
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
          >
            <X size={24} />
          </button>
        </div>

        {error && <div className="text-red-500 mb-4">{error}</div>}

        <div className="mb-6">
          <p className="text-gray-700 dark:text-gray-300 mb-2">
            Are you sure you want to delete this instance?
          </p>
          <div className="bg-gray-100 dark:bg-gray-700 p-3 rounded-md text-sm">
            <p>
              <strong>Date:</strong> {date}
            </p>
            <p>
              <strong>Time:</strong> {instance.startTime} - {instance.endTime}
            </p>
            <p>
              <strong>Duration:</strong> {instance.durationMinutes} minutes
            </p>
          </div>
        </div>

        <div className="flex gap-2">
          <button
            onClick={handleConfirmDelete}
            disabled={loading}
            className="flex-1 bg-red-600 hover:bg-red-700 disabled:bg-gray-400 text-white font-medium py-2 rounded-md transition"
          >
            {loading ? "Deleting..." : "Delete"}
          </button>
          <button
            onClick={onClose}
            className="flex-1 bg-gray-300 hover:bg-gray-400 dark:bg-gray-600 dark:hover:bg-gray-700 text-gray-900 dark:text-white font-medium py-2 rounded-md transition"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteActivityInstanceModal;
