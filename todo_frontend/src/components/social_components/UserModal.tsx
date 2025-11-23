import React from "react";
import UserHoverCard from "./UserHoverCard";

const UserModal: React.FC<{ user: any; onClose: () => void }> = ({
  user,
  onClose,
}) => {
  return (
    <div className="fixed inset-0 z-[999] flex items-center justify-center bg-black/70 px-4">
      <div className="w-full max-w-sm">
        <UserHoverCard user={user} />
        <button
            onClick={(e) => {
              e.stopPropagation();
              onClose();
            }}
            className="mt-3 w-full py-2 rounded-lg bg-white text-black font-semibold"
          >
           Close
        </button>
      </div>
    </div>
  );
};

export default UserModal;
