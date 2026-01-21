/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.{cshtml,razor}",
    "./Views/**/*.{cshtml,razor}",
    "./wwwroot/**/*.html",
  ],
  theme: {
    extend: {
      colors: {
        color1: "#BAC8B1", // offwhite
        color2: "#7B9669", //light green
        color3: "#404E3B", //dark green
        color4: "#c0c25892", //highlight yellow
        color5: "#6C8480", //dark dark
      },
    },
  },
  plugins: [],
};

