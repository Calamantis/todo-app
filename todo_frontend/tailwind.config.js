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
        'surface-0': 'var(--clr-surface-0)',
        'surface-1': 'var(--clr-surface-1)',
        'surface-2': 'var(--clr-surface-2)',
        'surface-3': 'var(--clr-surface-3)',
        'surface-4': 'var(--clr-surface-4)',
        'accent-0': 'var(--accent-color-0)',
        'accent-1': 'var(--accent-color-1)',
        'accent-2': 'var(--accent-color-2)',
        'accent-3': 'var(--accent-color-3)',
        'accent-4': 'var(--accent-color-4)',
        'text-0': 'var(--text-color-0)',
      }
    },
  },
  plugins: [],
}