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

  const isSystemCategory = category.categoryId >= 1 && category.categoryId <= 7;

  return (
    <div
      className="flex items-center justify-between p-3 rounded-lg cursor-pointer transition hover:opacity-75 border-2"
      style={{
        background: `linear-gradient(to right, ${category.colorHex} 75%, rgba(0,0,0,0) 75%)`,
        borderColor: category.colorHex,
      }}
    >

      
    <div className="flex-1 flex items-center gap-2 text-white font-semibold truncate">
      {category.name}

      {isSystemCategory && (
        <span
          title="System category"
          className="text-xs px-2 py-0.5 rounded bg-black/40 text-white/80"
        >
          system
        </span>
      )}
    </div>



      {/* Ikony po prawej */}
      <div className="flex items-center gap-3 pr-2">
        <button
          onClick={(e) => {
            e.stopPropagation();
            onEdit?.(category); // Przekazujemy całą kategorię do funkcji onEdit
          }}
          className="text-text-0 hover:text-yellow-400 transition"
        >
          <Pencil size={18} />
        </button>

        <button
          onClick={(e) => {
            e.stopPropagation();
            onDelete?.(category.categoryId); // Przekazujemy categoryId do funkcji onDelete
          }}
          className="text-text-0 hover:text-red-400 transition"
        >
          <Trash2 size={18} />
        </button>
      </div>
    </div>
  );
};

export default CategoryListItem;
