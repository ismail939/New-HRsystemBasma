document.addEventListener("DOMContentLoaded", () => {
  console.log("Hello");
  let startDate = null;
  let endDate = null;
  let chosenDepartment = "";
  let departmentId = null;
  const pdfStartDate = document.getElementById("pdfStartDate");
  const pdfEndDate = document.getElementById("pdfEndDate");

  pdfStartDate.addEventListener("change", (event) => {
    startDate = event.target.value; // get the selected date
    pdfEndDate._flatpickr.set("minDate", startDate);
    pdfEndDate.disabled = false; // enable the end date input
    if (endDate && endDate < startDate) {
      showDivFlex("dateWarningDiv");
    }
  });
  pdfEndDate.addEventListener("change", (event) => {
    endDate = event.target.value;
    hideDivFlex("dateWarningDiv");
  });

  function openSelectModal() {
    showDivFlex("selectModal");
  }
  window.openSelectModal = openSelectModal;
  function closeSelectModal() {
    hideDivFlex("selectModal");
  }
  window.closeSelectModal = closeSelectModal;
  document.getElementById("selectModal").addEventListener("click", (event) => {
    if (event.target === document.getElementById("selectModal")) {
      closeSelectModal();
    }
  });
  function openSelectDailyModal() {
    showDivFlex("selectDailyModal");
  }
  window.openSelectDailyModal = openSelectDailyModal;
  function closeSelectDailyModal() {
    hideDivFlex("selectDailyModal");
  }
  window.closeSelectDailyModal = closeSelectDailyModal;
  document.getElementById("selectDailyModal").addEventListener("click", (event) => {
    if (event.target === document.getElementById("selectDailyModal")) {
      closeSelectDailyModal();
    }
  });
// document.getElementById("departmentSelect").addEventListener("change", (event) => {
//     chosenDepartment = event.target.value;

//   });
  
    
    function downloadReport() {
      if (!startDate || !endDate) {
        alert("Please select both start and end dates.");
        return;
      }
      let select = document.getElementById("departmentSelect");
      let selectedOption = select.options[select.selectedIndex];

      let id = select.value;
      let code = selectedOption.getAttribute("data-code");
      const fileName = `report_${code}_${startDate}_to_${endDate}.pdf`;
      fetch(
        `/reports/employeesDH?id=${id}&startDate=${startDate}&endDate=${endDate}`,
      )
        .then((res) => res.blob())
        .then((blob) => {
          const url = window.URL.createObjectURL(blob);

          const a = document.createElement("a");
          a.href = url;
          a.download = fileName;
          document.body.appendChild(a);
          a.click();
          a.remove();

          window.URL.revokeObjectURL(url);
        });
    }
    window.downloadReport = downloadReport;
    function downloadDailyReport() {
      if (!startDate || !endDate) {
        alert("Please select both start and end dates.");
        return;
      }
      let select = document.getElementById("department2Select");
      let selectedOption = select.options[select.selectedIndex];

      let id = select.value;
      let code = selectedOption.getAttribute("data-code");
      const fileName = `report_${code}_${startDate}_to_${endDate}.pdf`;
      fetch(
        `/reports/employeesDaily?id=${id}&startDate=${startDate}&endDate=${endDate}`,
      )
        .then((res) => res.blob())
        .then((blob) => {
          const url = window.URL.createObjectURL(blob);

          const a = document.createElement("a");
          a.href = url;
          a.download = fileName;
          document.body.appendChild(a);
          a.click();
          a.remove();

          window.URL.revokeObjectURL(url);
        });
    }
    window.downloadDailyReport = downloadDailyReport;

    function downloadManagersReport() {
      if (!startDate || !endDate) {
        alert("Please select both start and end dates.");
        return;
      }
      const fileName = `Managers_report_${startDate}_to_${endDate}.pdf`;
      fetch(
        `/reports/managersDH?startDate=${startDate}&endDate=${endDate}`,
      )
        .then((res) => res.blob())
        .then((blob) => {
          const url = window.URL.createObjectURL(blob);

          const a = document.createElement("a");
          a.href = url;
          a.download = fileName;
          document.body.appendChild(a);
          a.click();
          a.remove();

          window.URL.revokeObjectURL(url);
        });
    }
    window.downloadManagersReport = downloadManagersReport;
});
