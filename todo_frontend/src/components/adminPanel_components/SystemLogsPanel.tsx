import React, { useEffect, useState } from "react";
import { List } from "lucide-react";

interface Log {
  logId: number;
  userId: number;
  action: string;
  entityType: string;
  entityId: number;
  description: string;
  createdAt: string;
}

interface Props {
  token: string;
}

const SystemLogsPanel: React.FC<Props> = ({ token }) => {
  const [logs, setLogs] = useState<Log[]>([]);

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  };

  useEffect(() => {
    fetch("/api/admin", { headers })
      .then((r) => r.json())
      .then(setLogs);
  }, []);

  return (
    <div className="bg-surface-1 p-6 rounded-xl shadow-lg h-fit">
      <h2 className="text-2xl font-semibold mb-4 flex gap-2">
System Logs
      </h2>

      <div className="max-h-[600px] overflow-y-auto space-y-3 custom-scrollbar">
        {logs.map((l) => (
          <div key={l.logId} className="p-3 bg-surface-2 rounded">
            <div className="font-semibold">
              {l.action} — {l.entityType} #{l.entityId}
            </div>
            <div className="text-sm opacity-70">{l.description}</div>
            <div className="text-xs opacity-50">
              {new Date(l.createdAt).toLocaleString()}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default SystemLogsPanel;
