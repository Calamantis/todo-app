import React, { useState } from "react";
import Avatar from "./Avatar";
import UserHoverCard from "./UserHoverCard";
import UserModal from "./UserModal";
import { UserPlus, ShieldBan, Eye } from "lucide-react";

const UserListItem: React.FC<{
  user: any;
  variant: "friend" | "browse" | "sent" | "received";
  compact?: boolean;
}> = ({ user, variant, compact }) => {
  const [hover, setHover] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);

  const isMobile =
    typeof window !== "undefined" && window.innerWidth < 1024;

  return (
    <div
      className="relative group rounded-lg px-2 py-2 flex items-center justify-between hover:bg-white/10 transition"
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      onClick={() => { setMobileOpen(true)}}
    >
      <div className="flex items-center gap-3 min-w-0">
        <Avatar src={user.profileImageUrl} size={compact ? 30 : 36} />
        <div className="min-w-0">
          <div className="font-medium truncate">{user.fullName}</div>
          {user.email && !compact && (
            <div className="text-xs opacity-70 truncate">{user.email}</div>
          )}
        </div>
      </div>

      <div className="flex items-center gap-2 opacity-80">
        {variant !== "friend" && (
        <button className="hover:opacity-100" title="Add friend">
          <UserPlus size={18} />
        </button> )}
        <button className="hover:opacity-100" title="Block">
          <ShieldBan size={18} />
        </button>

        {variant === "friend" && (
          <button className="hover:opacity-100" title="View timeline">
            <Eye size={18} />
          </button>
        )}
      </div>

      {!isMobile && hover && (
        <div className="absolute left-full top-0 ml-3 z-50 w-[280px]">
          <UserHoverCard user={user} />
        </div>
      )}

      {mobileOpen && (
        <UserModal user={user} onClose={() => setMobileOpen(false)} />
      )}
    </div>
  );
};

export default UserListItem;
