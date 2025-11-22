import React from "react";

const Footer: React.FC = () => {
  return (
    <footer className="bg-slate-800 text-white py-6 mt-10">
      <div className="max-w-6xl mx-auto px-6 flex flex-col items-center md:flex-row justify-between">
        {/* Left Section */}
        <div className="text-center md:text-left">
          <h3 className="text-lg font-semibold mb-2">TodoApp</h3>
          <p className="text-sm text-gray-400">
            Aplikacja do zarządzania codziennymi aktywnościami
          </p>
        </div>

        {/* Right Section */}
        <div className="mt-4 md:mt-0">
          <ul className="flex gap-6 justify-center md:justify-end text-sm">
            <li>
              <a href="/privacy-policy" className="hover:text-blue-400">
                Privacy Policy
              </a>
            </li>
            <li>
              <a href="/terms" className="hover:text-blue-400">
                Terms of Service
              </a>
            </li>
            <li>
              <a href="/contact" className="hover:text-blue-400">
                Contact Author
              </a>
            </li>
          </ul>
        </div>
      </div>

      {/* Bottom Copyright Section */}
      <div className="mt-6 border-t border-gray-700 pt-4 text-center text-sm text-gray-400">
        <p>&copy; 2025 TodoApp. All rights reserved.</p>
      </div>
    </footer>
  );
};

export default Footer;
