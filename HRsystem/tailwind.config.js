/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.{cshtml,razor}",
    "./Views/**/*.{cshtml,razor}",
    "./wwwroot/**/*.{html,js}",   // ✅ IMPORTANT
  ],
  theme: {
    extend: {
      colors: {
        color1: "var(--color1)", // offwhite
        color2: "var(--color2)", // light green
        color3: "var(--color3)", // dark green
        color4: "var(--color4)", // highlight yellow
        color5: "var(--color5)", // dark dark
        color6: "var(--color6)", // darker green
      },
    },
  },
  plugins: [],
};

