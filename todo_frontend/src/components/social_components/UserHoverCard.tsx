import React from "react";
import Avatar from "./Avatar";

const UserHoverCard: React.FC<{ user: any }> = ({ user }) => {


  // Backend root folder
  const backendBase = "http://localhost:5268";

  // Jeśli backend zwrócił ścieżkę typu "/5/5_profile.jpg"
  const resolvedSrc = user.backgroundImageUrl ? `${backendBase}/${user.backgroundImageUrl}?${Date.now()}` : `${backendBase}/DefaultProfileImage.jpg`;

  const bgFallback = "/UserProfileImages/DefaultBgImage.jpg";

  return (
    <div className="rounded-xl overflow-hidden shadow-2xl border border-white/10 bg-[#1f1f1f]">
      <div
        className="h-20 bg-cover bg-center"
        style={{
          backgroundImage: `url(${resolvedSrc || bgFallback})`,
        }}
      />

      <div className="p-3 flex gap-3">
        <Avatar src={user.profileImageUrl} size={50} />
        <div className="min-w-0">
          <div className="font-semibold text-white truncate">{user.fullName}</div>
          {user.synopsis && (
            <div className="text-xs text-white/80 mt-1 line-clamp-3">
              {user.synopsis}
            </div>
          )}
        </div>
      </div>

      <div className="px-3 pb-3 flex gap-2">
        <button className="flex-1 text-xs py-2 rounded bg-white/10 hover:bg-white/20 transition">
          Add friend
        </button>
        <button className="flex-1 text-xs py-2 rounded bg-white/10 hover:bg-red-500/60 transition">
          Block
        </button>
      </div>
    </div>
  );
};

export default UserHoverCard;
