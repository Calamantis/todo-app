import React from "react";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";

const PrivacyPolicyPage: React.FC = () => {
  return (
    <div>
        <NavigationWrapper/>
    <div className="min-h-screen bg-[var(--background-color)] text-[var(--text-color)] px-6 py-10">
      <div className="max-w-5xl mx-auto bg-white/5 border border-white/10 rounded-xl shadow-xl p-8 backdrop-blur-md">

        <h1 className="text-4xl font-bold mb-6 text-center">
          Privacy Policy
        </h1>

        <p className="opacity-80 mb-8 text-center">
          Last updated: {new Date().toLocaleDateString()}
        </p>

        <section className="space-y-6 leading-relaxed">
          <p>
            This Privacy Policy describes how this application (“the App”)
            collects, uses, and stores user data. The App was developed as part
            of an academic engineering project and is not intended for
            commercial use or public distribution.
          </p>

          <h2 className="text-2xl font-semibold">1. Data Collection</h2>
          <p>The App may collect the following information:</p>
          <ul className="list-disc pl-6">
            <li>Account information (email, display name)</li>
            <li>Profile details voluntarily provided by the user</li>
            <li>Activity data and usage statistics</li>
            <li>Technical information such as device type or browser</li>
          </ul>

          <h2 className="text-2xl font-semibold">2. Purpose of Data Use</h2>
          <p>Data is used solely to:</p>
          <ul className="list-disc pl-6">
            <li>Provide core functionality</li>
            <li>Personalize the user experience</li>
            <li>Ensure system performance and reliability</li>
          </ul>

          <h2 className="text-2xl font-semibold">3. Data Storage</h2>
          <p>
            Data is stored locally or on a development server. No data is sold,
            shared, or transferred to external third parties.
          </p>

          <h2 className="text-2xl font-semibold">4. Security</h2>
          <p>
            Reasonable technical measures are applied, but due to the academic
            nature of the project, production-grade security cannot be
            guaranteed.
          </p>

          <h2 className="text-2xl font-semibold">5. User Rights</h2>
          <p>Users may request:</p>
          <ul className="list-disc pl-6">
            <li>Access to their data</li>
            <li>Updates or modifications</li>
            <li>Deletion of personal information</li>
          </ul>

          <h2 className="text-2xl font-semibold">6. Cookies</h2>
          <p>
            Only technical cookies necessary for login and functionality are
            used. No advertising or tracking cookies are present.
          </p>

          <h2 className="text-2xl font-semibold">7. Changes to This Policy</h2>
          <p>
            This Privacy Policy may be updated as part of the project's ongoing
            development.
          </p>

          <h2 className="text-2xl font-semibold">8. Contact</h2>
          <p>
            For questions regarding this policy, contact the project author at:
          </p>
          <p className="font-semibold">your.email@example.com</p>
        </section>
      </div>
    </div>
    <Footer/>
    </div>
  );
};

export default PrivacyPolicyPage;
