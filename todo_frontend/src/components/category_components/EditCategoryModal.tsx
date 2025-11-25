import React, { useState } from "react";
import { useAuth } from "../AuthContext";

// Typ dla kategorii
interface EditCategoryModalProps {
  category: { categoryId: number; name: string; colorHex: string }; // Kategoria do edycji
  onClose: () => void; // Funkcja do zamknięcia modalu
  onEditCategory: (id: number, name: string, colorHex: string) => void; // Funkcja do aktualizacji kategorii
}

const EditCategoryModal: React.FC<EditCategoryModalProps> = ({
  category,
  onClose,
  onEditCategory,
}) => {
  const { user } = useAuth();
  const [name, setName] = useState(category.name);
  const [colorHex, setColorHex] = useState(category.colorHex);

  // Obsługuje formularz po kliknięciu "Save"
  const handleSubmit = async () => {
    if (!name || !colorHex) {
      alert("Please fill out both fields.");
      return;
    }

    try {
        if (!user) return;
      const response = await fetch(`/api/Category/update-category?id=${category.categoryId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`
        },
        body: JSON.stringify({ name, colorHex }),
      });

      if (!response.ok) {
        throw new Error("Failed to update category");
      }

      // Po udanej edycji wywołaj funkcję onEditCategory
      onEditCategory(category.categoryId, name, colorHex);
      onClose(); // Zamknij modal po edytowaniu kategorii
    } catch (error: unknown) {
      if (error instanceof Error) {
        alert(error.message);
      } else {
        alert("An unknown error occurred");
      }
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black/50">
      <div className="bg-white p-6 rounded-lg w-1/3">
        <h2 className="text-xl font-semibold mb-4">Edit Category</h2>

        {/* Name Input */}
        <div className="mb-4">
          <label className="block text-sm font-semibold mb-2" htmlFor="name">
            Category Name
          </label>
          <input
            id="name"
            type="text"
            className="w-full p-2 border border-gray-300 rounded"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>

        {/* Color Picker */}
        <div className="mb-4">
          <label className="block text-sm font-semibold mb-2" htmlFor="color">
            Color (Hex)
          </label>
          <input
            id="color"
            type="color"
            className="w-full p-2 border border-gray-300 rounded"
            value={colorHex}
            onChange={(e) => setColorHex(e.target.value)}
          />
        </div>

        {/* Buttons */}
        <div className="flex justify-end gap-4">
          <button onClick={onClose} className="px-4 py-2 bg-gray-400 text-white rounded">
            Cancel
          </button>
          <button onClick={handleSubmit} className="px-4 py-2 bg-blue-600 text-white rounded">
            Save Changes
          </button>
        </div>
      </div>
    </div>
  );
};

export default EditCategoryModal;
