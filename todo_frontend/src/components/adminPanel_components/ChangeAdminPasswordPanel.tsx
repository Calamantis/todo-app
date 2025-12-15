import React, { useState } from "react";
import { KeyRound } from "lucide-react";

interface Props {
  token: string;
}

const ChangeAdminPasswordPanel: React.FC<Props> = ({ token }) => {
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  const submit = async () => {
    if (!currentPassword || !newPassword) {
      alert("Fill both fields.");
      return;
    }

    setLoading(true);

    const res = await fetch("/api/admin/password", {
      method: "PATCH",
      headers,
      body: JSON.stringify({
        currentPassword,
        newPassword,
      }),
    });

    setLoading(false);

    if (res.ok) {
      alert("Password changed successfully.");
      setCurrentPassword("");
      setNewPassword("");
    } else {
      alert("Failed to change password.");
    }
  };

  return (
    <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
      <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
        <KeyRound size={20} /> Change Admin Password
      </h2>

      <div className="flex flex-col gap-3">
        <input
          type="password"
          placeholder="Current password"
          className="p-2 rounded bg-surface-2"
          value={currentPassword}
          onChange={(e) => setCurrentPassword(e.target.value)}
        />

        <input
          type="password"
          placeholder="New password"
          className="p-2 rounded bg-surface-2"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
        />
      </div>

      <button
        onClick={submit}
        disabled={loading}
        className="mt-4 w-full px-4 py-2 rounded bg-accent-0 hover:bg-accent-1 disabled:opacity-50"
      >
        {loading ? "Saving..." : "Change password"}
      </button>
    </div>
  );
};

export default ChangeAdminPasswordPanel;
