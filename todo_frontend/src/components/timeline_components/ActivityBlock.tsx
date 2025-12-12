import React, { useState } from "react";
import ActivityDetailsModal from "./ActivityDetailsModal";

// Interfejs dla aktywności
interface Activity {
  instanceId: number;
  occurrenceDate: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
  categoryName: string | null;
  categoryColorHex: string | null;
  activityId: number;
}

interface ActivityBlockProps {
  activity: Activity;
}

const ActivityBlock: React.FC<ActivityBlockProps> = ({ activity }) => {
  const { occurrenceDate, startTime, endTime, categoryName, categoryColorHex } = activity;
  const [modalOpen, setModalOpen] = useState(false);
  // const { user } = useAuth();

  // Parsowanie daty i godziny
  const datePart = occurrenceDate.split("T")[0]; // Wyciągamy tylko datę (np. "2025-11-24")
  const [startHours, startMinutes] = startTime.split(":").map(Number);
  const [endHours, endMinutes] = endTime.split(":").map(Number);

  // Tworzymy obiekty Date z datą i godziną
  const start = new Date(datePart);
  start.setHours(startHours, startMinutes);

  const end = new Date(datePart);
  end.setHours(endHours, endMinutes);

  // Obliczanie czasu trwania w minutach
  const duration = activity.durationMinutes;

  // Pozycjonowanie na osi czasu
  const top = (start.getHours() * 60 + start.getMinutes()); // Pozycja na osi czasu
  const dayOfWeek = (start.getDay() === 0 ? 6 : start.getDay() - 1); // Niedziela = 6, poniedziałek = 0
  const left = (dayOfWeek / 7) * 100; // Pozycja na osi dni tygodnia

  let height = duration; // Wysokość aktywności zależna od czasu trwania

  // Funkcja wywoływana po kliknięciu
  const handleClick = () => {
    console.log(`Clicked on activity ${activity.instanceId}`);
    setModalOpen(true);
  };

  // Tworzymy datę dla wyświetlenia dnia tygodnia
  const displayDate = new Date(datePart);
  const dayOfWeekStr = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"][dayOfWeek];
  const formattedDate = `${dayOfWeekStr}, ${displayDate.toLocaleDateString("pl-PL", {
    year: "numeric",
    month: "short",
    day: "numeric",
  })}`;

  return (
    <>
    <div
      className="absolute rounded-md text-xs sm:text-sm text-center px-2 py-1 overflow-hidden shadow-md border border-surface-1"
      style={{
        top: `${top}px`,
        left: `${left}%`,
        height: `${height}px`,
        width: `${100 / 7}%`,
        backgroundColor: categoryColorHex || "#3b82f6",
      }}
      onClick={handleClick} // Obsługa kliknięcia
    >
      <div className="font-semibold truncate">{categoryName || "Unknown Activity"}</div>
      <div className="text-[10px] opacity-80 h-4 justify-center flex items-center">
        {start.toLocaleTimeString([], {
          hour: "2-digit",
          minute: "2-digit",
        })}
      </div>
      <div className="text-[10px] opacity-80">{formattedDate}</div> {/* Dodana data obok dnia tygodnia */}
    </div>

      {modalOpen && (
        <ActivityDetailsModal 
          instance={activity}
          onClose={() => setModalOpen(false)}
        />
      )}

    </>
  );
};

export default ActivityBlock;
