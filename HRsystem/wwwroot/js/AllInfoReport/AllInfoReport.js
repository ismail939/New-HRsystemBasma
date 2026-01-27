function toggleData() {
    const id = document.getElementById('dataDiv');
    if (id.classList.contains('hidden')) {
        id.classList.remove('hidden');
        id.classList.add("grid");
    }
    else {
        id.classList.add('hidden');
        id.classList.remove("grid");
    }
}
// get data
async function getData() {
    const start = document.getElementById('startDatePicker').value;
    const end = document.getElementById('endDatePicker').value;
    const employeeId = document.getElementById('chooseEmployee').value;
    if (start && end && employeeId) {
        // fetch for the offdays and show them in the table
        const response = await fetch(`/getData?employeeId=${employeeId}&startDate=${start}&endDate=${end}`);
        const result = await response.json();
        document.getElementById("arrivalsNumber").innerText = result.arrivalsNumber;
        document.getElementById("absencesNumber").innerText = result.absencesNumber;
        document.getElementById("offdays").innerText = result.offdays;
        document.getElementById("offs").innerText = result.offs;
        document.getElementById("penalties").innerText = result.penalties;
    }
}
document.addEventListener("DOMContentLoaded", function () {
    fetchEmployeesForReport();
});
async function fetchEmployeesForReport() {
    try {
        const response = await fetch('/employeesList');
        const employees = await response.json();
        const chooseEmployeeSelect = document.getElementById('chooseEmployee');

        employees.forEach(emp => {
            const option = document.createElement('option');
            option.value = emp.Id;
            option.textContent = emp.Name;
            console.log(option);
            chooseEmployeeSelect.appendChild(option);
        });

        chooseEmployeeSelect.addEventListener('change', function () {
            const employeeId = this.value;
            // Fetch and display report data for selected employee
        });
    } catch (error) {
        console.error('Error fetching employees:', error);
    }
}
document.getElementById('startDatePicker').addEventListener('change', function () {
    const endDateInput = document.getElementById('endDatePicker');
    console.log(this.value);
    endDateInput.disabled = false;
    endDateInput.min = this.value;
    console.log(endDateInput.min);
});
flatpickr("#startDatePicker", {
    dateFormat: "Y-m-d",
    onChange: function (selectedDates, dateStr) {
        const endPicker = document.getElementById("endDatePicker")._flatpickr;

        // enable end date
        endPicker.set("minDate", dateStr);
        endPicker.input.disabled = false;
        if (endPicker.input.value) {
            showOffDays();
        }
        console.log("Start Date:", dateStr);
    }
});

flatpickr("#endDatePicker", {
    dateFormat: "Y-m-d",
    disable: [true],// disable until start date selected
    onChange: async function (selectedDates, dateStr) {
        console.log("End Date:", dateStr);
        getData();
        showArrivals();
        if (!document.getElementById("waitingForEndDateChange").classList.contains("hidden")) {
            document.getElementById("waitingForEndDateChange").classList.add("hidden");
        }
        if (document.getElementById("reportResultsDiv").classList.contains("hidden")) {
            document.getElementById("reportResultsDiv").classList.remove("hidden");
        }
    }
});
async function showOffDays() {
    const start = document.getElementById('startDatePicker').value;
    const end = document.getElementById('endDatePicker').value;
    const employeeId = document.getElementById('chooseEmployee').value;
    if (start && end && employeeId) {
        // fetch for the offdays and show them in the table
        const response = await fetch(`/allInfoReport/getOffDays?employeeId=${employeeId}&startDate=${start}&endDate=${end}`);
        const offDays = await response.json();
        if(offDays.length==0){
            showDiv("noRecordsDiv");
            hideDiv("allInfoReportTable");
            return;
        }
        console.log(JSON.stringify(offDays));
        // populate the table
        const headInnerHTML = `
            <tr>
                <th class="px-4 py-2 border-b">التاريخ</th>
                <th class="px-4 py-2 border-b">نوع الاجازة</th>
            </tr>
        `;
            showDiv("allInfoReportTable");
            hideDiv("noRecordsDiv");
            populateTable(headInnerHTML, offDays);
        

    } else {
        alert("من فضلك اختر الموظف اولا ثم التاريخين");
    }
}
async function showOffs() {
    const start = document.getElementById('startDatePicker').value;
    const end = document.getElementById('endDatePicker').value;
    const employeeId = document.getElementById('chooseEmployee').value;
    if (start && end && employeeId) {
        // fetch for the offdays and show them in the table
        const response = await fetch(`/allInfoReport/getOffs?employeeId=${employeeId}&startDate=${start}&endDate=${end}`);
        const offDays = await response.json();
        if(offDays.length==0){
            showDiv("noRecordsDiv");
            hideDiv("allInfoReportTable");
            return;
        }
        console.log(JSON.stringify(offDays));
        // populate the table
        const headInnerHTML = `
            <tr>
                <th class="px-4 py-2 border-b">التاريخ</th>
                <th class="px-4 py-2 border-b">نوع الاجازة</th>
            </tr>
        `;
            showDiv("allInfoReportTable");
            hideDiv("noRecordsDiv");
            populateTable(headInnerHTML, offDays);
        

    } else {
        alert("من فضلك اختر الموظف اولا ثم التاريخين");
    }
}
async function showAbsences() {
    const start = document.getElementById('startDatePicker').value;
    const end = document.getElementById('endDatePicker').value;
    const employeeId = document.getElementById('chooseEmployee').value;
    if (start && end && employeeId) {
        // fetch for the offdays and show them in the table
        const response = await fetch(`/allInfoReport/getAbsences?employeeId=${employeeId}&startDate=${start}&endDate=${end}`);
        const absences = await response.json();
        if(absences.length==0){
            showDiv("noRecordsDiv");
            hideDiv("allInfoReportTable");
            return;
        }
        console.log(JSON.stringify(absences));
        // populate the table
        const headInnerHTML = `
            <tr>
                <th class="px-4 py-2 border-b">التاريخ</th>
                <th class="px-4 py-2 border-b">النوع</th>
            </tr>
        `;
            showDiv("allInfoReportTable");
            hideDiv("noRecordsDiv");
            populateTable(headInnerHTML, absences);
        

    } else {
        alert("من فضلك اختر الموظف اولا ثم التاريخين");
    }
}
async function showArrivals() {
    const start = document.getElementById('startDatePicker').value;
    const end = document.getElementById('endDatePicker').value;
    const employeeId = document.getElementById('chooseEmployee').value;
    if (start && end && employeeId) {
        // fetch for the offdays and show them in the table
        const response = await fetch(`/allInfoReport/getArrivals?employeeId=${employeeId}&startDate=${start}&endDate=${end}`);
        const arrivals = await response.json();
        if(arrivals.length==0){
            showDiv("noRecordsDiv");
            hideDiv("allInfoReportTable");
            return;
        }
        console.log(JSON.stringify(arrivals));
        // populate the table
        const headInnerHTML = `
                <tr>
                    <th class="px-4 py-2 border-b">التاريخ</th>
                    <th class="px-4 py-2 border-b">النوع</th>
                </tr>
            `;
            showDiv("allInfoReportTable");
            hideDiv("noRecordsDiv");
            populateTable(headInnerHTML, arrivals);
        

    } else {
        alert("من فضلك اختر الموظف اولا ثم التاريخين");
    }
}
async function showPenalties() {
    const start = document.getElementById('startDatePicker').value;
    const end = document.getElementById('endDatePicker').value;
    const employeeId = document.getElementById('chooseEmployee').value;
    if (start && end && employeeId) {
        // fetch for the offdays and show them in the table
        const response = await fetch(`/allInfoReport/getPenalties?employeeId=${employeeId}&startDate=${start}&endDate=${end}`);
        const penalties = await response.json();
        if(penalties.length==0){
            showDiv("noRecordsDiv");
            hideDiv("allInfoReportTable");
            return;
        }
        console.log(JSON.stringify(penalties));
        // populate the table
        const headInnerHTML = `
            <tr>
                <th class="px-4 py-2 border-b">التاريخ</th>
                <th class="px-4 py-2 border-b">القرار</th>
                <th class="px-4 py-2 border-b">نقاط الجزاء</th>
                <th class="px-4 py-2 border-b">سبب الجزاء</th>
            </tr>
        `;
        showDiv("allInfoReportTable");
        hideDiv("noRecordsDiv");
        populateTable(headInnerHTML, penalties);
    } else {
        alert("من فضلك اختر الموظف اولا ثم التاريخين");
    }
}

function fromDateToMonth(start, end) {
    const startMonth = new Date(start).getMonth() + 1;
    const endMonth = new Date(end).getMonth() + 1;
    const startYear = new Date(start).getFullYear();
    const endYear = new Date(end).getFullYear();
    console.log(`startMonth: ${startMonth}, endMonth: ${endMonth}, startYear: ${startYear}, endYear: ${endYear}`);
    return {
        startMonth: startMonth,
        endMonth: endMonth,
        startYear: startYear,
        endYear: endYear
    };
}


async function showRates() {
    let start = document.getElementById('startDatePicker').value;
    let end = document.getElementById('endDatePicker').value;
    const employeeId = document.getElementById('chooseEmployee').value;
    if (start && end && employeeId) {
        // fetch for the offdays and show them in the table
        let { startMonth, endMonth, startYear, endYear } = fromDateToMonth(start, end);
        console.log(`startMonth: ${startMonth}, endMonth: ${endMonth}, startYear: ${startYear}, endYear: ${endYear}`);

        const response = await fetch(`/getRates?employeeId=${employeeId}&startMonth=${startMonth}&endMonth=${endMonth}&startYear=${startYear}&endYear=${endYear}`);
        const rates = await response.json();
        console.log(JSON.stringify(rates.rates));
        if(rates.success==false){
            console.log("No rates found");
            showDiv("noRecordsDiv");
            hideDiv("allInfoReportTable");
            return;
        }
        document.getElementById("tableHeading").innerText = `التقييم خلال الفترة:  ${rates.avgRate} `;
        document.getElementById("tableHeading").classList.remove("hidden");
        // populate the table
        const headInnerHTML = `
            <tr>
                <th class="px-4 py-2 border-b">الشهر</th>
                <th class="px-4 py-2 border-b">التقييم</th>
            </tr>
        `;
        showDiv("allInfoReportTable");
        hideDiv("noRecordsDiv");
        populateTable(headInnerHTML, rates.rates, true);
    } else {
        alert("من فضلك اختر الموظف اولا ثم التاريخين");
    }
}
document.getElementById('chooseEmployee').addEventListener('change', function () {
    const employeeId = this.value;
    console.log('Selected Employee ID:', employeeId);
    if (employeeId != "") {
        document.getElementById('startDatePicker').disabled = false;
    }
    // fetch request
    let employee = null;
    fetch(`/getEmployee?employeeId=${employeeId}`).then(res => res.json()).then(employee => {
        document.getElementById("empInfoName").innerText = employee.Name;
        document.getElementById("empInfoNationalId").innerText = employee.NationalId;
        document.getElementById("empInfoPhoneNumber").innerText = employee.PhoneNumber;
        document.getElementById("empInfoBirthDate").innerText = (employee.DateOfBirth) ? new Date(employee.DateOfBirth)
            .toISOString()
            .split("T")[0] : "";
        document.getElementById("empInfoReligion").innerText = employee.Religion;
        document.getElementById("empInfoSocialStatus").innerText = employee.MarriageStatus;
        document.getElementById("empInfoHireDate").innerText = (employee.HireDate) ? new Date(employee.HireDate)
            .toISOString()
            .split("T")[0] : "";
        document.getElementById("empInfoInsuranceNumber").innerText = employee.InsuranceNumber;
        document.getElementById("empInfoEndDate").innerText = (employee.EndDate) ? new Date(employee.EndDate)
            .toISOString()
            .split("T")[0] : "";
        document.getElementById("empInfoDepartment").innerText = employee.Department;
        document.getElementById("empInfoJobName").innerText = employee.JobName;
        document.getElementById("empInfoContractType").innerText = employee.ContractType;
    });

});
function populateTable(headHTML, data, key = false) {
    const tableHead = document.getElementById('allInfoReportHead');
    const tableBody = document.getElementById('allInfoReportBody');

    // Set the table head
    tableHead.innerHTML = headHTML;

    // Clear existing table body
    tableBody.innerHTML = '';

    // Populate table body
    data.forEach(item => {
        let row = {};
        if (key) {
            row = createRowWithoutDate(item)
        } else {
            row = createRow(item);
        }
        tableBody.appendChild(row);
    });

}
function createRow(item) {
    let row = document.createElement("tr");
    row.className = "snap-start";

    // Loop through each key/value in the object
    Object.keys(item).forEach(key => {
        let td = document.createElement("td");
        td.className = "px-4 py-2 border-b text-center";

        let value = item[key];

        // Optional: handle dates nicely
        if (value && typeof value === "string" && !isNaN(Date.parse(value))) {
            value = new Date(value).toLocaleDateString("en-CA"); // YYYY-MM-DD
        }

        td.textContent = value ?? ""; // fallback for null
        row.appendChild(td);
    });

    return row;
}
function createRowWithoutDate(item) {
    let row = document.createElement("tr");
    row.className = "snap-start";

    // Loop through each key/value in the object
    Object.keys(item).forEach(key => {
        let td = document.createElement("td");
        td.className = "px-4 py-2 border-b text-center";

        let value = item[key];

        td.textContent = value ?? ""; // fallback for null
        row.appendChild(td);
    });

    return row;
}

// for the tabs
document.querySelectorAll(".tab").forEach(tab => {
    tab.addEventListener("click", () => {
        // remove active from all tabs
        document.querySelectorAll(".tab").forEach(t => t.classList.remove("active"));

        document.getElementById("tableHeading").classList.add("hidden");

        // activate current
        tab.classList.add("active")
        //document.getElementById(tab.dataset.tab).classList.add("active");
    });
});

