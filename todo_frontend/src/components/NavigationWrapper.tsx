import React from "react";
import { useAuth } from "./AuthContext";
import Navbar from "./Navbar";
import GuestNavbar from "./GuestNavbar";
import AdminNavbar from "./AdminNavbar";

const NavigationWrapper: React.FC = () => {
  const { user } = useAuth();

if (!user) return <GuestNavbar />;

if (user.role === "Admin" || user.role === "Moderator") {
    return <AdminNavbar />;
}

return <Navbar />;

};

export default NavigationWrapper;
