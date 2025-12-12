import React, { useState, useEffect, useMemo } from "react";
import { useAuth } from "../components/AuthContext";
import { ArrowLeft, ArrowRight } from "lucide-react";
import ActivityBlock from "../components/timeline_components/ActivityBlock";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";

import ActivitySuggestionForm from "../components/activitySuggestion_components/ActivitySuggestionForm";
import ActivitySuggestionModal from "../components/activitySuggestion_components/ActivitySuggestionModal";

import PlacementForm from "../components/activitySuggestion_components/PlacementForm";
import ActivityPlacementModal from "../components/activitySuggestion_components/ActivityPlacementModal";

import ShiftedPlacementForm from "../components/activitySuggestion_components/ShiftedPlacementForm";
import ShiftedPlacementModal from "../components/activitySuggestion_components/ShiftedPlacementModal";

import AccordionItem from "../components/timeline_components/AccordionItem";
import { AccordionGroup } from "../components/timeline_components/AccordionGroup";

import TimelineCalendar from "../components/timeline_components/TimelineCalendar";


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
      <div className="flex items-center justify-center h-screen bg-primary text-text-0">
        <p>Loading timeline...</p>
      </div>
    );
  }

  return (
    <div>
      <NavigationWrapper/>

        <div className="min-h-screen bg-surface-0 text-text-0 p-6">

 
<AccordionGroup>
  <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4 items-start mb-6">

    <AccordionItem id="export" title="Export Tools">
      <button
        onClick={downloadWeekPdf}
        className="px-4 py-2 rounded bg-accent-0 text-text-0 font-semibold hover:bg-accent-1 transition"
      >
        Print current week to PDF
      </button>
    </AccordionItem>

    <AccordionItem id="suggestions" title="Activity Suggestions">
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
    </AccordionItem>

    <AccordionItem id="placement" title="Placement Finder">
      <PlacementForm
        onResults={(results, actId, isRecurring) => {
          setPlacementResults(results);
          setActivityId(actId);
          setRecurring(isRecurring);
          setModalOpen(true);
        }}
      />
    </AccordionItem>

    <AccordionItem id="shift" title="Shifted Placement Finder">
      <ShiftedPlacementForm
        onResults={(results, actId, recurring) => {
          setShiftedResults(results);
          setShiftActivityId(actId);
          setShiftRecurring(recurring);
          setShiftedOpen(true);
        }}
      />
    </AccordionItem>

  </div>
</AccordionGroup>

      
<TimelineCalendar/>

          
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
