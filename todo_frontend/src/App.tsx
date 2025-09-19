// import { useState } from 'react'
// import reactLogo from './assets/react.svg'
// import viteLogo from '/vite.svg'
// import './App.css'

// function App() {
//   const [count, setCount] = useState(0)

//   return (
//     <>
//       <div>
//         <a href="https://vite.dev" target="_blank">
//           <img src={viteLogo} className="logo" alt="Vite logo" />
//         </a>
//         <a href="https://react.dev" target="_blank">
//           <img src={reactLogo} className="logo react" alt="React logo" />
//         </a>
//       </div>
//       <h1>Vite + React</h1>
//       <div className="card">
//         <button onClick={() => setCount((count) => count + 1)}>
//           count is {count}
//         </button>
//         <p>
//           Edit <code>src/App.tsx</code> and save to test HMR
//         </p>
//       </div>
//       <p className="read-the-docs">
//         Click on the Vite and React logos to learn more
//       </p>
//     </>
//   )
// }

// export default App


import { useEffect, useState } from "react";
import { getUsers, addUser } from "./api";
import type { User } from "./api";

function App() {
  // lista użytkowników (tablica Userów)
  const [users, setUsers] = useState<User[]>([]);

  // nowy użytkownik (bez id, bo to nada backend)
  const [newUser, setNewUser] = useState<Omit<User, "id">>({
    name: "",
    email: "",
  });

  useEffect(() => {
    getUsers().then(setUsers);
  }, []);

  const handleAddUser = async () => {
    const created = await addUser(newUser);
    setUsers([...users, created]);
    setNewUser({ name: "", email: "" });
  };

  return (
    <div>
      <h1>Lista użytkowników</h1>
      <ul>
        {users.map((u) => (
          <li key={u.id}>
            {u.name} ({u.email})
          </li>
        ))}
      </ul>

      <h2>Dodaj użytkownika</h2>
      <input
        type="text"
        placeholder="Imię"
        value={newUser.name}
        onChange={(e) => setNewUser({ ...newUser, name: e.target.value })}
      />
      <input
        type="email"
        placeholder="Email"
        value={newUser.email}
        onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
      />
      <button onClick={handleAddUser}>Dodaj</button>
    </div>
  );
}

export default App;
