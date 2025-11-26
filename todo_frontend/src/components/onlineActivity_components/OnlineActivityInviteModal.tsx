import React, { useEffect, useState } from "react";
import { X, UserPlus } from "lucide-react";
import { useAuth } from "../AuthContext";
import Avatar from "../../components/social_components/Avatar";

interface UserPreview {
  userId: number;
  fullName: string;
  email?: string;
  profileImageUrl?: string;
  backgroundImageUrl?: string;
  synopsis?: string;
}

interface OnlineActivityInviteModalProps {
  activityId: number;
  onClose: () => void;
}

const OnlineActivityInviteModal: React.FC<OnlineActivityInviteModalProps> = ({
  activityId,
  onClose,
}) => {
  const { user } = useAuth();

  const [friends, setFriends] = useState<UserPreview[]>([]);
  const [filtered, setFiltered] = useState<UserPreview[]>([]);
  const [search, setSearch] = useState("");

  const [loading, setLoading] = useState(false);

  // LOAD FRIENDS FROM API
  const fetchFriends = async () => {
    if (!user) return;

    setLoading(true);

    const res = await fetch("/api/UserFriendActions/me", {
      method: "GET",
      headers: {
        Authorization: `Bearer ${user.token}`,
      },
    });

    const data = await res.json();
    setFriends(data);
    setFiltered(data);
    setLoading(false);
  };

  useEffect(() => {
    fetchFriends();
  }, []);

  // SEARCH FILTER
//   useEffect(() => {
//     const s = search.toLowerCase();
//     setFiltered(
//       friends.filter(
//         (f) =>
//           f.fullName.toLowerCase().includes(s) ||
//           f.email?.toLowerCase().includes(s)
//       )
//     );
//   }, [search, friends]);

useEffect(() => {
  const s = search.toLowerCase();

  setFiltered(
    friends.filter((f) => {
      const name = f.fullName?.toLowerCase() ?? "";
      const email = f.email?.toLowerCase() ?? "";
      return name.includes(s) || email.includes(s);
    })
  );
}, [search, friends]);


  // SEND INVITE
  const sendInvite = async (friendId: number) => {
    if (!user) return;

    const res = await fetch(
      `/api/ActivityMember/send-invite?activityId=${activityId}&invitedUserId=${friendId}`,
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      }
    );

    if (res.ok) {
      alert("Invitation sent!");
      onClose();
    } else {
      alert("Could not send invite.");
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-30">
      <div className="bg-[var(--card-bg)] text-[var(--text-color)] p-6 rounded-xl w-full max-w-md max-h-[80vh] overflow-y-auto shadow-xl border border-white/10">
        
        {/* HEADER */}
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">Invite Friend</h2>
          <button onClick={onClose} className="opacity-70 hover:opacity-100">
            <X size={22} />
          </button>
        </div>

        {/* SEARCH INPUT */}
        <input
          type="text"
          placeholder="Search friend..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="w-full mb-4 p-2 rounded-lg bg-black/20 border border-white/10 focus:outline-none focus:ring-2 focus:ring-accent"
        />

        {/* FRIEND LIST */}
        {loading ? (
          <div>Loading...</div>
        ) : filtered.length === 0 ? (
          <div>No friends found.</div>
        ) : (
          <div className="flex flex-col gap-2">
            {filtered.map((f) => (
              <div
                key={f.userId}
                className="relative group rounded-lg px-2 py-2 flex items-center justify-between hover:bg-white/10 transition"
              >
                <div className="flex items-center gap-3 min-w-0">
                  <Avatar
                    src={f.profileImageUrl}
                    size={36}
                  />
                  <div className="min-w-0">
                    <div className="font-medium truncate">
                      {f.fullName}
                    </div>
                    {f.email && (
                      <div className="text-xs opacity-70 truncate">
                        {f.email}
                      </div>
                    )}
                  </div>
                </div>

                {/* Invite Button */}
                <button
                  className="opacity-80 hover:opacity-100"
                  title="Invite to activity"
                  onClick={() => sendInvite(f.userId)}
                >
                  <UserPlus size={18} />
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default OnlineActivityInviteModal;
