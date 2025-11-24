import { useEffect, useMemo, useState } from "react";

interface TimelineEvent {
  activityId: number;
  title: string;
  startTime: string; // pełny ISO DateTime
  endTime?: string;
  colorHex?: string;
  isRecurring: boolean;
  plannedDurationMinutes?: number;
}

// 🔹 helper: wyciągnięcie userId z JWT
function getUserIdFromToken(token: string): number | null {
  try {
    const [, payload] = token.split(".");
    const json = JSON.parse(atob(payload));
    const nameId =
      json["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ??
      json["nameid"] ??
      json["sub"];

    if (!nameId) return null;
    return parseInt(nameId, 10);
  } catch {
    return null;
  }
}

export default function TimelinePage() {
  const [events, setEvents] = useState<TimelineEvent[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentWeekStart, setCurrentWeekStart] = useState(getStartOfWeek(new Date()));

  // 🔹 zawsze zaczynamy tydzień od PONIEDZIAŁKU 00:00
  function getStartOfWeek(date: Date) {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - (day === 0 ? 6 : day - 1); // poniedziałek jako start
    const monday = new Date(d.setDate(diff));
    monday.setHours(0, 0, 0, 0); // północ
    return monday;
  }

  function changeWeek(offset: number) {
    const newDate = new Date(currentWeekStart);
    newDate.setDate(currentWeekStart.getDate() + offset * 7);
    setCurrentWeekStart(getStartOfWeek(newDate));
  }

  // 🔹 generuj dni tygodnia od poniedziałku do niedzieli
  const daysOfWeek = useMemo(() => {
    return Array.from({ length: 7 }).map((_, i) => {
      const d = new Date(currentWeekStart);
      d.setDate(currentWeekStart.getDate() + i);
      return d;
    });
  }, [currentWeekStart]);

  // 🔹 pobieranie danych — od poniedziałku 00:00 do niedzieli 23:59:59
  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (!token) {
      window.location.href = "/";
      return;
    }

    const userId = getUserIdFromToken(token);
    if (!userId) {
      console.error("Nie udało się odczytać userId z tokena.");
      window.location.href = "/";
      return;
    }

    setLoading(true);
    setEvents([]);

    const from = new Date(currentWeekStart);
    from.setHours(0, 0, 0, 0);
    const to = new Date(from);
    to.setDate(from.getDate() + 7);
    to.setHours(23, 59, 59, 999);

    fetch(
      `/api/Timeline/user/get-timeline?userId=${userId}&from=${from.toISOString()}&to=${to.toISOString()}`,
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    )
      .then((res) => {
        if (!res.ok) {
          throw new Error(`HTTP ${res.status}`);
        }
        return res.json();
      })
      .then((data: any[]) => {
        // data = ActivityInstanceDto:
        // { instanceId, occurrenceDate, startTime, endTime, durationMinutes, ... }
        const mapped: TimelineEvent[] = data.map((d) => {
          // occurrenceDate: "2025-11-10T00:00:00"
          // startTime: "20:00:00"
          const base = new Date(d.occurrenceDate);
          const [hh, mm, ss] = (d.startTime as string).split(":").map(Number);
          base.setHours(hh ?? 0, mm ?? 0, ss ?? 0, 0);

          // jeśli chcesz użyć prawdziwego endTime z API, możesz podobnie złożyć z d.endTime
          return {
            activityId: d.activityId,
            // możesz tu później podmienić na d.title, jeśli backend zacznie go zwracać
            title: `Aktywność #${d.activityId}`,
            startTime: base.toISOString(),
            plannedDurationMinutes: d.durationMinutes,
            colorHex: undefined,
            isRecurring: d.recurrenceRuleId != null,
          };
        });

        setEvents(mapped);
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setLoading(false);
      });
  }, [currentWeekStart]);

  // 🔹 lokalne przeliczenie czasu – teraz BEZ kombinowania z offsetem
  function toLocal(dateStr: string) {
    return new Date(dateStr);
  }

  // 🔹 filtr aktywności w obrębie bieżącego tygodnia
  const weekEvents = events.filter((e) => {
    const start = toLocal(e.startTime);
    return (
      start >= currentWeekStart &&
      start <
        new Date(currentWeekStart.getTime() + 7 * 24 * 60 * 60 * 1000)
    );
  });

  // 🔹 ustawienia osi czasu
  const HOURS_START = 6;
  const HOURS_END = 22;
  const PIXELS_PER_HOUR = 100;
  const PIXELS_PER_MINUTE = PIXELS_PER_HOUR / 60;
  const TOTAL_HOURS = HOURS_END - HOURS_START;
  const MIN_HEIGHT = 40;

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen bg-gray-900 text-white">
        <p>Loading timeline...</p>
      </div>
    );
  }

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
        <div
          className="relative w-full"
          style={{ height: `${TOTAL_HOURS * PIXELS_PER_HOUR}px` }}
        >
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
              {Array.from({ length: TOTAL_HOURS + 1 }).map((_, i) => (
                <div
                  key={`row-${i}`}
                  className="absolute w-full border-t border-gray-800"
                  style={{ top: `${i * PIXELS_PER_HOUR}px` }}
                />
              ))}

              {/* 🔹 Aktywności */}
              {weekEvents.map((e, idx) => {
                const start = toLocal(e.startTime);
                const durationMinutes = e.plannedDurationMinutes ?? 60;
                const end = new Date(
                  start.getTime() + durationMinutes * 60 * 1000
                );

                const dayOfWeek = start.getDay() === 0 ? 6 : start.getDay() - 1;
                const startMinutes = start.getHours() * 60 + start.getMinutes();
                const top =
                  (startMinutes - HOURS_START * 60) * PIXELS_PER_MINUTE;
                const left = (dayOfWeek / 7) * 100;

                let height = durationMinutes * PIXELS_PER_MINUTE;
                if (height < MIN_HEIGHT) height = MIN_HEIGHT;

                if (start.getHours() < HOURS_START || start.getHours() >= HOURS_END)
                  return null;

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
                      hour: "2-digit",
                      minute: "2-digit",
                    })} → ${end.toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })})`}
                  >
                    <div className="font-semibold truncate">{e.title}</div>
                    <div className="text-[10px] opacity-80">
                      {start.toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
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
