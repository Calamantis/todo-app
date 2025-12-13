import React, { useState } from "react";
import { useAuth } from "../AuthContext";

interface AddActivityModalProps {
  onClose: () => void;
  onCreate?: () => void; // called after successful creation
  categories?: Array<{ categoryId: number; name: string }>;
}

const AddActivityModal: React.FC<AddActivityModalProps> = ({ onClose, onCreate, categories = [] }) => {
  const { user } = useAuth();
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [isRecurring, setIsRecurring] = useState(false);
  const [categoryId, setCategoryId] = useState<number | null>(categories.length > 0 ? categories[0].categoryId : null);
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
      };

      const res = await fetch("/api/Activity/create-activity", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(body),
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Failed to create activity");
      }

      // success
      onCreate?.();
      onClose();
    } catch (err: unknown) {
      if (err instanceof Error) alert(err.message);
      else alert("An unknown error occurred");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black/50 z-50 text-text-0">
      <div className="bg-surface-1 p-6 rounded-lg w-96">
        <h2 className="text-xl  font-semibold mb-4">Add Activity</h2>

        <div className="mb-3">
          <label className="block text-sm font-medium mb-1">Title</label>
          <input value={title} onChange={(e) => setTitle(e.target.value)} className="w-full p-2  rounded bg-secondary bg-surface-2" />
        </div>

        <div className="mb-3">
          <label className="block text-sm font-medium mb-1">Description</label>
          <textarea value={description} onChange={(e) => setDescription(e.target.value)} className="w-full p-2 bg-surface-2 rounded bg-secondary" />
        </div>

        <div className="mb-3 flex items-center gap-3">
          <label className="flex items-center gap-2">
            <input type="checkbox" checked={isRecurring} onChange={(e) => setIsRecurring(e.target.checked)}  className="accent-accent-0"/>
            <span className="text-sm">Recurring</span>
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-sm font-medium mb-1">Category</label>
          <select value={categoryId ?? ""} onChange={(e) => setCategoryId(e.target.value ? Number(e.target.value) : null)} className="w-full p-2 bg-surface-2 rounded bg-secondary">
            <option value="">No category</option>
            {categories.map((c) => (
              <option key={c.categoryId} value={c.categoryId}>{c.name}</option>
            ))}
          </select>
        </div>

        <div className="flex justify-end gap-3">
          <button onClick={onClose} className="px-4 py-2 bg-red-600 hover:bg-red-500 text-text-0 rounded">Cancel</button>
          <button onClick={handleSubmit} disabled={loading} className="px-4 py-2 bg-surface-3 hover:bg-accent-1 text-text-0 rounded">{loading ? 'Creating...' : 'Create'}</button>
        </div>
      </div>
    </div>
  );
};

export default AddActivityModal;
