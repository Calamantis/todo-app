// import { useState } from "react";
// import { Trash2, Check } from "lucide-react";
// import { useAuth } from "../AuthContext";

// export interface ActivityDetails {
//   activityId: number;
//   title: string;
//   description: string;
//   isRecurring: boolean;
//   categoryId: number | null;
//   categoryName: string | null;
//   colorHex: string | null;
//   joinCode: string | null;
//   isFriendsOnly: boolean;
// }

// export interface ActivityInstanceData {
//   instanceId: number;
//   activityId: number;
//   occurrenceDate: string;
//   startTime: string;
//   endTime: string;
//   durationMinutes: number;
// }

// interface OfflineActivityModalProps {
//   activity: ActivityDetails;
//   instance: ActivityInstanceData;
// }

// const OfflineActivityModal: React.FC<OfflineActivityModalProps> = ({ activity, instance}) => {
//   const { user } = useAuth();

//   const [start, setStart] = useState(instance.startTime);
//   const [end, setEnd] = useState(instance.endTime);

//   const editTime = async () => {
//     if (!user) return;
//     await fetch(`/api/ActivityInstance/update-time?id=${instance.instanceId}`, {
//       method: "PATCH",
//       headers: { 
//         Authorization: `Bearer ${user.token}`,
//         "Content-Type": "application/json"
//       },
//       body: JSON.stringify({
//         startTime: start,
//         endTime: end
//       })
//     });

//     alert("Updated!");
//   };

//   const removeInstance = async () => {
//     if (!user) return;
//     await fetch(`/api/ActivityInstance/delete-instance?InstanceId=${instance.instanceId}`, {
//       method: "DELETE",
//       headers: { Authorization: `Bearer ${user.token}` }
//     });

//     alert("Removed!");
//   };

//   return (
//     <div className="flex flex-col gap-3">

//       <h2 className="text-xl font-bold">{activity.title}</h2>
//       <p className="text-sm opacity-80">{activity.description}</p>

//       <div className="flex flex-col gap-2">
//         <label>Start time:</label>
//         <input
//           type="time"
//           value={start}
//           onChange={e => setStart(e.target.value)}
//           className="bg-black/20 p-2 rounded border border-white/20"
//         />

//         <label>End time:</label>
//         <input
//           type="time"
//           value={end}
//           onChange={e => setEnd(e.target.value)}
//           className="bg-black/20 p-2 rounded border border-white/20"
//         />
//       </div>

//       {/* BUTTONS */}
//       <div className="flex justify-between mt-3">
//         <button
//           onClick={removeInstance}
//           className="px-3 py-2 bg-red-600 text-white rounded flex items-center gap-1"
//         >
//           <Trash2 size={18} /> Delete
//         </button>

//         <button
//           onClick={editTime}
//           className="px-3 py-2 bg-green-500 text-black rounded flex items-center gap-1"
//         >
//           <Check size={18} /> Save
//         </button>
//       </div>

//     </div>
//   );
// };

// export default OfflineActivityModal;


import React, { useState } from "react";
import { Check, X } from "lucide-react";
import { useAuth } from "../AuthContext";

export interface ActivityDetails {
  activityId: number;
  title: string;
  description: string;
  isRecurring: boolean;
  categoryName: string | null;
  colorHex: string | null;
}

export interface ActivityInstanceData {
  instanceId: number;
  activityId: number;
  occurrenceDate: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
  isActive?: boolean;
  didOccur?: boolean;
  isException?: boolean;
}

interface OfflineActivityModalProps {
  activity: ActivityDetails;
  instance: ActivityInstanceData;
  onClose: () => void;
}

const OfflineActivityModal: React.FC<OfflineActivityModalProps> = ({
  activity,
  instance,
  onClose,
}) => {
  const { user } = useAuth();

  // states do edycji
  const [startTime, setStartTime] = useState(instance.startTime);
  const [endTime, setEndTime] = useState(instance.endTime);

  const [isActive, setIsActive] = useState(instance.isActive ?? true);
  const [didOccur, setDidOccur] = useState(instance.didOccur ?? true);
  const [isException, setIsException] = useState(instance.isException ?? false);

  // liczenie duration
  const computeDuration = (start: string, end: string) => {
    if (!start || !end) return instance.durationMinutes;

    const [sh, sm] = start.split(":").map(Number);
    const [eh, em] = end.split(":").map(Number);

    return eh * 60 + em - (sh * 60 + sm);
  };

  const durationMinutes = computeDuration(startTime, endTime);

  const saveChanges = async () => {
    if (!user) return;

    const payload = {
      occurrenceDate: instance.occurrenceDate,
      startTime: startTime,
      endTime: endTime,
      durationMinutes: durationMinutes,
      isActive: isActive,
      didOccur: didOccur,
      isException: isException,
      activityId: instance.activityId,
      recurrenceRuleId: 0
    };

    console.log("EDIT PAYLOAD:", payload);

    const res = await fetch(
      `/api/ActivityInstance/edit-instance?instanceId=${instance.instanceId}`,
      {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${user.token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      }
    );

    if (res.ok) {
      alert("Instance updated!");
      onClose();
    } else {
      alert("Failed to update.");
    }
  };

  return (
    <div>
      <h2 className="text-xl font-semibold mb-2">{activity.title}</h2>
      <p className="opacity-70 mb-4">{activity.description}</p>

      {/* TIME FIELDS */}
      <div className="flex flex-col gap-3 mb-4">
        <label className="text-sm">
          Start:
          <input
            type="time"
            value={startTime}
            onChange={(e) => setStartTime(e.target.value)}
            className="w-full p-2 mt-1 rounded bg-black/20 border border-white/10"
          />
        </label>

        <label className="text-sm">
          End:
          <input
            type="time"
            value={endTime}
            onChange={(e) => setEndTime(e.target.value)}
            className="w-full p-2 mt-1 rounded bg-black/20 border border-white/10"
          />
        </label>

        <div className="text-sm opacity-80">
          Duration: <b>{durationMinutes}</b> minutes
        </div>
      </div>

      {/* CHECKBOXES */}
      <div className="flex flex-col gap-2 mb-4">
        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={isActive}
            onChange={() => setIsActive(!isActive)}
          />
          Active?
        </label>

        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={didOccur}
            onChange={() => setDidOccur(!didOccur)}
          />
          Did occur?
        </label>

        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={isException}
            onChange={() => setIsException(!isException)}
          />
          Exception?
        </label>
      </div>

      {/* BUTTONS */}
      <div className="flex justify-end gap-3 mt-4">
        <button
          onClick={onClose}
          className="px-4 py-2 bg-red-500/20 border border-red-500/40 rounded text-red-300 hover:bg-red-500/30"
        >
          <X size={16} />
        </button>

        <button
          onClick={saveChanges}
          className="px-4 py-2 bg-accent text-black rounded font-semibold flex items-center gap-2"
        >
          <Check size={18} /> Save
        </button>
      </div>
    </div>
  );
};

export default OfflineActivityModal;
