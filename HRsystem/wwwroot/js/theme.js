(function () {
  const STORAGE_KEY = "hr-theme";
  const DEFAULT_THEME = "light";

  function getSystemTheme() {
    if (window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches) {
      return "dark";
    }
    return DEFAULT_THEME;
  }

  function getStoredTheme() {
    if (typeof localStorage === "undefined") return getSystemTheme();
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored === "light" || stored === "dark") return stored;
    return getSystemTheme();
  }

  function applyTheme(theme) {
    const root = document.documentElement;
    if (!root.hasAttribute("data-theme") || root.getAttribute("data-theme") !== theme) {
      root.setAttribute("data-theme", theme);
    }
  }

  function initTheme() {
    const theme = getStoredTheme();
    applyTheme(theme);
    return theme;
  }

  function updateIcon(theme) {
    const icon = document.getElementById("themeIcon");
    if (!icon) return;
    if (theme === "dark") {
      icon.className = "bi bi-moon-stars-fill";
    } else {
      icon.className = "bi bi-brightness-high-fill";
    }
  }

  window.HRTheme = {
    getCurrent: function () {
      return getStoredTheme();
    },
    toggle: function () {
      const current = getStoredTheme();
      const next = current === "light" ? "dark" : "light";
      if (typeof localStorage !== "undefined") {
        localStorage.setItem(STORAGE_KEY, next);
      }
      applyTheme(next);
      updateIcon(next);
      return next;
    },
  };

  const initialTheme = initTheme();
  updateIcon(initialTheme);
})();