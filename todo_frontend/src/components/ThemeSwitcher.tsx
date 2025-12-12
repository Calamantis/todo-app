import React from 'react';
import { useTheme } from './ThemeContext';

const ThemeSwitcher: React.FC = () => {
  const { theme, setTheme } = useTheme();

  const themes = ['light', 'dark', 'blue', 'green'];

  return (
    <div className="flex flex-col gap-3 bg-surface-1 p-4 rounded-lg shadow-md">

      <h3 className="text-xl font-semibold mb-2 text-center text-text-0">
        Application Theme
      </h3>

      {themes.map((t) => (
        <button
          key={t}
          onClick={() => setTheme(t as any)}
          className={`w-full py-2 rounded-lg text-center capitalize
            ${theme === t ? 'bg-accent-0 hover:bg-accent-1 text-text-0' : 'bg-surface-2 text-text-0 hover:bg-surface-3'}
            transition-all`}
        >
          {t}
        </button>
      ))}

    </div>
  );
};

export default ThemeSwitcher;
