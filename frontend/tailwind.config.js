/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {
    extend: {
      colors: {
        surface: {
          950: '#0d0d0f',
          900: '#141416',
          800: '#1c1c20',
          700: '#252529',
          600: '#2e2e33',
          500: '#3a3a40',
          400: '#52525a',
        },
        amber: {
          400: '#fbbf24',
          500: '#f59e0b',
          600: '#d97706',
        },
        coral: '#e8603c',
        pine: '#2d6a4f',
        slate: {
          300: '#cbd5e1',
          400: '#94a3b8',
          500: '#64748b',
        },
      },
      fontFamily: {
        mono: ['JetBrains Mono', 'Fira Code', 'monospace'],
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
};
