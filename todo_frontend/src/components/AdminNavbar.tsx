import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import { useAuth } from "./AuthContext";

import {
  Menu,
  X,
  ShieldCheck,
  ShieldAlert,
  LogOut,
  FileText,
} from "lucide-react";

const AdminNavbar: React.FC = () => {
  const [open, setOpen] = useState(false);

  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <nav className="w-full bg-surface-1 text-text-0 shadow-md">
      <div className="max-w-6xl mx-auto flex items-center justify-between px-6 py-4">

        {/* Logo */}
        <Link to="/" className="text-xl font-bold flex items-center gap-2">
          <ShieldCheck size={22} />
          Admin Panel
        </Link>

        {/* MOBILE BUTTON */}
        <button
          onClick={() => setOpen(!open)}
          className="lg:hidden p-2 rounded hover:bg-white/10"
        >
          {open ? <X size={28} /> : <Menu size={28} />}
        </button>

        {/* DESKTOP MENU */}
        <div className="hidden lg:flex items-center gap-8">

          {/* Moderation */}
          <div className="flex flex-col">
            <span className="uppercase text-xs opacity-80">Moderative actions</span>
            <div className="flex gap-6 mt-1">

              <Link className="flex items-center gap-1 hover:text-accent-1" to="/moderation-panel">
                <ShieldAlert size={20} />
                Moderation Panel
              </Link>
            </div>
          </div>

          {/* Administrative Actions */}
          <div className="flex flex-col">
            <span className="uppercase text-xs opacity-80">Administrative actions</span>
            <div className="flex gap-6 mt-1">

              <Link className="flex items-center gap-1 hover:text-accent-1" to="/administrative-panel">
                <ShieldAlert size={20} />
                Administrative Panel
              </Link>
            </div>
          </div>

          {/* Logout */}
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
          <div className="flex flex-col gap-5 mt-4">

            {/* Moderation */}
            <div>
              <span className="uppercase text-xs opacity-80">Moderation</span>
              <div className="flex flex-col gap-3 mt-2">
                <Link to="/moderation-panel" onClick={() => setOpen(false)} className="flex items-center gap-2">
                  <ShieldAlert size={20} /> Moderation Panel
                </Link>
              </div>
            </div>

            {/* Administrative actions */}
            <div>
              <span className="uppercase text-xs opacity-80">Administrative actions</span>
              <div className="flex flex-col gap-3 mt-2">
                <Link to="/administrative-panel" onClick={() => setOpen(false)} className="flex items-center gap-2">
                  <FileText size={20} /> System Logs
                </Link>
              </div>
            </div>

            {/* Logout */}
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

export default AdminNavbar;
