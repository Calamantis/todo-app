import React, { useEffect, useState } from "react";
import { X } from "lucide-react";
import { useAuth } from "../AuthContext";
import OfflineActivityModal from "./OfflineActivityModal";
import OnlineActivityModal from "./OnlineActivityModal";

interface ActivityDetails {
  activityId: number;
  title: string;
  description: string;
  isRecurring: boolean;
  categoryId: number | null;
  categoryName: string | null;
  colorHex: string | null;
  joinCode: string | null;
  isFriendsOnly: boolean;
}

interface ActivityInstanceData {
  instanceId: number;
  activityId: number;
  occurrenceDate: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
}

interface ActivityDetailsModalProps {
  instance: ActivityInstanceData;
  onClose: () => void;
}

const ActivityDetailsModal: React.FC<ActivityDetailsModalProps> = ({ instance, onClose }) => {
  const { user } = useAuth();
  const [activity, setActivity] = useState<ActivityDetails | null>(null);

  useEffect(() => {
    if (!user) return;

    fetch(`/api/Activity/get-activity-by-id?activityId=${instance.activityId}`, {
      headers: { Authorization: `Bearer ${user.token}` },
    })
      .then(r => r.json())
      .then(setActivity);
  }, []);

  if (!activity) return null;

  const isOnline = activity.isFriendsOnly || activity.joinCode !== null;

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-[9999]">
      <div className="bg-surface-1 text-text-0 rounded-xl w-full max-w-lg p-6 shadow-2xl relative">
        
        {/* CLOSE BUTTON */}
        <button
          onClick={onClose}
          className="absolute top-3 right-3 p-1 rounded hover:bg-surface-2"
        >
          <X size={22} />
        </button>

        {/* CONTENT */}
        {isOnline ? (
          <OnlineActivityModal activity={activity} instance={instance} />
        ) : (
          <OfflineActivityModal activity={activity} instance={instance} onClose={onClose} />
        )}

      </div>
    </div>
  );
};

export default ActivityDetailsModal;
