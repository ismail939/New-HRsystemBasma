document.addEventListener("DOMContentLoaded", () => {
    console.log("Hello");
    let startDate = null;
    let endDate = null;
    const pdfStartDate = document.getElementById("pdfStartDate");
    const pdfEndDate = document.getElementById("pdfEndDate");

    pdfStartDate.addEventListener("change", (event) => {
        startDate = event.target.value; // get the selected date
        pdfEndDate._flatpickr.set("minDate", startDate);
        pdfEndDate.disabled = false; // enable the end date input
        if(endDate && endDate < startDate) {
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
        }});
});
