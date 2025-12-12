import React, { useEffect, useState } from "react";
import { X, Send } from "lucide-react";
import { useAuth } from "../AuthContext";

interface OnlineActivity {
  activityId: number;
  title: string;
  description: string;
  joinCode: string | null;
  isFriendsOnly?: boolean;
  colorHex: string | null;
}

interface Props {
  friendId: number;
  onClose: () => void;
}

const OnlineActivityInviteSingleModal: React.FC<Props> = ({ friendId, onClose }) => {
  const { user } = useAuth();
  const [activities, setActivities] = useState<OnlineActivity[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user) return;

    fetch("/api/Activity/user/get-activities", {
      headers: { Authorization: `Bearer ${user.token}` }
    })
      .then(r => r.json())
      .then((data) => {
        const onlyOnline = data.filter(
          (a: OnlineActivity) => a.joinCode !== null || a.isFriendsOnly
        );
        setActivities(onlyOnline);
      })
      .finally(() => setLoading(false));
  }, []);

  const sendInvite = async (activityId: number) => {
    if (!user) return;

    const res = await fetch(
      `/api/ActivityMember/send-invite?activityId=${activityId}&invitedUserId=${friendId}`,
      { method: "POST", headers: { Authorization: `Bearer ${user.token}` } }
    );

    if (res.ok) {
      alert("Invite sent!");
      onClose();
    } else alert("Error sending invite.");
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[9999]">
      <div className="bg-surface-1 text-text-0 rounded-xl w-full max-w-md p-6 relative">

        <button onClick={onClose} className="absolute top-3 right-3">
          <X size={22} />
        </button>

        <h2 className="text-xl font-semibold mb-4">Invite to Activity</h2>

        {loading ? (
          <div>Loading…</div>
        ) : activities.length === 0 ? (
          <div>No online activities found.</div>
        ) : (
          <div className="space-y-2 max-h-[60vh] overflow-y-auto">
            {activities.map(a => (
              <div
                key={a.activityId}
                className="p-3 rounded bg-surface-2 flex justify-between items-center"
              >
                <div>
                  <div className="font-semibold">{a.title}</div>
                  <div className="text-xs opacity-70">{a.description}</div>
                </div>

                <button
                  onClick={() => sendInvite(a.activityId)}
                  className="hover:text-accent-0"
                >
                  <Send size={20} />
                </button>
              </div>
            ))}
          </div>
        )}

      </div>
    </div>
  );
};

export default OnlineActivityInviteSingleModal;
