import React, { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";
import Avatar from "../social_components/Avatar";
import { Check, X } from "lucide-react";

interface ReceivedInvite {
  activityId: number;
  activityTitle: string;
  invitedUserId: number;
  fullName: string;
  email: string;
  profileImage: string;
  backgroundImage: string;
  status: string;     // pending
  role: string;       // participant
}

const ReceivedInvitationsList: React.FC = () => {
  const { user } = useAuth();
  const [invites, setInvites] = useState<ReceivedInvite[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchInvites = async () => {
    if (!user) return;

    setLoading(true);

    const res = await fetch(`/api/ActivityMember/browse-recieved-invites`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${user.token}`,
      },
    });

    if (res.ok) {
      const data = await res.json();
      setInvites(Array.isArray(data) ? data : []);
    }

    setLoading(false);
  };

  useEffect(() => {
    fetchInvites();
  }, []);

  const updateStatus = async (activityId: number, status: "accepted" | "declined") => {
    if (!user) return;

    const body = {
      userId: user.userId,  // userId zawsze twój
      status: status,
    };

    const res = await fetch(
      `/api/ActivityMember/update-invite-status?activityId=${activityId}`,
      {
        method: "PATCH",
        headers: {
          Authorization: `Bearer ${user.token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(body),
      }
    );

    if (res.ok) {
      // usuń element lokalnie
      setInvites((prev) => prev.filter((i) => i.activityId !== activityId));
    } else {
      alert("Failed to update invitation status.");
    }
  };

  if (loading) return <div>Loading...</div>;
  if (invites.length === 0) return <div>No received invitations.</div>;

  return (
    <div className="flex flex-col gap-2">
      {invites.map((i) => (
        <div
          key={i.activityId}
          className="rounded-lg px-2 py-2 flex items-center justify-between hover:bg-white/10 transition"
        >
          <div className="flex items-center gap-3 min-w-0">
            <Avatar src={i.profileImage} size={36} />

            <div className="min-w-0">
              <div className="font-medium truncate">{i.fullName}</div>
              <div className="text-xs opacity-70 truncate">{i.email}</div>
              <div className="text-xs opacity-50">
                Activity: {i.activityTitle}
              </div>
            </div>
          </div>

          {/* ACTION BUTTONS */}
          <div className="flex items-center gap-3">

            {/* Accept */}
            <button
              onClick={() => updateStatus(i.activityId, "accepted")}
              className="opacity-70 hover:opacity-100 text-green-400 hover:text-green-500 transition"
              title="Accept invitation"
            >
              <Check size={20} />
            </button>

            {/* Decline */}
            <button
              onClick={() => updateStatus(i.activityId, "declined")}
              className="opacity-70 hover:opacity-100 text-red-400 hover:text-red-600 transition"
              title="Decline invitation"
            >
              <X size={20} />
            </button>

          </div>
        </div>
      ))}
    </div>
  );
};

export default ReceivedInvitationsList;
