import React, { useState } from "react";
import { ChevronDown, Pencil, Trash2 } from "lucide-react";
import { useAuth } from "../AuthContext";
import ActivityListItem from "./ActivityListItem";
import EditRecurrenceRuleModal from "./EditRecurrenceRuleModal";
import DeleteRecurrenceRuleModal from "./DeleteRecurrenceRuleModal";
import AddRecurrenceRuleModal from "./AddRecurrenceRuleModal";

interface Activity {
  activityId: number;
  title: string;
  description: string;
  isRecurring: boolean;
  categoryId: number | null;
  categoryName: string | null;
  colorHex: string | null;
  joinCode: string | null;
  isFriendsOnly?: boolean;
}

interface RecurrenceRule {
  recurrenceRuleId: number;
  activityId: number;
  type: string;
  daysOfWeek?: string;
  daysOfMonth?: string;
  dayOfYear?: string;
  interval?: number;
  startTime: string;
  endTime: string;
  dateRangeStart: string;
  dateRangeEnd: string;
  durationMinutes: number;
  isActive: boolean;
}

interface Props {
  activity: Activity;
  onEdit?: (activity: Activity) => void;
  onDelete?: (activityId: number) => void;
}

const ExpandableActivityItem: React.FC<Props> = ({ activity, onEdit, onDelete }) => {
  const { user } = useAuth();
  const [expanded, setExpanded] = useState(false);
  const [rules, setRules] = useState<RecurrenceRule[]>([]);
  const [loadingRules, setLoadingRules] = useState(false);
  const [showEditRuleModal, setShowEditRuleModal] = useState(false);
  const [ruleToEdit, setRuleToEdit] = useState<RecurrenceRule | null>(null);
  const [showDeleteRuleModal, setShowDeleteRuleModal] = useState(false);
  const [ruleToDelete, setRuleToDelete] = useState<RecurrenceRule | null>(null);
  const [showAddRuleModal, setShowAddRuleModal] = useState(false);

  const fetchRecurrenceRules = async () => {
    if (!user || !activity.isRecurring) return;
    setLoadingRules(true);
    try {
      const res = await fetch(
        `/api/ActivityRecurrenceRule/activity/get-activity-recurrence-rules?activityId=${activity.activityId}`,
        {
          method: "GET",
          headers: { Authorization: `Bearer ${user.token}` },
        }
      );
      if (!res.ok) throw new Error("Failed to fetch recurrence rules");
      const data = await res.json();
      setRules(data);
    } catch (err) {
      console.error("Error fetching recurrence rules:", err);
      setRules([]);
    } finally {
      setLoadingRules(false);
    }
  };

  const toggleExpand = async () => {
    if (!expanded && activity.isRecurring) {
      await fetchRecurrenceRules();
    }
    setExpanded(!expanded);
  };

  const handleEditRuleClick = (rule: RecurrenceRule) => {
    setRuleToEdit(rule);
    setShowEditRuleModal(true);
  };

  const handleEditRule = (updatedRule: RecurrenceRule) => {
    setRules(rules.map((r) => (r.recurrenceRuleId === updatedRule.recurrenceRuleId ? updatedRule : r)));
    setShowEditRuleModal(false);
  };

  const handleDeleteRuleClick = (rule: RecurrenceRule) => {
    setRuleToDelete(rule);
    setShowDeleteRuleModal(true);
  };

  const handleDeleteRule = (recurrenceRuleId: number) => {
    setRules(rules.filter((r) => r.recurrenceRuleId !== recurrenceRuleId));
    setShowDeleteRuleModal(false);
  };

  const handleCreateRule = (createdRule: RecurrenceRule) => {
    // Append the newly created rule and ensure expanded state shows it
    setRules((prev) => [createdRule, ...prev]);
    setShowAddRuleModal(false);
  };

  return (
    <div>
      <div className="relative flex items-center">
        <ActivityListItem activity={activity} onEdit={onEdit} onDelete={onDelete} />
        {activity.isRecurring && (
          <button
            onClick={(e) => {
              e.stopPropagation();
              toggleExpand();
            }}
            className="absolute right-16 text-gray-600 hover:text-gray-900 transition"
          >
            <ChevronDown size={18} style={{ transform: expanded ? "rotate(180deg)" : "rotate(0deg)", transition: "transform 0.3s" }} />
          </button>
        )}
      </div>

      {expanded && activity.isRecurring && (
        <div className="ml-4 mt-2 pl-3 border-l-2 border-gray-300">
          <div className="flex items-center justify-between mb-2">
            {loadingRules ? (
              <div className="text-sm text-gray-500">Loading rules...</div>
            ) : (
              <div className="text-sm text-gray-500">{rules.length === 0 ? "No recurrence rules." : `${rules.length} rule(s)`}</div>
            )}

            <div>
              <button onClick={() => setShowAddRuleModal(true)} className="px-2 py-1 bg-green-600 text-white text-xs rounded hover:bg-green-700">Add rule</button>
            </div>
          </div>

          {rules.map((rule) => {
            const dateStart = new Date(rule.dateRangeStart).toLocaleDateString("pl-PL");
            const dateEnd = new Date(rule.dateRangeEnd).toLocaleDateString("pl-PL");

            // Format recurrence type and interval
            let recurrenceText = "";
            if (rule.type === "DAY") {
              if (rule.interval === 1 || !rule.interval) {
                recurrenceText = "Everyday";
              } else {
                recurrenceText = `Every ${rule.interval} days`;
              }
            } else if (rule.type === "WEEK") {
              const weekText = rule.interval && rule.interval !== 1 ? `Every ${rule.interval} weeks` : "Every week";
              const daysOfWeekLabels = (rule.daysOfWeek || "").split(",").map(day => {
                const dayNum = parseInt(day);
                const dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
                return dayNames[dayNum] || day;
              }).join(", ");
              recurrenceText = `${weekText}${daysOfWeekLabels ? ` on ${daysOfWeekLabels}` : ""}`;
            } else if (rule.type === "MONTH") {
              const monthText = rule.interval && rule.interval !== 1 ? `Every ${rule.interval} months` : "Every month";
              const daysDisplay = rule.daysOfMonth ? rule.daysOfMonth.replace(/LAST/g, "Last day") : "";
              recurrenceText = `${monthText}${daysDisplay ? ` on day(s): ${daysDisplay}` : ""}`;
            } else if (rule.type === "YEAR") {
              recurrenceText = "Every year";
              if (rule.dayOfYear) {
                recurrenceText += ` on day ${rule.dayOfYear}`;
              }
            }

            return (
              <div key={rule.recurrenceRuleId} className="text-xs text-gray-700 dark:text-gray-300 mb-3 p-3 bg-gray-50 dark:bg-slate-700 rounded border-l-2 border-gray-300">
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <div className="mb-2"><strong>Recurrence:</strong> {recurrenceText}</div>
                    <div><strong>Time:</strong> {rule.startTime} - {rule.endTime}</div>
                    <div><strong>Duration:</strong> {rule.durationMinutes} minutes</div>
                    <div><strong>Active From:</strong> {dateStart} <strong>to</strong> {dateEnd}</div>
                    <div><strong>Status:</strong> {rule.isActive ? "✓ Active" : "✗ Inactive"}</div>
                  </div>
                  <div className="flex gap-2 flex-shrink-0 ml-2">
                    <button
                      onClick={() => handleEditRuleClick(rule)}
                      className="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300"
                    >
                      <Pencil size={16} />
                    </button>
                    <button
                      onClick={() => handleDeleteRuleClick(rule)}
                      className="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300"
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {showEditRuleModal && ruleToEdit && (
        <EditRecurrenceRuleModal
          rule={ruleToEdit}
          onClose={() => setShowEditRuleModal(false)}
          onEditRule={handleEditRule}
        />
      )}

      {showDeleteRuleModal && ruleToDelete && (
        <DeleteRecurrenceRuleModal
          rule={ruleToDelete}
          onClose={() => setShowDeleteRuleModal(false)}
          onDelete={handleDeleteRule}
        />
      )}

      {showAddRuleModal && (
        <AddRecurrenceRuleModal
          activityId={activity.activityId}
          onClose={() => setShowAddRuleModal(false)}
          onCreateRule={handleCreateRule}
        />
      )}
    </div>
  );
};

export default ExpandableActivityItem;
