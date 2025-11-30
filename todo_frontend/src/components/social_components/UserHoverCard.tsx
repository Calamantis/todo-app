import React, { useState } from "react";
import Avatar from "./Avatar";
import FriendTimelineModal from "./FriendTimelineModal";
import OnlineActivityInviteSingleModal from "./OnlineActivityInviteSingleModal";
import {
  UserPlus,
  UserMinus,
  ShieldBan,
  ShieldOff,
  Eye,
  CalendarPlus,
  X,
  Check
} from "lucide-react";
import { socialApi } from "../../services/socialActions";
import { useAuth } from "../AuthContext";

const backendBase = "http://localhost:5268";

const UserHoverCard: React.FC<{
  user: any;
  variant?: "friend" | "browse" | "sent" | "received" | "blocked";
  modal?: boolean;
  onClose?: () => void;
  onAction?: () => void;
}> = ({ user, variant, modal = false, onClose, onAction }) => {

  const [timelineModalOpen, setTimelineModalOpen] = useState(false);
  const [inviteModalOpen, setInviteModalOpen] = useState(false);


  // ===== AUTH & USERID =====
  const { user: auth } = useAuth();
  const token = auth?.token || "";

  const userId =
    user.userId || user.friendId || user.blockedUserId || null;

  const wrap = async (fn: () => Promise<any>) => {
    try {
      await fn();
      onAction?.();
    } catch (e) {
      console.error(e);
    }
  };

  // ===== ACTIONS =====
  const doSendInvite = () => wrap(() => socialApi.sendInvite(userId, token));
  const doRemoveFriend = () => wrap(() => socialApi.removeFriend(userId, token));
  const doBlock = () => wrap(() => socialApi.blockUser(userId, token));
  const doUnblock = () => wrap(() => socialApi.unblockUser(userId, token));
  const doAcceptInvite = () => wrap(() => socialApi.acceptInvite(userId, token));
  const doRejectInvite = () => wrap(() => socialApi.rejectInvite(userId, token));
  const doCancelInvite = () => wrap(() => socialApi.cancelInvite(userId, token));

  // ===== IMAGES =====
  const bgUrl = user.backgroundImageUrl
    ? `${backendBase}/${user.backgroundImageUrl}?${Date.now()}`
    : `${backendBase}/DefaultBgImage.jpg`;

  return (
    <>
      {/* Modal overlay */}
      {modal && (
        <div
          className="fixed inset-0 bg-black/70 flex items-center justify-center z-[999]"
          onClick={onClose}
        />
      )}

      {/* CARD */}
      <div
        className={
          modal
            ? "fixed z-[1000] w-full max-w-sm mx-auto px-4 top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"
            : "rounded-xl shadow-2xl border border-white/10 bg-[#1f1f1f]"
        }
        onClick={(e) => e.stopPropagation()}
      >
        <div className="relative rounded-xl overflow-hidden bg-[#1f1f1f] border border-white/10">

          {/* Close btn */}
          {modal && (
            <button
              onClick={onClose}
              className="absolute top-2 right-2 bg-black/40 hover:bg-black/70 p-1 rounded-full z-50"
            >
              <X size={18} className="text-white" />
            </button>
          )}

          {/* BACKGROUND + BADGE */}
          <div
            className="relative h-20 bg-cover bg-center"
            style={{ backgroundImage: `url(${bgUrl})` }}
          >
            <div
              className="
                absolute top-1 left-1 
                px-2 py-0.5 
                rounded-xl 
                text-[10px] font-bold uppercase tracking-wide
                bg-black/50 text-white
              "
            >
              {variant === "friend" && "Friend"}
              {variant === "browse" && "Stranger"}
              {variant === "sent" && "Invite Sent"}
              {variant === "received" && "Invite Received"}
              {variant === "blocked" && "Blocked"}
            </div>
          </div>

          {/* PROFILE */}
          <div className="p-3 flex gap-3">
            <Avatar src={user.profileImageUrl} size={50} />
            <div className="min-w-0">
              <div className="font-semibold text-white truncate">
                {user.fullName}
              </div>
              {user.synopsis && (
                <div className="text-xs text-white/80 mt-1 line-clamp-3">
                  {user.synopsis}
                </div>
              )}
            </div>
          </div>

          {/* ACTIONS */}
          <div className="px-3 pb-3 flex items-center justify-center gap-3 text-white/80">

            {/* FRIEND */}
            {variant === "friend" && (
              <>
                <button
                  onClick={() => setTimelineModalOpen(true)}
                  className="hover:text-white"
                  title="View timeline"
                >
                  <Eye size={18} />
                </button>

                <button
                  onClick={() => setInviteModalOpen(true)}
                  className="hover:text-white"
                >
                  <CalendarPlus size={18} />
                </button>


                <div className="mx-2 opacity-40">|</div>

                <button
                  onClick={doRemoveFriend}
                  className="hover:text-white"
                  title="Remove friend"
                >
                  <UserMinus size={18} />
                </button>

                <button
                  onClick={() => wrap(async () => {
                    await socialApi.removeFriend(userId, token);
                    await socialApi.blockUser(userId, token);
                  })}
                  className="hover:text-white"
                  title="Remove & Block"
                >
                  <ShieldBan size={18} />
                </button>
              </>
            )}

            {/* BROWSE */}
            {variant === "browse" && (
              <>
                <button
                  onClick={doSendInvite}
                  className="hover:text-white"
                  title="Add friend"
                >
                  <UserPlus size={18} />
                </button>

                <button
                  onClick={doBlock}
                  className="hover:text-white"
                  title="Block"
                >
                  <ShieldBan size={18} />
                </button>
              </>
            )}

            {/* BLOCKED */}
            {variant === "blocked" && (
              <button
                onClick={doUnblock}
                className="hover:text-white"
                title="Unblock"
              >
                <ShieldOff size={18} />
              </button>
            )}

            {/* SENT INVITE */}
            {variant === "sent" && (
              <button
                onClick={doCancelInvite}
                className="hover:text-white"
                title="Cancel invite"
              >
                <X size={18} />
              </button>
            )}

            {/* RECEIVED INVITE */}
            {variant === "received" && (
              <>
                <button
                  onClick={doAcceptInvite}
                  className="hover:text-white"
                  title="Accept"
                >
                  <Check size={18} />
                </button>

                <button
                  onClick={doRejectInvite}
                  className="hover:text-white"
                  title="Reject"
                >
                  <X size={18} />
                </button>
              </>
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
        </div>
      </div>
    </>
  );
};

export default UserHoverCard;
