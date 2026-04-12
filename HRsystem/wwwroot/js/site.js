document.addEventListener("DOMContentLoaded", function () {
  flatpickr('input[type="date"]', {
    dateFormat: "Y-m-d",
    locale: "ar", // full Arabic locale (digits + weekdays RTL)
  });
  flatpickr("input[type='time']", {
    enableTime: true,
    noCalendar: true,
    dateFormat: "H:i",
    time_24hr: false,
    direction: "ltr", // Ensure the time picker is RTL
    // Center the time input text
});
flatpickr("#startWeekDate", {
    dateFormat: "Y-m-d",
    enable: [
        date => date.getDay() === 6
    ],
    onChange: function(selectedDates) {
        if (selectedDates.length > 0) {
            let start = selectedDates[0];

            let end = new Date(start);
            end.setDate(start.getDate() + 6); // الجمعة

            endPicker.setDate(end);
        }
    }
});

const endPicker = flatpickr("#endWeekDate", {
    dateFormat: "Y-m-d",
    enable: [
        date => date.getDay() === 5
    ]
});
});

function toLocalISODate(date) {
    const offset = date.getTimezoneOffset(); // handles DST automatically
    const local = new Date(date.getTime() - offset * 60000);
    return local.toISOString().split("T")[0];
}
function formatHours(decimalHours) {
    if (decimalHours == null) return "";

    const totalMinutes = Math.round(decimalHours * 60);
    const hours = Math.floor(totalMinutes / 60);
    const minutes = totalMinutes % 60;

    return `${hours}:${minutes.toString().padStart(2, '0')}`;
}
function clearFlatpickrInModal(formId) {
  console.log("🔴Clearing Flatpickr in form:", formId);
  const form = document.getElementById(formId);
  if (!form) return;
  console.log("🔵Modal found:", form);
  form.querySelectorAll('input[type="text"]').forEach((el) => {
    console.log("🟢Clearing Flatpickr for element:", el);
    if (el._flatpickr) {
      console.log("🟡Flatpickr instance found, clearing...");
      el._flatpickr.clear(); // clears selection
      el._flatpickr.setDate(null); // fully reset internal state
      el.value = ""; // clear the input itself
    } else {
      el.value = "";
    }
  });

  // Optional: reset all fields inside the form
  form
    .querySelectorAll("input, textarea, select")
    .forEach((f) => (f.value = ""));
}
window.clearFlatpickrInModal = clearFlatpickrInModal;


// Show Loading
function showLoading() {
  document.getElementById("loadingOverlay").classList.remove("hidden");
}

// Hide Loading
function hideLoading() {
  document.getElementById("loadingOverlay").classList.add("hidden");
}

// Wrap fetch globally
const originalFetch = window.fetch;
window.fetch = async (...args) => {
  showLoading();
  try {
    const result = await originalFetch(...args);
    return result;
  } finally {
    hideLoading();
  }
};

function isNumber(value) {
  return value !== '' && !isNaN(value);
}

function toggleVisibility(id, isFlex) {
  const element = document.getElementById(id);
  if (isFlex) {
    if (element.classList.contains("hidden")) {
      element.classList.remove("hidden");
      element.classList.add("flex");
    } else {
      element.classList.remove("flex");
      element.classList.add("hidden");
    }
  }
  else {
    if (element.classList.contains("hidden")) {
      element.classList.remove("hidden");
    } else {
      element.classList.add("hidden");
    }
  }
}
window.toggleVisibility = toggleVisibility;

function formatDate(date) {
  date = new Date(date);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`;
}

function showDiv(id) {
  document.getElementById(id).classList.remove("hidden");
}
function hideDiv(id) {
  document.getElementById(id).classList.add("hidden");
}
function showDivFlex(id) {
  document.getElementById(id).classList.remove("hidden");
  document.getElementById(id).classList.add("flex");
}
function hideDivFlex(id) {
  document.getElementById(id).classList.remove("flex");
  document.getElementById(id).classList.add("hidden");
}