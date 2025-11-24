export const socialApi = {
  sendInvite: async (friendId: number, token: string) => {
    const res = await fetch(`/api/UserFriendActions/sent-invite`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ friendId }),
    });

    if (!res.ok) throw new Error("Failed to send invite");
    return res;
  },

  cancelInvite: async (friendId: number, token: string) => {
    const res = await fetch(
      `/api/UserFriendActions/cancel-send-invite?friendId=${friendId}`,
      {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      }
    );
    if (!res.ok) throw new Error("Failed to cancel invite");
    return res;
  },

  acceptInvite: async (friendId: number, token: string) => {
    const res = await fetch(
      `/api/UserFriendActions/accept-invite?friendId=${friendId}`,
      {
        method: "PATCH",
        headers: { Authorization: `Bearer ${token}` },
      }
    );
    if (!res.ok) throw new Error("Failed to accept invite");
    return res;
  },

  rejectInvite: async (requesterId: number, token: string) => {
    const res = await fetch(
      `/api/UserFriendActions/reject-invite?requesterId=${requesterId}`,
      {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      }
    );
    if (!res.ok) throw new Error("Failed to reject invite");
    return res;
  },

  removeFriend: async (friendId: number, token: string) => {
    const res = await fetch(
      `/api/UserFriendActions/remove-friend?friendId=${friendId}`,
      {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      }
    );
    if (!res.ok) throw new Error("Failed to remove friend");
    return res;
  },

  blockUser: async (targetUserId: number, token: string) => {
    const res = await fetch(`/api/BlockedUsers/${targetUserId}`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) throw new Error("Failed to block user");
    return res;
  },

  unblockUser: async (targetUserId: number, token: string) => {
    const res = await fetch(`/api/BlockedUsers/${targetUserId}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!res.ok) throw new Error("Failed to unblock user");
    return res;
  },
};
