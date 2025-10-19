// api.ts (albo api.tsx, ale tutaj lepiej .ts)
export interface User {
  id?: number;   // opcjonalne, bo backend sam nadaje
  name: string;
  email: string;
}

const API_URL = "http://localhost:5268/api";

export async function getUsers(): Promise<User[]> {
  const res = await fetch(`${API_URL}/User/all-users`);
  return res.json();
}

export async function addUser(user: User): Promise<User> {
  const res = await fetch(`${API_URL}/User/all-users`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(user),
  });
  return res.json();
}
