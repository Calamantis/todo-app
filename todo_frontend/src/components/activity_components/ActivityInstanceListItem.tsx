import React from "react";
import { Pencil, Trash2 } from "lucide-react";

interface ActivityInstance {
  instanceId: number;
  occurrenceDate: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
  isActive: boolean;
  didOccur: boolean;
  isException: boolean;
  categoryName?: string;
  categoryColorHex?: string;
  activityId: number;
  recurrenceRuleId?: number;
  userId: number;
}

interface ActivityInstanceListItemProps {
  instance: ActivityInstance;
  onEdit?: (instance: ActivityInstance) => void;
  onDelete?: (instanceId: number) => void;
}

const ActivityInstanceListItem: React.FC<ActivityInstanceListItemProps> = ({ 
  instance, 
  onEdit, 
  onDelete 
}) => {
  const date = new Date(instance.occurrenceDate).toLocaleDateString("pl-PL");
  const accentBg = instance.categoryColorHex
    ? `linear-gradient(to right, ${instance.categoryColorHex} 75%, rgba(0,0,0,0) 75%)`
    : `linear-gradient(to right, #636060 75%, rgba(0,0,0,0) 75%)`;

  return (
    <div
      className="flex items-center justify-between p-3 mb-2 rounded-lg text-sm dark:text-gray-100"
      style={{ background: accentBg }}
    >
      <div className="flex-1">
        <div className="font-semibold text-gray-900 dark:text-gray-100">{date}</div>
        <div className="text-xs text-gray-600 dark:text-gray-400">
          {instance.startTime} - {instance.endTime} ({instance.durationMinutes}m)
        </div>
        <div className="mt-1 text-xs flex items-center gap-2">
          {instance.isException && (
            <span className="text-orange-600 dark:text-orange-400 font-semibold">⚠ Exception</span>
          )}
          {!instance.isActive && (
            <span className="text-gray-500 dark:text-gray-400">Inactive</span>
          )}
        </div>
      </div>

      {/* Action Buttons */}
      <div className="flex items-center gap-2 flex-shrink-0 ml-2">
        <button
          onClick={(e) => {
            e.stopPropagation();
            onEdit?.(instance);
          }}
          className="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300"
          title="Edit instance"
        >
          <Pencil size={16} />
        </button>
        <button
          onClick={(e) => {
            e.stopPropagation();
            onDelete?.(instance.instanceId);
          }}
          className="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300"
          title="Delete instance"
        >
          <Trash2 size={16} />
        </button>
      </div>
    </div>
  );
};

export default ActivityInstanceListItem;
