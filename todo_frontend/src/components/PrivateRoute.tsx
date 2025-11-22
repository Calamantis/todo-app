import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

interface PrivateRouteProps {
  element: React.ReactNode;
  role: 'Guest' | 'User' | 'Moderator' | 'Admin';
}

const PrivateRoute: React.FC<PrivateRouteProps> = ({ element, role }) => {
  const { user, loading } = useAuth();

  // Jeśli dane są jeszcze ładowane, możemy pokazać np. spinner
  if (loading) {
    return <div>Loading...</div>;
  }

  // Jeśli użytkownik nie jest zalogowany, przekieruj do logowania
  if (!user || !user.token) {
    return <Navigate to="/login" />;
  }

  // Jeśli użytkownik ma niewłaściwą rolę, przekieruj na Unauthorized
  if (role && user.role !== role) {
    return <Navigate to="/unauthorized" />;
  }

  // Jeśli wszystko jest ok, renderujemy element
  return <>{element}</>;
};


export default PrivateRoute;
