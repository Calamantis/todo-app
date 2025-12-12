import React, { useEffect, useState } from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
  LabelList,
  Cell
} from "recharts";
import { useAuth } from "../components/AuthContext";
import Footer from "../components/Footer";
import NavigationWrapper from "../components/NavigationWrapper";

interface StatItem {
  categoryId: number;
  categoryName: string;
  totalDurationMinutes: number;
  instanceCount: number;
  colorHex: string;
}

const StatisticsPage: React.FC = () => {
  const { user } = useAuth();

  const [weekStats, setWeekStats] = useState<StatItem[]>([]);
  const [allStats, setAllStats] = useState<StatItem[]>([]);
  const [customStats, setCustomStats] = useState<StatItem[]>([]);

  const [customStart, setCustomStart] = useState("");
  const [customEnd, setCustomEnd] = useState("");

  const [error, setError] = useState("");

  const apiUrl = "/api/Statistics/show-statistics";

  const fetchStats = async (
    start: string,
    end: string,
    setter: (data: StatItem[]) => void
  ) => {
    try {
      const response = await fetch(`${apiUrl}?start=${start}&end=${end}`, {
        headers: {
          Authorization: `Bearer ${user?.token}`,
          role: user?.role ?? "",
          userId: user?.userId.toString() ?? ""
        }
      });

      if (!response.ok) throw new Error("Could not fetch statistics");

      setter(await response.json());
    } catch {
      setError("Failed loading statistics.");
    }
  };

  const now = new Date();
  const iso = (d: Date) => d.toISOString();
  const oneWeekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
  const allTimeStart = new Date("2020-01-01");

  useEffect(() => {
    if (!user) return;

    fetchStats(iso(oneWeekAgo), iso(now), setWeekStats);
    fetchStats(iso(allTimeStart), iso(now), setAllStats);
  }, [user]);

  const handleCustomSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!customStart || !customEnd) {
      setError("Please enter both start and end date.");
      return;
    }
    fetchStats(customStart, customEnd, setCustomStats);
  };

  const Chart = ({ title, data }: { title: string; data: StatItem[] }) => (
    <div className="bg-surface-1 p-4 rounded-xl shadow h-[380px]">
      <h2 className="text-lg font-semibold mb-2 text-center text-text-0">
        {title}
      </h2>

      <ResponsiveContainer width="100%" height="90%">
        <BarChart data={data} margin={{ top: 20}}>
          
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="categoryName" tick={{ fill: "var(--text-color-0)", fontSize: 12 }}/>
          <YAxis />
          <Tooltip />
          <Bar dataKey="totalDurationMinutes">
            <LabelList dataKey="instanceCount" position="top" />
            {data.map((entry, i) => (
              <Cell key={i} fill={entry.colorHex} />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );

  return (
    <div className="bg-surface-0 text-text-0 min-h-screen">
      <NavigationWrapper />

      <div className="p-6 max-w-7xl mx-auto flex flex-col gap-8">
        {error && <p className="text-red-400 text-center">{error}</p>}

        {/* TOP CHARTS */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Chart title="Last 7 Days" data={weekStats} />
          <Chart title="All Time" data={allStats} />
        </div>

        {/* CUSTOM RANGE */}
        <div className="bg-surface-1 p-6 rounded-xl shadow flex flex-col gap-6">
          <h2 className="text-xl font-semibold text-center">
            Custom Date Range
          </h2>

          <form
            onSubmit={handleCustomSubmit}
            className="grid grid-cols-1 md:grid-cols-3 gap-4"
          >
            <div>
              <label className="text-sm font-medium">Start date</label>
              <input
                type="datetime-local"
                value={customStart}
                onChange={(e) => setCustomStart(e.target.value)}
                className="w-full p-2 rounded bg-surface-2 text-text-0"
              />
            </div>

            <div>
              <label className="text-sm font-medium">End date</label>
              <input
                type="datetime-local"
                value={customEnd}
                onChange={(e) => setCustomEnd(e.target.value)}
                className="w-full p-2 rounded bg-surface-2 text-text-0"
              />
            </div>

            <div className="flex items-end">
              <button
                type="submit"
                className="w-full py-2 rounded bg-accent-0 hover:bg-accent-1 font-semibold"
              >
                Load chart
              </button>
            </div>
          </form>

          {customStats.length > 0 && (
            <div className="h-[350px]">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={customStats}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="categoryName" tick={{ fill: "var(--text-color-0)", fontSize: 12 }}/>
                  <YAxis />
                  <Tooltip />
                  <Bar dataKey="totalDurationMinutes">
                    {customStats.map((e, i) => (
                      <Cell key={i} fill={e.colorHex} />
                    ))}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </div>
          )}
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default StatisticsPage;
