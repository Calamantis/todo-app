import React, { useEffect, useMemo, useState } from "react";
import { Trash2, Search } from "lucide-react";

interface User {
  userId: number;
  email: string;
  fullName: string;
  role: string;
}

interface Props {
  token: string;
}

const UsersListPanel: React.FC<Props> = ({ token }) => {
  const [users, setUsers] = useState<User[]>([]);
  const [q, setQ] = useState("");

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  const load = async () => {
    const res = await fetch("/api/moderation/users", { headers });
    const data = await res.json();
    setUsers(data);
  };

  const remove = async (id: number) => {
    if (!confirm("Delete user permanently?")) return;
    const res = await fetch(`/api/admin/users/${id}`, {
      method: "DELETE",
      headers,
    });
    if (res.ok) load();
  };

  useEffect(() => { load(); }, []);

  const filtered = useMemo(() => {
  const s = q.toLowerCase();

  return users.filter((u) =>
    (u.email?.toLowerCase().includes(s) ?? false) ||
    (u.fullName?.toLowerCase().includes(s) ?? false)
  );
}, [q, users]);


  return (
    <div className="bg-surface-1 p-6 rounded-xl shadow-lg">
      <h2 className="text-2xl font-semibold mb-4">Users</h2>

      <div className="flex gap-2 mb-4 bg-surface-2 p-2 rounded">
        <Search size={18} />
        <input
          value={q}
          onChange={(e) => setQ(e.target.value)}
          placeholder="Search users..."
          className="bg-transparent outline-none flex-1"
        />
      </div>
        <div className="max-h-[547px] overflow-y-auto space-y-3 custom-scrollbar">
        {filtered.map((u) => (
            <div key={u.userId}
            className="p-3 mb-2 bg-surface-2 rounded flex justify-between">
            <div>
                <div className="font-semibold">{u.fullName}</div>
                <div className="text-sm opacity-70">{u.email}</div>
                <div className="text-xs opacity-50">{u.role}</div>
            </div>

            <button
                onClick={() => remove(u.userId)}
                className="px-2 py-1 rounded bg-red-600/30 hover:bg-red-600/60">
                <Trash2 size={16} />
            </button>
            </div>
        
      ))}
      </div>
    </div>
  );
};

export default UsersListPanel;
