import { Check, Trash2 } from "lucide-react";

interface NotificationItemProps {
  notificationId: number;
  title: string;
  message: string;
  createdAt: string;
  isRead: boolean;
  onMarkRead: (id: number) => void;
  onDelete: (id: number) => void;
}

const NotificationItem: React.FC<NotificationItemProps> = ({
  notificationId,
  title,
  message,
  createdAt,
  isRead,
  onMarkRead,
  onDelete,
}) => {
  return (
    <div
      className={`p-4 mb-3 rounded-lg border flex justify-between items-start ${
        isRead
          ? "bg-white/5 border-white/10"
          : "bg-blue-600/20 border-blue-400/40"
      }`}
    >
      <div>
        <div className="font-semibold text-lg">{title}</div>
        <div className="opacity-80">{message}</div>
        <div className="text-xs opacity-50 mt-1">
          {new Date(createdAt).toLocaleString()}
        </div>
      </div>

      <div className="flex flex-col gap-2">
        {!isRead && (
          <button
            onClick={() => onMarkRead(notificationId)}
            className="px-3 py-1 bg-green-600 hover:bg-green-700 text-white rounded flex items-center gap-1"
          >
            <Check size={16} /> Read
          </button>
        )}

        <button
          onClick={() => onDelete(notificationId)}
          className="px-3 py-1 bg-red-600 hover:bg-red-700 text-white rounded flex items-center gap-1"
        >
          <Trash2 size={16} /> Remove
        </button>
      </div>
    </div>
  );
};

export default NotificationItem;
