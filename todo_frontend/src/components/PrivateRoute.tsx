import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

interface PrivateRouteProps {
  element: React.ReactNode;
  allowedRoles: Array<'Guest' | 'User' | 'Moderator' | 'Admin'>;
}

const PrivateRoute: React.FC<PrivateRouteProps> = ({ element, allowedRoles }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!user || !user.token) {
    return <Navigate to="/login" />;
  }

  // NOWA LOGIKA — sprawdzamy tablicę ról zamiast jednej roli
  if (!allowedRoles.includes(user.role)) {
    return <Navigate to="/unauthorized" />;
  }

  return <>{element}</>;
};

export default PrivateRoute;
