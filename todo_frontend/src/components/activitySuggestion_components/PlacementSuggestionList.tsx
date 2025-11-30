// import React from "react";
// import { Check } from "lucide-react";
// import { useAuth } from "../AuthContext";

// interface Props {
//   suggestions: any[];
//   activityId: number;
// }

// const PlacementSuggestionList: React.FC<Props> = ({ suggestions, activityId }) => {
//   const { user } = useAuth();

//   const createInstance = async (s: any) => {
//     if (!user) return;

//     const body = {
//       activityId,
//       occurrenceDate: s.dateLocal,
//       startTime: s.suggestedStart,
//       endTime: s.suggestedEnd,
//       isException: false
//     };

//     const res = await fetch("/api/ActivityInstance/create", {
//       method: "POST",
//       headers: {
//         "Content-Type": "application/json",
//         Authorization: `Bearer ${user.token}`,
//       },
//       body: JSON.stringify(body),
//     });

//     if (res.ok) {
//       alert("Instance created!");
//     } else {
//       alert("Failed to create instance");
//     }
//   };

//   return (
//     <div className="mt-6">
//       <h3 className="text-lg font-semibold mb-3">Available time slots</h3>

//       <div className="flex flex-col gap-3">
//         {suggestions.map((s, i) => (
//           <div
//             key={i}
//             className="p-3 rounded-lg bg-white/10 flex justify-between items-center border border-white/10"
//           >
//             <div>
//               <div className="font-medium">
//                 {s.dateLocal.substring(0, 10)}
//               </div>
//               <div className="text-xs opacity-80">
//                 {s.suggestedStart} → {s.suggestedEnd}
//               </div>
//               <div className="text-xs opacity-60">
//                 Free: {s.totalFreeMinutes} minutes
//               </div>
//             </div>

//             <button
//               onClick={() => createInstance(s)}
//               className="text-green-400 hover:text-green-500 transition"
//             >
//               <Check size={22} />
//             </button>
//           </div>
//         ))}
//       </div>
//     </div>
//   );
// };

// export default PlacementSuggestionList;
