import React from "react";
import { Ban } from "lucide-react";

const BlockedListItem: React.FC<{
  fullName: string;
  blockedAt: string;
}> = ({ fullName, blockedAt }) => {
  return (
    <div className="rounded-lg px-2 py-2 flex items-center justify-between hover:bg-white/10 transition">
      <div className="flex items-center gap-3">
        <div className="w-9 h-9 rounded-full bg-white/10 flex items-center justify-center">
          <Ban size={18} />
        </div>
        <div>
          <div className="font-medium">{fullName}</div>
          <div className="text-xs opacity-70">
            blocked at {new Date(blockedAt).toLocaleString()}
          </div>
        </div>
      </div>

      <button className="text-xs px-2 py-1 rounded bg-white/10 hover:bg-white/20 transition">
        Unblock
      </button>
    </div>
  );
};

export default BlockedListItem;
