import React from 'react';
import { useTheme } from './ThemeContext'; // Importujemy hook do ThemeContext

const ThemeSwitcher: React.FC = () => {
  const { theme, setTheme } = useTheme(); // Używamy motywu z ThemeContext

  // Lista dostępnych motywów
  const themes = [
    'light', 'dark', 'blue', 'green'
  ];

  return (
    <div className="flex flex-wrap justify-center gap-4 mt-4">
      <h3 className="text-xl font-semibold mb-4">Select Theme</h3>
      
      {themes.map((t) => (
        <button
          key={t}
          onClick={() => setTheme(t as any)} // Przypisanie motywu
          className={`px-4 py-2 rounded ${theme === t ? 'bg-blue-500 text-white' : 'bg-gray-300 text-black'} transition-colors duration-300`}
        >
          {t.charAt(0).toUpperCase() + t.slice(1)} {/* Wyświetlenie nazwy motywu */}
        </button>
      ))}
    </div>
  );
};

export default ThemeSwitcher;
