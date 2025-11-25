import React from "react";
import { useAuth } from "../AuthContext";

// Typ dla kategorii
interface DeleteCategoryModalProps {
  category: { categoryId: number; name: string; colorHex: string };
  onDelete: (id: number) => void; // Funkcja usuwająca kategorię
  onClose: () => void; // Funkcja do zamknięcia modalu
}

const DeleteCategoryModal: React.FC<DeleteCategoryModalProps> = ({
  category,
  onDelete,
  onClose,
}) => {
  const { user } = useAuth();
  const handleDelete = async () => {
    console.log(category);
    try {
        if (!user) return;
        const response = await fetch(`/api/Category/delete-category?id=${category.categoryId}`, {
            method: "DELETE",
            headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`
            },
        });

        if (!response.ok) {
            throw new Error("Failed to delete category");
        }

        // Po udanym usunięciu wywołaj funkcję onDelete
        onDelete(category.categoryId); // Wywołanie funkcji usuwającej kategorię z listy
        onClose(); // Zamknięcie modalu
        } catch (error: unknown) {
        if (error instanceof Error) {
            alert(error.message); // Wyświetlenie komunikatu błędu
        } else {
            alert("An unknown error occurred");
        }
        }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black/50">
      <div className="bg-white p-6 rounded-lg w-1/3">
        <h2 className="text-xl font-semibold mb-4">Delete Category</h2>
        <p className="mb-4">Are you sure you want to delete the category "{category.name}"?</p>

        <div className="flex justify-end gap-4">
          <button onClick={onClose} className="px-4 py-2 bg-gray-400 text-white rounded">
            Cancel
          </button>
          <button onClick={handleDelete} className="px-4 py-2 bg-red-600 text-white rounded">
            Delete
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteCategoryModal;
