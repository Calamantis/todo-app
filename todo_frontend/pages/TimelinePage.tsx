import { useEffect, useMemo, useState } from "react";

interface TimelineEvent {
  activityId: number;
  title: string;
  startTime: string;
  endTime?: string;
  colorHex?: string;
  isRecurring: boolean;
  plannedDurationMinutes?: number;
}

export default function TimelinePage() {
  const [events, setEvents] = useState<TimelineEvent[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentWeekStart, setCurrentWeekStart] = useState(getStartOfWeek(new Date()));

  function getStartOfWeek(date: Date) {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - (day === 0 ? 6 : day - 1);
    return new Date(d.setDate(diff));
  }

  function changeWeek(offset: number) {
    const newDate = new Date(currentWeekStart);
    newDate.setDate(currentWeekStart.getDate() + offset * 7);
    setCurrentWeekStart(newDate);
  }

  const daysOfWeek = useMemo(() => {
    return Array.from({ length: 7 }).map((_, i) => {
      const d = new Date(currentWeekStart);
      d.setDate(currentWeekStart.getDate() + i);
      return d;
    });
  }, [currentWeekStart]);

useEffect(() => {
  const token = sessionStorage.getItem("token");
  if (!token) {
    window.location.href = "/";
    return;
  }

  setLoading(true);
  setEvents([]); // 🔹 wyczyść listę przed nowym requestem

  const from = currentWeekStart.toISOString();
  const to = new Date(currentWeekStart.getTime() + 6 * 24 * 60 * 60 * 1000).toISOString();

  fetch(`/api/TimelineActivity/get-timeline?from=${from}&to=${to}`, {
    headers: { Authorization: `Bearer ${token}` },
  })
    .then((res) => res.json())
    .then((data: TimelineEvent[]) => {
      setEvents(data); // 🔹 nadpisz, nie doklejaj
      setLoading(false);
    })
    .catch((err) => {
      console.error(err);
      setLoading(false);
    });
}, [currentWeekStart]);


  const startOfWeek = daysOfWeek[0];
  const startOfWeekMidnight = new Date(startOfWeek);
startOfWeekMidnight.setHours(0, 0, 0, 0);
const endOfWeekMidnight = new Date(startOfWeekMidnight);
endOfWeekMidnight.setDate(startOfWeekMidnight.getDate() + 7);

const weekEvents = events.filter((e) => {
  const start = new Date(e.startTime);
  return start >= startOfWeekMidnight && start < endOfWeekMidnight;
});

  // 🔹 ustawienia osi czasu
  const HOURS_START = 6;
  const HOURS_END = 22;
  const PIXELS_PER_HOUR = 100; // 1h = 100px
  const PIXELS_PER_MINUTE = PIXELS_PER_HOUR / 60;
  const TOTAL_HOURS = HOURS_END - HOURS_START;
  const MIN_HEIGHT = 40;

  return (
    <div className="min-h-screen bg-gray-900 text-white p-6">
      <h1 className="text-3xl font-bold mb-4 text-center">My Weekly Timeline</h1>

      {/* 🔹 Nawigacja tygodni */}
      <div className="flex justify-between items-center max-w-4xl mx-auto mb-4">
        <button
          onClick={() => changeWeek(-1)}
          className="px-4 py-2 bg-gray-800 hover:bg-gray-700 rounded-md transition"
        >
          ← Previous
        </button>

        <div className="text-lg font-medium">
          {daysOfWeek[0].toLocaleDateString("pl-PL")} –{" "}
          {daysOfWeek[6].toLocaleDateString("pl-PL")}
        </div>

        <button
          onClick={() => changeWeek(1)}
          className="px-4 py-2 bg-gray-800 hover:bg-gray-700 rounded-md transition"
        >
          Next →
        </button>
      </div>

      {/* 🔹 Główny grid */}
      <div className="overflow-x-auto overflow-y-auto max-h-[80vh] rounded-lg border border-gray-700 bg-gray-950/40">
        <div className="relative w-full" style={{ height: `${TOTAL_HOURS * PIXELS_PER_HOUR}px` }}>
          {/* Nagłówki dni */}
          <div className="grid grid-cols-8 bg-gray-800 border-b border-gray-700 sticky top-0 z-10">
            <div className="p-2"></div>
            {daysOfWeek.map((day, idx) => (
              <div
                key={idx}
                className="p-2 text-center font-semibold border-l border-gray-700"
              >
                {day.toLocaleDateString("pl-PL", {
                  weekday: "short",
                  month: "short",
                  day: "numeric",
                })}
              </div>
            ))}
          </div>

          {/* Siatka godzin */}
          <div className="grid grid-cols-8 relative w-full">
            {/* Kolumna z godzinami */}
            <div className="flex flex-col border-r border-gray-700 text-right text-gray-400 text-xs sm:text-sm">
              {Array.from({ length: TOTAL_HOURS + 1 }).map((_, i) => (
                <div
                  key={i}
                  className="border-t border-gray-800 pr-2"
                  style={{ height: `${PIXELS_PER_HOUR}px` }}
                >
                  {HOURS_START + i}:00
                </div>
              ))}
            </div>

            {/* Kolumny dni */}
            <div className="col-span-7 grid grid-cols-7 border-t border-gray-700 relative">
              {/* Linie godzinowe */}
              {Array.from({ length: TOTAL_HOURS + 1 }).map((_, i) => (
                <div
                  key={`row-${i}`}
                  className="absolute w-full border-t border-gray-800"
                  style={{ top: `${i * PIXELS_PER_HOUR}px` }}
                />
              ))}

              {/* Aktywności */}






{weekEvents.map((e, idx) => {
  const start = new Date(e.startTime);
  const durationMinutes = e.plannedDurationMinutes ?? 60;

  // ✅ end zawsze liczymy jako start + plannedDurationMinutes
  const end = new Date(start.getTime() + durationMinutes * 60 * 1000);

  const dayOfWeek = start.getDay() === 0 ? 6 : start.getDay() - 1;
  const startMinutes = start.getHours() * 60 + start.getMinutes();
  const top = (startMinutes - HOURS_START * 60) * PIXELS_PER_MINUTE;

  let height = durationMinutes * PIXELS_PER_MINUTE;
  if (height < MIN_HEIGHT) height = MIN_HEIGHT;

  const left = (dayOfWeek / 7) * 100;

  // Ukryj aktywności spoza widocznego zakresu (opcjonalne)
  if (start.getHours() < HOURS_START || start.getHours() >= HOURS_END) return null;

  return (
    <div
      key={idx}
      className="absolute rounded-md text-xs sm:text-sm text-center px-2 py-1 overflow-hidden shadow-md border border-gray-700"
      style={{
        top: `${top}px`,
        left: `${left}%`,
        height: `${height}px`,
        width: `${100 / 7}%`,
        backgroundColor: e.colorHex || "#3b82f6",
      }}
      title={`${e.title} (${start.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit',
      })} → ${end.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit',
      })})`}
    >
      <div className="font-semibold truncate">{e.title}</div>
      <div className="text-[10px] opacity-80">
        {start.toLocaleTimeString([], {
          hour: '2-digit',
          minute: '2-digit',
        })}
      </div>
    </div>
  );
})}



            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
