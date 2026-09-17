/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{html,ts}'],
  important: true,
  darkMode: 'class',
  theme: {
    extend: {
      screens: {
        // Between lg (1024px) and the point where the full nav bar (7 items + CTA) fits
        // comfortably on one line without wrapping - the nav uses tighter sizing in that gap.
        navlg: '1360px',
      },
      colors: {
        primary: {
          DEFAULT: '#0000FF',
          50: '#EBEBFF',
          100: '#D6D6FF',
          200: '#ADADFF',
          300: '#8585FF',
          400: '#5C5CFF',
          500: '#3333FF',
          600: '#0000FF',
          700: '#0000AA',
          800: '#000066',
          900: '#000022',
        },
        accent: '#10A67A',
        ink: '#0F172A',
        muted: '#64748B',
        line: '#DBE3EF',
        surface: '#F7FAFF',
        valid: '#10A67A',
        warn: '#cf222e',
      },
      fontFamily: {
        sans: [
          'Inter',
          'Segoe UI',
          'Arial',
          'sans-serif',
        ],
        display: [
          'Quicksand',
          'Inter',
          'Segoe UI',
          'Arial',
          'sans-serif',
        ],
      },
      boxShadow: {
        card: '0 22px 60px rgba(15, 23, 42, 0.12)',
      },
    },
  },
  plugins: [],
};
