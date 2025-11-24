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
    ? `${profileData.profileImageUrl}?${new Date().getTime()}` 
    : `${backendBase}/UserProfileImages/DefaultProfileImage.jpg`;

  const backgroundImg = profileData.backgroundImageUrl 
    ? `${profileData.backgroundImageUrl}?${new Date().getTime()}` 
    : `${backendBase}/UserProfileImages/DefaultBackGround.jpg`;


  const handleDeleteAccount = async () => {
  try {
    const response = await fetch("/api/UserAccount/delete-account", {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${user!.token}`,
      },
    });

    if (!response.ok) {
      throw new Error("Failed to delete account");
    }

    // Jeśli usunięto, możesz przekierować użytkownika do strony logowania
    alert("Your account has been deleted");
    // Przekierowanie użytkownika na stronę logowania
    window.location.href = "/login"; // Możesz również użyć React Routera, aby przekierować
  } catch (error) {
    console.error("Error deleting account:", error);
    setError("Failed to delete account. Please try again.");
  }
};

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


      
      setError(""); // Clear any previous errors
      setProfileData(await response.json()); // Update profile with response data
    } catch (err) {
      setError('Error saving changes.');
    }
  };

  return (
    <div className="min-h-screen bg-[var(--background-color)]">
      <NavigationWrapper />
      {/* Background banner */}
      <div
        className="w-full h-60 bg-cover bg-center"
        style={{ backgroundImage: `url(${backgroundImg})` }}
      ></div>

      {/* Content container */}
<div className="max-w-6xl mx-auto relative px-6 mb-10">

  <div className="mt-10 flex gap-10">
<div className="flex-1 pt-2 pb-10 bg-primary rounded-xl shadow-lg p-6">
          {/* Flexbox: obrazek + tekst obok */}
          <div className="flex items-center gap-6">
            
            {/* Profile Image */}
            <div className="relative">
              <img
                src={profileImg}
                className="w-40 h-40 rounded-full border-4 border-white shadow-lg object-cover"
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
              <label className="text-xl text-white font-semibold mb-2">Full Name</label>
              <input
                type="text"
                name="fullName"
                value={profileData.fullName}
                onChange={handleInputChange}
                className="p-2 mb-2 border border-gray-300 rounded-md"
              />
              
              <label className="text-xl text-white font-semibold mb-2 mt-4">Email</label>
              <input
                type="email"
                name="email"
                value={profileData.email}
                onChange={handleInputChange}
                className="p-2 mb-2 border border-gray-300 rounded-md"
              />
            </div>
          </div>

          {/* Editable Synopsis */}
          <div className="mt-6">
            <label className="font-medium text-white">Synopsis:</label>
            <textarea
              name="synopsis"
              value={profileData.synopsis || ''}
              onChange={handleInputChange}
              className="w-full p-2 mt-2 border border-gray-300 rounded-md"
            />
          </div>

          {/* Editable Checkbox - Allow Friend Invites */}
          <div className="flex items-center gap-3 mt-6">
            <label className="font-medium text-white w-56">Allow Friend Invites</label>
            <input
              type="checkbox"
              name="allowFriendInvites"
              checked={profileData.allowFriendInvites}
              onChange={handleCheckboxChange}
              className="w-5 h-5"
            />
          </div>

          {/* Editable Checkbox - Allow Data Statistics */}
          <div className="flex items-center gap-3 mt-4">
            <label className="font-medium text-white w-56">Allow Data Statistics</label>
            <input
              type="checkbox"
              name="allowDataStatistics"
              checked={profileData.allowDataStatistics}
              onChange={handleCheckboxChange}
              className="w-5 h-5"
            />
          </div>

          {/* Buttons to change background image */}
          <div className="mt-6">
            <label className="font-medium text-white">Background Image</label>
            <input
              type="file"
              onChange={handleBackgroundImageChange}
              className="block mt-2"
            />
          </div>

          {/* Save Changes Button */}
          <div className="mt-6 gap-6 flex justify-center">
            <button
              onClick={handleSaveChanges}
              className="bg-blue-500 text-white py-2 px-6 rounded-md hover:bg-blue-600"
            >
              Save Changes
            </button>
            <button
              onClick={handleDeleteAccount}
              className="bg-red-500 text-white py-2 px-6 rounded-md hover:bg-red-600"
            >
              Delete Account
            </button>
          </div>
        </div>
        <div className="w-64 bg-primary text-white rounded-xl shadow-lg p-6 h-fit">
      <ThemeSwitcher />
    </div>

</div>
      </div>
      <Footer />
    </div>
  );
};

export default UserProfilePage;
