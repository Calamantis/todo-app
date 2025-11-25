import React, { useState } from "react";
import { useAuth } from "../AuthContext";

// Typ dla kategorii
interface AddCategoryModalProps {
  onClose: () => void; // Funkcja do zamknięcia modalu
  onAddCategory: () => void; // Funkcja dodająca kategorię
}

const AddCategoryModal: React.FC<AddCategoryModalProps> = ({
  onClose,
  onAddCategory,
}) => {
  const [name, setName] = useState("");
  const [colorHex, setColorHex] = useState("#000000"); // Domyślny kolor czarny
  const { user } = useAuth();

  const handleSubmit = async () => {
    if (!name || !colorHex) {
      alert("Please fill out both fields.");
      return;
    }

    try {
       if (!user) return;
      const response = await fetch("/api/Category/create-category", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`
        },

        body: JSON.stringify({ name, colorHex }),
      });

      if (!response.ok) {
        throw new Error("Failed to create category");
      }

      // Po udanym dodaniu, wywołaj funkcję onAddCategory
      onAddCategory();
      onClose(); // Zamknij modal
    } catch (error: unknown) {
        if (error instanceof Error) {
        alert(error.message); // Typowanie error jako Error
        } else {
        alert("An unknown error occurred"); // Jeśli błąd nie jest instancją Error
        }
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black/50">
      <div className="bg-white p-6 rounded-lg w-1/3">
        <h2 className="text-xl font-semibold mb-4">Add Category</h2>

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

        <div className="flex justify-end gap-4">
          <button onClick={onClose} className="px-4 py-2 bg-gray-400 text-white rounded">
            Cancel
          </button>
          <button onClick={handleSubmit} className="px-4 py-2 bg-blue-600 text-white rounded">
            Add Category
          </button>
        </div>
      </div>
    </div>
  );
};

export default AddCategoryModal;
