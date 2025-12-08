import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import { AuthProvider } from './components/AuthContext';
import PrivateRoute from './components/PrivateRoute';

import { ThemeProvider } from './components/ThemeContext';

import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import AboutPage from './pages/AboutPage';
import GettingStartedPage from './pages/GettingStartedPage';

import OnlineActivitiesPage from './pages/OnlineActivitiesPage';

import TimelinePage from './pages/TimelinePage';
import ActivityPage from "./pages/ActivityPage";
import UserProfilePage from './pages/UserProfilePage';
import StatisticsPage from './pages/StatisticsPage';

import ModerationPanelPage from "./pages/ModerationPanelPage";

import AdminPanelPage from './pages/AdminPanelPage';

import NotFoundPage from './pages/NotFoundPage';
import UnauthorizedPage from "./pages/UnauthorizedPage";
import SocialPage from "./pages/SocialPage";
import PrivacyPolicyPage from "./pages/PrivacyPolicyPage";
import TermsOfServicePage from "./pages/TermsOfServicePage";
import ContactPage from "./pages/ContactPage";

import NotificationsPage from "./pages/NotificationsPage";



export default function App() {
  return (
    <ThemeProvider>
    <AuthProvider>
    <Router>
      <Routes>
        {/* guest */}
        <Route path="/" element={<HomePage/>} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage/>} />
        <Route path="/about" element={<AboutPage/>} />
        <Route path="/contact" element={<ContactPage/>} />
        <Route path="/privacy-policy" element={<PrivacyPolicyPage/>} />
        <Route path="/terms-of-service" element={<TermsOfServicePage/>} />
        <Route path="/getting-started" element={<GettingStartedPage/>} />
        

        {/* user */}
        <Route path="/timeline" element={<PrivateRoute element={<TimelinePage/>} allowedRoles={['User', 'Moderator', 'Admin']} />} />
        <Route path="/activity" element={<PrivateRoute element={<ActivityPage/>} allowedRoles={['User', 'Moderator', 'Admin']}/>} />
        <Route path="/profile" element={<PrivateRoute element={<UserProfilePage />} allowedRoles={['User', 'Moderator', 'Admin']} />}/>
        <Route path="/statistics" element={<PrivateRoute element={<StatisticsPage />} allowedRoles={['User', 'Moderator', 'Admin']} />} /> 
        <Route path="/social" element={<PrivateRoute element={<SocialPage />} allowedRoles={['User', 'Moderator', 'Admin']}/>} /> 
        <Route path="/online-activity" element={<PrivateRoute element={<OnlineActivitiesPage/>} allowedRoles={['User', 'Moderator', 'Admin']}/>} />
        <Route path="/activity" element={<PrivateRoute element={<ActivityPage/>} allowedRoles={['User', 'Moderator', 'Admin']}/>} />
        <Route path="/notifications" element={<PrivateRoute element={<NotificationsPage/>} allowedRoles={['User', 'Moderator', 'Admin']}/>} />

        {/* moderator */} 

        <Route path="/moderation-panel" element={<PrivateRoute element={<ModerationPanelPage/>} allowedRoles={['Moderator', 'Admin']} /> } />

        {/* admin */} 

        <Route path="/administrative-panel" element={<PrivateRoute element={<AdminPanelPage/>} allowedRoles={['Admin']} /> } />


        {/* not found */} 

        <Route path="*" element={<NotFoundPage />} />

        {/* unauthorized */}

        <Route path="/unauthorized" element={<UnauthorizedPage/>} /> 

      </Routes>
    </Router>
    </AuthProvider>
    </ThemeProvider>
  );
}