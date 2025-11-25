import React, { useState, useEffect } from "react";
import { useAuth } from "../components/AuthContext";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";
import Panel from "../components/social_components/Panel";

import CategoryListItem from "../components/category_components/CategoryListItem"; 
import AddCategoryModal from "../components/category_components/AddCategoryModal";
import EditCategoryModal from "../components/category_components/EditCategoryModal";
import DeleteCategoryModal from "../components/category_components/DeleteCategoryModal";



const ActivityPage: React.FC = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [categories, setCategories] = useState<any[]>([]);
  const [showAddCategoryModal, setShowAddCategoryModal] = useState(false);
  const [showEditCategoryModal, setShowEditCategoryModal] = useState(false);
  const [categoryToEdit, setCategoryToEdit] = useState<any>(null);
  const [categoryToDelete, setCategoryToDelete] = useState<any>(null);
  const [showDeleteCategoryModal, setShowDeleteCategoryModal] = useState(false);
  

  // Funkcja do pobrania danych z API
  const fetchCategories = async () => {
    if (!user) return;
    setLoading(true);
    setError("");
    try {
      const response = await fetch("/api/Category/browse-categories",
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      }
    );
      if (!response.ok) throw new Error("Failed to load categories");
      const data = await response.json();
      setCategories(data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Pobieranie kategorii po załadowaniu komponentu
  useEffect(() => {
    fetchCategories();
  }, []);

  const handleAddCategory = () => {
    // Kategorię dodał już backend, zamknij modal i odśwież listę
    setShowAddCategoryModal(false);
    fetchCategories(); // Odśwież kategorii z API
  };

   const handleEditCategory = (id: number, name: string, colorHex: string) => {
    // Zaktualizuj kategorię w liście
    setCategories(
      categories.map((category) =>
        category.categoryId === id ? { ...category, name, colorHex } : category
      )
    );
    setShowEditCategoryModal(false); // Zamknij modal po edycji
    fetchCategories(); // Odśwież kategorii z API
  };

  const handleDeleteCategory = (id: number) => {
    setCategories(categories.filter((category) => category.categoryId !== id)); // Usuwamy kategorię
    console.log(id);
    setShowDeleteCategoryModal(false); // Zamknij modal po usunięciu
    fetchCategories(); // Odśwież kategorii z API
  };

  const handleEditButtonClick = (category: any) => {
    console.log(category);
    setCategoryToEdit(category);
    setShowEditCategoryModal(true);
  };

    const handleDeleteButtonClick = (categoryId: number) => {
      console.log("Category ID to delete:", categoryId);
    const categoryToDelete = categories.find((cat) => cat.categoryId === categoryId);
    if (categoryToDelete) {
      console.log(categoryToDelete);
      setCategoryToDelete(categoryToDelete); // Ustawiamy kategorię do usunięcia
      setShowDeleteCategoryModal(true); // Wyświetlamy modal usuwania
    }
  };

    const handleCloseModal = () => {
    setShowAddCategoryModal(false);
    setShowEditCategoryModal(false);
    setShowDeleteCategoryModal(false);
  };

  useEffect(() => {
  console.log("Category to edit", categoryToEdit); // Wypisuje po zaktualizowaniu stanu
}, [categoryToEdit]);

  useEffect(() => {
  console.log("Category to delete:", categoryToDelete); // Wypisuje po zaktualizowaniu stanu
}, [categoryToDelete]);

  return (
    <div>
      <NavigationWrapper />
      <div className="min-h-screen bg-[var(--background-color)] text-[var(--text-color)] p-4 md:p-6">
        <div className="max-w-9xl mx-auto">
          <div className="min-h-[80vh] grid grid-cols-1 lg:grid-cols-3 gap-4">
            {/* Panel 1: Activity */}
            <Panel title="Activity" onAdd={() => console.log("Add Activity")}>
              {/* Aktywności */}
              1
            </Panel>

            {/* Panel 2: Activity Recurrence Rule */}
            <Panel title="Activity Recurrence Rule" onAdd={() => console.log("Add Recurrence Rule")}>
              {/* Zasady rekurencji */}
              2
            </Panel>

            <div className="flex flex-col gap-4 min-h-[80vh]">
              {/* Panel 3: Activity Instance */}
              <Panel title="Activity Instance" small onAdd={() => console.log("Add Instance")}>
                {/* Instancje aktywności */}
                3
              </Panel>

              {/* Panel 4: Category */}
              <Panel title="Category" small onAdd={() => setShowAddCategoryModal(true)}>
                {/* Kategorie */}
                {loading && <div>Loading categories...</div>}
                {error && <div className="text-red-500">{error}</div>}
                {categories.length > 0 ? (
                  categories.map((category) => (
                    <CategoryListItem key={category.categoryId} category={category} onEdit={handleEditButtonClick} onDelete={handleDeleteButtonClick}  />
                  ))
                ) : (
                  <div>No categories found.</div>
                )}
              </Panel>
              
            </div>
          </div>
        </div>
      </div>

      {showAddCategoryModal && (
        <AddCategoryModal onAddCategory={handleAddCategory} onClose={handleCloseModal} />
      )}

      {showEditCategoryModal && (
        <EditCategoryModal
          category={categoryToEdit}
          onEditCategory={handleEditCategory}
          onClose={handleCloseModal}
        />
      )}

      {/* Modal dla usuwania kategorii */}
      {showDeleteCategoryModal && categoryToDelete && (
        <DeleteCategoryModal
          category={categoryToDelete}
          onDelete={handleDeleteCategory}
          onClose={handleCloseModal}
        />
      )}

      <Footer />
    </div>
  );
};

export default ActivityPage;
