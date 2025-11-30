import React, { useState } from "react";
import Avatar from "./Avatar";
import UserHoverCard from "./UserHoverCard";
//import UserModal from "./UserModal";
import { socialApi } from "../../services/socialActions";
import { useAuth } from "../AuthContext";

import FriendTimelineModal from "../social_components/FriendTimelineModal";
import OnlineActivityInviteSingleModal from "../social_components/OnlineActivityInviteSingleModal";


import {
  UserPlus,
  UserMinus,
  ShieldBan,
  ShieldOff,
  Eye,
  X,
  Check,
  CalendarPlus,
} from "lucide-react";

const UserListItem: React.FC<{
  user: any;
  variant: "friend" | "browse" | "sent" | "received" | "blocked";
  compact?: boolean;
  onAction?: () => void;
}> = ({ user, variant, compact, onAction }) => {
  const [mobileOpen, setMobileOpen] = useState(false);

  const [inviteModalOpen, setInviteModalOpen] = useState(false);
  const [timelineModalOpen, setTimelineModalOpen] = useState(false);

  const { user: auth } = useAuth();
  const token = auth?.token || "";

  const userId = user.userId || user.friendId || user.blockedUserId || null;

  if (!userId) {
    console.warn("UserListItem: Missing userId for: ", user);
  }

  const wrap = async (fn: () => Promise<any>) => {
    try {
      await fn();
      onAction?.();
    } catch (e) {
      console.error(e);
    }
  };

  const doSendInvite = () => wrap(() => socialApi.sendInvite(userId, token));
  const doRemoveFriend = () => wrap(() => socialApi.removeFriend(userId, token));
  const doCancelInvite = () => wrap(() => socialApi.cancelInvite(userId, token));
  const doAcceptInvite = () => wrap(() => socialApi.acceptInvite(userId, token));
  const doRejectInvite = () => wrap(() => socialApi.rejectInvite(userId, token));
  const doBlock = () => wrap(() => socialApi.blockUser(userId, token));
  const doUnblock = () => wrap(() => socialApi.unblockUser(userId, token));

  return (
    <div
      className="relative group rounded-lg px-2 py-2 flex items-center justify-between hover:bg-white/10 transition"
      onClick={() => setMobileOpen(true)}
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

      {/* ACTION BUTTONS */}
      <div className="flex items-center gap-2 opacity-80">

        {variant === "friend" && (
          <>

            <button
              className="hover:opacity-100"
              title="View timeline"
              onClick={(e) => {
                e.stopPropagation();
                setTimelineModalOpen(true);
              }}
            >
              <Eye size={18} />
            </button>



            <button
              className="hover:opacity-100"
              title="Invite to activity"
              onClick={(e) => {
                e.stopPropagation();
                setInviteModalOpen(true);
              }}
            >
              <CalendarPlus size={18} />
            </button>

            <div className="mx-2">|</div>

            <button
              onClick={(e) => {
                e.stopPropagation();
                doRemoveFriend();
              }}
              className="hover:opacity-100"
              title="Remove"
            >
              <UserMinus size={18} />
            </button>

            <button
              onClick={(e) => {
                e.stopPropagation();
                wrap(async () => {
                  await socialApi.removeFriend(userId, token);
                  await socialApi.blockUser(userId, token);
                });
              }}
              className="hover:opacity-100"
              title="Remove & Block"
            >
              <ShieldBan size={18} />
            </button>
          </>
        )}

        {variant === "blocked" && (
          <button
            onClick={(e) => {
              e.stopPropagation();
              doUnblock();
            }}
            className="hover:opacity-100"
            title="Unblock"
          >
            <ShieldOff size={18} />
          </button>
        )}

        {variant === "browse" && (
          <>
            <button
              onClick={(e) => {
                e.stopPropagation();
                doSendInvite();
              }}
              className="hover:opacity-100"
              title="Add friend"
            >
              <UserPlus size={18} />
            </button>

            <button
              onClick={(e) => {
                e.stopPropagation();
                doBlock();
              }}
              className="hover:opacity-100"
              title="Block"
            >
              <ShieldBan size={18} />
            </button>
          </>
        )}

        {variant === "sent" && (
          <button
            onClick={(e) => {
              e.stopPropagation();
              doCancelInvite();
            }}
            className="hover:opacity-100"
            title="Cancel"
          >
            <X size={18} />
          </button>
        )}

        {variant === "received" && (
          <>
            <button
              onClick={(e) => {
                e.stopPropagation();
                doAcceptInvite();
              }}
              className="hover:opacity-100"
              title="Accept"
            >
              <Check size={18} />
            </button>

            <button
              onClick={(e) => {
                e.stopPropagation();
                doRejectInvite();
              }}
              className="hover:opacity-100"
              title="Reject"
            >
              <X size={18} />
            </button>
          </>
        )}
      </div>

        {mobileOpen && (
          <UserHoverCard
            modal
            user={user}
            variant={variant}
            onClose={() => setMobileOpen(false)}
          />
        )}

        {timelineModalOpen && (
          <FriendTimelineModal
            userId={userId}
            onClose={() => setTimelineModalOpen(false)}
          />
        )}

        {inviteModalOpen && (
          <OnlineActivityInviteSingleModal
            friendId={userId}
            onClose={() => setInviteModalOpen(false)}
          />
        )}


    </div>
  );
};

export default UserListItem;
