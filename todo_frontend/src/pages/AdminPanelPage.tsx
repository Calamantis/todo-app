import React from "react";
import { useAuth } from "../components/AuthContext";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";

import CreateModeratorPanel from "../components/adminPanel_components/CreateModeratorPanel";
import ModeratorsListPanel from "../components/adminPanel_components/ModeratorsListPanel";
import UsersListPanel from "../components/adminPanel_components/UsersListPanel";
import DeleteActivityPanel from "../components/adminPanel_components/DeleteActivityPanel";
import SystemLogsPanel from "../components/adminPanel_components/SystemLogsPanel";
import ChangeAdminPasswordPanel from "../components/adminPanel_components/ChangeAdminPasswordPanel";


const AdminPanelPage: React.FC = () => {
  const { user } = useAuth();
  const token = user?.token ?? "";

  return (
    <div className="min-h-screen flex flex-col bg-surface-0 text-text-0">
      <NavigationWrapper />

      <div className="flex-1 p-6 mx-auto w-full">

        {/* GRID PANELS */}
        <div className="grid grid-cols-1 xl:grid-cols-4 gap-6">
          {/* Column 1 */}
          <div className="flex flex-col gap-6">
            <ChangeAdminPasswordPanel token={token} />
            <CreateModeratorPanel token={token} />
            <ModeratorsListPanel token={token} />
          </div>

          {/* Column 2 */}
          <div className="flex flex-col gap-6">
            <UsersListPanel token={token} />
          </div>

          <div className="flex flex-col gap-6">
            <DeleteActivityPanel token={token} />
          </div>

          {/* Column 3-4 (wide logs panel) */}
          <div className="flex flex-col gap-6">
            <SystemLogsPanel token={token} />
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default AdminPanelPage;
