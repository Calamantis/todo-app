import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "./AuthContext";
import { Calendar, Home, Bell, User, ChartColumn , LogOut } from "lucide-react"; // Importujemy ikony

const Navbar: React.FC = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <nav className="w-full bg-slate-800 text-white shadow-md">
      <div className="max-w-6xl mx-auto flex items-center justify-between px-6 py-4">

        {/* Logo */}
        <Link to="/timeline" className="text-xl font-bold flex items-center gap-2">
          <Home size={22} />
          TodoApp
        </Link>

        {/* Menu buttons */}
        <div className="flex items-center gap-6">

          {/* Timeline */}
          <Link className="flex items-center gap-1 hover:text-blue-400" to="/timeline">
            <Calendar size={20} />
            <span>Timeline</span>
          </Link>

          {/* Notifications */}
          <Link className="flex items-center gap-1 hover:text-blue-400" to="/notifications">
            <Bell size={20} />
            <span>Notifications</span>
          </Link>

          {/* Profile */}
          <Link className="flex items-center gap-1 hover:text-blue-400" to="/profile">
            <User size={20} />
            <span>Profile</span>
          </Link>

          {/* Settings */}
          <Link className="flex items-center gap-1 hover:text-blue-400" to="/settings">
            <ChartColumn size={20} />
            <span>Statistics</span>
          </Link>

          {/* Logout */}
          <button
            onClick={handleLogout}
            className="flex items-center gap-1 hover:text-red-400"
          >
            <LogOut size={20} />
            <span>Logout</span>
          </button>

        </div>
      </div>
    </nav>
  );
};

export default Navbar;
