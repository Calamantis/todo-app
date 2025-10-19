import { useState } from "react";
import type { FormEvent } from "react";

export default function LoginPage() {
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [error, setError] = useState<string>("");

  const handleLogin = async (e: FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      const res = await fetch("/api/Auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      if (!res.ok) {
        setError("Invalid email or password.");
        return;
      }

      const data = await res.json();
      sessionStorage.setItem("token", data.token);

      window.location.href = "/timeline";
    } catch (err) {
      console.error(err);
      setError("Server error.");
    }
  };

   const handleLogout = () => {
        sessionStorage.removeItem("token");   // usuń token
        sessionStorage.removeItem("userId");  // jeśli zapisujesz też id
        sessionStorage.removeItem("email");   // jeśli przechowujesz dane użytkownika
  };

  return (
    <div className="flex items-center justify-center h-screen bg-gray-900 text-white">
      <form
        onSubmit={handleLogin}
        className="bg-gray-800 p-8 rounded-xl shadow-md w-96"
      >
        <h2 className="text-2xl font-bold mb-6 text-center">Log in</h2>

        <label className="block mb-3">
          <span>Email</span>
          <input
            type="email"
            className="w-full mt-1 p-2 rounded bg-gray-700 border border-gray-600 focus:outline-none"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </label>

        <label className="block mb-4">
          <span>Password</span>
          <input
            type="password"
            className="w-full mt-1 p-2 rounded bg-gray-700 border border-gray-600 focus:outline-none"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </label>

        {error && <p className="text-red-400 mb-4 text-center">{error}</p>}

        <button
          type="submit"
          className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded"
        >
          Log in
        </button>
      </form>

    <div style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            padding: "1rem",
            backgroundColor: "#2a2a2a",
            color: "white"
        }}>
            <h1>TodoApp</h1>
            <button
                onClick={handleLogout}
                style={{
                    background: "#e74c3c",
                    color: "white",
                    border: "none",
                    padding: "0.5rem 1rem",
                    borderRadius: "8px",
                    cursor: "pointer",
                    fontWeight: 600
                }}
            >
                Logout
            </button>
        </div>

    </div>
  );
}
