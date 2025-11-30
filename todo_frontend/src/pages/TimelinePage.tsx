import React, { useState, useEffect, useMemo } from "react";
import { useAuth } from "../components/AuthContext";
import { Activity, ArrowLeft, ArrowRight } from "lucide-react";
import ActivityBlock from "../components/timeline_components/ActivityBlock";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";

import ActivitySuggestionForm from "../components/activitySuggestion_components/ActivitySuggestionForm";
import ActivitySuggestionModal from "../components/activitySuggestion_components/ActivitySuggestionModal";

import PlacementForm from "../components/activitySuggestion_components/PlacementForm";
import ActivityPlacementModal from "../components/activitySuggestion_components/ActivityPlacementModal";

import ShiftedPlacementForm from "../components/activitySuggestion_components/ShiftedPlacementForm";
import ShiftedPlacementModal from "../components/activitySuggestion_components/ShiftedPlacementModal";

// Helper: Oblicz daty dla danego tygodnia (od poniedziałku do niedzieli)
const getWeekDates = (date: Date) => {
  const startOfWeek = new Date(date);
  const endOfWeek = new Date(date);

  // Poniedziałek jako początek tygodnia
  const day = startOfWeek.getDay();
  const diff = (day === 0 ? 6 : day-1); // Niedziela = 0, poniedziałek = 1
  startOfWeek.setDate(startOfWeek.getDate() - diff); // Zmieniamy na poniedziałek
  endOfWeek.setDate(startOfWeek.getDate() + 6); // Niedziela

  startOfWeek.setHours(1, 0, 0, 0);
  endOfWeek.setHours(1, 0, 0, 0);

  return { startOfWeek, endOfWeek };
};

const TimelinePage: React.FC = () => {
  const { user } = useAuth();
  const [activities, setActivities] = useState<any[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string>("");
  const [currentWeekStart, setCurrentWeekStart] = useState<Date>(new Date());

  const [suggestions, setSuggestions] = useState<any[]>([]);
  const [showSuggestionsModal, setShowSuggestionsModal] = useState(false);
  const [suggestionStart, setSuggestionStart] = useState("");
  const [suggestionEnd, setSuggestionEnd] = useState("");

  const [placementResults, setPlacementResults] = useState<any[]>([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [activityId, setActivityId] = useState<number | null>(null);
  const [isRecurring, setRecurring] = useState<boolean>(false);

  const [shiftedResults, setShiftedResults] = useState<any[]>([]);
  const [shiftedOpen, setShiftedOpen] = useState(false);
  const [shiftActivityId, setShiftActivityId] = useState<number | null>(null);
  const [shiftRecurring, setShiftRecurring] = useState(false);


  // Wybór daty: początek i koniec tygodnia
  const { startOfWeek, endOfWeek } = useMemo(() => getWeekDates(currentWeekStart), [currentWeekStart]);

  useEffect(() => {
    if (!user) return;

    const fetchTimeline = async () => {
      try {
        setLoading(true);
        setActivities([]);  // Czyszczenie poprzednich danych przed załadowaniem nowych

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
        setActivities(data);  // Załadowanie nowych danych do stanu
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchTimeline();
  }, [user, startOfWeek, endOfWeek]);  // Ponowne załadowanie danych przy zmianie tygodnia

  const changeWeek = (offset: number) => {
    const newDate = new Date(currentWeekStart);
    console.log(newDate);
    newDate.setDate(currentWeekStart.getDate() + offset * 7); // Zmiana tygodnia o 7 dni
    console.log(newDate);
    setCurrentWeekStart(newDate);
  };

const getWeekEnd = (weekStart: Date) => {
  const end = new Date(weekStart);
  end.setDate(end.getDate() + 6);
  return end;
};

const downloadWeekPdf = async () => {
  if (!user) return;

  const weekEnd = getWeekEnd(currentWeekStart);

  // tutaj currentWeekEnd to Date (np. niedziela)
  const weekEndIso = new Date(weekEnd)
    .toISOString()
    .substring(0, 10);

  const url = `/api/Timeline/user/week-pdf?userId=${user.userId}&date=${weekEndIso}`;

  const res = await fetch(url, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${user.token}`,
    },
  });

  if (!res.ok) {
    alert("Failed to download PDF.");
    return;
  }

  const blob = await res.blob();
  const fileURL = window.URL.createObjectURL(blob);

  const link = document.createElement("a");
  link.href = fileURL;
  link.download = `timeline_week_${weekEndIso}.pdf`;
  link.click();

  window.URL.revokeObjectURL(fileURL);
};





  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen bg-primary text-white">
        <p>Loading timeline...</p>
      </div>
    );
  }

  return (
    <div>
      <NavigationWrapper/>

        <div className="min-h-screen bg-[var(--background-color)] text-white p-6">

        <button
          onClick={downloadWeekPdf}
          className="px-4 py-2 rounded bg-accent text-black font-semibold hover:opacity-80 transition"
        >
          Print current week to PDF
        </button>

          <div className="flex justify-between items-center mb-6">
            <button
              onClick={() => changeWeek(-1)}
              className="px-4 py-2 bg-primary hover:bg-secondary rounded-md transition"
            >
              <ArrowLeft size={18} />
            </button>

            <div className="text-lg font-medium">
              {startOfWeek.toLocaleDateString()} - {endOfWeek.toLocaleDateString()}
            </div>

            <button
              onClick={() => changeWeek(1)}
              className="px-4 py-2 bg-primary hover:bg-secondary rounded-md transition"
            >
              <ArrowRight size={18} />
            </button>
          </div>

          <div className="overflow-x-auto overflow-y-auto max-h-[80vh] rounded-lg border border-[var(--background-color)] bg-primary custom-scrollbar">
            <div className="relative w-full">
              <div className="grid grid-cols-8 bg-primary border-b border-[var(--background-color)] sticky top-0 z-10">
                <div className="p-2"></div>
                {Array.from({ length: 7 }).map((_, index) => {
                  const day = new Date(startOfWeek);
                  day.setDate(startOfWeek.getDate() + index); // Wyznaczamy datę dla każdego dnia tygodnia
                  return (
                    <div key={index} className="p-2 text-center font-semibold border-l border-[var(--background-color)]">
                      {["Mon, ", "Tue, ", "Wed, ", "Thu, ", "Fri, ", "Sat, ", "Sun, "][index]}{" "}
                      {day.toLocaleDateString("en-EN", { month: "short", day: "numeric" })}
                    </div>
                  );
                })}
              </div>

              <div>
                
                <ActivitySuggestionForm
                  onResults={(res) => {
                    setSuggestions(res);
                    setShowSuggestionsModal(true);
                  }}
                  onTimeChange={(start, end) => {
                    setSuggestionStart(start);
                    setSuggestionEnd(end);
                  }}
                />


                  <PlacementForm
                    onResults={(results, actId, isRecurring) => {
                      setPlacementResults(results);
                      setActivityId(actId);
                      setRecurring(isRecurring);
                      setModalOpen(true);
                    }}
                  />

                  <ShiftedPlacementForm
                    onResults={(results, actId, recurring) => {
                      setShiftedResults(results);
                      setShiftActivityId(actId);
                      setShiftRecurring(recurring);
                      setShiftedOpen(true);
                    }}
                  />

                {/* Siatka godzin */}
                <div className="grid grid-cols-8 relative w-full">
                  <div className="flex flex-col border-r border-[var(--background-color)] text-right text-gray-400 text-xs sm:text-sm">
                    {Array.from({ length: 24 }).map((_, i) => (
                      <div key={i} className="border-t border-[var(--background-color)] pr-2" style={{ height: "60px" }}>
                        {i}:00
                      </div>
                    ))}
                  </div>

                  {/* Kolumny dni */}
                  <div className="col-span-7 grid grid-cols-7 relative">
                    {Array.from({ length: 24 }).map((_, i) => (
                      <div
                        key={`row-${i}`}
                        className="absolute w-full border-t border-[var(--background-color)]"
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


            {showSuggestionsModal && suggestions.length > 0 && (
              <ActivitySuggestionModal
                suggestions={suggestions}
                onClose={() => setShowSuggestionsModal(false)}
                startTime={suggestionStart}
                endTime={suggestionEnd}
              />
            )}

              {modalOpen && activityId !== null && (
                <ActivityPlacementModal
                  activityId={activityId}
                  results={placementResults}
                  isRecurring={isRecurring}
                  onClose={() => setModalOpen(false)}
                />
              )}

              {shiftedOpen && shiftActivityId !== null && (
                <ShiftedPlacementModal
                  activityId={shiftActivityId}
                  results={shiftedResults}
                  isRecurring={shiftRecurring}
                  onClose={() => setShiftedOpen(false)}
                />
              )}

        </div>
      <Footer/>            
    </div>
  );
};

export default TimelinePage;
