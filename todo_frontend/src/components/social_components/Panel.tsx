// import React from "react";

// const Panel: React.FC<{
//   title: string;
//   icon?: React.ReactNode;
//   children: React.ReactNode;
//   small?: boolean;
// }> = ({ title, icon, children, small }) => {
//   return (
//     <div
//       className={[
//         "bg-white/5 border border-white/10 rounded-xl shadow-md",
//         "p-4 flex flex-col",
//         small ? "min-h-[600px]" : "min-h-[600px]"
//       ].join(" ")}
//     >
//       <div className="flex items-center justify-center gap-2 mb-3">
//         {icon}
//         <h2 className="text-lg font-semibold">{title}</h2>
//       </div>

//       <div className="flex-1 overflow-y-auto overflow-x-hidden pr-1">{children}</div>
//     </div>
//   );
// };

// export default Panel;


import React from "react";
import { Plus } from "lucide-react";

const Panel: React.FC<{
  title: string;
  icon?: React.ReactNode;
  children: React.ReactNode;
  small?: boolean;
  onAdd?: () => void; // Callback do dodawania nowego elementu
}> = ({ title, icon, children, small, onAdd }) => {
  return (
    <div
      className={[
        "bg-white/5 border border-white/10 rounded-xl shadow-md",
        "p-4 flex flex-col",
        small ? "min-h-[20vh] max-h-[60vh]" : "min-h-[20vh] max-h-[121.7vh]",
      ].join(" ")}
    >
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-2">
          {icon}
          <h2 className="text-lg font-semibold">{title}</h2>
        </div>
        {/* Przycisk dodawania po prawej stronie */}
        {onAdd && (
           <button
              onClick={(e) => {
                e.stopPropagation();
                onAdd();
              }}
            >
              <Plus size={22} className="text-quaternary hover:text-accent"/>
            </button>
        )}
      </div>

      <div className="flex-1 overflow-y-auto overflow-x-hidden pr-1 pretty-scroll">{children}</div>
    </div>
  );
};

export default Panel;
