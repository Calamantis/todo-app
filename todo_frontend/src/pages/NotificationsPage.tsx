import React, { useEffect, useState } from "react";
import { useAuth } from "../components/AuthContext";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";
import NotificationList from "../components/notification_components/NotificationList";

import { Bell, XCircle, Loader2 } from "lucide-react";

interface Notification {
  notificationId: number;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  visibleFrom?: string | null;
}

const NotificationsPage: React.FC = () => {
  const { user } = useAuth();
  const token = user?.token ?? "";
  const userId = user?.userId ?? 0;

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);

  // CREATOR
  const [title, setTitle] = useState("");
  const [message, setMessage] = useState("");
  const [visibleFrom, setVisibleFrom] = useState("");

  // GET notifications
  const fetchNotifications = async () => {
    setLoading(true);
    try {
      const res = await fetch(`/api/Notification/${userId}`, {
        method: "GET",
        headers,
      });

      const data = await res.json();
      setNotifications(data);
    } catch (err) {
      console.error(err);
    }
    setLoading(false);
  };

  // Mark as read
  const markAsRead = async (notificationId: number) => {
    try {
      await fetch(
        `/api/Notification/${notificationId}/mark-as-read?userId=${userId}`,
        {
          method: "POST",
          headers,
        }
      );

      setNotifications((prev) =>
        prev.map((n) =>
          n.notificationId === notificationId ? { ...n, isRead: true } : n
        )
      );
    } catch (err) {
      console.error(err);
    }
  };

  // Delete single
  const deleteNotification = async (notificationId: number) => {
    try {
      await fetch(`/api/Notification/${notificationId}?userId=${userId}`, {
        method: "DELETE",
        headers,
      });

      setNotifications((prev) =>
        prev.filter((n) => n.notificationId !== notificationId)
      );
    } catch (err) {
      console.error(err);
    }
  };

  // Delete all read
  const deleteAllRead = async () => {
    try {
      await fetch(`/api/Notification/delete-read?userId=${userId}`, {
        method: "DELETE",
        headers,
      });

      setNotifications((prev) => prev.filter((n) => !n.isRead));
    } catch (err) {
      console.error(err);
    }
  };

  // Create notification
  const createNotification = async () => {
    if (!title || !message) return alert("Fill all fields!");

    const body = {
      userId,
      title,
      message,
      visibleFrom: visibleFrom ? new Date(visibleFrom).toISOString() : null,
    };

    try {
      const res = await fetch("/api/Notification/create-notification", {
        method: "POST",
        headers,
        body: JSON.stringify(body),
      });

      if (res.ok) {
        setTitle("");
        setMessage("");
        setVisibleFrom("");
        fetchNotifications();
      }
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    fetchNotifications();
  }, []);

  if (loading) {
    return (
      <div className="h-[50vh] flex justify-center items-center bg-surface-0">
        <Loader2 size={40} className="animate-spin" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-surface-0 text-text-0 flex flex-col">
      <NavigationWrapper />

      <div className="flex-1 p-6 max-w-5xl mx-auto">
        <h1 className="text-3xl font-bold mb-6 flex items-center gap-2">
          <Bell size={28} /> Notifications
        </h1>

        {/* CREATE NOTIFICATION */}
        <div className="bg-surface-1 p-6 rounded-xl shadow-lg mb-8">
          <h2 className="text-xl mb-3 font-semibold flex items-center gap-2">
            Create Notification
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <input
              className="p-2 rounded bg-surface-2"
              placeholder="Title..."
              value={title}
              onChange={(e) => setTitle(e.target.value)}
            />
            <input
              className="p-2 rounded bg-surface-2"
              placeholder="Message..."
              value={message}
              onChange={(e) => setMessage(e.target.value)}
            />
            <input
              type="datetime-local"
              className="p-2 rounded bg-surface-2"
              value={visibleFrom}
              onChange={(e) => setVisibleFrom(e.target.value)}
            />
          </div>

          <button
            onClick={createNotification}
            className="mt-4 px-4 py-2 bg-accent-0 hover:bg-accent-1 rounded text-text-0"
          >
            Create Notification
          </button>
        </div>

        {/* DELETE ALL READ */}
        <div className="mb-6 flex justify-end">
          <button
            onClick={deleteAllRead}
            className="px-4 py-2 bg-red-600 hover:bg-red-700 rounded flex items-center gap-2 text-text-0"
          >
            <XCircle size={18} /> Delete all read
          </button>
        </div>

        {/* NOTIFICATION LIST */}
        <div className="bg-surface-1 p-6 rounded-xl shadow-lg">
          <NotificationList
            items={notifications}
            onMarkRead={markAsRead}
            onDelete={deleteNotification}
          />
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default NotificationsPage;
