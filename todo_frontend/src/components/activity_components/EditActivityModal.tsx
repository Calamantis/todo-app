import React, { useState } from "react";
import { useAuth } from "../AuthContext";

interface EditActivityModalProps {
  activity: {
    activityId: number;
    title: string;
    description: string;
    isRecurring: boolean;
    categoryId: number | null;
  };
  categories?: Array<{ categoryId: number; name: string }>;
  onClose: () => void;
  onEditActivity: () => void;
}

const EditActivityModal: React.FC<EditActivityModalProps> = ({
  activity,
  categories = [],
  onClose,
  onEditActivity,
}) => {
  const { user } = useAuth();
  const [title, setTitle] = useState(activity.title);
  const [description, setDescription] = useState(activity.description);
  const [isRecurring, setIsRecurring] = useState(activity.isRecurring);
  const [categoryId, setCategoryId] = useState<number | null>(activity.categoryId);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async () => {
    if (!title) {
      alert("Title is required");
      return;
    }

    try {
      if (!user) return;
      setLoading(true);

      const body = {
        title,
        description,
        isRecurring,
        categoryId: categoryId ?? null,
        recurrenceRule: "", // optional, can be extended later
      };

      const response = await fetch(`/api/Activity/edit-activity?activityId=${activity.activityId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        const text = await response.text();
        throw new Error(text || "Failed to update activity");
      }

      // Success - call callback and close
      onEditActivity();
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
      <div className="bg-surface-1 text-text-0 p-6 rounded-lg w-96">
        <h2 className="text-xl font-semibold mb-4">Edit Activity</h2>

        <div className="mb-3">
          <label className="block text-sm font-medium mb-1">Title</label>
          <input
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="w-full p-2 bg-surface-2 rounded"
          />
        </div>

        <div className="mb-3">
          <label className="block text-sm font-medium mb-1">Description</label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="w-full p-2 bg-surface-2 rounded"
          />
        </div>

        <div className="mb-3 flex items-center gap-2">
          <input
            type="checkbox"
            checked={isRecurring}
            onChange={(e) => setIsRecurring(e.target.checked)}
            id="recurring"
            className="accent-accent-0"
          />
          <label htmlFor="recurring" className="text-sm">Recurring</label>
        </div>

        <div className="mb-4">
          <label className="block text-sm font-medium mb-1">Category</label>
          <select
            value={categoryId ?? ""}
            onChange={(e) => setCategoryId(e.target.value ? Number(e.target.value) : null)}
            className="w-full p-2 bg-surface-2 rounded"
          >
            <option value="">No category</option>
            {categories.map((c) => (
              <option key={c.categoryId} value={c.categoryId}>{c.name}</option>
            ))}
          </select>
        </div>

        <div className="flex justify-end gap-3">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-red-600 hover:bg-red-500 text-text-0 rounded"
            disabled={loading}
          >
            Cancel
          </button>
          <button
            onClick={handleSubmit}
            className="px-4 py-2 bg-surface-3 hover:bg-accent-0 text-text-0 rounded"
            disabled={loading}
          >
            {loading ? "Saving..." : "Save Changes"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default EditActivityModal;
