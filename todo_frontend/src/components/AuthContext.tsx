import React, { createContext, useState, useEffect, useContext } from 'react';
import type { ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';

interface User {
  token: string;
  role: 'Guest' | 'User' | 'Moderator' | 'Admin';
  userId: string;
}

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (token: string, role: string, userId: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);  // Dodajemy stan loading

  useEffect(() => {
    // Zamiast 'sessionStorage', od razu używamy 'localStorage' po załadowaniu strony
    const token = localStorage.getItem('token') ?? '';
    const role = localStorage.getItem('role') ?? '';
    const userId = localStorage.getItem('userId') ?? '';

    if (token && role) {
      const decodedUserId = userId || getUserIdFromToken(token);
      setUser({
        token,
        role: role as 'Guest' | 'User' | 'Moderator' | 'Admin',
        userId: decodedUserId || '',
      });
    }

    setLoading(false);  // Ustawiamy loading na false po załadowaniu danych
  }, []);

  const getUserIdFromToken = (token: string) => {
    try {
      const decoded: any = jwtDecode(token);
      return decoded.sub;  // Zmienna 'sub' w JWT często przechowuje userId
    } catch (error) {
      console.error("Failed to decode JWT:", error);
      return null;
    }
  };

  const login = (token: string, role: string, userId: string) => {
    setUser({
      token,
      role: role as 'Guest' | 'User' | 'Moderator' | 'Admin',
      userId,
    });
    localStorage.setItem('token', token);
    localStorage.setItem('role', role);
    localStorage.setItem('userId', userId);
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('userId');
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
