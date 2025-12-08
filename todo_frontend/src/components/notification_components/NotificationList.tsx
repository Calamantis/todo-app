import NotificationItem from "./NotificationItem";

interface Notification {
  notificationId: number;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

interface NotificationListProps {
  items: Notification[];
  onMarkRead: (id: number) => void;
  onDelete: (id: number) => void;
}

const NotificationList: React.FC<NotificationListProps> = ({
  items,
  onMarkRead,
  onDelete,
}) => {
  if (items.length === 0) return <div>No notifications yet.</div>;

  return (
    <>
      {items.map((n) => (
        <NotificationItem
          key={n.notificationId}
          {...n}
          onMarkRead={onMarkRead}
          onDelete={onDelete}
        />
      ))}
    </>
  );
};

export default NotificationList;
