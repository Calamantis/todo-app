import React from "react";
import { Pencil, Trash2, Globe, Users, Lock } from "lucide-react";
import { useAuth } from "../AuthContext";

interface Activity {
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

interface ActivityListItemProps {
  activity: Activity;
  onEdit?: (activity: Activity) => void;
  onDelete?: (activityId: number) => void;
  onSelect?: (activity: Activity) => void;
  isSelected?: boolean;
  onPrivacyChanged?: () => void; // callback po zmianie prywatności
}

const ActivityListItem: React.FC<ActivityListItemProps> = ({
  activity,
  onEdit,
  onDelete,
  onSelect,
  isSelected,
  onPrivacyChanged,
}) => {

  const { user } = useAuth();

  const accentBg = activity.colorHex
    ? `linear-gradient(to right, ${activity.colorHex} 75%, rgba(0,0,0,0) 75%)`
    : `linear-gradient(to right, #f3f4f6 75%, rgba(0,0,0,0) 75%)`;

  // derive privacy/status from joinCode and isFriendsOnly
  const status = activity.joinCode ? "Public" : activity.isFriendsOnly ? "Friends-only" : "Private";
  // ----- PRIVACY PATCH CALLS -----

  const patchPrivacy = async (path: string) => {
    if (!user) return;

    const res = await fetch(`/api/Activity/${path}?activityId=${activity.activityId}`, {
      method: "PATCH",
      headers: {
        Authorization: `Bearer ${user.token}`,
      },
    });

    if (!res.ok) {
      alert("Failed to change privacy");
      return;
    }

    onPrivacyChanged?.();
  };

  const setPublic = () => patchPrivacy("convert-activity-to-online");
  const setFriendsOnly = () => patchPrivacy("convert-activity-to-friendsonly");
  const setPrivate = () => patchPrivacy("convert-activity-to-offline");

  return (
    <div
      onClick={() => onSelect?.(activity)}
      className={`flex items-center justify-between p-3 rounded-lg cursor-pointer transition hover:opacity-75 border-2 w-[80%] ${
        isSelected ? "ring-2 ring-surface-2" : ""
      }`}
      style={{
        background: accentBg,
        borderColor: activity.colorHex || "#f3f4f6",
      }}
    >
      {/* LEFT SIDE */}
      <div className="flex-1 min-w-0 pr-4">
        <div className="font-semibold text-text-0 truncate">
          {activity.title}
        </div>

        <div className="text-sm text-text-0 opacity-80 truncate">
          {activity.description}
        </div>

        {activity.categoryName && (
          <div className="text-xs text-text-0 opacity-80 mt-1 truncate">
            Category: {activity.categoryName}
          </div>
        )}

        <div className="mt-1 text-xs flex items-center gap-2">
          {activity.isRecurring && (
            <span className="text-text-0 opacity-80 font-semibold">Recurring</span>
          )}
          {activity.isRecurring && <span className="text-gray-300">|</span>}
          <span className="text-text-0 opacity-80">{status}</span>
          {activity.joinCode && (
          <span className="text-text-0 opacity-80">| &nbsp; {activity.joinCode}</span>
          )}
        </div>
      </div>

      {/* RIGHT SIDE ACTIONS */}
      <div className="flex items-center gap-3 flex-shrink-0 w-fit">

        {/* PRIVACY ICONS */}
        <button
          onClick={(e) => {
            e.stopPropagation();
            setPublic();
          }}
          className="text-text-0 hover:text-green-600 transition"
          title="Make Public"
        >
          <Globe size={18} />
        </button>

        <button
          onClick={(e) => {
            e.stopPropagation();
            setFriendsOnly();
          }}
          className="text-text-0 hover:text-blue-600 transition"
          title="Friends Only"
        >
          <Users size={18} />
        </button>

        <button
          onClick={(e) => {
            e.stopPropagation();
            setPrivate();
          }}
          className="text-text-0 hover:text-purple-600 transition"
          title="Private"
        >
          <Lock size={18} />
        </button>

        {/* EDIT */}
        <button
          onClick={(e) => {
            e.stopPropagation();
            onEdit?.(activity);
          }}
          className="text-text-0 hover:text-yellow-600 transition"
        >
          <Pencil size={18} />
        </button>

        {/* DELETE */}
        <button
          onClick={(e) => {
            e.stopPropagation();
            onDelete?.(activity.activityId);
          }}
          className="text-text-0 hover:text-red-600 transition"
        >
          <Trash2 size={18} />
        </button>
      </div>
    </div>
  );
};

export default ActivityListItem;
