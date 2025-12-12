import { useEffect, useState } from "react";
import Avatar from "../social_components/Avatar";
import { useAuth } from "../AuthContext";
import type { ActivityDetails, ActivityInstanceData } from "./OfflineActivityModal";

interface Participant {
  invitedUserId: number;
  fullName: string;
  email: string;
  profileImage: string;
  status?: string;
  role?: string;
}

interface OnlineActivityModalProps {
  activity: ActivityDetails;
  instance: ActivityInstanceData;
}

const OnlineActivityModal: React.FC<OnlineActivityModalProps> = ({ activity, instance }) => {
  const { user } = useAuth();
  const [participants, setParticipants] = useState<Participant[]>([]);

  useEffect(() => {
    if (!user) return;

    fetch(`/api/ActivityMember/browse-participants?activityId=${activity.activityId}`, {
      headers: { Authorization: `Bearer ${user.token}` }
    })
      .then(r => r.json())
      .then(setParticipants);
      
  }, []);

  return (
    <div className="flex flex-col gap-4">

      <h2 className="text-xl font-bold">{activity.title}</h2>
      <p className="text-sm opacity-80">{activity.description}</p>

      <div className="mt-2">
        <b>Participants:</b>
        <div className="flex flex-col gap-2 mt-2">
          {participants.map(p => (
            <div key={p.invitedUserId} className="flex items-center gap-3 bg-surface-2 p-2 rounded">
              <Avatar src={p.profileImage} size={32} />
              <div>
                <div className="font-semibold">{p.fullName}</div>
                <div className="text-xs opacity-70">{p.email}</div>
              </div>
            </div>
          ))}
        </div>
      </div>

    </div>
  );
};

export default OnlineActivityModal;
