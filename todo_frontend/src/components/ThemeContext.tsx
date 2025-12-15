import React, { createContext, useState, useContext, useEffect } from 'react';
import type { ReactNode } from 'react';

// Typy dostępnych motywów
type Theme = 'light' | 'dark' | 'blue' | 'green' | 'contrast';

interface ThemeContextType {
  theme: Theme;
  setTheme: (theme: Theme) => void;
  themeStyles: Record<Theme, string>;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

// Hook do używania ThemeContext
export const useTheme = (): ThemeContextType => {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
};

// Provider do ustawiania motywu
export const ThemeProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [theme, setTheme] = useState<Theme>('dark'); // Domyślny motyw

  const themeStyles: Record<Theme, string> = {
    light: 'bg-surface-1 text-text-0',
    dark: 'bg-surface-1 text-text-0',
    blue: 'bg-blue-500 text-white',
    green: 'bg-green-500 text-white',
    contrast: 'bg-black text-yellow-400',
  };

  // Ustawiamy motyw przy pierwszym załadowaniu aplikacji
  useEffect(() => {
    const savedTheme = localStorage.getItem('theme') as Theme | null;
    if (savedTheme) {
      setTheme(savedTheme);
      document.documentElement.setAttribute("data-theme", savedTheme);
    } else {
      document.documentElement.setAttribute("data-theme", theme);
    }
  }, []);


  // Funkcja zmieniająca motyw
  const changeTheme = (theme: Theme) => {
    setTheme(theme);
    localStorage.setItem('theme', theme); // Zapisywanie wybranego motywu w localStorage
    document.documentElement.setAttribute('data-theme', theme); // Ustawienie odpowiedniej klasy
  };

  return (
    <ThemeContext.Provider value={{ theme, setTheme: changeTheme, themeStyles }}>
      {children}
    </ThemeContext.Provider>
  );
};
