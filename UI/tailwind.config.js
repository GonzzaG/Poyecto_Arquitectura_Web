/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./*.aspx",
    "./*.master",
    "./*.Master",
    "./*.ascx",
    "./**/*.aspx",
    "./**/*.master",
    "./**/*.Master",
    "./**/*.ascx",
    "./Scripts/**/*.js"
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Inter", "ui-sans-serif", "system-ui", "sans-serif"]
      }
    }
  },
  plugins: []
};
