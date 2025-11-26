import React from "react";
import { useAuth } from "../AuthContext";

interface DeleteActivityModalProps {
  activity: { activityId: number; title: string };
  onDelete: (id: number) => void;
  onClose: () => void;
}

const DeleteActivityModal: React.FC<DeleteActivityModalProps> = ({
  activity,
  onDelete,
  onClose,
}) => {
  if (!activity) return null;
  const { user } = useAuth();

  const handleDelete = async () => {
    try {
      if (!user) return;
      const response = await fetch(`/api/Activity/delete-activity?activityId=${activity.activityId}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Failed to delete activity");
      }

      onDelete(activity.activityId);
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
    <div className="fixed inset-0 flex items-center justify-center bg-black/50">
      <div className="bg-white dark:bg-slate-800 p-6 rounded-lg w-1/3">
        <h2 className="text-xl font-semibold mb-4">Delete Activity</h2>
        <p className="mb-4">Are you sure you want to delete the activity "{activity.title}"?</p>

        <div className="flex justify-end gap-4">
          <button onClick={onClose} className="px-4 py-2 bg-gray-400 text-white rounded">
            Cancel
          </button>
          <button onClick={handleDelete} className="px-4 py-2 bg-red-600 text-white rounded">
            Delete
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteActivityModal;
