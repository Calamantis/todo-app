/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
    "./pages/**/*.{js,ts,jsx,tsx}",
    "./components/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: 'var(--primary-color)',
        secondary: 'var(--secondary-color)',
        teritary: 'var(--teritary-color)',
        quaternary: 'var(--quaternary-color)',
        accent: 'var(--accent-color)',

        background: 'var(--background-color)',
        text: 'var(--text-color)',
        'text-primary': 'var(--text-primary-color)',
        'text-secondary': 'var(--text-secondary-color)',
      }
    },
  },
  plugins: [],
}