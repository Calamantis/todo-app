import React, { useState } from "react";
import { UserPlus } from "lucide-react";

interface Props {
  token: string;
}

const CreateModeratorPanel: React.FC<Props> = ({ token }) => {
  const [email, setEmail] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  const createModerator = async () => {
    if (!email || !fullName || !password) {
      alert("Fill all fields");
      return;
    }

    const res = await fetch("/api/admin/moderators", {
      method: "POST",
      headers,
      body: JSON.stringify({ email, fullName, password }),
    });

    if (res.ok) {
      alert("Moderator created");
      setEmail("");
      setFullName("");
      setPassword("");
    } else {
      alert("Failed to create moderator");
    }
  };

  return (
    <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
      <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
        <UserPlus size={20} /> Create Moderator
      </h2>

      <div className="grid gap-3">
        <input className="p-2 rounded bg-surface-2" placeholder="Email"
          value={email} onChange={(e) => setEmail(e.target.value)} />
        <input className="p-2 rounded bg-surface-2" placeholder="Full name"
          value={fullName} onChange={(e) => setFullName(e.target.value)} />
        <input type="password" className="p-2 rounded bg-surface-2"
          placeholder="Password" value={password}
          onChange={(e) => setPassword(e.target.value)} />
      </div>

      <button
        onClick={createModerator}
        className="mt-4 w-full px-4 py-2 rounded bg-accent-0 hover:bg-accent-1"
      >
        Create
      </button>
    </div>
  );
};

export default CreateModeratorPanel;
