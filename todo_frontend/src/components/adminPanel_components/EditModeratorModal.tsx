import React, { useState } from "react";
import { X, Save } from "lucide-react";

interface Moderator {
  userId: number;
  email: string;
  fullName: string;
}

interface Props {
  moderator: Moderator;
  token: string;
  onClose: () => void;
  onSaved: () => void;
}

const EditModeratorModal: React.FC<Props> = ({
  moderator,
  token,
  onClose,
  onSaved,
}) => {
  const [email, setEmail] = useState(moderator.email);
  const [fullName, setFullName] = useState(moderator.fullName);
  const [newPassword, setNewPassword] = useState("");

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  const save = async () => {
    const body: any = {};

    if (email !== moderator.email) body.email = email;
    if (fullName !== moderator.fullName) body.fullName = fullName;
    if (newPassword.trim()) body.newPassword = newPassword;

    const res = await fetch(
      `/api/admin/moderators/${moderator.userId}`,
      {
        method: "PATCH",
        headers,
        body: JSON.stringify(body),
      }
    );

    if (res.ok) {
      onSaved();
      onClose();
    } else {
      alert("Failed to update moderator.");
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60">
      <div className="bg-surface-1 p-6 rounded-xl shadow-xl w-full max-w-md">
        {/* HEADER */}
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">Edit Moderator</h2>
          <button onClick={onClose}>
            <X />
          </button>
        </div>

        {/* FORM */}
        <div className="space-y-3">
          <input
            className="w-full p-2 rounded bg-surface-2"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email"
          />

          <input
            className="w-full p-2 rounded bg-surface-2"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            placeholder="Full name"
          />

          <input
            type="password"
            className="w-full p-2 rounded bg-surface-2"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            placeholder="New password (optional)"
          />
        </div>

        {/* ACTIONS */}
        <div className="flex justify-end gap-2 mt-5">
          <button
            onClick={onClose}
            className="px-4 py-2 rounded bg-surface-2"
          >
            Cancel
          </button>

          <button
            onClick={save}
            className="px-4 py-2 rounded bg-accent-0 hover:bg-accent-1 flex items-center gap-1"
          >
            <Save size={16} /> Save
          </button>
        </div>
      </div>
    </div>
  );
};

export default EditModeratorModal;
