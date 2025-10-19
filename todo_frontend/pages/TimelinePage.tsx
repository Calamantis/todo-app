import { useEffect, useState } from "react";

interface TimelineEvent {
  activityId: number;
  title: string;
  startTime: string;
  endTime?: string;
  colorHex?: string;
  isRecurring: boolean;
}

export default function TimelinePage() {
  const [events, setEvents] = useState<TimelineEvent[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (!token) {
      window.location.href = "/";
      return;
    }

    fetch("/api/timeline?daysAhead=30", {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((res) => res.json())
      .then((data: TimelineEvent[]) => {
        setEvents(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setLoading(false);
      });
  }, []);

  if (loading)
    return (
      <div className="flex items-center justify-center h-screen bg-gray-900 text-white">
        <p>Loading timeline...</p>
      </div>
    );

  return (
    <div className="min-h-screen bg-gray-900 text-white p-6">
      <h1 className="text-3xl font-bold mb-6 text-center">My Timeline</h1>

      <div className="max-w-3xl mx-auto">
        {events.length === 0 ? (
          <p className="text-center text-gray-400">No activities found.</p>
        ) : (
          events
            .sort(
              (a, b) =>
                new Date(a.startTime).getTime() -
                new Date(b.startTime).getTime()
            )
            .map((e, idx) => (
              <div
                key={idx}
                className="flex items-start mb-6 border-l-4 border-blue-500 pl-4 relative"
              >
                <div
                  className="absolute left-[-0.6rem] top-2 w-3 h-3 rounded-full"
                  style={{ backgroundColor: e.colorHex || "#3b82f6" }}
                ></div>
                <div>
                  <h2 className="text-xl font-semibold">{e.title}</h2>
                  <p className="text-sm text-gray-300">
                    {new Date(e.startTime).toLocaleString()}{" "}
                    {e.endTime
                      ? "→ " + new Date(e.endTime).toLocaleTimeString()
                      : ""}
                  </p>
                </div>
              </div>
            ))
        )}
      </div>
    </div>
  );
}
