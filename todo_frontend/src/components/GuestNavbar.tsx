import { Link } from "react-router-dom";
import { useState } from "react";

import {
  Menu,
  X,
  Play,
  Home,
  FileText,
  Send,
  LogIn,
  BadgeInfo
} from "lucide-react";

const GuestNavbar: React.FC = () => {
  const [open, setOpen] = useState(false);

  return (
    <nav className="w-full bg-primary text-white shadow-md">
      <div className="max-w-6xl mx-auto flex items-center justify-between px-6 py-4">

        {/* Logo */}
        <Link to="/" className="text-xl font-bold flex items-center gap-2">
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

          <Link className="flex items-center gap-1 hover:text-blue-400" to="/getting-started">
            <Play size={20} />
            Getting Started
          </Link>

          <Link className="flex items-center gap-1 hover:text-blue-400" to="/terms-of-service">
            <FileText size={20} />
            Terms of Service
          </Link>

          <Link className="flex items-center gap-1 hover:text-blue-400" to="/contact">
            <Send size={20} />
            Contact
          </Link>

          <Link className="flex items-center gap-1 hover:text-blue-400" to="/about">
            <BadgeInfo size={20} />
            About Us
          </Link>

          <Link className="flex items-center gap-1 hover:text-blue-400" to="/login">
            <LogIn size={20} />
            Login
          </Link>
        </div>
      </div>

      {/* MOBILE DROPDOWN MENU */}
      {open && (
        <div className="lg:hidden bg-primary border-t border-white/20 px-6 pb-6 animate-fadeIn">
          <div className="flex flex-col gap-4 mt-4">

            <Link to="/GettingStartedPage" className="flex items-center gap-2" onClick={() => setOpen(false)}>
              <Play size={20} /> Getting Started
            </Link>

            <Link to="/RulesPage" className="flex items-center gap-2" onClick={() => setOpen(false)}>
              <FileText size={20} /> Terms of Service
            </Link>

            <Link to="/ContactPage" className="flex items-center gap-2" onClick={() => setOpen(false)}>
              <Send size={20} /> Contact
            </Link>

            <Link to="/AboutPage" className="flex items-center gap-2" onClick={() => setOpen(false)}>
              <BadgeInfo size={20} /> About Us
            </Link>

            <Link to="/login" className="flex items-center gap-2" onClick={() => setOpen(false)}>
              <LogIn size={20} /> Login
            </Link>

          </div>
        </div>
      )}
    </nav>
  );
};

export default GuestNavbar;
