import React, { useState } from "react";
import NavigationWrapper from "../components/NavigationWrapper";
import Footer from "../components/Footer";

const ContactPage: React.FC = () => {
  const [title, setTitle] = useState("");
  const [message, setMessage] = useState("");
  const [status, setStatus] = useState<"idle" | "sent" | "error">("idle");

  // Dummy send handler (replace later)
  const handleSend = () => {
    if (!title.trim() || !message.trim()) {
      setStatus("error");
      return;
    }

    // Simulate sending
    setTimeout(() => {
      console.log("Dummy send:", { title, message });
      setStatus("sent");
      setTitle("");
      setMessage("");
    }, 600);
  };

  return (
  <div className="min-h-screen flex flex-col bg-surface-0">
    
    {/* Navbar */}
    <NavigationWrapper />

    {/* Main content – rozciąga się automatycznie */}
    <div className="flex-1">
      <div className="relative text-text-0 px-6 py-10">
        <div className="max-w-4xl mx-auto bg-surface-1 rounded-xl shadow-xl p-8 backdrop-blur-md">

          <h1 className="text-4xl font-bold text-center mb-8">Contact Us</h1>

          <p className="text-center opacity-80 mb-10 max-w-2xl mx-auto">
            Have questions, suggestions, or found an issue?  
            Send us a message — this is a prototype contact form for the engineering project.
          </p>

          <div className="space-y-6">
            {/* Title */}
            <div>
              <label className="block mb-2 font-semibold">Title</label>
              <input
                type="text"
                value={title}
                onChange={(e) => { setTitle(e.target.value); setStatus("idle"); }}
                placeholder="Enter message title..."
                className="w-full p-3 rounded-lg bg-surface-2"
              />
            </div>

            {/* Message */}
            <div>
              <label className="block mb-2 font-semibold">Message</label>
              <textarea
                rows={6}
                value={message}
                onChange={(e) => { setMessage(e.target.value); setStatus("idle"); }}
                placeholder="Write your message here..."
                className="w-full p-3 rounded-lg bg-surface-2  resize-none"
              />
            </div>

            {/* Status */}
            {status === "sent" && <p className="text-green-400 font-semibold">Message sent!</p>}
            {status === "error" && <p className="text-red-400 font-semibold">Please fill out all fields.</p>}

            {/* Send button */}
            <div className="flex justify-center">
              <button
                onClick={handleSend}
                className="px-8 py-3 rounded-xl bg-primary text-text-0 font-semibold bg-accent-0 hover:bg-accent-1 transition shadow-lg"
              >
                Send
              </button>
            </div>
          </div>

        </div>
      </div>
    </div>

    {/* Footer – zawsze na dole */}
    <Footer />
  </div>
);
};

export default ContactPage;
