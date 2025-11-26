import React from "react";
import { Pencil, Trash2 } from "lucide-react";

// Typ aktywności
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
}

const ActivityListItem: React.FC<ActivityListItemProps> = ({
  activity,
  onEdit,
  onDelete,
  onSelect,
  isSelected,
}) => {
  const accentBg = activity.colorHex
    ? `linear-gradient(to right, ${activity.colorHex} 75%, rgba(0,0,0,0) 75%)`
    : `linear-gradient(to right, #f3f4f6 75%, rgba(0,0,0,0) 75%)`;

    // derive privacy/status from joinCode and isFriendsOnly
    const status = activity.joinCode
      ? "Public"
      : activity.isFriendsOnly
      ? "Friends-only"
      : "Private";

  return (
    <div
      onClick={() => onSelect?.(activity)}
      className={`flex items-center justify-between p-3 rounded-lg cursor-pointer transition hover:opacity-75 border-2 w-[80%] ${
        isSelected ? "ring-2 ring-blue-500 dark:ring-blue-400" : ""
      }`}
      style={{ 
        background: accentBg,
        borderColor: activity.colorHex || "#f3f4f6",
      }}
    >
      {/* Informacje o aktywności */}
      <div className="flex-1 min-w-0">
        <div className="font-semibold text-gray-900">{activity.title}</div>
        <div className="text-sm text-gray-600">{activity.description}</div>
        {activity.categoryName && (
          <div className="text-xs text-gray-500 mt-1">Category: {activity.categoryName}</div>
        )}
        <div className="mt-1 text-xs flex items-center gap-2">
          {activity.isRecurring && (
            <span className="text-blue-600 font-semibold">Recurring</span>
          )}
          {activity.isRecurring && <span className="text-gray-300">|</span>}
          <span className="text-gray-500">{status}</span>
        </div>
      </div>

      {/* Ikony po prawej */}
      <div className="flex items-center gap-3 pr-2 flex-shrink-0 ml-3">
        <button
          onClick={(e) => {
            e.stopPropagation();
            onEdit?.(activity);
          }}
          className="text-gray-600 hover:text-yellow-600 transition"
        >
          <Pencil size={18} />
        </button>

        <button
          onClick={(e) => {
            e.stopPropagation();
            onDelete?.(activity.activityId);
          }}
          className="text-gray-600 hover:text-red-600 transition"
        >
          <Trash2 size={18} />
        </button>
      </div>
    </div>
  );
};

export default ActivityListItem;
