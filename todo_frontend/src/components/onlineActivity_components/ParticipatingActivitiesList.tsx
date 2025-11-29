import React, { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";
import Avatar from "../../components/social_components/Avatar";
import { LogOut } from "lucide-react";

interface ParticipatingActivity {
  activityId: number;
  activityTitle: string;
  invitedUserId: number;
  fullName: string;
  email: string;
  profileImage: string;
  backgroundImage: string;
  status: string;     // accepted
  role: string;       // participant
}

const ParticipatingActivitiesList: React.FC = () => {
  const { user } = useAuth();
  const [activities, setActivities] = useState<ParticipatingActivity[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchActivities = async () => {
    if (!user) return;
    
    setLoading(true);

    const res = await fetch(`/api/ActivityMember/browse-online-activities`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${user.token}`
      }
    });

    if (res.ok) {
      const data = await res.json();
      setActivities(Array.isArray(data) ? data : []);
    }

    setLoading(false);
  };

  useEffect(() => {
    fetchActivities();
  }, []);

  // ===== LEAVE ACTIVITY =====
  const leaveActivity = async (activityId: number) => {
    if (!user) return;

    const res = await fetch(
      `/api/ActivityMember/leave-activity?activityId=${activityId}`,
      {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${user.token}`
        }
      }
    );

    if (res.ok) {
      // Usuń z listy lokalnie
      setActivities((prev) => prev.filter((a) => a.activityId !== activityId));
    } else {
      alert("Could not leave activity.");
    }
  };

  if (loading) return <div>Loading...</div>;
  if (activities.length === 0) return <div>You are not participating in any activities.</div>;

  return (
    <div className="flex flex-col gap-2">
      {activities.map((a) => (
        <div
          key={a.activityId}
          className="rounded-lg px-2 py-2 flex items-center justify-between hover:bg-white/10 transition"
        >
          <div className="flex items-center gap-3 min-w-0">
            <Avatar src={a.profileImage} size={36} />

            <div className="min-w-0">
              <div className="font-semibold truncate">{a.activityTitle}</div>

              <div className="text-xs opacity-70 truncate">
                Owner: {a.fullName}
              </div>

              <div className="text-xs opacity-50 truncate">{a.email}</div>
            </div>
          </div>

          {/* LEAVE BUTTON */}
          <button
            onClick={() => leaveActivity(a.activityId)}
            className="opacity-75 hover:opacity-100 text-red-400 hover:text-red-600 transition"
            title="Leave activity"
          >
            <LogOut size={20} />
          </button>
        </div>
      ))}
    </div>
  );
};

export default ParticipatingActivitiesList;
