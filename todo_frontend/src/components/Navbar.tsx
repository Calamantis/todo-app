import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "./AuthContext";
import { useState } from "react";

import {
  Menu,
  X,
  Calendar,
  Home,
  Bell,
  User,
  ChartColumn,
  LogOut,
  Users,
  CalendarPlus,
  Calendars
} from "lucide-react";

const Navbar: React.FC = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <nav className="w-full bg-surface-1 text-text-0 shadow-md">
      <div className="max-w-6xl mx-auto flex items-center justify-between px-6 py-4">

        {/* Logo */}
        <Link
          to="/timeline"
          className="text-xl font-bold flex items-center gap-2"
        >
          <Home size={22} />
          TodoApp
        </Link>

        {/* MOBILE BUTTON */}
        <button
          onClick={() => setOpen(!open)}
          className="lg:hidden p-2 rounded hover:bg-white/10"
        >
          {open ? <X size={28} /> : <Menu size={28} />}
        </button>

        {/* DESKTOP MENU */}
        <div className="hidden lg:flex items-center gap-6">

          <Link className="flex items-center gap-1 hover:text-accent-1" to="/timeline">
            <Calendar size={20} />
            <span>Timeline</span>
          </Link>

          <Link className="flex items-center gap-1 hover:text-accent-1" to="/notifications">
            <Bell size={20} />
            <span>Notifications</span>
          </Link>

          <Link className="flex items-center gap-1 hover:text-accent-1" to="/activity">
            <CalendarPlus size={20} />
            <span>Activity</span>
          </Link>

          <Link className="flex items-center gap-1 hover:text-accent-1" to="/online-activity">
            <Calendars size={20} />
            <span>Online activities</span>
          </Link>

          <Link className="flex items-center gap-1 hover:text-accent-1" to="/social">
            <Users size={20} />
            <span>Social</span>
          </Link>

          <Link className="flex items-center gap-1 hover:text-accent-1" to="/profile">
            <User size={20} />
            <span>Profile</span>
          </Link>

          <Link className="flex items-center gap-1 hover:text-accent-1" to="/statistics">
            <ChartColumn size={20} />
            <span>Statistics</span>
          </Link>

          <button
            onClick={handleLogout}
            className="flex items-center gap-1 hover:text-accent-1"
          >
            <LogOut size={20} />
            <span>Logout</span>
          </button>

        </div>
      </div>

      {/* MOBILE DROPDOWN MENU */}
      {open && (
        <div className="lg:hidden bg-primary border-t border-white/20 px-6 pb-6 animate-fadeIn">
          <div className="flex flex-col gap-4 mt-4">

            <Link className="flex items-center gap-2" to="/timeline" onClick={() => setOpen(false)}>
              <Calendar size={20} />
              Timeline
            </Link>

            <Link className="flex items-center gap-2" to="/notifications" onClick={() => setOpen(false)}>
              <Bell size={20} />
              Notifications
            </Link>

            <Link className="flex items-center gap-2" to="/social" onClick={() => setOpen(false)}>
              <Users size={20} />
              Social
            </Link>

            <Link className="flex items-center gap-2" to="/profile" onClick={() => setOpen(false)}>
              <User size={20} />
              Profile
            </Link>

            <Link className="flex items-center gap-2" to="/statistics" onClick={() => setOpen(false)}>
              <ChartColumn size={20} />
              Statistics
            </Link>

            <button
              onClick={handleLogout}
              className="flex items-center gap-2 hover:text-accent-1"
            >
              <LogOut size={20} />
              Logout
            </button>
          </div>
        </div>
      )}
    </nav>
  );
};

export default Navbar;
