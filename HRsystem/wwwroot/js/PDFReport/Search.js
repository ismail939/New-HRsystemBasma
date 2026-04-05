document.addEventListener("DOMContentLoaded", () => {
  console.log("Hello");
  let startDate = null;
  let endDate = null;
  let chosenDepartment = "";
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
document.getElementById("departmentSelect").addEventListener("change", (event) => {
    chosenDepartment = event.target.value;
  });
  
    
    function downloadReport() {
      if (!startDate || !endDate) {
        alert("Please select both start and end dates.");
        return;
      }
      const fileName = `report_${chosenDepartment}_${startDate}_to_${endDate}.pdf`;
      fetch(
        `/reports/employeesDH?id=1&startDate=${startDate}&endDate=${endDate}`,
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
});
