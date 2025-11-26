import React from "react";
import { useAuth } from "../AuthContext";

interface RecurrenceRule {
  recurrenceRuleId: number;
  activityId: number;
  type: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
}

interface DeleteRecurrenceRuleModalProps {
  rule: RecurrenceRule;
  onClose: () => void;
  onDelete: (recurrenceRuleId: number) => void;
}

const DeleteRecurrenceRuleModal: React.FC<DeleteRecurrenceRuleModalProps> = ({
  rule,
  onClose,
  onDelete,
}) => {
  const { user } = useAuth();

  const handleDelete = async () => {
    try {
      if (!user) return;
      const response = await fetch(
        `/api/ActivityRecurrenceRule/delete-recurrence-rule?recurrenceRuleId=${rule.recurrenceRuleId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to delete recurrence rule");
      }

      onDelete(rule.recurrenceRuleId);
      onClose();
    } catch (error: unknown) {
      if (error instanceof Error) {
        alert(error.message);
      } else {
        alert("An unknown error occurred");
      }
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black/50 z-50">
      <div className="bg-white dark:bg-slate-800 p-6 rounded-lg w-full max-w-sm">
        <h2 className="text-xl font-semibold mb-4">Delete Recurrence Rule</h2>
        <p className="mb-4">
          Are you sure you want to delete this recurrence rule? ({rule.startTime} - {rule.endTime})
        </p>

        <div className="flex justify-end gap-4">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-400 text-white rounded hover:bg-gray-500"
          >
            Cancel
          </button>
          <button
            onClick={handleDelete}
            className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteRecurrenceRuleModal;
