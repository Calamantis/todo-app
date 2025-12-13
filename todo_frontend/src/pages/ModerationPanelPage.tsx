import ModerationUsersSection from "../components/moderationPanel_components/ModerationUsersSection";
import ModerationActivitiesSection from "../components/moderationPanel_components/ModerationActivitiesSection";


import React, { useEffect, useState } from "react";
import { Loader2 } from "lucide-react";

import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";
import { useAuth } from "../components/AuthContext";

const ModerationPanelPage: React.FC = () => {
  const { user } = useAuth();
  const token = user?.token ?? "";

  const [users, setUsers] = useState([]);
  const [activities, setActivities] = useState([]);

  const [loading, setLoading] = useState(true);

  // SEARCH STATES
  const [userSearch, setUserSearch] = useState("");
  const [activitySearch, setActivitySearch] = useState("");

  // EDIT MODAL STATE
  const [editModal, setEditModal] = useState<{
    id: number;
    type: "user" | "activity";
    field: string;
  } | null>(null);

  const [newValue, setNewValue] = useState("");

  // REQUEST HEADERS
  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  // FETCH ALL DATA
  const fetchData = async () => {
    setLoading(true);

    try {
      const usersRes = await fetch("/api/moderation/users", { headers });
      const activitiesRes = await fetch("/api/moderation/activities", { headers });

      setUsers(await usersRes.json());
      setActivities(await activitiesRes.json());
    } catch (e) {
      console.error("Moderation load error", e);
    }

    setLoading(false);
  };

  useEffect(() => {
    fetchData();
  }, []);

  // UPDATE FIELD
  const doUpdate = async () => {
    if (!editModal) return;

    const { id, type, field } = editModal;

    const endpoint =
      type === "user"
        ? `/api/moderation/users/${id}/${field}?newValue=${encodeURIComponent(
            newValue
          )}`
        : `/api/moderation/activities/${id}/${field}?newValue=${encodeURIComponent(
            newValue
          )}`;

    await fetch(endpoint, {
      method: "PUT",
      headers,
      body: JSON.stringify({ newValue }),
    });

    setEditModal(null);
    setNewValue("");
    fetchData();
  };

  // DELETE FIELD
  const doDelete = async (type: "user" | "activity", id: number, field: string) => {
    const endpoint =
      type === "user"
        ? `/api/moderation/users/${id}/${field}`
        : `/api/moderation/activities/${id}/${field}`;

    await fetch(endpoint, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });

    fetchData();
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-surface-1">
        <Loader2 className="animate-spin" size={40} />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-surface-0 flex flex-col justify-between">
      <NavigationWrapper />

      <div className="p-6 text-text-0 max-w-8xl mx-auto w-full">
        <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">

          {/* USERS SECTION */}
          <ModerationUsersSection
            users={users}
            token={token}
            search={userSearch}
            setSearch={setUserSearch}
            setEditModal={setEditModal}
            doDelete={doDelete}
          />

          {/* ACTIVITIES SECTION */}
          <ModerationActivitiesSection
            activities={activities}
            token={token}
            search={activitySearch}
            // setSearch={setActivitySearch}
            setEditModal={setEditModal}
            doDelete={doDelete}
          />

        </div>
      </div>

      {/* EDIT MODAL */}
      {editModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-surface-1 p-6 rounded-xl shadow-2xl w-full max-w-md text-text-0">
            <h3 className="text-lg font-semibold mb-4">
              Edit {editModal.field.replace("-", " ")}
            </h3>

            <input
              className="w-full p-2 rounded bg-surface-2"
              value={newValue}
              onChange={(e) => setNewValue(e.target.value)}
              placeholder="Enter new value..."
            />

            <div className="flex justify-end gap-3 mt-4">
              <button
                className="px-4 py-2 rounded bg-red-600 hover:bg-red-500 text-text-0"
                onClick={() => setEditModal(null)}
              >
                Cancel
              </button>

              <button
                className="px-4 py-2 rounded bg-surface-3 hover:bg-accent-0 text-text-0"
                onClick={doUpdate}
              >
                Save changes
              </button>
            </div>
          </div>
        </div>
      )}

      <Footer />
    </div>
  );
};

export default ModerationPanelPage;
