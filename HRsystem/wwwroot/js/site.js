document.addEventListener("DOMContentLoaded", function () {
  flatpickr('input[type="date"]', {
    dateFormat: "Y-m-d",
    locale: "ar", // full Arabic locale (digits + weekdays RTL)
  });
  
});


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

    function toggleVisibility(id, isFlex){
      const element = document.getElementById(id);
      if(isFlex){
        if(element.classList.contains("hidden")){
          element.classList.remove("hidden");
          element.classList.add("flex");
        }else{
          element.classList.remove("flex");
          element.classList.add("hidden");
        }
      }
      else{
        if(element.classList.contains("hidden")){
          element.classList.remove("hidden");
        }else{
          element.classList.add("hidden");
        }
      }
    }
    window.toggleVisibility = toggleVisibility;
