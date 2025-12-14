import React, { useEffect, useState } from "react";
import { Trash2, Shield } from "lucide-react";

interface Moderator {
  userId: number;
  email: string;
  fullName: string;
}

interface Props {
  token: string;
}

const ModeratorsListPanel: React.FC<Props> = ({ token }) => {
  const [moderators, setModerators] = useState<Moderator[]>([]);
  const [loading, setLoading] = useState(true);

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  const fetchModerators = async () => {
    try {
      const res = await fetch("/api/admin/moderators", { headers });
      const data = await res.json();
      setModerators(data);
    } catch (e) {
      console.error("Failed to load moderators", e);
    } finally {
      setLoading(false);
    }
  };

  const removeModerator = async (id: number) => {
    if (!window.confirm("Remove moderator role from this user?")) return;

    const res = await fetch(`/api/admin/moderators/${id}/remove`, {
      method: "PATCH",
      headers,
    });

    if (res.ok) {
      alert("Moderator removed.");
      fetchModerators();
    } else {
      alert("Failed to remove moderator.");
    }
  };

  useEffect(() => {
    fetchModerators();
  }, []);

  return (
    <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
      <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
        <Shield size={20} /> Moderators
      </h2>

      {loading ? (
        <div>Loading moderators…</div>
      ) : moderators.length === 0 ? (
        <div>No moderators found.</div>
      ) : (
        moderators.map((m) => (
          <div
            key={m.userId}
            className="p-4 mb-3 bg-surface-2 rounded-lg flex justify-between items-center"
          >
            <div>
              <div className="font-semibold">{m.fullName}</div>
              <div className="text-sm opacity-70">{m.email}</div>
            </div>

            <button
              onClick={() => removeModerator(m.userId)}
              className="px-2 py-1 rounded bg-red-600/30 hover:bg-red-600/60 flex items-center gap-1"
            >
              <Trash2 size={18} />
            </button>
          </div>
        ))
      )}
    </div>
  );
};

export default ModeratorsListPanel;
