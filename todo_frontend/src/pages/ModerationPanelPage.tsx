// import React, { useEffect, useState, useMemo } from "react";
// import { useAuth } from "../components/AuthContext";
// import {
//   Pencil,
//   Trash2,
//   Image,
//   User,
//   FileText,
//   Loader2,
//   Search
// } from "lucide-react";
// import Footer from "../components/Footer";
// import NavigationWrapper from "../components/NavigationWrapper";

// interface ModerationUser {
//   userId: number;
//   email: string;
//   fullName: string;
//   profileImageUrl?: string;
//   backgroundImageUrl?: string;
//   displayName?: string;
//   description?: string;
// }

// interface ModerationActivity {
//   activityId: number;
//   title: string;
//   description: string;
// }

// const ModerationPanelPage: React.FC = () => {
//   const { user } = useAuth();
//   const token = user?.token ?? "";

//   const [users, setUsers] = useState<ModerationUser[]>([]);
//   const [activities, setActivities] = useState<ModerationActivity[]>([]);
//   const [loading, setLoading] = useState(true);

//   const [userSearch, setUserSearch] = useState("");
//   const [activitySearch, setActivitySearch] = useState("");

//     // MEMOIZED FILTERS
//   const filteredUsers = useMemo(() => {
//     const q = userSearch.trim().toLowerCase();
//     if (!q) return users;

//     return users.filter(
//       (u) =>
//         u.displayName?.toLowerCase().includes(q) ||
//         u.email?.toLowerCase().includes(q)
//     );
//   }, [userSearch, users]);

//   const filteredActivities = useMemo(() => {
//     const q = activitySearch.trim().toLowerCase();
//     if (!q) return activities;

//     return activities.filter(
//       (a) =>
//         a.title?.toLowerCase().includes(q) ||
//         a.description?.toLowerCase().includes(q)
//     );
//   }, [activitySearch, activities]);

//   // modal
//   const [editModal, setEditModal] = useState<{
//     id: number;
//     type: "user" | "activity";
//     field: string;
//   } | null>(null);

//   const [newValue, setNewValue] = useState("");

//   const headers = {
//     Authorization: `Bearer ${token}`,
//     "Content-Type": "application/json",
//   };

//   // FETCH ALL
//   const fetchData = async () => {
//     setLoading(true);
//     try {
//       const usersRes = await fetch("/api/moderation/users", { headers });
//       const activitiesRes = await fetch("/api/moderation/activities", { headers });

//       setUsers(await usersRes.json());
//       setActivities(await activitiesRes.json());
//     } catch (e) {
//       console.error("Failed to load moderation data", e);
//     }
//     setLoading(false);
//   };

//   useEffect(() => {
//     fetchData();
//   }, []);

//   // PUT update
//   const doUpdate = async () => {
//     if (!editModal) return;

//     const { id, type, field } = editModal;

//     const endpoint =
//       type === "user"
//         ? `/api/moderation/users/${id}/${field}?newValue=${encodeURIComponent(newValue)}`
//         : `/api/moderation/activities/${id}/${field}?newValue=${encodeURIComponent(newValue)}`;

//     await fetch(endpoint, {
//       method: "PUT",
//       headers,
//       body: JSON.stringify({ newValue }),
//     });

//     setEditModal(null);
//     setNewValue("");
//     fetchData();
//   };

//   // DELETE field
//   const doDelete = async (type: "user" | "activity", id: number, field: string) => {
//     const endpoint =
//       type === "user"
//         ? `/api/moderation/users/${id}/${field}`
//         : `/api/moderation/activities/${id}/${field}`;

//     await fetch(endpoint, {
//       method: "DELETE",
//       headers: { Authorization: `Bearer ${token}` },
//     });

//     fetchData();
//   };

//   if (loading) {
//     return (
//       <div className="p-10 flex justify-center items-center">
//         <Loader2 className="animate-spin" size={40} />
//       </div>
//     );
//   }

//   return (
//     <div className = "min-h-screen bg-[var(--background-color)] flex flex-col justify-between">
//         <NavigationWrapper />
//     <div className="p-6 text-[var(--text-color)]">
//       <h1 className="text-3xl font-bold mb-6">Moderation Panel</h1>

//       <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">

//        {/* USERS PANEL */}
// <div className="bg-[var(--card-bg)] border border-white/10 p-5 rounded-xl shadow-lg">

//               {/* SEARCH BAR */}
//         <div className="flex items-center justify-center gap-2 bg-white/5 px-3 py-2 rounded-lg border border-white/10 w-full mb-4">
//           <Search size={18} />
//           <input
//             type="text"
//             placeholder="Search users…"
//             value={userSearch}
//             onChange={(e) => setUserSearch(e.target.value)}
//             className="bg-transparent outline-none flex-1"
//           />
//         </div>

//   <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
//     <User size={20} /> Users
//   </h2>

//   {users.length === 0 && <div>No users found.</div>}

//   {filteredUsers.map((u) => {
//     const profile = u.profileImageUrl
//       ? `http://localhost:5268/${u.profileImageUrl}`
//       : `http://localhost:5268/UserProfileImages/DefaultProfileImage.jpg`;

//     const bg = u.backgroundImageUrl
//       ? `http://localhost:5268/${u.backgroundImageUrl}`
//       : `http://localhost:5268/DefaultBgImage.jpg`;
      
//     return (
//       <div
//         key={u.userId}
//         className="mb-5 rounded-xl overflow-hidden bg-white/5 border border-white/10 shadow-sm"
//       >
//         {/* HEADER — BACKGROUND */}
//         <div
//           className="h-20 bg-cover bg-center relative"
//           style={{ backgroundImage: `url(${bg})` }}
//         >
//           {/* PROFILE IMAGE */}
//           <img
//             src={profile}
//             className="
//               absolute -bottom-6 left-4 
//               w-14 h-14 rounded-full object-cover border-2 border-[var(--card-bg)]
//               shadow-lg
//             "
//           />
//         </div>

//         <div className="pt-8 px-4 pb-4">
//           <div className="font-semibold text-lg">{u.displayName ?? u.email}</div>
//           <div className="text-sm opacity-70">{u.email}</div>

//           {u.description && (
//             <div className="text-sm mt-1 opacity-80">{u.description}</div>
//           )}

//           {/* ACTIONS */}
//           <div className="flex flex-wrap gap-3 mt-4">

//             {/* DISPLAY NAME */}
//             <button
//               onClick={() =>
//                 setEditModal({ id: u.userId, type: "user", field: "display-name" })
//               }
//               className="flex items-center gap-1 px-2 py-1 rounded bg-blue-600/20 hover:bg-blue-600/40"
//             >
//               <Pencil size={16} /> Edit name
//             </button>
//             <button
//               onClick={() => doDelete("user", u.userId, "display-name")}
//               className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
//             >
//               <Trash2 size={16} /> Remove
//             </button>

//             {/* DESCRIPTION */}
//             <button
//               onClick={() =>
//                 setEditModal({ id: u.userId, type: "user", field: "description" })
//               }
//               className="flex items-center gap-1 px-2 py-1 rounded bg-blue-600/20 hover:bg-blue-600/40"
//             >
//               <Pencil size={16} /> Edit desc
//             </button>
//             <button
//               onClick={() => doDelete("user", u.userId, "description")}
//               className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
//             >
//               <Trash2 size={16} /> Remove
//             </button>

//             {/* IMAGES */}
//             <button
//               onClick={() =>
//                 setEditModal({ id: u.userId, type: "user", field: "profile-image" })
//               }
//               className="flex items-center gap-1 px-2 py-1 rounded bg-green-600/20 hover:bg-green-600/40"
//             >
//               <Image size={16} /> Set profile
//             </button>
//             <button
//               onClick={() => doDelete("user", u.userId, "profile-image")}
//               className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
//             >
//               <Trash2 size={16} /> Remove profile
//             </button>

//             <button
//               onClick={() =>
//                 setEditModal({ id: u.userId, type: "user", field: "background-image" })
//               }
//               className="flex items-center gap-1 px-2 py-1 rounded bg-green-600/20 hover:bg-green-600/40"
//             >
//               <Image size={16} /> Set background
//             </button>
//             <button
//               onClick={() => doDelete("user", u.userId, "background-image")}
//               className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
//             >
//               <Trash2 size={16} /> Remove bg
//             </button>
//           </div>
//         </div>
//       </div>
//     );
//   })}
// </div>


//         {/* ACTIVITIES PANEL */}
//         <div className="bg-[var(--card-bg)] border border-white/10 p-5 rounded-xl shadow-lg">

//                 {/* SEARCH BAR */}
//         <div className="flex items-center gap-2 bg-white/5 px-3 py-2 rounded-lg border border-white/10 w-full mb-4">
//           <Search size={18} />
//           <input
//             type="text"
//             placeholder="Search activities…"
//             value={activitySearch}
//             onChange={(e) => setActivitySearch(e.target.value)}
//             className="bg-transparent outline-none flex-1"
//           />
//         </div>

//           <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
//             <FileText size={20} /> Activities
//           </h2>

//           {activities.length === 0 && <div>No activities found.</div>}

//           {filteredActivities.map((a) => (
//             <div
//               key={a.activityId}
//               className="mb-4 p-4 rounded-lg bg-white/5 border border-white/10"
//             >
//               <div className="font-semibold">{a.title}</div>
//               <div className="text-sm opacity-80 mt-1">
//                 Desc: {a.description ?? <i>none</i>}
//               </div>

//               <div className="flex flex-wrap gap-3 mt-3">
//                 {/* TITLE */}
//                 <button
//                   onClick={() =>
//                     setEditModal({ id: a.activityId, type: "activity", field: "title" })
//                   }
//                   className="flex items-center gap-1 px-2 py-1 rounded bg-blue-600/20 hover:bg-blue-600/40"
//                 >
//                   <Pencil size={16} /> Edit title
//                 </button>

//                 <button
//                   onClick={() => doDelete("activity", a.activityId, "title")}
//                   className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
//                 >
//                   <Trash2 size={16} /> Remove title
//                 </button>

//                 {/* DESCRIPTION */}
//                 <button
//                   onClick={() =>
//                     setEditModal({ id: a.activityId, type: "activity", field: "description" })
//                   }
//                   className="flex items-center gap-1 px-2 py-1 rounded bg-blue-600/20 hover:bg-blue-600/40"
//                 >
//                   <Pencil size={16} /> Edit desc
//                 </button>

//                 <button
//                   onClick={() => doDelete("activity", a.activityId, "description")}
//                   className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
//                 >
//                   <Trash2 size={16} /> Remove desc
//                 </button>
//               </div>
//             </div>
//           ))}
//         </div>

//       </div>

//       {/* EDIT MODAL */}
//       {editModal && (
//         <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
//           <div className="bg-[var(--card-bg)] p-6 rounded-xl border border-white/10 shadow-2xl w-full max-w-md">
//             <h3 className="text-lg font-semibold mb-4">
//               Edit {editModal.field.replace("-", " ")}
//             </h3>

//             <input
//               className="w-full p-2 rounded bg-black/20 border border-white/10"
//               value={newValue}
//               onChange={(e) => setNewValue(e.target.value)}
//               placeholder="Enter new value..."
//             />

//             <div className="flex justify-end gap-3 mt-4">
//               <button
//                 className="px-4 py-2 rounded bg-white/10 hover:bg-white/20"
//                 onClick={() => setEditModal(null)}
//               >
//                 Cancel
//               </button>

//               <button
//                 className="px-4 py-2 rounded bg-blue-600 hover:bg-blue-700 text-white"
//                 onClick={doUpdate}
//               >
//                 Save changes
//               </button>
//             </div>
//           </div>
//         </div>
//       )}

//     </div>
//     <Footer />
//     </div>
//   );
// };

// export default ModerationPanelPage;



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
      <div className="min-h-screen flex items-center justify-center bg-[var(--background-color)]">
        <Loader2 className="animate-spin" size={40} />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[var(--background-color)] flex flex-col justify-between">
      <NavigationWrapper />

      <div className="p-6 text-[var(--text-color)] max-w-8xl mx-auto w-full">
        <h1 className="text-3xl font-bold mb-6">Moderation Panel</h1>

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
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
          <div className="bg-[var(--card-bg)] p-6 rounded-xl border border-white/10 shadow-2xl w-full max-w-md">
            <h3 className="text-lg font-semibold mb-4">
              Edit {editModal.field.replace("-", " ")}
            </h3>

            <input
              className="w-full p-2 rounded bg-black/20 border border-white/10"
              value={newValue}
              onChange={(e) => setNewValue(e.target.value)}
              placeholder="Enter new value..."
            />

            <div className="flex justify-end gap-3 mt-4">
              <button
                className="px-4 py-2 rounded bg-white/10 hover:bg-white/20"
                onClick={() => setEditModal(null)}
              >
                Cancel
              </button>

              <button
                className="px-4 py-2 rounded bg-blue-600 hover:bg-blue-700 text-white"
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
