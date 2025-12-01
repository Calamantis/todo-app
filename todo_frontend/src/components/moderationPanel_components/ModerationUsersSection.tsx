import React, { useMemo } from "react";
import {
  Pencil,
  Trash2,
  Image,
  User,
  Search
} from "lucide-react";

interface ModerationUser {
  userId: number;
  email: string;
  fullName?: string;
  displayName?: string;
  description?: string;

  profileImageUrl?: string;      // /1/1_profile.jpg
  backgroundImageUrl?: string;   // /1/1_bg.jpg
}

interface Props {
  users: ModerationUser[];
  token: string;

  search: string;
  setSearch: (v: string) => void;

  setEditModal: (data: any) => void; 
  doDelete: (type: "user", id: number, field: string) => void;
}

const ModerationUsersSection: React.FC<Props> = ({
  users,
  token,
  search,
  setSearch,
  setEditModal,
  doDelete
}) => {

  const backend = "http://localhost:5268";

  // FILTER SEARCH
  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return users;

    return users.filter(
      (u) =>
        u.displayName?.toLowerCase().includes(q) ||
        u.email?.toLowerCase().includes(q)
    );
  }, [search, users]);

  return (
    <div className="bg-[var(--card-bg)] border border-white/10 p-5 rounded-xl shadow-lg">

      {/* SEARCH BAR */}
      <div className="flex items-center gap-2 bg-white/5 px-3 py-2 rounded-lg border border-white/10 w-full mb-4">
        <Search size={18} />
        <input
          type="text"
          placeholder="Search users…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="bg-transparent outline-none flex-1"
        />
      </div>

      <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
        <User size={20} /> Users
      </h2>

      {filtered.length === 0 && <div>No users found.</div>}

      {filtered.map((u) => {
        const profile = u.profileImageUrl
          ? `${backend}/${u.profileImageUrl}`
          : `${backend}/UserProfileImages/DefaultProfileImage.jpg`;

        const bg = u.backgroundImageUrl
          ? `${backend}/${u.backgroundImageUrl}`
          : `${backend}/DefaultBgImage.jpg`;

        return (
          <div
            key={u.userId}
            className="mb-5 rounded-xl overflow-hidden bg-white/5 border border-white/10 shadow-sm"
          >
            {/* BACKGROUND HEADER */}
            <div
              className="h-20 bg-cover bg-center relative"
              style={{ backgroundImage: `url(${bg})` }}
            >
              {/* PROFILE IMAGE */}
              <img
                src={profile}
                className="
                  absolute -bottom-6 left-4
                  w-14 h-14 rounded-full object-cover 
                  border-2 border-[var(--card-bg)]
                  shadow-lg
                "
              />
            </div>

            <div className="pt-8 px-4 pb-4">
              <div className="font-semibold text-lg">
                {u.displayName ?? u.email}
              </div>

              <div className="text-sm opacity-70">{u.email}</div>

              {u.description && (
                <div className="text-sm mt-1 opacity-80">
                  {u.description}
                </div>
              )}

              {/* ACTION BUTTONS */}
              <div className="flex flex-wrap gap-3 mt-4">

                {/* DISPLAY NAME */}
                <button
                  onClick={() =>
                    setEditModal({
                      id: u.userId,
                      type: "user",
                      field: "display-name",
                    })
                  }
                  className="flex items-center gap-1 px-2 py-1 rounded bg-blue-600/20 hover:bg-blue-600/40"
                >
                  <Pencil size={16} /> Edit name
                </button>

                <button
                  onClick={() => doDelete("user", u.userId, "display-name")}
                  className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
                >
                  <Trash2 size={16} /> Remove
                </button>

                {/* DESCRIPTION */}
                <button
                  onClick={() =>
                    setEditModal({
                      id: u.userId,
                      type: "user",
                      field: "description",
                    })
                  }
                  className="flex items-center gap-1 px-2 py-1 rounded bg-blue-600/20 hover:bg-blue-600/40"
                >
                  <Pencil size={16} /> Edit desc
                </button>

                <button
                  onClick={() => doDelete("user", u.userId, "description")}
                  className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
                >
                  <Trash2 size={16} /> Remove
                </button>

                {/* PROFILE IMAGE */}
                <button
                  onClick={() =>
                    setEditModal({
                      id: u.userId,
                      type: "user",
                      field: "profile-image",
                    })
                  }
                  className="flex items-center gap-1 px-2 py-1 rounded bg-green-600/20 hover:bg-green-600/40"
                >
                  <Image size={16} /> Set profile
                </button>

                <button
                  onClick={() => doDelete("user", u.userId, "profile-image")}
                  className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
                >
                  <Trash2 size={16} /> Remove profile
                </button>

                {/* BACKGROUND IMAGE */}
                <button
                  onClick={() =>
                    setEditModal({
                      id: u.userId,
                      type: "user",
                      field: "background-image",
                    })
                  }
                  className="flex items-center gap-1 px-2 py-1 rounded bg-green-600/20 hover:bg-green-600/40"
                >
                  <Image size={16} /> Set background
                </button>

                <button
                  onClick={() => doDelete("user", u.userId, "background-image")}
                  className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40"
                >
                  <Trash2 size={16} /> Remove bg
                </button>

              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default ModerationUsersSection;
