import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../components/AuthContext'; // Jeśli używasz AuthContext
import { useTheme } from '../components/ThemeContext';
import NavigationWrapper from '../components/NavigationWrapper';
import Footer from '../components/Footer';

const RegisterPage: React.FC = () => {
  const { login } = useAuth();  // Funkcja login z AuthContext
  const { theme, themeStyles } = useTheme(); // theme z kontekstu
  const navigate = useNavigate();  // Hook do nawigacji
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [fullName, setFullName] = useState('');  // Pole FullName
  const [error, setError] = useState<string>('');  // Stan do błędów

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');  // Resetujemy ewentualny wcześniejszy błąd

    try {
      // Wysyłamy dane rejestracyjne do backendu
      const res = await fetch('/api/Auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password, fullName }),  // Zmieniony request body
      });

      if (!res.ok) {
        const data = await res.json();
        setError(data.message || 'Invalid email or password.');  // Jeśli błąd rejestracji
        return;
      }

      const data = await res.json();
      const { token, role, userId } = data;  // Pobieramy token i rolę z odpowiedzi

      // Zapisujemy token i rolę w AuthContext oraz localStorage
      login(token, role, userId);

      // Przekierowanie na stronę "timeline" po udanej rejestracji
      navigate('/timeline');
    } catch (err) {
      console.error(err);
      setError('Server error.');  // Błąd serwera
    }
  };

  const formClasses = themeStyles[theme]; // Dynamiczne przypisanie klas motywu
  const inputClasses = `w-full mt-1 p-2 rounded bg-surface-2 focus:outline-none`;
  const buttonClasses = `w-full bg-accent-0 hover:bg-accent-1 text-text-0 font-semibold py-2 rounded`;
  const bgClasses = `bg-surface-0`;

  return (
    <div>
      <NavigationWrapper />
      <div className={`flex items-center justify-center h-screen ${bgClasses}`}>
        <form
          onSubmit={handleRegister}
          className={`p-8 rounded-xl shadow-md w-96 ${formClasses}`}
        >
          <h2 className="text-2xl font-bold mb-6 text-center">Register</h2>

          {/* Full Name */}
          <label className="block mb-3">
            <span>Full Name</span>
            <input
              type="text"
              className={`w-full mt-1 p-2 rounded ${inputClasses} focus:outline-none`}
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              required
            />
          </label>

          {/* Email */}
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

          {/* Password */}
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

          {/* Error message */}
          {error && <p className="text-red-400 mb-4 text-center">{error}</p>}

          <button
            type="submit"
            className={`w-full ${buttonClasses} text-text-0 font-semibold py-2 rounded`}
          >
            Register
          </button>

          {/* Link to Login */}
          <div className="mt-4 text-center">
            <a href="/login" className="text-xs underline text-accent-0">
              Already have an account? Log in here.
            </a>
          </div>
        </form>
      </div>
      <Footer />
    </div>
  );
};

export default RegisterPage;
