// // src/components/CategoryListItem.tsx
// import React from "react";
// import { Pencil, Trash2 } from "lucide-react";

// interface Category {
//   name: string;
//   colorHex: string;
// }

// interface CategoryListItemProps {
//   category: Category;
//   onEdit?: (categoryName: string) => void;
//   onDelete?: (categoryName: string) => void;
// }

// const CategoryListItem: React.FC<CategoryListItemProps> = ({
//   category,
//   onEdit,
//   onDelete,
// }) => {
//   return (
//     <div
//       className="flex items-center justify-between p-3 mb-3 rounded-lg cursor-pointer transition hover:opacity-75"
//       style={{
//         background: `linear-gradient(to right, ${category.colorHex} 75%, rgba(0,0,0,0) 75%)`,
//       }}
//     >
//       {/* Nazwa kategorii */}
//       <div className="flex-1 text-white font-semibold truncate">
//         {category.name}
//       </div>

//       {/* Ikony po prawej */}
//       <div className="flex items-center gap-3 pr-2">
//         <button
//           onClick={(e) => {
//             e.stopPropagation();
//             onEdit?.(category.name);
//           }}
//           className="text-white/80 hover:text-yellow-400 transition"
//         >
//           <Pencil size={18} />
//         </button>

//         <button
//           onClick={(e) => {
//             e.stopPropagation();
//             onDelete?.(category.name);
//           }}
//           className="text-white/80 hover:text-red-400 transition"
//         >
//           <Trash2 size={18} />
//         </button>
//       </div>
//     </div>
//   );
// };

// export default CategoryListItem;


import React from "react";
import { Pencil, Trash2 } from "lucide-react";

// Typ kategorii
interface Category {
  categoryId: number;
  id?: number;
  name: string;
  colorHex: string;
}

interface CategoryListItemProps {
  category: Category;
  onEdit?: (category: Category) => void;
  onDelete?: (category: number) => void;
}

const CategoryListItem: React.FC<CategoryListItemProps> = ({
  category,
  onEdit,
  onDelete,
}) => {
  return (
    <div
      className="flex items-center justify-between p-3 rounded-lg cursor-pointer transition hover:opacity-75 border-2"
      style={{
        background: `linear-gradient(to right, ${category.colorHex} 75%, rgba(0,0,0,0) 75%)`,
        borderColor: category.colorHex,
      }}
    >
      {/* Nazwa kategorii */}
      <div className="flex-1 text-white font-semibold truncate">
        {category.name}
      </div>

      {/* Ikony po prawej */}
      <div className="flex items-center gap-3 pr-2">
        <button
          onClick={(e) => {
            e.stopPropagation();
            onEdit?.(category); // Przekazujemy całą kategorię do funkcji onEdit
          }}
          className="text-white/80 hover:text-yellow-400 transition"
        >
          <Pencil size={18} />
        </button>

        <button
          onClick={(e) => {
            e.stopPropagation();
            onDelete?.(category.categoryId); // Przekazujemy categoryId do funkcji onDelete
          }}
          className="text-white/80 hover:text-red-400 transition"
        >
          <Trash2 size={18} />
        </button>
      </div>
    </div>
  );
};

export default CategoryListItem;
