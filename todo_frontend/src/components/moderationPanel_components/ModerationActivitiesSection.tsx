// src/components/moderation/ModerationActivitiesSection.tsx

import React, { useMemo, useState } from "react";
import { Pencil, Trash2, Globe, Layers, Clock } from "lucide-react";

interface ModerationActivity {
  activityId: number;
  ownerEmail: string;

  title: string;
  description: string;

  isRecurring: boolean;
  isOnline: boolean;
  instancesCount: number;
}

interface Props {
  activities: ModerationActivity[];
  token: string;
  search: string;
  setEditModal: (value: any) => void;
  doDelete: (type: "activity", id: number, field: string) => void;
}

const ModerationActivitiesSection: React.FC<Props> = ({
  activities,
  token,
  search,
  setEditModal,
  doDelete
}) => {

  const filteredActivities = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return activities;

    return activities.filter(
      (a) =>
        a.title?.toLowerCase().includes(q) ||
        a.description?.toLowerCase().includes(q) ||
        a.ownerEmail?.toLowerCase().includes(q)
    );
  }, [search, activities]);

  return (
    <div className="bg-surface-1 border border-surface-2 p-5 rounded-xl shadow-lg">

      <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
        Activities
      </h2>

      {filteredActivities.length === 0 && <div>No activities found.</div>}

      {filteredActivities.map((a) => (
        <div
          key={a.activityId}
          className="mb-4 p-4 rounded-lg bg-surface-2"
        >
          {/* HEADER */}
          <div className="flex justify-between items-start">
            <div>
              <div className="font-semibold text-lg">{a.title}</div>
              <div className="opacity-70 text-sm">
                Owner: {a.ownerEmail}
              </div>
            </div>            
          </div>

          {/* DESCRIPTION */}
          <div className="text-sm opacity-80 mt-1">
            Desc: {a.description || <i>none</i>}
          </div>

          {/* BADGES */}
          <div className="mt-3 flex gap-3 flex-wrap text-xs">

            {/* ONLINE */}
            <div
              className={`px-3 py-1 rounded-full flex items-center gap-1 font-semibold
                ${
                  a.isOnline
                    ? "bg-green-600/20 text-green-300"
                    : "bg-blue-600/20 text-blue-300"
                }`}
            >
              <Globe size={14} />
              {a.isOnline ? "Online" : "Offline"}
            </div>

            {/* RECURRING */}
            <div
              className={`px-3 py-1 rounded-full flex items-center gap-1 font-semibold
                ${
                  a.isRecurring
                    ? "bg-purple-600/20 text-purple-300"
                    : "bg-gray-500/20 text-gray-300"
                }`}
            >
              <Clock size={14} />
              {a.isRecurring ? "Recurring" : "One-time"}
            </div>

            {/* INSTANCES COUNT */}
            <div className="px-3 py-1 rounded-full flex items-center gap-1 font-semibold bg-white/10 text-white">
              <Layers size={14} />
              {a.instancesCount} instances
            </div>
          </div>

          {/* EDIT DESCRIPTION */}
          <div className="grid grid-cols-2 gap-3 mt-4 md:grid-cols-4">


              <button
                onClick={() =>
                  setEditModal({
                    id: a.activityId,
                    type: "activity",
                    field: "title",
                  })
                }
                className="flex items-center gap-1 px-2 py-1 rounded bg-blue-600/20 hover:bg-blue-600/40 justify-center"
              >
                <Pencil size={18} /> Edit title
              </button>

              <button
                onClick={() => doDelete("activity", a.activityId, "title")}
                className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40 justify-center"
              >
                <Trash2 size={18} /> Remove title
              </button>


            <button
              onClick={() =>
                setEditModal({
                  id: a.activityId,
                  type: "activity",
                  field: "description",
                })
              }
              className="flex items-center gap-1 px-2 py-1 rounded bg-blue-600/20 hover:bg-blue-600/40 justify-center"
            >
              <Pencil size={16} /> Edit desc
            </button>

            <button
              onClick={() => doDelete("activity", a.activityId, "description")}
              className="flex items-center gap-1 px-2 py-1 rounded bg-red-600/20 hover:bg-red-600/40 justify-center"
            >
              <Trash2 size={16} /> Remove desc
            </button>
          </div>
        </div>
      ))}
    </div>
  );
};

export default ModerationActivitiesSection;
