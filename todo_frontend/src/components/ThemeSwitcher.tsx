import React from 'react';
import { useTheme } from './ThemeContext';

const ThemeSwitcher: React.FC = () => {
  const { theme, setTheme } = useTheme();

  const themes = ['light', 'dark', 'blue', 'green'];

  return (
    <div className="flex flex-col gap-3">

      <h3 className="text-xl font-semibold mb-2 text-center">
        Application Theme
      </h3>

      {themes.map((t) => (
        <button
          key={t}
          onClick={() => setTheme(t as any)}
          className={`w-full py-2 rounded-lg text-center capitalize
            ${theme === t ? 'bg-surface-0 text-text-0' : 'bg-[var(--tertiary-color)] text-[var(--text-color)] hover:bg-[var(--background-color)]-100'}
            transition-all`}
        >
          {t}
        </button>
      ))}

    </div>
  );
};

export default ThemeSwitcher;
