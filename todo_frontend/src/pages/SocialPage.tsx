import React, { useEffect, useMemo, useState } from "react";
import { useAuth } from "../components/AuthContext";
import { Search, Users, Ban, Send, Inbox } from "lucide-react";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";

import Panel from "../components/social_components/Panel";
import EmptyState from "../components/social_components/EmptyState";
import UserListItem from "../components/social_components/UserListItem";



// ===== Unified preview type =====
type UserPreview = {
  userId: number;
  fullName: string;
  email?: string;
  profileImageUrl?: string;
  backgroundImageUrl?: string;
  synopsis?: string;
};

// ===== SocialPage =====
const SocialPage: React.FC = () => {
  const { user } = useAuth();

  const [friends, setFriends] = useState<UserPreview[]>([]);
  const [blocked, setBlocked] = useState<UserPreview[]>([]);
  const [sentInvites, setSentInvites] = useState<UserPreview[]>([]);
  const [receivedInvites, setReceivedInvites] = useState<UserPreview[]>([]);
  const [browseUsers, setBrowseUsers] = useState<UserPreview[]>([]);

  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const headers = useMemo(
    () => ({
      Authorization: `Bearer ${user?.token ?? ""}`,
      role: user?.role ?? "",
      userId: user?.userId?.toString?.() ?? "",
    }),
    [user]
  );

  // 🔧 FIX — Moved fetchAll OUTSIDE of useEffect
  const fetchAll = async () => {
    if (!user) return;

    setLoading(true);
    setError("");

    try {
      const [
        friendsRes,
        sentRes,
        receivedRes,
        browseRes,
        blockedRes,
      ] = await Promise.all([
        fetch("/api/UserFriendActions/me", { headers }),
        fetch("/api/UserFriendActions/sent-invites", { headers }),
        fetch("/api/UserFriendActions/recieved-invites", { headers }),
        fetch("/api/UserFriendActions/browse-users", { headers }),
        fetch("/api/BlockedUsers/blocked-users", { headers }),
      ]);

      if (!friendsRes.ok) throw new Error("Failed to load friends");
      if (!sentRes.ok) throw new Error("Failed to load sent invites");
      if (!receivedRes.ok) throw new Error("Failed to load received invites");
      if (!browseRes.ok) throw new Error("Failed to load users");
      if (!blockedRes.ok) throw new Error("Failed to load blocked users");

      const friendsData: UserPreview[] = await friendsRes.json();
      const sentData: UserPreview[] = await sentRes.json();
      const receivedData: UserPreview[] = await receivedRes.json();
      const browseData: UserPreview[] = await browseRes.json();
      const blockedData: UserPreview[] = await blockedRes.json();

      // common mapper for friend-like objects
      const mapToPreview = (raw: any): UserPreview => ({
        userId: raw.userId ?? raw.friendId ?? raw.blockedUserId,
        fullName:
          raw.friendFullName ??
          raw.fullName ??
          "",

        email:
          raw.friendEmail ??
          raw.email ??
          undefined,

        profileImageUrl:
          raw.friendImage ??
          raw.profileImageUrl ??
          undefined,

        backgroundImageUrl:
          raw.friendBackground ??
          raw.backgroundImageUrl ??
          undefined,

        synopsis: raw.synopsis ?? undefined,
      });




      setFriends(friendsData.map(mapToPreview));
      setSentInvites(sentData.map(mapToPreview));
      setReceivedInvites(receivedData.map(mapToPreview));
      setBrowseUsers(
        browseData.map((u) => ({
          userId: u.userId,
          fullName: u.fullName,
          email: u.email,
          profileImageUrl: u.profileImageUrl,
          backgroundImageUrl: u.backgroundImageUrl,
          synopsis: u.synopsis,
        }))
      );
      setBlocked(blockedData.map(mapToPreview)); // 🔧 FIX mapped same way
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Unexpected error");
    } finally {
      setLoading(false);
    }
  };





  
  // 🔧 FIX — only ONE useEffect
  useEffect(() => {
    fetchAll();
  }, [user]);

  // ---------- refresh function passed to children ----------
  const refresh = () => fetchAll(); // 🔧 FIX working refresh

  // filtered search
  const filteredBrowse = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return browseUsers;

    return browseUsers.filter(
      (u) =>
        u.fullName.toLowerCase().includes(q) ||
        (u.email?.toLowerCase().includes(q) ?? false)
    );
  }, [search, browseUsers]);

  // loading screen
  if (loading) {
    return (
      <div className="min-h-screen bg-[var(--background-color)] text-[var(--text-color)] flex items-center justify-center">
        Loading social…
      </div>
    );
  }

  return (
    <div>
      <NavigationWrapper />
      <div className="bg-[var(--background-color)] text-[var(--text-color)] p-4 md:p-6">
        <div className="max-w-9xl mx-auto">
          {error && <p className="text-red-400 mb-4">{error}</p>}

          <div className="min-h-[80vh] grid grid-cols-1 lg:grid-cols-4 gap-4">
            {/* FRIENDS */}
            <Panel title="Friends" icon={<Users size={18} />}>
              {friends.length === 0 ? (
                <EmptyState text="No friends yet." />
              ) : (
                friends.map((f) => (
                  <UserListItem
                    key={f.email}
                    user={f}
                    variant="friend"
                    onAction={refresh}
                  />
                ))
              )}
            </Panel>

            {/* BLOCKED */}
            <Panel title="Blocked users" icon={<Ban size={18} />}>
              {blocked.length === 0 ? (
                <EmptyState text="No blocked users." />
              ) : (
                blocked.map((b) => (
                  <UserListItem
                    key={b.email}
                    user={b}
                    variant="blocked" // 🔧 FIX unified variant
                    onAction={refresh}
                  />
                ))
              )}
            </Panel>

            {/* BROWSE USERS */}
            <Panel title="Find users" icon={<Search size={18} />}>
              <div className="top-0 bg-[var(--primary-color)] z-10">
                <div className="flex items-center gap-2 bg-white/5 border border-white/10 rounded-lg px-3 py-2">
                  <input
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    placeholder="Search by name or email"
                    className="bg-transparent outline-none flex-1 text-sm"
                  />
                </div>
              </div>

              {filteredBrowse.length === 0 ? (
                <EmptyState text="No users found." />
              ) : (
                <div className="space-y-2 pt-3">
                  {filteredBrowse.map((u) => (
                    <UserListItem
                      key={u.email}
                      user={u}
                      variant="browse"
                      onAction={refresh}
                    />
                  ))}
                </div>
              )}
            </Panel>

            {/* RIGHT COLUMN: INVITES */}
            <div className="flex flex-col gap-4 min-h-[80vh]">
              <Panel title="Sent invites" icon={<Send size={18} />} small>
                {sentInvites.length === 0 ? (
                  <EmptyState text="No sent invites." />
                ) : (
                  sentInvites.map((u) => (
                    <UserListItem
                      key={u.email}
                      user={u}
                      variant="sent"
                      compact
                      onAction={refresh}
                    />
                  ))
                )}
              </Panel>

              <Panel title="Received invites" icon={<Inbox size={18} />} small>
                {receivedInvites.length === 0 ? (
                  <EmptyState text="No received invites." />
                ) : (
                  receivedInvites.map((u) => (
                    <UserListItem
                      key={u.email}
                      user={u}
                      variant="received"
                      compact
                      onAction={refresh}
                    />
                  ))
                )}
              </Panel>
            </div>
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default SocialPage;
