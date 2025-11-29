import React, { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";
import Avatar from "../../components/social_components/Avatar";
import { X } from "lucide-react"

interface SentInvite {
  activityId: number;
  activityTitle: string;
  invitedUserId: number;
  fullName: string;
  email: string;
  profileImage: string;
  backgroundImage: string;
  status: string;     // pending / accepted / rejected
  role: string;
}

interface SentInvitationsListProps {
  activityId: number | null;
}

const SentInvitationsList: React.FC<SentInvitationsListProps> = ({ activityId }) => {
  const { user } = useAuth();
  const [invites, setInvites] = useState<SentInvite[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchSentInvites = async () => {
    if (!user || !activityId) return;

    setLoading(true);

    const res = await fetch(
      `/api/ActivityMember/browse-sent-invites?activityId=${activityId}`,
      {
        method: "GET",
        headers: { Authorization: `Bearer ${user.token}` },
      }
    );

    if (res.ok) {
      const data = await res.json();
      setInvites(Array.isArray(data) ? data : []);
    }

    setLoading(false);
  };

  const cancelInvite = async (targetUserId: number) => {
  if (!user || !activityId) return;

  const res = await fetch(
    `/api/ActivityMember/cancel-invite?activityId=${activityId}&targetUserId=${targetUserId}`,
    {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${user.token}`,
      },
    }
  );

  if (res.ok) {
    // Usuń z listy bez ponownego fetchowania
    setInvites((prev) => prev.filter((i) => i.invitedUserId !== targetUserId));
  } else {
    alert("Failed to cancel invitation.");
  }
 };



  useEffect(() => {
    fetchSentInvites();
  }, [activityId]);

  if (!activityId) {
    return <div>Select an activity to view sent invitations.</div>;
  }

  if (loading) {
    return <div>Loading…</div>;
  }

  if (invites.length === 0) {
    return <div>No sent invitations for this activity.</div>;
  }

  return (
    <div className="flex flex-col gap-2">
      {invites.map((i) => (
        <div
          key={i.invitedUserId}
          className="rounded-lg px-2 py-2 flex items-center justify-between hover:bg-white/10 transition"
        >
          <div className="flex items-center gap-3 min-w-0">
            <Avatar src={i.profileImage} size={36} />

            <div className="min-w-0">
              <div className="font-medium truncate">{i.fullName}</div>
              <div className="text-xs opacity-70 truncate">{i.email}</div>
              <div className="text-xs opacity-50">
                Status: {i.status}
              </div>
            </div>
          </div>

 {/* Cancel Invite */}
    <button
      onClick={() => cancelInvite(i.invitedUserId)}
      className="opacity-60 hover:opacity-100 text-white hover:text-red-600 transition"
      title="Cancel invitation"
    >
      <X size={18} />
    </button>

        </div>
      ))}
    </div>
  );
};

export default SentInvitationsList;
