import React, { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";
import Avatar from "../../components/social_components/Avatar";
import { X, Slash } from "lucide-react";

interface Participant {
  activityId: number;
  activityTitle: string;
  invitedUserId: number;
  fullName: string;
  email: string;
  profileImage: string;
  backgroundImage: string;
  status: string;     // accepted
  role: string;       // owner / participant
}

interface ParticipantsListProps {
  activityId: number | null;
}

const ParticipantsList: React.FC<ParticipantsListProps> = ({ activityId }) => {
  const { user } = useAuth();
  const [participants, setParticipants] = useState<Participant[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchParticipants = async () => {
    if (!user || !activityId) return;

    setLoading(true);

    const res = await fetch(
      `/api/ActivityMember/browse-participants?activityId=${activityId}`,
      {
        method: "GET",
        headers: { Authorization: `Bearer ${user.token}` },
      }
    );

    if (res.ok) {
      const data = await res.json();
      setParticipants(Array.isArray(data) ? data : []);
    }

    setLoading(false);
  };

  useEffect(() => {
    fetchParticipants();
  }, [activityId]);

  const removeParticipant = async (targetUserId: number) => {
    if (!user || !activityId) return;

    const res = await fetch(
      `/api/ActivityMember/remove-participant?activityId=${activityId}&targetUserId=${targetUserId}`,
      {
        method: "DELETE",
        headers: { Authorization: `Bearer ${user.token}` },
      }
    );

    if (res.ok) {
      setParticipants((prev) =>
        prev.filter((p) => p.invitedUserId !== targetUserId)
      );
    } else {
      alert("Failed to remove participant.");
    }
  };

  const removeAndBlock = async (targetUserId: number) => {
    if (!user || !activityId) return;

    const res = await fetch(
      `/api/ActivityMember/remove-and-block?activityId=${activityId}&targetUserId=${targetUserId}`,
      {
        method: "DELETE",
        headers: { Authorization: `Bearer ${user.token}` },
      }
    );

    if (res.ok) {
      setParticipants((prev) =>
        prev.filter((p) => p.invitedUserId !== targetUserId)
      );
    } else {
      alert("Failed to remove & block participant.");
    }
  };

  if (!activityId) return <div>Select an activity to view participants.</div>;
  if (loading) return <div>Loading…</div>;
  if (participants.length === 0) return <div>No participants yet.</div>;

  return (
    <div className="flex flex-col gap-2">
      {participants.map((p) => (
        <div
          key={p.invitedUserId}
          className="rounded-lg px-2 py-2 flex items-center justify-between hover:bg-white/10 transition"
        >
          <div className="flex items-center gap-3 min-w-0">
            <Avatar src={p.profileImage} size={36} />

            <div className="min-w-0">
              <div className="font-medium truncate">{p.fullName}</div>
              <div className="text-xs opacity-70 truncate">{p.email}</div>
              <div className="text-xs opacity-50">
                Role: {p.role} | Status: {p.status}
              </div>
            </div>
          </div>

          {/* ACTION BUTTONS */}
          <div className="flex items-center gap-3">

            {/* Remove */}
            <button
              onClick={() => removeParticipant(p.invitedUserId)}
              className="opacity-60 hover:opacity-100 text-yellow-400 hover:text-yellow-500 transition"
              title="Remove participant"
            >
              <X size={18} />
            </button>

            {/* Remove & Block */}
            <button
              onClick={() => removeAndBlock(p.invitedUserId)}
              className="opacity-60 hover:opacity-100 text-red-400 hover:text-red-600 transition"
              title="Remove & block participant"
            >
              <Slash size={18} />
            </button>

          </div>
        </div>
      ))}
    </div>
  );
};

export default ParticipantsList;
