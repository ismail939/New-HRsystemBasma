document.addEventListener("DOMContentLoaded", () => {
    console.log("Hello");

    const pdfStartDate = document.getElementById("pdfStartDate");
    const pdfEndDate = document.getElementById("pdfEndDate");

    pdfStartDate.addEventListener("change", (event) => {
        const selectedDate = event.target.value; // get the selected date
        pdfEndDate._flatpickr.set("minDate", selectedDate);
    });
});
