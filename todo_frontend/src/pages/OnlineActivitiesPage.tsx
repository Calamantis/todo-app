import React, { useEffect, useState } from "react";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";
import Panel from "../components/social_components/Panel";
import OnlineActivityListItem from "../components/onlineActivity_components/OnlineActivityListItem";
import type { OnlineActivity } from "../components/onlineActivity_components/OnlineActivityListItem";
import { useAuth } from "../components/AuthContext";
import OnlineActivityInviteModal from "../components/onlineActivity_components/OnlineActivityInviteModal";

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
            <Panel title="(Reserved)">
              <div>This panel is reserved for future features.</div>
            </Panel>

            {/* Panel 3 — Participants placeholder */}
            <Panel title="Participants" small>
              {!selectedActivity ? (
                <div>Select an activity to view participants.</div>
              ) : (
                <div>
                  <div className="font-semibold mb-2">
                    Activity: {selectedActivity.title}
                  </div>
                  (Participants will be fetched here)
                </div>
              )}
            </Panel>

          </div>

          {/* BOTTOM PANELS */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 mt-4">

            <Panel title="Received Invitations" small>
              <div>(Future feature)</div>
            </Panel>

            <Panel title="Sent Invitations" small>
              <div>(Future feature)</div>
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
