import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import landingVideo from "../assets/LandingVideo.mp4"; // <- upewnij się, że ścieżka pasuje
import Footer from "../components/Footer";
import NavigationWrapper from "../components/NavigationWrapper";

const HomePage: React.FC = () => {
  const navigate = useNavigate();

  const lines = [
    "NO MORE MESSY NOTES",
    "EVERYTHING IN ONE PLACE",
    "PERFECTLY ORGANIZED",
  ];

  const [visibleCount, setVisibleCount] = useState(0);

  useEffect(() => {
    // pokazuj linijki co 600ms
    if (visibleCount >= lines.length) return;

    const t = setTimeout(() => {
      setVisibleCount((v) => v + 1);
    }, 600);

    return () => clearTimeout(t);
  }, [visibleCount]);

  return (
    <div>
        <NavigationWrapper/>
    <div className="relative min-h-screen w-full overflow-hidden bg-black">
      {/* Background video */}
      <video
        className="absolute inset-0 h-full w-full object-cover scale-105
                   blur-md brightness-50"
        src={landingVideo}
        autoPlay
        muted
        loop
        playsInline
      />

      {/* Dark overlay gradient for readability */}
      <div className="absolute inset-0 bg-gradient-to-b from-black/50 via-black/40 to-black/70" />

      {/* Content */}
      <div className="relative z-10 flex min-h-screen  pt-[20vh] justify-center px-6">
        <div className="text-center max-w-3xl">
          {/* Animated lines */}
          <div className="space-y-4">
            {lines.map((line, i) => (
              <h1
                key={line}
                className={`text-white font-extrabold tracking-tight
                            text-3xl sm:text-4xl md:text-5xl lg:text-6xl
                            transition-all duration-700 ease-out
                            ${i < visibleCount ? "opacity-100 translate-y-0" : "opacity-0 translate-y-3"}`}
                style={{ transitionDelay: `${i * 120}ms` }}
              >
                {line}
              </h1>
            ))}
          </div>

          {/* CTA button (appears after last line) */}
          <div
            className={`mt-10 transition-all duration-700
                        ${visibleCount >= lines.length ? "opacity-100 translate-y-0" : "opacity-0 translate-y-3"}`}
          >
            <button
              onClick={() => navigate("/register")}
              className="px-8 py-3 rounded-2xl font-semibold text-lg
                         bg-white text-black hover:bg-gray-200
                         shadow-xl shadow-black/40 transition"
            >
              JOIN NOW
            </button>
          </div>

          {/* small subtitle optional */}
          <p className="mt-6 text-gray-200/80 text-sm sm:text-base">
            Organize your life, build habits, stay on track.
          </p>
        </div>
      </div>
    </div>
    <Footer/>
    </div>
  );
};

export default HomePage;
