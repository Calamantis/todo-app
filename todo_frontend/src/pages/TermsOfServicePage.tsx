import React from "react";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";

const TermsOfServicePage: React.FC = () => {
  return (
    <div>
        <NavigationWrapper/>
    <div className="min-h-screen bg-surface-0 text-text-0 px-6 py-10">
      <div className="max-w-5xl mx-auto bg-surface-1 rounded-xl shadow-xl p-8 backdrop-blur-md">

        <h1 className="text-4xl font-bold mb-6 text-center">
          Terms of Service
        </h1>

        <p className="opacity-80 mb-8 text-center">
          Last updated: {new Date().toLocaleDateString()}
        </p>

        <section className="space-y-6 leading-relaxed">

          <p>
            These Terms of Service govern the use of this application (“the
            App”). The App was developed solely for academic and educational
            purposes and is not intended for commercial distribution.
          </p>

          <h2 className="text-2xl font-semibold">1. Acceptance of Terms</h2>
          <p>
            By using the App, you agree to these Terms. If you do not agree,
            please discontinue use of the App.
          </p>

          <h2 className="text-2xl font-semibold">2. Intended Use</h2>
          <p>The App is intended for:</p>
          <ul className="list-disc pl-6">
            <li>Academic and educational demonstration</li>
            <li>Non-commercial research and development</li>
            <li>Testing and prototyping purposes</li>
          </ul>

          <h2 className="text-2xl font-semibold">3. User Responsibilities</h2>
          <ul className="list-disc pl-6">
            <li>Provide accurate information when necessary</li>
            <li>Use the App only for its intended purpose</li>
            <li>Avoid attempting to harm or misuse the system</li>
          </ul>

          <h2 className="text-2xl font-semibold">4. Limitation of Liability</h2>
          <p>
            The App is provided “as is” without warranties of any kind. The
            author is not responsible for data loss, errors, or system
            instability due to the prototype nature of the project.
          </p>

          <h2 className="text-2xl font-semibold">5. Accounts and Data</h2>
          <p>
            User accounts and related data may be modified, reset, or removed at
            any time during development.
          </p>

          <h2 className="text-2xl font-semibold">6. Intellectual Property</h2>
          <p>
            All source code, design, and content remain the property of the
            developer and may be used only for academic purposes unless stated
            otherwise.
          </p>

          <h2 className="text-2xl font-semibold">7. Modifications</h2>
          <p>
            The author may modify, suspend, or discontinue any part of the App
            at any time without notice.
          </p>

          <h2 className="text-2xl font-semibold">8. Governing Law</h2>
          <p>
            These Terms do not constitute a legally binding commercial
            agreement. They are created solely for the context of academic
            project development.
          </p>
        </section>
      </div>
    </div>
    <Footer/>
    </div>
  );
};

export default TermsOfServicePage;
