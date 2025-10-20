// import { useEffect, useState } from "react";

// interface TimelineEvent {
//   activityId: number;
//   title: string;
//   startTime: string;
//   endTime?: string;
//   colorHex?: string;
//   isRecurring: boolean;
// }

// export default function TimelinePage() {
//   const [events, setEvents] = useState<TimelineEvent[]>([]);
//   const [loading, setLoading] = useState<boolean>(true);

//   useEffect(() => {
//     const token = sessionStorage.getItem("token");
//     if (!token) {
//       window.location.href = "/";
//       return;
//     }

//     fetch("/api/TimelineActivity/get-timeline", {
//       headers: { Authorization: `Bearer ${token}` },
//     })
//       .then((res) => res.json())
//       .then((data: TimelineEvent[]) => {
//         setEvents(data);
//         setLoading(false);
//       })
//       .catch((err) => {
//         console.error(err);
//         setLoading(false);
//       });
//   }, []);

//   if (loading)
//     return (
//       <div className="flex items-center justify-center h-screen bg-gray-900 text-white">
//         <p>Loading timeline...</p>
//       </div>
//     );

//   return (
//   <div className="min-h-screen bg-gray-900 text-white p-6">
//     <h1 className="text-3xl font-bold mb-6 text-center">My Weekly Timeline</h1>

//     {events.length === 0 ? (
//       <p className="text-center text-gray-400">No activities found.</p>
//     ) : (
//       <div className="overflow-x-auto">
//         {/* Główny wrapper z pozycjonowaniem */}
//         <div className="relative w-full border border-gray-700 rounded-lg overflow-hidden">
//           {/* Grid nagłówka dni */}
//           <div className="grid grid-cols-8 bg-gray-800 border-b border-gray-700">
//             <div className="p-2"></div>
//             {["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"].map((day) => (
//               <div
//                 key={day}
//                 className="p-2 text-center font-semibold border-l border-gray-700"
//               >
//                 {day}
//               </div>
//             ))}
//           </div>

//           {/* Główna siatka czasu */}
//           <div className="relative grid grid-cols-8" style={{ height: "960px" }}>
//             {/* Kolumna z godzinami */}
//             <div className="flex flex-col border-r border-gray-700 text-right text-gray-400 text-sm">
//               {Array.from({ length: 17 }).map((_, i) => {
//                 const hour = 6 + i;
//                 return (
//                   <div
//                     key={hour}
//                     className="h-[56px] border-t border-gray-800 pr-2 flex items-start"
//                   >
//                     {hour}:00
//                   </div>
//                 );
//               })}
//             </div>

//             {/* 7 kolumn dni */}
//             <div className="col-span-7 grid grid-cols-7 border-t border-gray-700 relative">
//               {/* Linie siatki */}
//               {Array.from({ length: 17 }).map((_, rowIdx) => (
//                 <div
//                   key={`row-${rowIdx}`}
//                   className="absolute w-full border-t border-gray-800"
//                   style={{ top: `${(rowIdx * 100) / 17}%` }}
//                 />
//               ))}

//               {/* Aktywności */}
//               {events.map((e, idx) => {
//                 const start = new Date(e.startTime);
//                 const end = e.endTime ? new Date(e.endTime) : null;
//                 const day = start.getDay() === 0 ? 6 : start.getDay() - 1; // Niedziela na końcu
//                 const startHour = start.getHours() + start.getMinutes() / 60;
//                 const endHour = end
//                   ? end.getHours() + end.getMinutes() / 60
//                   : startHour + 1;

//                 const top = ((startHour - 6) / 16) * 100; // % wysokości
//                 const height = ((endHour - startHour) / 16) * 100;
//                 const left = (day / 7) * 100;

//                 return (
//                   <div
//                     key={idx}
//                     className="absolute rounded-md text-xs text-center px-1 py-0.5 overflow-hidden shadow-md"
//                     style={{
//                       top: `${top}%`,
//                       left: `${left}%`,
//                       height: `${height}%`,
//                       width: `${100 / 7}%`,
//                       backgroundColor: e.colorHex || "#3b82f6",
//                     }}
//                     title={`${e.title} (${start.toLocaleTimeString()}${
//                       end ? " → " + end.toLocaleTimeString() : ""
//                     })`}
//                   >
//                     <div className="font-semibold truncate">{e.title}</div>
//                     <div className="text-[10px] opacity-80">
//                       {start.toLocaleTimeString([], {
//                         hour: "2-digit",
//                         minute: "2-digit",
//                       })}
//                     </div>
//                   </div>
//                 );
//               })}
//             </div>
//           </div>
//         </div>
//       </div>
//     )}
//   </div>
// );

// }
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
  const [currentWeekStart, setCurrentWeekStart] = useState<Date>(
    getStartOfWeek(new Date())
  );

  // 👉 helper do obliczenia poniedziałku danego tygodnia
  function getStartOfWeek(date: Date) {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - (day === 0 ? 6 : day - 1);
    return new Date(d.setDate(diff));
  }

  // 👉 kolejne / poprzednie tygodnie
  function changeWeek(offset: number) {
    const newDate = new Date(currentWeekStart);
    newDate.setDate(currentWeekStart.getDate() + offset * 7);
    setCurrentWeekStart(newDate);
  }

  // 👉 generuje dni tygodnia (Pon–Niedz)
  const daysOfWeek = Array.from({ length: 7 }).map((_, i) => {
    const d = new Date(currentWeekStart);
    d.setDate(currentWeekStart.getDate() + i);
    return d;
  });

  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (!token) {
      window.location.href = "/";
      return;
    }

    setLoading(true);
    fetch("/api/TimelineActivity/get-timeline", {
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
      <h1 className="text-3xl font-bold mb-2 text-center">My Weekly Timeline</h1>

      {/* 🔹 Nawigacja tygodni */}
      <div className="flex justify-between items-center max-w-4xl mx-auto mb-4">
        <button
          onClick={() => changeWeek(-1)}
          className="px-3 py-1 bg-gray-800 hover:bg-gray-700 rounded-md"
        >
          ← Previous
        </button>

        <div className="text-lg font-medium">
          {daysOfWeek[0].toLocaleDateString()} –{" "}
          {daysOfWeek[6].toLocaleDateString()}
        </div>

        <button
          onClick={() => changeWeek(1)}
          className="px-3 py-1 bg-gray-800 hover:bg-gray-700 rounded-md"
        >
          Next →
        </button>
      </div>

      {/* 🔹 Główny grid */}
      <div className="overflow-x-auto">
        <div className="relative w-full border border-gray-700 rounded-lg overflow-hidden">
          {/* Nagłówki dni + daty */}
          <div className="grid grid-cols-8 bg-gray-800 border-b border-gray-700">
            <div className="p-2"></div>
            {daysOfWeek.map((day, idx) => (
              <div
                key={idx}
                className="p-2 text-center font-semibold border-l border-gray-700"
              >
                {day.toLocaleDateString("en-US", {
                  weekday: "short",
                  month: "short",
                  day: "numeric",
                })}
              </div>
            ))}
          </div>

          {/* Siatka godzin */}
          <div className="relative grid grid-cols-8" style={{ height: "960px" }}>
            {/* Kolumna z godzinami */}
            <div className="flex flex-col border-r border-gray-700 text-right text-gray-400 text-sm">
              {Array.from({ length: 17 }).map((_, i) => {
                const hour = 6 + i;
                return (
                  <div
                    key={hour}
                    className="h-[56px] border-t border-gray-800 pr-2 flex items-start"
                  >
                    {hour}:00
                  </div>
                );
              })}
            </div>

            {/* Kolumny dni */}
            <div className="col-span-7 grid grid-cols-7 border-t border-gray-700 relative">
              {Array.from({ length: 17 }).map((_, rowIdx) => (
                <div
                  key={`row-${rowIdx}`}
                  className="absolute w-full border-t border-gray-800"
                  style={{ top: `${(rowIdx * 100) / 17}%` }}
                />
              ))}

              {/* Aktywności */}
              {events.map((e, idx) => {
                const start = new Date(e.startTime);
                const end = e.endTime ? new Date(e.endTime) : null;
                const day = start.getDay() === 0 ? 6 : start.getDay() - 1;
                const startHour = start.getHours() + start.getMinutes() / 60;
                const endHour = end
                  ? end.getHours() + end.getMinutes() / 60
                  : startHour + 1;

                const top = ((startHour - 6) / 16) * 100;
                const height = ((endHour - startHour) / 16) * 100;
                const left = (day / 7) * 100;

                return (
                  <div
                    key={idx}
                    className="absolute rounded-md text-xs text-center px-1 py-0.5 overflow-hidden shadow-md"
                    style={{
                      top: `${top}%`,
                      left: `${left}%`,
                      height: `${height}%`,
                      width: `${100 / 7}%`,
                      backgroundColor: e.colorHex || "#3b82f6",
                    }}
                    title={`${e.title} (${start.toLocaleTimeString()}${
                      end ? " → " + end.toLocaleTimeString() : ""
                    })`}
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
