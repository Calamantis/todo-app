import React, { useEffect, useState, useMemo } from "react";
import { useAuth } from "../components/AuthContext";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";
import { UserPlus, Trash2, Search, Loader2, List } from "lucide-react";

interface AdminUser {
  userId: number;
  email: string;
  fullName: string;
  role: string;
  profileImageUrl?: string;
}

interface SystemLog {
  logId: number;
  userId: number;
  action: string;
  entityType: string;
  entityId: number;
  description: string;
  createdAt: string;
}

const AdminPanelPage: React.FC = () => {
  const { user } = useAuth();
  const token = user?.token ?? "";

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  const [users, setUsers] = useState<AdminUser[]>([]);
  const [logs, setLogs] = useState<SystemLog[]>([]);
  const [loading, setLoading] = useState(true);
  const [activityIdToDelete, setActivityIdToDelete] = useState("");


  // SEARCH
  const [search, setSearch] = useState("");

  // CREATE MODERATOR FIELDS
  const [email, setEmail] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");

  // FILTER USERS
  const filteredUsers = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return users;

    return users.filter(
      (u) =>
        u.email.toLowerCase().includes(q) ||
        u.fullName.toLowerCase().includes(q)
    );
  }, [search, users]);

  // LOAD USERS
  const fetchUsers = async () => {
    try {
      const res = await fetch("/api/moderation/users", { headers });
      const data = await res.json();

      setUsers(
        data.map((u: any) => ({
          userId: u.userId,
          email: u.email,
          fullName: u.displayName ?? u.fullName ?? u.email,
          role: u.role,
          profileImageUrl: u.profileImageUrl,
        }))
      );
    } catch (e) {
      console.error("Failed to load users", e);
    }
  };

  // LOAD LOGS
  const fetchLogs = async () => {
    try {
      const res = await fetch("/api/admin", { headers });
      const data = await res.json();
      setLogs(data);
    } catch (e) {
      console.error("Failed to load system logs", e);
    }
  };

  useEffect(() => {
    Promise.all([fetchUsers(), fetchLogs()]).then(() => setLoading(false));
  }, []);

  // CREATE MODERATOR
  const createModerator = async () => {
    if (!email || !fullName || !password) return alert("Fill all fields!");

    const body = { email, fullName, password };
    const res = await fetch("/api/admin/moderators", {
      method: "POST",
      headers,
      body: JSON.stringify(body),
    });

    if (res.ok) {
      alert("Moderator created!");
      setEmail("");
      setFullName("");
      setPassword("");
      fetchUsers();
    } else {
      alert("Failed to create moderator.");
    }
  };

  // DELETE USER
  const deleteUser = async (id: number) => {
    if (!window.confirm("Delete this user permanently?")) return;

    const res = await fetch(`/api/admin/users/${id}`, {
      method: "DELETE",
      headers,
    });

    if (res.ok) {
      alert("User removed.");
      fetchUsers();
    } else {
      alert("Failed to delete user.");
    }
  };

  if (loading) {
    return (
      <div className="h-[50vh] flex justify-center items-center">
        <Loader2 size={40} className="animate-spin" />
      </div>
    );
  }

  // DELETE ACTIVITY
  const deleteActivity = async () => {
  if (!activityIdToDelete) return alert("Enter activity ID!");

  if (!window.confirm(`Delete activity #${activityIdToDelete}?`)) return;

  const res = await fetch(`/api/admin/activities/${activityIdToDelete}`, {
    method: "DELETE",
    headers,
  });

  if (res.ok) {
    alert("Activity deleted.");
    setActivityIdToDelete("");
    fetchLogs(); // odśwież logi
  } else {
    alert("Failed to delete activity.");
  }
};


  return (
    <div className="min-h-screen flex flex-col bg-surface-0 text-text-0">
      <NavigationWrapper />

      <div className="flex-1 p-6 mx-auto">
        {/* GRID 3 PANELS */}
        <div className="grid lg:grid-cols-1 xl:grid-cols-4 gap-6">

          {/* PANEL 1 — CREATE MODERATOR */}
          <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
            <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
              <UserPlus size={20} /> Create Moderator
            </h2>

            <div className="grid grid-cols-1 gap-3">
              <input
                className="p-2 rounded bg-surface-2"
                placeholder="Email…"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
              <input
                className="p-2 rounded bg-surface-2"
                placeholder="Full name…"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
              />
              <input
                type="password"
                className="p-2 rounded bg-surface-2"
                placeholder="Password…"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>

            <button
              onClick={createModerator}
              className="mt-4 px-4 py-2 rounded bg-accent-0 hover:bg-accent-1 text-text-0 w-full"
            >
              Create Moderator
            </button>
          </div>

          {/* PANEL 3 — SYSTEM LOGS */}
          <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
            <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2 text-text-0">
              <List size={20} /> System Logs
            </h2>

            <div className="max-h-[600px] overflow-y-auto pr-2 space-y-3">
              {logs.length === 0 ? (
                <div>No logs found.</div>
              ) : (
                logs.map((log) => (
                  <div
                    key={log.logId}
                    className="p-3 bg-surface-2 rounded-lg"
                  >
                    <div className="font-semibold">
                      {log.action} — {log.entityType} #{log.entityId}
                    </div>
                    <div className="text-sm opacity-70">
                      Moderator ID: {log.userId}
                    </div>
                    <div className="text-sm">{log.description}</div>
                    <div className="text-xs opacity-50 mt-1">
                      {new Date(log.createdAt).toLocaleString()}
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>


              {/* PANEL 4 — DELETE ACTIVITY */}
              <div className="bg-surface-1 p-6 rounded-xl h-fit shadow-lg">
                <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
                  <Trash2 size={20} /> Delete Activity
                </h2>

                <input
                  type="number"
                  placeholder="Activity ID…"
                  value={activityIdToDelete}
                  onChange={(e) => setActivityIdToDelete(e.target.value)}
                  className="p-2 rounded bg-surface-2 w-full"
                />

                <button
                  onClick={deleteActivity}
                  className="mt-4 px-4 py-2 rounded bg-red-600 hover:bg-red-700 text-white w-full"
                >
                  Delete Activity Permanently
                </button>
              </div>

          {/* PANEL 2 — USER LIST */}
          <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
            <h2 className="text-2xl font-semibold mb-4">Delete users account</h2>

            <div className="flex items-center gap-2 bg-surface-2 px-3 py-2 rounded-lg  mb-4">
              <Search size={18} />
              <input
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search users…"
                className="bg-transparent outline-none flex-1"
              />
            </div>

            {filteredUsers.map((u) => {
              const profile = u.profileImageUrl
                ? `http://localhost:5268/${u.profileImageUrl}`
                : `http://localhost:5268/UserProfileImages/DefaultProfileImage.jpg`;

              return (
                <div
                  key={u.userId}
                  className="p-4 mb-3 bg-surface-2 rounded-lg flex justify-between items-center"
                >
                  <div className="flex items-center gap-3">
                    <img
                      src={profile}
                      className="w-12 h-12 rounded-full object-cover border border-surface-2"
                    />
                    <div>
                      <div className="font-semibold">{u.fullName}</div>
                      <div className="text-sm opacity-70">{u.email}</div>
                      <div className="text-xs opacity-50">{u.role}</div>
                    </div>
                  </div>

                  <button
                    onClick={() => deleteUser(u.userId)}
                    className="px-2 py-1 rounded bg-red-600/30 hover:bg-red-600/60 flex items-center gap-1 text-text-0"
                  >
                    <Trash2 size={18} /> Delete
                  </button>
                </div>
              );
            })}
          </div>


        </div>
      </div>

      <Footer />
    </div>
  );
};

export default AdminPanelPage;
