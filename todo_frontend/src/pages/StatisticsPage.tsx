import React, { useEffect, useState } from "react";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid, LabelList, Cell } from "recharts";
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
  const [monthStats, setMonthStats] = useState<StatItem[]>([]);
  const [allStats, setAllStats] = useState<StatItem[]>([]);
  const [customStats, setCustomStats] = useState<StatItem[]>([]);

  const [customStart, setCustomStart] = useState("");
  const [customEnd, setCustomEnd] = useState("");

  const [error, setError] = useState("");

  const apiUrl = "/api/Statistics/show-statistics";

  // Helper to fetch stats
  const fetchStats = async (start: string, end: string, setter: (data: StatItem[]) => void) => {
    try {
      const response = await fetch(`${apiUrl}?start=${start}&end=${end}`, {
        headers: {
          Authorization: `Bearer ${user?.token}`,
          role: user?.role ?? "",
          userId: user?.userId.toString() ?? ""
        }
      });

      if (!response.ok) throw new Error("Could not fetch statistics");

      const data = await response.json();
      setter(data);
    } catch (err) {
      setError("Failed loading statistics.");
    }
  };

  // Calculate date ranges
  const now = new Date();
  const iso = (d: Date) => d.toISOString();

  const oneWeekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
  const oneMonthAgo = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
  const allTimeStart = new Date("2020-01-01");

  // Load the 3 auto charts
  useEffect(() => {
    if (!user) return;

    fetchStats(iso(oneWeekAgo), iso(now), setWeekStats);
    fetchStats(iso(oneMonthAgo), iso(now), setMonthStats);
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
    <div className="bg-[var(--primary-color)] p-4 rounded-xl shadow w-full h-[400px]">
      <h2 className="text-xl font-bold mb-2 text-center">{title}</h2>
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={data}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="categoryName" />
          <YAxis />
          <Tooltip />
          <Bar dataKey="totalDurationMinutes" fill="var(--primary-color)">

                <LabelList dataKey="instanceCount" position="top" />

                {data.map((entry, index) => (
                <Cell key={index} fill={entry.colorHex} />
                ))}

          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );

  return (
    <div className="bg-[var(--background-color)] text-[var(--text-color)]">
    <NavigationWrapper/>
    <div className="min-h-screen p-6">
      {/* <h1 className="text-3xl font-semibold mb-6 text-center pt-6">Activity Statistics</h1> */}

      {error && <p className="text-red-400 text-center mb-4">{error}</p>}

      {/* GRID OF 4 CHARTS */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 max-w-7xl mx-auto">

        {/* Left top */}
        <Chart title="Last Week" data={weekStats} />

        {/* Right top */}
        <Chart title="Last Month" data={monthStats} />

        {/* Left bottom
        <Chart title="All Time" data={allStats} /> */}



{/* Right bottom — custom */}
<div className="col-span-1 lg:col-span-2 flex flex-col gap-6">

  {/* Custom RANGE FORM */}
  <div className="bg-primary text-black p-4 rounded-xl shadow">
    <h2 className="text-xl font-bold mb-4">Choose Custom Date Range</h2>

    <form onSubmit={handleCustomSubmit} className="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div>
        <label className="font-medium">Start Date</label>
        <input
          type="datetime-local"
          value={customStart}
          onChange={(e) => setCustomStart(e.target.value)}
          className="p-2 border rounded w-full"
        />
      </div>

      <div>
        <label className="font-medium">End Date</label>
        <input
          type="datetime-local"
          value={customEnd}
          onChange={(e) => setCustomEnd(e.target.value)}
          className="p-2 border rounded w-full"
        />
      </div>

      <div className="flex items-end">
        <button
          className="w-full py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
          type="submit"
        >
          Load Chart
        </button>
      </div>
    </form>
  </div>

  {/* Custom CHART BOX */}
  <div className="bg-primary p-4 rounded-xl shadow w-full h-[350px]">
    <h2 className="text-xl font-bold mb-2">Custom Range Chart</h2>
    <ResponsiveContainer width="100%" height="100%">
      <BarChart data={customStats}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="categoryName" />
        <YAxis />
        <Tooltip />
        <Bar dataKey="totalDurationMinutes" fill="var(--secondary-color)" />
      </BarChart>
    </ResponsiveContainer>
  </div>

</div>




      </div>
        </div>
      <Footer/>
    </div>
  );
};

export default StatisticsPage;
