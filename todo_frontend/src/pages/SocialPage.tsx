import React, { useEffect, useMemo, useState } from "react";
import { useAuth } from "../components/AuthContext";
import { Search, Users, Ban, Send, Inbox } from "lucide-react";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";

import Panel from '../components/social_components/Panel';
import EmptyState from "../components/social_components/EmptyState";
import UserListItem from "../components/social_components/UserListItem";
import BlockedListItem from "../components/social_components/BlockedListItem";

// ===== Types from backend =====
type FriendApi = {
  friendsSince: string;
  friendFullName: string;
  friendEmail: string;
  friendImage: string;
  friendBackground: string;
  synopsis: string;
};

type BrowseUserApi = {
  email: string;
  fullName: string;
  profileImageUrl: string;
  backgroundImageUrl: string;
  synopsis: string;
};

type BlockedApi = {
  blockedUserId: number;
  fullName: string;
  blockedAt: string;
};

// ===== Unified preview type used in UI =====
type UserPreview = {
  fullName: string;
  email?: string;
  profileImageUrl?: string;
  backgroundImageUrl?: string;
  synopsis?: string;
};

// ===== Page =====
const SocialPage: React.FC = () => {
  const { user } = useAuth();

  const [friends, setFriends] = useState<UserPreview[]>([]);
  const [blocked, setBlocked] = useState<BlockedApi[]>([]);
  const [sentInvites, setSentInvites] = useState<UserPreview[]>([]);
  const [receivedInvites, setReceivedInvites] = useState<UserPreview[]>([]);
  const [browseUsers, setBrowseUsers] = useState<UserPreview[]>([]);

  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const authHeaders = useMemo(() => ({
    Authorization: `Bearer ${user?.token ?? ""}`,
    role: user?.role ?? "",
    userId: user?.userId?.toString?.() ?? ""
  }), [user]);

  useEffect(() => {
    if (!user) return;

    const fetchAll = async () => {
      setLoading(true);
      setError("");

      try {
        const [
          friendsRes,
          sentRes,
          receivedRes,
          browseRes,
          blockedRes
        ] = await Promise.all([
          fetch("/api/UserFriendActions/me", { headers: authHeaders }),
          fetch("/api/UserFriendActions/sent-invites", { headers: authHeaders }),
          fetch("/api/UserFriendActions/recieved-invites", { headers: authHeaders }),
          fetch("/api/UserFriendActions/browse-users", { headers: authHeaders }),
          fetch("/api/BlockedUsers/blocked-users", { headers: authHeaders }),
        ]);

        if (!friendsRes.ok) throw new Error("Failed to load friends");
        if (!sentRes.ok) throw new Error("Failed to load sent invites");
        if (!receivedRes.ok) throw new Error("Failed to load received invites");
        if (!browseRes.ok) throw new Error("Failed to load users");
        if (!blockedRes.ok) throw new Error("Failed to load blocked users");

        const friendsData: FriendApi[] = await friendsRes.json();
        const sentData: FriendApi[] = await sentRes.json();
        const receivedData: FriendApi[] = await receivedRes.json();
        const browseData: BrowseUserApi[] = await browseRes.json();
        const blockedData: BlockedApi[] = await blockedRes.json();

        const mapFriend = (f: FriendApi): UserPreview => ({
          fullName: f.friendFullName,
          email: f.friendEmail,
          profileImageUrl: f.friendImage,
          backgroundImageUrl: f.friendBackground,
          synopsis: f.synopsis,
        });

        setFriends(friendsData.map(mapFriend));
        setSentInvites(sentData.map(mapFriend));
        setReceivedInvites(receivedData.map(mapFriend));
        setBrowseUsers(
          browseData.map(u => ({
            fullName: u.fullName,
            email: u.email,
            profileImageUrl: u.profileImageUrl,
            backgroundImageUrl: u.backgroundImageUrl,
            synopsis: u.synopsis
          }))
        );
        setBlocked(blockedData);
      } catch (e: unknown) {
        setError(e instanceof Error ? e.message : "Unexpected error");
      } finally {
        setLoading(false);
      }
    };

    fetchAll();
  }, [user, authHeaders]);

  const filteredBrowse = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return browseUsers;

    return browseUsers.filter(u =>
      u.fullName.toLowerCase().includes(q) ||
      (u.email?.toLowerCase().includes(q) ?? false)
    );
  }, [search, browseUsers]);

  if (loading) {
    return (
      <div className="min-h-screen bg-[var(--background-color)] text-[var(--text-color)] flex items-center justify-center">
        Loading social…
      </div>
    );
  }

  return (
    <div>
        <NavigationWrapper/>
    <div className=" bg-[var(--background-color)] text-[var(--text-color)] p-4 md:p-6">
      <div className="max-w-9xl mx-auto">

        {/* <h1 className="text-3xl font-semibold mb-6">Social</h1> */}
        {error && <p className="text-red-400 mb-4">{error}</p>}

        {/* ===== Desktop: 4 columns; last col split into two ===== */}
        <div className="min-h-[80vh] grid grid-cols-1 lg:grid-cols-4 gap-4">

          {/* 1) Friends */}
          <Panel title="Friends" icon={<Users size={18} />}>
            {friends.length === 0 ? (
              <EmptyState text="No friends yet." />
            ) : (
              <div className="space-y-2">
                {friends.map((f) => (
                  <UserListItem key={f.email ?? f.fullName} user={f} variant="friend" />
                ))}
              </div>
            )}
          </Panel>

          {/* 2) Blocked */}
          <Panel title="Blocked users" icon={<Ban size={18} />}>
            {blocked.length === 0 ? (
              <EmptyState text="No blocked users." />
            ) : (
              <div className="space-y-2">
                {blocked.map((b) => (
                  <BlockedListItem key={b.blockedUserId} fullName={b.fullName} blockedAt={b.blockedAt} />
                ))}
              </div>
            )}
          </Panel>

          {/* 3) Browse/Search */}
          <Panel title="Find users" icon={<Search size={18} />}>
            <div className="top-0 bg-[var(--primary-color)] z-10">
              <div className="flex items-center gap-2 bg-white/5 border border-white/10 rounded-lg px-3 py-2">
                <input
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  placeholder="Search by name or email…"
                  className="bg-transparent outline-none flex-1 text-sm"
                />
              </div>
            </div>

            {filteredBrowse.length === 0 ? (
              <EmptyState text="No users found." />
            ) : (
              <div className="space-y-2 pt-3">
                {filteredBrowse.map((u) => (
                  <UserListItem key={u.email ?? u.fullName} user={u} variant="browse" />
                ))}
              </div>
            )}
          </Panel>

          {/* 4) Right column split into 2 small panels */}
          <div className="flex flex-col gap-4 min-h-[80vh]">

            {/* 4) Sent invites */}
            <Panel title="Sent invites" icon={<Send size={18} />} small>
              {sentInvites.length === 0 ? (
                <EmptyState text="No sent invites." />
              ) : (
                <div className="space-y-2">
                  {sentInvites.map((u) => (
                    <UserListItem key={u.email ?? u.fullName} user={u} variant="sent" compact />
                  ))}
                </div>
              )}
            </Panel>

            {/* 5) Received invites */}
            <Panel title="Received invites" icon={<Inbox size={18} />} small>
              {receivedInvites.length === 0 ? (
                <EmptyState text="No received invites." />
              ) : (
                <div className="space-y-2">
                  {receivedInvites.map((u) => (
                    <UserListItem key={u.email ?? u.fullName} user={u} variant="received" compact />
                  ))}
                </div>
              )}
            </Panel>

          </div>
        </div>
      </div>
    </div>
    <Footer/>
    </div>
  );
};

export default SocialPage;