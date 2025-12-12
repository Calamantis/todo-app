import React, { useState, useEffect, useMemo } from "react";
import { ArrowLeft, ArrowRight } from "lucide-react";
import { useAuth } from "../AuthContext";
import ActivityBlock from "./ActivityBlock";

const getWeekDates = (date: Date) => {
  const startOfWeek = new Date(date);
  const endOfWeek = new Date(date);

  const day = startOfWeek.getDay();
  const diff = day === 0 ? 6 : day - 1;
  startOfWeek.setDate(startOfWeek.getDate() - diff);
  endOfWeek.setDate(startOfWeek.getDate() + 6);

  startOfWeek.setHours(1, 0, 0, 0);
  endOfWeek.setHours(1, 0, 0, 0);

  return { startOfWeek, endOfWeek };
};

const TimelineCalendar: React.FC = () => {
  const { user } = useAuth();

  const [currentWeekStart, setCurrentWeekStart] = useState(new Date());
  const [activities, setActivities] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  const { startOfWeek, endOfWeek } = useMemo(
    () => getWeekDates(currentWeekStart),
    [currentWeekStart]
  );

  useEffect(() => {
    if (!user) return;

    const fetchTimeline = async () => {
      setLoading(true);

      const res = await fetch(
        `/api/Timeline/user/get-timeline?userId=${user.userId}&from=${startOfWeek.toISOString()}&to=${endOfWeek.toISOString()}`,
        { headers: { Authorization: `Bearer ${user.token}` } }
      );

      const data = await res.json();
      setActivities(data);
      setLoading(false);
    };

    fetchTimeline();
  }, [user, startOfWeek, endOfWeek]);

  const changeWeek = (offset: number) => {
    setCurrentWeekStart(prev => {
      const d = new Date(prev);
      d.setDate(d.getDate() + offset * 7);
      return d;
    });
  };

  if (loading) {
    return <div className="p-6">Loading timeline…</div>;
  }

  return (
    <>
      {/* HEADER */}
        <div className="flex justify-between items-center mb-6">
            <button
              onClick={() => changeWeek(-1)}
              className="px-4 py-2 bg-surface-1 hover:bg-surface-2 rounded-md transition"
            >
              <ArrowLeft size={18} />
            </button>

            <div className="text-lg font-medium">
              {startOfWeek.toLocaleDateString()} - {endOfWeek.toLocaleDateString()}
            </div>

            <button
              onClick={() => changeWeek(1)}
              className="px-4 py-2 bg-surface-1 hover:bg-surface-2 rounded-md transition"
            >
              <ArrowRight size={18} />
            </button>
        </div>

      {/* GRID */}
      <div className="overflow-x-auto overflow-y-auto max-h-[80vh] rounded-lg custom-scrollbar">
            <div className="relative w-full">
              <div className="grid grid-cols-8 bg-surface-1 sticky top-0 z-10">
                <div className="p-2"></div>
                {Array.from({ length: 7 }).map((_, index) => {
                  const day = new Date(startOfWeek);
                  day.setDate(startOfWeek.getDate() + index);
                  return (
                    <div key={index} className="p-2 text-center font-semibold border-l border-surface-3">
                      {["Mon, ", "Tue, ", "Wed, ", "Thu, ", "Fri, ", "Sat, ", "Sun, "][index]}{" "}
                      {day.toLocaleDateString("en-EN", { month: "short", day: "numeric" })}
                    </div>
                  );
                })}
              </div>

              <div>
                {/* Siatka godzin */}
                <div className="grid grid-cols-8 relative w-full bg-surface-1">
                  <div className="flex flex-col border-r border-surface-3 text-right text-text-0 text-xs sm:text-sm">
                    {Array.from({ length: 24 }).map((_, i) => (
                      <div key={i} className="border-t border-surface-3 pr-2" style={{ height: "60px" }}>
                        {i}:00
                      </div>
                    ))}
                  </div>

                  {/* Kolumny dni */}
                  <div className="col-span-7 grid grid-cols-7 relative">
                    {Array.from({ length: 24 }).map((_, i) => (
                      <div
                        key={`row-${i}`}
                        className="absolute w-full border-t border-surface-3"
                        style={{ top: `${i * 60}px` }}
                      />
                    ))}

                    {/* 🔹 Aktywności */}
                    {activities.map((activity, idx) => (
                      <ActivityBlock key={idx} activity={activity} />
                    ))}
                  </div>
                </div>
              </div>
            </div>
          </div>
    </>
  );
};

export default TimelineCalendar;
