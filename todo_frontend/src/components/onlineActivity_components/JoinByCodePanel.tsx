import React, { useState } from "react";
import { useAuth } from "../AuthContext";

interface JoinByCodePanelProps {
  onJoined?: () => void; // callback do odświeżenia paneli
}

const JoinByCodePanel: React.FC<JoinByCodePanelProps> = ({ onJoined }) => {
  const { user } = useAuth();
  const [joinCode, setJoinCode] = useState("");
  const [loading, setLoading] = useState(false);

  const joinByCode = async () => {
    if (!user || !joinCode) return;

    setLoading(true);

const res = await fetch(
  `/api/ActivityMember/join-by-code?joinCode=${encodeURIComponent(joinCode)}`,
  {
    method: "POST",
    headers: {
      Authorization: `Bearer ${user.token}`
    }
  }
);

setLoading(false);

const resultText = await res.text(); // ⬅️ czytamy body RAZ

if (res.ok && resultText === "true") {
  alert("Joined activity successfully!");
  setJoinCode("");
  onJoined?.();
} else {
  alert("Failed to join activity");
}

  };

  return (
    <div className="flex flex-col gap-3">
      <div className="text-sm opacity-80">Join an activity using a join code:</div>

      <input
        type="text"
        value={joinCode}
        onChange={(e) => setJoinCode(e.target.value)}
        placeholder="Enter join code..."
        className="p-2 rounded-lg bg-surface-2 border border-white/10 focus:outline-none focus:ring-2 focus:ring-accent"
      />

      <button
        onClick={joinByCode}
        disabled={loading || joinCode.length === 0}
        className="px-4 py-2 rounded-lg bg-accent-0 text-text-0 font-semibold hover:opacity-90 transition disabled:opacity-40"
      >
        {loading ? "Joining..." : "Join Activity"}
      </button>
    </div>
  );
};

export default JoinByCodePanel;
