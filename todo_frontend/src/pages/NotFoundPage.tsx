import React from "react";
import { Link } from "react-router-dom";

const NotFoundPage: React.FC = () => {
  return (
    <div className="min-h-screen w-full bg-surface-0 flex items-center justify-center">
      <div className="text-center text-text-0">
        <p className="text-2xl opacity-80 mb-6">Not Found | 404</p>

        <Link
          to="/"
          className="text-accent-0 small underline"
        >
          Go back to homepage
        </Link>
      </div>
    </div>
  );
};

export default NotFoundPage;
