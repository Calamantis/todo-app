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
        <Route path="/timeline" element={<PrivateRoute element={<TimelinePage/>} role="User"/>} />
        <Route path="/activity" element={<PrivateRoute element={<ActivityPage/>} role="User"/>} />
        <Route path="/profile" element={<PrivateRoute element={<UserProfilePage />} role="User"/>}/>
        <Route path="/statistics" element={<PrivateRoute element={<StatisticsPage />} role="User" />} /> 
        <Route path="/social" element={<PrivateRoute element={<SocialPage />} role="User" />} /> 
        <Route path="/online-activity" element={<PrivateRoute element={<OnlineActivitiesPage/>} role="User"/>} />
        {/* <Route path="/activity-creator" element={<ActivityCreatorPage />} />
        <Route path="/notification-creator" element={<NotificationCreatorPage />} />
        <Route path="/statistics" element={<StatisticsPage/>} /> */}

        {/* moderator */} 

        <Route path="/moderation-panel" element={<PrivateRoute element={<ModerationPanelPage/>} role="Moderator"/> } />

        {/* admin */} 

        <Route path="/administrative-panel" element={<PrivateRoute element={<AdminPanelPage/>} role="Admin" /> } />


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