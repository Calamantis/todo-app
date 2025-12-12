import React, { useState, useEffect } from "react";
import { useAuth } from "../components/AuthContext";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";
import Panel from "../components/social_components/Panel";

import ExpandableActivityItem from "../components/activity_components/ExpandableActivityItem";
import AddActivityModal from "../components/activity_components/AddActivityModal";
import EditActivityModal from "../components/activity_components/EditActivityModal";
import DeleteActivityModal from "../components/activity_components/DeleteActivityModal";
import AddActivityInstanceModal from "../components/activity_components/AddActivityInstanceModal";
import EditActivityInstanceModal from "../components/activity_components/EditActivityInstanceModal";
import DeleteActivityInstanceModal from "../components/activity_components/DeleteActivityInstanceModal";
import ActivityInstanceListItem from "../components/activity_components/ActivityInstanceListItem";
import CategoryListItem from "../components/category_components/CategoryListItem"; 
import AddCategoryModal from "../components/category_components/AddCategoryModal";
import EditCategoryModal from "../components/category_components/EditCategoryModal";
import DeleteCategoryModal from "../components/category_components/DeleteCategoryModal";



const ActivityPage: React.FC = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [activities, setActivities] = useState<any[]>([]);
  const [categories, setCategories] = useState<any[]>([]);
  const [showAddActivityModal, setShowAddActivityModal] = useState(false);
  const [showEditActivityModal, setShowEditActivityModal] = useState(false);
  const [activityToEdit, setActivityToEdit] = useState<any>(null);
  const [showDeleteActivityModal, setShowDeleteActivityModal] = useState(false);
  const [activityToDelete, setActivityToDelete] = useState<any>(null);
  const [showAddInstanceModal, setShowAddInstanceModal] = useState(false);
  const [selectedActivity, setSelectedActivity] = useState<any>(null);
  const [instances, setInstances] = useState<any[]>([]);
  const [loadingInstances, setLoadingInstances] = useState(false);
  const [showAddCategoryModal, setShowAddCategoryModal] = useState(false);
  const [showEditCategoryModal, setShowEditCategoryModal] = useState(false);
  const [categoryToEdit, setCategoryToEdit] = useState<any>(null);
  const [categoryToDelete, setCategoryToDelete] = useState<any>(null);
  const [showDeleteCategoryModal, setShowDeleteCategoryModal] = useState(false);
  const [showEditInstanceModal, setShowEditInstanceModal] = useState(false);
  const [instanceToEdit, setInstanceToEdit] = useState<any>(null);
  const [showDeleteInstanceModal, setShowDeleteInstanceModal] = useState(false);
  const [instanceToDelete, setInstanceToDelete] = useState<any>(null);
  

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

  // Funkcja do pobrania aktywności z API
  const fetchActivities = async () => {
    if (!user) return;
    setLoading(true);
    setError("");
    try {
      const response = await fetch("/api/Activity/user/get-activities",
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      }
    );
      if (!response.ok) throw new Error("Failed to load activities");
      const data = await response.json();
      setActivities(data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Funkcja do pobrania instancji aktywności z API
  const fetchInstances = async (activityId: number) => {
    if (!user) return;
    setLoadingInstances(true);
    try {
      const response = await fetch(
        `/api/ActivityInstance/activity/get-activity-instances?activityId=${activityId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        }
      );
      if (!response.ok) throw new Error("Failed to load instances");
      const data = await response.json();
      // Sort instances by occurrence date
      const sortedData = data.sort((a: any, b: any) => {
        return new Date(a.occurrenceDate).getTime() - new Date(b.occurrenceDate).getTime();
      });
      setInstances(sortedData);
    } catch (err: any) {
      setError(err.message);
      setInstances([]);
    } finally {
      setLoadingInstances(false);
    }
  };

  // Funkcja do obsługi wyboru aktywności
  const handleSelectActivity = (activity: any) => {
    setSelectedActivity(activity);
    fetchInstances(activity.activityId);
  };

  // Pobieranie kategorii i aktywności po załadowaniu komponentu
  useEffect(() => {
    fetchCategories();
    fetchActivities();
  }, [user]);

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

  const handleEditActivityClick = (activity: any) => {
    setActivityToEdit(activity);
    setShowEditActivityModal(true);
  };

  const handleDeleteActivityClick = (activityId: number) => {
    console.log("Delete activity:", activityId);
    const activity = activities.find((act) => act.activityId === activityId);
    if (activity) {
      setActivityToDelete(activity);
      setShowDeleteActivityModal(true);
    }
  };

  const handleDeleteActivity = (id: number) => {
    setActivities(activities.filter((activity) => activity.activityId !== id));
    setShowDeleteActivityModal(false);
    fetchActivities();
  };

  const handleCreateInstance = () => {
    if (selectedActivity) {
      fetchInstances(selectedActivity.activityId);
    }
    setShowAddInstanceModal(false);
  };

  const handleEditInstanceClick = (instance: any) => {
    setInstanceToEdit(instance);
    setShowEditInstanceModal(true);
  };

  const handleDeleteInstanceClick = (instanceId: number) => {
    const instance = instances.find((inst) => inst.instanceId === instanceId);
    if (instance) {
      setInstanceToDelete(instance);
      setShowDeleteInstanceModal(true);
    }
  };

  const handleDeleteInstance = (instanceId: number) => {
    setInstances(instances.filter((instance) => instance.instanceId !== instanceId));
    setShowDeleteInstanceModal(false);
    if (selectedActivity) {
      fetchInstances(selectedActivity.activityId);
    }
  };

    const handleCloseModal = () => {
    setShowAddCategoryModal(false);
    setShowEditCategoryModal(false);
    setShowDeleteCategoryModal(false);
    setShowAddActivityModal(false);
    setShowEditActivityModal(false);
    setShowDeleteActivityModal(false);
    setShowAddInstanceModal(false);
    setShowEditInstanceModal(false);
    setShowDeleteInstanceModal(false);
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
      <div className="min-h-screen bg-surface-0 text-text-0 p-4 md:p-6">
        <div className="max-w-9xl mx-auto">
          {/* Panel 1: Category - Full Width */}
          <div className="mb-4">
            <Panel title="Category" onAdd={() => setShowAddCategoryModal(true)}>
              {/* Kategorie */}
              {loading && <div>Loading categories...</div>}
              {error && <div className="text-red-500">{error}</div>}
              {categories.length > 0 ? (
                <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-3">
                  {categories.map((category) => (
                    <CategoryListItem key={category.categoryId} category={category} onEdit={handleEditButtonClick} onDelete={handleDeleteButtonClick}  />
                  ))}
                </div>
              ) : (
                <div>No categories found.</div>
              )}
            </Panel>
          </div>

          {/* Panel 2 & 3: Activities and Activity Instances - Side by Side */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
            {/* Panel 2: Activities */}
            <div>
              <Panel title="Activities" onAdd={() => setShowAddActivityModal(true)}>
                {/* Aktywności */}
                {loading && <div>Loading activities...</div>}
                {error && <div className="text-red-500">{error}</div>}
                {activities.length > 0 ? (
                  activities.map((activity) => (
                    <ExpandableActivityItem 
                      key={activity.activityId} 
                      activity={activity} 
                      onEdit={handleEditActivityClick} 
                      onDelete={handleDeleteActivityClick} 
                      onSelect={handleSelectActivity}
                      isSelected={selectedActivity?.activityId === activity.activityId}
                      onPrivacyChanged={fetchActivities}
                    />
                  ))
                ) : (
                  <div>No activities found.</div>
                )}
              </Panel>
            </div>

            {/* Panel 3: Activity Instance */}
            <div>
              <Panel title="Activity Instance" onAdd={() => selectedActivity && setShowAddInstanceModal(true)}>
                {selectedActivity ? (
                  <>
                    <div className="mb-2 text-xs font-semibold text-gray-600 dark:text-gray-400">
                      {selectedActivity.title}
                    </div>
                    {loadingInstances && <div className="text-sm text-gray-500">Loading instances...</div>}
                    {!loadingInstances && instances.length === 0 && (
                      <div className="text-sm text-gray-500">No instances found.</div>
                    )}
                    {instances.map((instance) => (
                      <ActivityInstanceListItem 
                        key={instance.instanceId} 
                        instance={instance}
                        onEdit={handleEditInstanceClick}
                        onDelete={handleDeleteInstanceClick}
                      />
                    ))}
                  </>
                ) : (
                  <div className="text-sm text-gray-500">Select an activity to view instances</div>
                )}
              </Panel>
            </div>
          </div>
        </div>
      </div>

      {showAddActivityModal && (
        <AddActivityModal
          onClose={() => setShowAddActivityModal(false)}
          onCreate={() => fetchActivities()}
          categories={categories.map((c) => ({ categoryId: c.categoryId, name: c.name }))}
        />
      )}

      {showEditActivityModal && activityToEdit && (
        <EditActivityModal
          activity={activityToEdit}
          onClose={() => setShowEditActivityModal(false)}
          onEditActivity={() => fetchActivities()}
          categories={categories.map((c) => ({ categoryId: c.categoryId, name: c.name }))}
        />
      )}

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

      {/* Modal dla usuwania aktywności */}
      {showDeleteActivityModal && activityToDelete && (
        <DeleteActivityModal
          activity={activityToDelete}
          onDelete={handleDeleteActivity}
          onClose={handleCloseModal}
        />
      )}

      {/* Modal dla dodawania instancji aktywności */}
      {showAddInstanceModal && selectedActivity && (
        <AddActivityInstanceModal
          activityId={selectedActivity.activityId}
          categoryName={selectedActivity.categoryName}
          categoryColorHex={selectedActivity.colorHex}
          onClose={handleCloseModal}
          onCreateInstance={handleCreateInstance}
        />
      )}

      {/* Modal dla edycji instancji aktywności */}
      {showEditInstanceModal && instanceToEdit && (
        <EditActivityInstanceModal
          instance={instanceToEdit}
          onClose={handleCloseModal}
          onEditInstance={() => {
            if (selectedActivity) {
              fetchInstances(selectedActivity.activityId);
            }
          }}
        />
      )}

      {/* Modal dla usuwania instancji aktywności */}
      {showDeleteInstanceModal && instanceToDelete && (
        <DeleteActivityInstanceModal
          instance={instanceToDelete}
          onClose={handleCloseModal}
          onDelete={handleDeleteInstance}
        />
      )}

      <Footer />
    </div>
  );
};

export default ActivityPage;
