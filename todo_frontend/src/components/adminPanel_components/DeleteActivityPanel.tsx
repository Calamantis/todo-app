// import React, { useState } from "react";
// import { Trash2 } from "lucide-react";

// interface Props {
//   token: string;
// }

// const DeleteActivityPanel: React.FC<Props> = ({ token }) => {
//   const [id, setId] = useState("");

//   const headers = {
//     Authorization: `Bearer ${token}`,
//     "Content-Type": "application/json",
//   };

//   const remove = async () => {
//     if (!id) return;
//     if (!confirm(`Delete activity #${id}?`)) return;

//     const res = await fetch(`/api/admin/activities/${id}`, {
//       method: "DELETE",
//       headers,
//     });

//     if (res.ok) setId("");
//     else alert("Failed");
//   };

//   return (
//     <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
//       <h2 className="text-2xl font-semibold mb-4 flex gap-2">
//         <Trash2 size={20} /> Delete Activity
//       </h2>

//       <input
//         type="number"
//         value={id}
//         onChange={(e) => setId(e.target.value)}
//         className="p-2 rounded bg-surface-2 w-full"
//         placeholder="Activity ID"
//       />

//       <button
//         onClick={remove}
//         className="mt-4 w-full px-4 py-2 rounded bg-red-600 hover:bg-red-700">
//         Delete
//       </button>
//     </div>
//   );
// };

// export default DeleteActivityPanel;


import React, { useEffect, useMemo, useState } from "react";
import { Trash2, Search } from "lucide-react";

interface ModerationActivity {
  activityId: number;
  title: string;
  ownerEmail: string;
  isRecurring: boolean;
  isOnline: boolean;
}

interface Props {
  token: string;
}

const DeleteActivityPanel: React.FC<Props> = ({ token }) => {
  const [activities, setActivities] = useState<ModerationActivity[]>([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  // 🔹 LOAD ACTIVITIES
  const fetchActivities = async () => {
    try {
      const res = await fetch("/api/moderation/activities", { headers });
      const data = await res.json();
      setActivities(data);
    } catch (e) {
      console.error("Failed to load activities", e);
    } finally {
      setLoading(false);
    }
  };

  // 🔹 DELETE ACTIVITY
  const deleteActivity = async (id: number) => {
    if (!window.confirm(`Delete activity permanently?`)) return;

    const res = await fetch(`/api/admin/activities/${id}`, {
      method: "DELETE",
      headers,
    });

    if (res.ok) {
      setActivities((prev) => prev.filter((a) => a.activityId !== id));
    } else {
      alert("Failed to delete activity.");
    }
  };

  useEffect(() => {
    fetchActivities();
  }, []);

  // 🔹 SEARCH FILTER
  const filteredActivities = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return activities;

    return activities.filter(
      (a) =>
        a.title?.toLowerCase().includes(q) ||
        a.ownerEmail?.toLowerCase().includes(q)
    );
  }, [search, activities]);

  return (
    <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
      <h2 className="text-2xl font-semibold mb-4">
        Delete Activities
      </h2>

      {/* SEARCH */}
      <div className="flex items-center gap-2 bg-surface-2 px-3 py-2 rounded mb-4">
        <Search size={18} />
        <input
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search by title or owner…"
          className="bg-transparent outline-none flex-1"
        />
      </div>

      {loading && <div>Loading activities…</div>}

      {!loading && filteredActivities.length === 0 && (
        <div>No activities found.</div>
      )}

      {/* LIST */}
      <div className="space-y-3 max-h-[547px] overflow-y-auto pr-1 custom-scrollbar">
        {filteredActivities.map((a) => (
          <div
            key={a.activityId}
            className="p-3 bg-surface-2 rounded flex justify-between items-center"
          >
            <div>
              <div className="font-semibold">{a.title}</div>
              <div className="text-sm opacity-70">
                Owner: {a.ownerEmail}
              </div>
              <div className="text-xs opacity-50">
                {a.isOnline ? "Online" : "Offline"} ·{" "}
                {a.isRecurring ? "Recurring" : "One-time"}
              </div>
            </div>

            <button
              onClick={() => deleteActivity(a.activityId)}
              className="px-2 py-1 rounded bg-red-600/30 hover:bg-red-600/60"
              title="Delete activity"
            >
              <Trash2 size={18} />
            </button>
          </div>
        ))}
      </div>
    </div>
  );
};

export default DeleteActivityPanel;
