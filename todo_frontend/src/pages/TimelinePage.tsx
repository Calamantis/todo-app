import React, { useState, useEffect, useMemo } from "react";
import { useAuth } from "../components/AuthContext";
import { ArrowLeft, ArrowRight } from "lucide-react";

// Helper: Oblicz daty dla danego tygodnia (od niedzieli do niedzieli)
const getWeekDates = (date: Date) => {
  const startOfWeek = new Date(date);
  const endOfWeek = new Date(date);
  startOfWeek.setDate(startOfWeek.getDate() - startOfWeek.getDay()); // ustawienie na niedzielę
  endOfWeek.setDate(endOfWeek.getDate() + 6 - endOfWeek.getDay()); // ustawienie na sobotę
  startOfWeek.setHours(0, 0, 0, 0);
  endOfWeek.setHours(23, 59, 59, 999);
  return { startOfWeek, endOfWeek };
};

const TimelinePage: React.FC = () => {
  const { user } = useAuth();
  const [activities, setActivities] = useState<any[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string>("");
  const [currentWeekStart, setCurrentWeekStart] = useState<Date>(new Date());

  // Ustawienia osi czasu
  const HOURS_START = 0;
  const HOURS_END = 24;
  const PIXELS_PER_HOUR = 100;
  const PIXELS_PER_MINUTE = PIXELS_PER_HOUR / 60;
  const TOTAL_HOURS = HOURS_END - HOURS_START;
  const MIN_HEIGHT = 40;

  // Wybór daty: początek i koniec tygodnia
  const { startOfWeek, endOfWeek } = useMemo(() => getWeekDates(currentWeekStart), [currentWeekStart]);

  useEffect(() => {
    if (!user) return;

    const fetchTimeline = async () => {
      try {
        setLoading(true);
        const res = await fetch(
          `/api/Timeline/user/get-timeline?userId=${user.userId}&from=${startOfWeek.toISOString()}&to=${endOfWeek.toISOString()}`,
          {
            method: "GET",
            headers: {
              Authorization: `Bearer ${user.token}`,
            },
          }
        );

        if (!res.ok) {
          throw new Error("Failed to fetch activities");
        }

        const data = await res.json();
        setActivities(data);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchTimeline();
  }, [user, startOfWeek, endOfWeek]);

  const changeWeek = (offset: number) => {
    const newDate = new Date(currentWeekStart);
    newDate.setDate(currentWeekStart.getDate() + offset * 7); // Zmiana tygodnia o 7 dni
    setCurrentWeekStart(newDate);
  };

  // Generowanie aktywności w osi czasu
//   const renderTimeline = () => {
//   return activities.map((activity, index) => {
//     const { occurrenceDate, startTime, endTime, categoryName, categoryColorHex } = activity;

//     // Parsowanie daty i godziny
//     const start = new Date(occurrenceDate);
//     const [startHours, startMinutes] = startTime.split(":").map(Number);
//     start.setHours(startHours, startMinutes);

//     // Obliczanie czasu trwania w minutach
//     const duration = (new Date(endTime).getTime() - start.getTime()) / (1000 * 60);

//     // Pozycja na osi czasu (od góry, w zależności od godziny)
//     const topPosition = (start.getHours() * 60 + start.getMinutes()) * PIXELS_PER_MINUTE;

//     // Pozycja na osi dni (od lewej do prawej, 100% szerokości podzielone przez 7 dni)
//     const leftPosition = (start.getDay() * 100) / 7; // Procentowa pozycja na osi dni tygodnia

//     // Zwracamy blok aktywności
//     return (
//       <div
//         key={index}
//         className="absolute rounded-md"
//         style={{
//           top: `${topPosition}px`, // Wysokość na osi czasu
//           left: `${leftPosition}%`, // Szerokość na osi dni tygodnia
//           height: `${duration * PIXELS_PER_MINUTE}px`, // Wysokość aktywności zależna od jej trwania
//           backgroundColor: categoryColorHex || "#3490dc", // Kolor tła aktywności
//         }}
//       >
//         <div className="text-white text-xs sm:text-sm truncate">{categoryName || "Unknown Activity"}</div>
//       </div>
//     );
//   });
// };


const renderTimeline = () => {
  return activities.map((e, idx) => {
    const { occurrenceDate, startTime, endTime, categoryName, categoryColorHex } = e;

    // Parsowanie daty i godziny
    const start = new Date(occurrenceDate);
    const [startHours, startMinutes] = startTime.split(":").map(Number);
    start.setHours(startHours, startMinutes);

    // Obliczanie czasu trwania w minutach
    const duration = (new Date(endTime).getTime() - start.getTime()) / (1000 * 60);

    // Pozycjonowanie na osi czasu - top = minuta w ciągu dnia (od godziny 6:00)
    const top =
      (start.getHours() * 60 + start.getMinutes()) * PIXELS_PER_MINUTE;

    // Pozycja na osi dni (lewa pozycja) - (dzięki temu aktywność jest przypisana do odpowiedniego dnia tygodnia)
    const left = (start.getDay() / 7) * 100; // Podzielamy przez 7, aby uzyskać 1/7 szerokości na każdy dzień

    // Ustawiamy wysokość aktywności na osi czasu na podstawie jej długości
    let height = duration * PIXELS_PER_MINUTE;
    if (height < MIN_HEIGHT) height = MIN_HEIGHT; // Jeśli aktywność jest krótsza niż minimalna wysokość, ustawiamy ją na MIN_HEIGHT

    // Jeśli aktywność wychodzi poza zakres godzin (przed 6:00 lub po 22:00), nie renderujemy jej
    
    return (
      <div
        key={idx}
        className="absolute rounded-md text-xs sm:text-sm text-center px-2 py-1 overflow-hidden shadow-md border border-gray-700"
        style={{
          top: `${top}px`, // Pozycja aktywności na osi czasu (pionowo)
          left: `${left}%`, // Pozycja aktywności na osi dni tygodnia (poziomo)
          height: `${height}px`, // Wysokość aktywności zależna od jej trwania
          width: `${100 / 7}%`, // Szerokość aktywności na każdy dzień
          backgroundColor: categoryColorHex || "#3b82f6", // Kolor aktywności
        }}
        title={`${categoryName} (${start.toLocaleTimeString([], {
          hour: "2-digit",
          minute: "2-digit",
        })} → ${new Date(endTime).toLocaleTimeString([], {
          hour: "2-digit",
          minute: "2-digit",
        })})`} // Tooltip pokazujący godzinę rozpoczęcia i zakończenia
      >
        <div className="font-semibold truncate">{categoryName || "Unknown Activity"}</div>
        <div className="text-[10px] opacity-80">
          {start.toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
          })}
        </div>
      </div>
    );
  });
};



  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen bg-gray-900 text-white">
        <p>Loading timeline...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-900 text-white p-6">
      <div className="flex justify-between items-center mb-6">
        <button
          onClick={() => changeWeek(-1)}
          className="px-4 py-2 bg-gray-800 hover:bg-gray-700 rounded-md transition"
        >
          <ArrowLeft size={18} />
        </button>

        <div className="text-lg font-medium">
          {startOfWeek.toLocaleDateString()} - {endOfWeek.toLocaleDateString()}
        </div>

        <button
          onClick={() => changeWeek(1)}
          className="px-4 py-2 bg-gray-800 hover:bg-gray-700 rounded-md transition"
        >
          <ArrowRight size={18} />
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
            {Array.from({ length: 7 }).map((_, index) => (
              <div key={index} className="p-2 text-center font-semibold border-l border-gray-700">
                {["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"][index]}
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
              {renderTimeline()}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TimelinePage;
