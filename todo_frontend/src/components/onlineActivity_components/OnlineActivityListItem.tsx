import React from "react";
import { UserPlus } from "lucide-react";

export interface OnlineActivity {
  activityId: number;
  title: string;
  description: string;
  isRecurring: boolean;
  categoryId: number | null;
  categoryName: string | null;
  colorHex: string | null;
  joinCode: string | null;
  isFriendsOnly?: boolean;
}

interface OnlineActivityListItemProps {
  activity: OnlineActivity;
  onSelect?: (activity: OnlineActivity) => void;
  onInvite?: (activity: OnlineActivity) => void;
}

const OnlineActivityListItem: React.FC<OnlineActivityListItemProps> = ({
  activity,
  onSelect,
  onInvite,
}) => {

  // status logic
  const status = activity.joinCode
    ? "Public"
    : activity.isFriendsOnly
    ? "Friends-only"
    : "Private";

  const accentBg = activity.colorHex
    ? `linear-gradient(to right, ${activity.colorHex} 75%, rgba(0,0,0,0) 75%)`
    : `linear-gradient(to right, #f3f4f6 75%, rgba(0,0,0,0) 75%)`;

  return (
    <div
      onClick={() => onSelect?.(activity)}
      className="flex items-center justify-between p-3 rounded-lg cursor-pointer transition hover:opacity-75 border-2"
      style={{
        background: accentBg,
        borderColor: activity.colorHex || "#f3f4f6",
      }}
    >
      {/* LEFT */}
      <div className="flex-1 min-w-0 pr-4">
        <div className="font-semibold text-gray-900 truncate">
          {activity.title}
        </div>

        <div className="text-sm text-gray-600 truncate">
          {activity.description}
        </div>

        <div className="mt-1 text-xs text-gray-500">
          {status}
        </div>
      </div>

      {/* RIGHT (icons) */}
      <div className="flex items-center gap-3 flex-shrink-0">

        {/* Invite */}
            <button
            onClick={(e) => {
                e.stopPropagation();
                onInvite?.(activity);
            }}
            className="text-gray-600 hover:text-blue-600 transition"
            >
            <UserPlus size={20} />
            </button>

      </div>
    </div>
  );
};

export default OnlineActivityListItem;
