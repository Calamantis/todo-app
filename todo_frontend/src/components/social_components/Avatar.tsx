import React from "react";

const Avatar: React.FC<{ src?: string; size?: number }> = ({
  src,
  size = 36
}) => {
  // Backend root folder
  const backendBase = "http://localhost:5268";

  // Jeśli backend zwrócił ścieżkę typu "/5/5_profile.jpg"
  const resolvedSrc = src ? `${backendBase}/${src}?${Date.now()}` : `${backendBase}/DefaultProfileImage.jpg`;

  const fallback = `${backendBase}/UserProfileImages/DefaultProfileImage.jpg`;

  return (
    <img
      src={resolvedSrc}
      alt="avatar"
      style={{ width: size, height: size }}
      className="rounded-full object-cover border border-white/20"
      onError={(e) => {
        const img = e.currentTarget as HTMLImageElement;

        // Jeśli już ustawiono fallback — nie rób nic
        if (img.src.includes("DefaultProfileImage.jpg")) return;

        img.src = fallback;
        img.onerror = null;
      }}
    />
  );
};

export default Avatar;
