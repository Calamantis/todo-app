import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../components/AuthContext';
import { useTheme } from '../components/ThemeContext';
import ThemeSwitcher from '../components/ThemeSwitcher';

const LoginPage: React.FC = () => {
  const { login } = useAuth();  // Funkcja login z AuthContext
  const { theme, themeStyles } = useTheme(); // theme z kontekstu
  const navigate = useNavigate();  // Hook do nawigacji
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string>('');  // Stan do błędów

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');  // Resetujemy ewentualny wcześniejszy błąd

    try {
      // Wysyłamy dane logowania do backendu
      const res = await fetch('/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
      });

      if (!res.ok) {
        setError('Invalid email or password.');  // Jeśli błąd logowania
        return;
      }

      const data = await res.json();
      const { token, role, userId } = data;  // Pobieramy token i rolę z odpowiedzi

      // Zapisujemy token i rolę w AuthContext oraz localStorage
      login(token, role, userId);

      // Przekierowanie na stronę "timeline" po udanym logowaniu
      navigate('/timeline');
    } catch (err) {
      console.error(err);
      setError('Server error.');  // Błąd serwera
    }
  };

  const formClasses = themeStyles[theme]; // Dynamiczne przypisanie klas motywu
  const inputClasses = `w-full mt-1 p-2 rounded bg-[var(--background-color)] border-[var(--primary-color)] focus:outline-none`;
  const buttonClasses = `w-full bg-[var(--primary-color)] hover:bg-[var(--secondary-color)] text-white font-semibold py-2 rounded`;
  const bgClasses = `bg-[var(--background-color)]`;

  return (
    <div className={`flex items-center justify-center h-screen ${bgClasses}`}>
      <ThemeSwitcher/>
      <form
        onSubmit={handleLogin}
        className={`p-8 rounded-xl shadow-md w-96 ${formClasses}`}
      >
        <h2 className="text-2xl font-bold mb-6 text-center">Log in</h2>

        <label className="block mb-3">
          <span>Email</span>
          <input
            type="email"
            className={`w-full mt-1 p-2 rounded ${inputClasses} focus:outline-none`}
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </label>

        <label className="block mb-4">
          <span>Password</span>
          <input
            type="password"
            className={`w-full mt-1 p-2 rounded ${inputClasses} focus:outline-none`}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </label>

        {error && <p className="text-red-400 mb-4 text-center">{error}</p>}

        <button
          type="submit"
          className={`w-full ${buttonClasses} text-white font-semibold py-2 rounded`}
        >
          Log in
        </button>
      </form>
    </div>
  );
};

export default LoginPage;
