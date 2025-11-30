import React, { useEffect, useState } from "react";
import { X } from "lucide-react";
import { useAuth } from "../AuthContext";

interface TimelineItem {
  instanceId: number;
  activityId: number;
  title: string;
  categoryName: string | null;
  colorHex: string | null;
  occurrenceDate: string;
  startTime: string;
  endTime: string;
}

interface FriendTimelineModalProps {
  userId: number;
  onClose: () => void;
}

const FriendTimelineModal: React.FC<FriendTimelineModalProps> = ({ userId, onClose }) => {
  const { user } = useAuth();
  const [items, setItems] = useState<TimelineItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user) return;

    const now = new Date();
    const from = new Date(now.getFullYear(), now.getMonth(), now.getDate()).toISOString();
    const to = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 7).toISOString();

    fetch(`/api/Timeline/user/get-timeline?userId=${userId}&from=${from}&to=${to}`, {
      headers: { Authorization: `Bearer ${user.token}` }
    })
      .then(r => r.json())
      .then(setItems)
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="fixed inset-0 bg-black/70 flex items-center justify-center z-[9999]">
      <div className="bg-[var(--card-bg)] text-[var(--text-color)] rounded-xl w-full max-w-lg p-6 border border-white/10 relative">

        <button onClick={onClose} className="absolute top-3 right-3">
          <X size={22} />
        </button>

        <h2 className="text-xl font-semibold mb-4">Friend's Timeline</h2>

        {loading ? (
          <div>Loading…</div>
        ) : items.length === 0 ? (
          <div>No upcoming activities.</div>
        ) : (
          <div className="space-y-3 max-h-[60vh] overflow-y-auto">
            {items.map(i => (
              <div
                key={i.instanceId}
                className="p-3 rounded border border-white/10 bg-white/5"
              >
                <div className="font-semibold">{i.title}</div>
                <div className="text-xs opacity-70">{i.categoryName}</div>
                <div className="text-sm mt-1">
                  {i.occurrenceDate.split("T")[0]} — {i.startTime} → {i.endTime}
                </div>
              </div>
            ))}
          </div>
        )}

      </div>
    </div>
  );
};

export default FriendTimelineModal;
