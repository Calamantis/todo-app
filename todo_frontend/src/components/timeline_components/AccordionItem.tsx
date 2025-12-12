import React from "react";
import { useAccordion } from "./AccordionGroup";
import { ChevronDown } from "lucide-react";

interface Props {
  id: string;
  title: string;
  children: React.ReactNode;
}

const AccordionItem: React.FC<Props> = ({ id, title, children }) => {
  const { openId, setOpenId } = useAccordion();
  const isOpen = openId === id;

  return (
    <div
      className="
         rounded-xl bg-surface-1
        shadow-sm hover:shadow transition-shadow
        p-4 flex flex-col
      "
    >
      {/* HEADER — static button */}
      <button
        onClick={() => setOpenId(isOpen ? null : id)}
        className="
          flex justify-between items-center w-full 
          text-text-0 font-medium tracking-wide
          py-1 px-4 rounded-lg
          hover:bg-surface-2/50 transition-colors
        "
      >
        {title}
        <ChevronDown
          size={18}
          className={`transition-transform duration-300 ${isOpen ? "rotate-180" : ""}`}
        />
      </button>

      {/* CONTENT */}
      <div
        className={`
          overflow-hidden transition-all duration-300
          ${isOpen ? "max-h-[600px] opacity-100 mt-4" : "max-h-0 opacity-0 mt-0"}
        `}
      >
        <div className="pt-1">
          {children}
        </div>
      </div>
    </div>
  );
};

export default AccordionItem;
