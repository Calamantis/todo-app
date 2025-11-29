import React, { useEffect, useState } from "react";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";
import Panel from "../components/social_components/Panel";
import OnlineActivityListItem from "../components/onlineActivity_components/OnlineActivityListItem";
import type { OnlineActivity } from "../components/onlineActivity_components/OnlineActivityListItem";
import { useAuth } from "../components/AuthContext";
import OnlineActivityInviteModal from "../components/onlineActivity_components/OnlineActivityInviteModal";
import SentInvitationsList from "../components/onlineActivity_components/SentInvitationsList";
import ParticipantsList from "../components/onlineActivity_components/ParticipantsList";
import ReceivedInvitationsList from "../components/onlineActivity_components/ReceivedInvitationsList";
import ParticipatingActivitiesList from "../components/onlineActivity_components/ParticipatingActivitiesList";
import JoinByCodePanel from "../components/onlineActivity_components/JoinByCodePanel";





const OnlineActivitiesPage: React.FC = () => {
  const { user } = useAuth();

  const [activities, setActivities] = useState<OnlineActivity[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [selectedActivity, setSelectedActivity] = useState<OnlineActivity | null>(null);
  const [inviteModalActivity, setInviteModalActivity] = useState<OnlineActivity | null>(null);


  // ===== FETCH =====
  const fetchActivities = async () => {
    if (!user) return;

    setLoading(true);
    setError("");

    try {
      const response = await fetch("/api/Activity/user/get-activities", {
        method: "GET",
        headers: { Authorization: `Bearer ${user.token}` },
      });

      if (!response.ok) throw new Error("Failed to load activities");

      const data = await response.json();
      setActivities(data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchActivities();
  }, []);

  // ===== FILTER ONLY ONLINE =====
  const onlineActivities = activities.filter(
    (a) => a.joinCode !== null || a.isFriendsOnly === true
  );

  return (
    <div>
      <NavigationWrapper />

      <div className="min-h-screen bg-[var(--background-color)] text-[var(--text-color)] p-4 md:p-6">
        <div className="max-w-9xl mx-auto">

          {/* TOP */}
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">

            {/* Panel 1 — ONLY Online Activities */}
            <Panel title="My Online Activities">
              {loading && <div>Loading...</div>}
              {error && <div className="text-red-500">{error}</div>}

              {onlineActivities.length === 0 && !loading ? (
                <div>No online activities found.</div>
              ) : (
                onlineActivities.map((a) => (
                  <OnlineActivityListItem
                    key={a.activityId}
                    activity={a}
                    onSelect={setSelectedActivity}
                    onInvite={(activity) => setInviteModalActivity(activity)}
                  />
                ))
              )}
            </Panel>

            {/* Panel 2 — Placeholder */}
            <Panel title="Online activities you participate in">
                <ParticipatingActivitiesList />
            </Panel>

            {/* Panel 3 — Participants placeholder */}
            <Panel title="Participants" small>
              <ParticipantsList activityId={selectedActivity?.activityId ?? null} />
            </Panel>

          </div>

          {/* BOTTOM PANELS */}
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 mt-4">

            <Panel title="Received Invitations" small>
              <ReceivedInvitationsList />
            </Panel>

          <Panel title="Sent Invitations" small>
            <SentInvitationsList activityId={selectedActivity?.activityId ?? null} />
          </Panel>

                        <Panel title="Join Activity via Code" small>
            <JoinByCodePanel onJoined={() => fetchActivities()} />
          </Panel>

          </div>


        </div>


                                {inviteModalActivity && (
    <OnlineActivityInviteModal
        activityId={inviteModalActivity.activityId}
        onClose={() => setInviteModalActivity(null)}
    />
    )}

      </div>
      <Footer />
    </div>
  );
};

export default OnlineActivitiesPage;
