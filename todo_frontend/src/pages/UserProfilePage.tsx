import React, { useState, useEffect } from 'react';
import { useAuth } from "../components/AuthContext"; // Jeśli używasz AuthContext
import NavigationWrapper from '../components/NavigationWrapper';
import Footer from '../components/Footer';
import ThemeSwitcher from '../components/ThemeSwitcher';

const UserProfilePage: React.FC = () => {
  const { user } = useAuth();
  const [profileData, setProfileData] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [profileImage, setProfileImage] = useState<File | null>(null);
  const [backgroundImage, setBackgroundImage] = useState<File | null>(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [imageVersion, setImageVersion] = useState(Date.now());



  useEffect(() => {
    const fetchProfile = async () => {
      if (!user) return;

      try {
        const res = await fetch("/api/UserAccount/account-details", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
            role: user.role,
            userId: user.userId.toString(),
          },
        });

        if (!res.ok) throw new Error("Failed to fetch profile");

        const data = await res.json();
        setProfileData(data);
      } catch (err: unknown) {
        setError(err instanceof Error ? err.message : "Unexpected error");
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, [user]);

  if (loading) return <div>Loading...</div>;
  if (error) return <div className="text-red-500">{error}</div>;

  // Backend zwraca same ścieżki "/1/1_profile.jpg"
  const backendBase = "http://localhost:5268";
  
const profileImg = profileData.profileImageUrl
  ? `${backendBase}${profileData.profileImageUrl}?v=${imageVersion}`
  : `${backendBase}/UserProfileImages/DefaultProfileImage.jpg`;

const backgroundImg = profileData.backgroundImageUrl
  ? `${backendBase}${profileData.backgroundImageUrl}?v=${imageVersion}`
  : `${backendBase}/UserProfileImages/DefaultBackGround.jpg`;

  // Handle input changes
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setProfileData((prevData: any) => ({
      ...prevData,
      [name]: value,
    }));
  };

  // Handle checkbox changes
  const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, checked } = e.target;
    setProfileData((prevData: any) => ({
      ...prevData,
      [name]: checked,
    }));
  };

  // Handle file input changes (Profile Image)
  const handleProfileImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setProfileImage(e.target.files[0]);
    }
  };

  // Handle file input changes (Background Image)
  const handleBackgroundImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setBackgroundImage(e.target.files[0]);
    }
  };

  // Save changes (call the relevant API endpoint)
  const handleSaveChanges = async () => {
    const formData = new FormData();
    
    // Adding text fields to formData
    formData.append('fullName', profileData.fullName);
    formData.append('email', profileData.email);
    formData.append('synopsis', profileData.synopsis);
    formData.append('allowFriendInvites', profileData.allowFriendInvites.toString());
    formData.append('allowDataStatistics', profileData.allowDataStatistics.toString());

    // Adding images if they exist
    if (profileImage) {
      formData.append('profileImage', profileImage);
    }
    if (backgroundImage) {
      formData.append('backgroundImage', backgroundImage);
    }

    try {
      const response = await fetch('/api/UserAccount/update-account-details', {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${user!.token}`,
        },
        body: formData, // Send the form data including files
      });

      if (!response.ok) {
        throw new Error('Failed to save changes');
      }

      const updated = await response.json();
        setProfileData(updated);
        setImageVersion(Date.now());
    } catch (err) {
      setError('Error saving changes.');
    }
  };

  return (
    <div className="min-h-screen bg-surface-0">
      <NavigationWrapper />
      {/* Background banner */}
      <div
        className="w-full h-60 bg-cover bg-center"
        style={{ backgroundImage: `url(${backgroundImg})` }}
      ></div>

      {/* Content container */}
<div className="max-w-6xl mx-auto relative px-6 mb-10">

  <div className="mt-10 flex gap-10">
<div className="flex-1 pt-2 pb-10 bg-primary shadow-lg p-6 bg-surface-1 rounded-xl">
          {/* Flexbox: obrazek + tekst obok */}
          <div className="flex items-center gap-6">
            
            {/* Profile Image */}
            <div className="relative">
              <img
                src={profileImg}
                className="w-40 h-40 rounded-full shadow-lg object-cover"
                alt="Profile"
              />
              {/* Change profile image */}
              <input
                type="file"
                onChange={handleProfileImageChange}
                className="absolute bottom-0 right-0 text-sm opacity-0 cursor-pointer"
              />
            </div>

            {/* Editable Name & email */}
            <div className="flex flex-col">
              <label className="text-xl text-text-0 font-semibold mb-2">Full Name</label>
              <input
                type="text"
                name="fullName"
                value={profileData.fullName}
                onChange={handleInputChange}
                className="p-2 mb-2 bg-surface-2 text-text-0 rounded-md"
              />
              
              <label className="text-xl text-text-0 font-semibold mb-2 mt-4">Email</label>
              <input
                type="email"
                name="email"
                value={profileData.email}
                onChange={handleInputChange}
                className="p-2 mb-2 bg-surface-2 text-text-0 rounded-md"
              />
            </div>
          </div>

          {/* Editable Synopsis */}
          <div className="mt-6">
            <label className="font-medium text-text-0">Synopsis:</label>
            <textarea
              name="synopsis"
              value={profileData.synopsis || ''}
              onChange={handleInputChange}
              className="w-full p-2 mt-2 bg-surface-2 text-text-0 rounded-md"
            />
          </div>

          {/* Editable Checkbox - Allow Friend Invites */}
          <div className="flex items-center gap-3 mt-6">
            <label className="font-medium text-text-0 w-56">Allow Friend Invites</label>
            <input
              type="checkbox"
              name="allowFriendInvites"
              checked={profileData.allowFriendInvites}
              onChange={handleCheckboxChange}
              className="w-5 h-5 accent-accent-0"
            />
          </div>

          {/* Editable Checkbox - Allow Data Statistics */}
          <div className="flex items-center gap-3 mt-4">
            <label className="font-medium text-text-0 w-56">Allow Data Statistics</label>
            <input
              type="checkbox"
              name="allowDataStatistics"
              checked={profileData.allowDataStatistics}
              onChange={handleCheckboxChange}
              className="w-5 h-5 accent-accent-0"
            />
          </div>

          {/* Buttons to change background image */}
          <div className="mt-6">
            <label className="font-medium text-text-0">Background Image</label>
            <input
              type="file"
              onChange={handleBackgroundImageChange}
              className="block mt-2 text-text-0 opacity-80"
            />
          </div>

          {/* Save Changes Button */}
          <div className="mt-6 gap-16 flex justify-center">
            <button
              onClick={() => setShowDeleteModal(true)}
              className="bg-red-500 text-text-0 py-2 px-6 rounded-md hover:bg-red-600"
            >
              Delete Account
            </button>

            <button
              onClick={handleSaveChanges}
              className="text-text-0 bg-accent-0 hover:bg-accent-1 py-2 px-6 rounded-md"
            >
              Save Changes
            </button>
          </div>
        </div>
        <div className="w-64 bg-primary text-white rounded-xl shadow-lg h-fit">
      <ThemeSwitcher />
    </div>

{showDeleteModal && (
  <div className="fixed inset-0 z-50 bg-black/60 flex items-center justify-center">
    <div className="bg-surface-1 text-text-0 rounded-xl shadow-xl max-w-md w-full p-6">

      <h2 className="text-xl font-semibold mb-4 text-red-400">
        Delete account permanently?
      </h2>

      <p className="text-sm opacity-80 mb-6">
        This action <b>cannot be undone</b>.  
        All your data, activities and statistics will be permanently removed.
      </p>

      <div className="flex justify-end gap-4">
        <button
          onClick={() => setShowDeleteModal(false)}
          className="px-4 py-2 rounded bg-surface-2 hover:bg-surface-3"
          disabled={deleting}
        >
          Cancel
        </button>

        <button
          onClick={async () => {
            try {
              setDeleting(true);

              const response = await fetch("/api/UserAccount/delete-account", {
                method: "DELETE",
                headers: {
                  Authorization: `Bearer ${user!.token}`,
                },
              });

              if (!response.ok) {
                throw new Error("Failed to delete account");
              }

              alert("Your account has been deleted.");
              window.location.href = "/login";
            } catch (err) {
              console.error(err);
              setError("Failed to delete account. Please try again.");
            } finally {
              setDeleting(false);
              setShowDeleteModal(false);
            }
          }}
          className="px-4 py-2 rounded bg-red-600 hover:bg-red-700 font-semibold"
          disabled={deleting}
        >
          {deleting ? "Deleting…" : "Yes, delete permanently"}
        </button>
      </div>
    </div>
  </div>
)}


</div>
      </div>
      <Footer />
    </div>
  );
};

export default UserProfilePage;
