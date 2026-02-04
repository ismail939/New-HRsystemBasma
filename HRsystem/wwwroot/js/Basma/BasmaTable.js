document.addEventListener('DOMContentLoaded', function () {
    console.log("Table.js loaded");
    globalBasmaList = [];
    showBasmaListForDay();
    setTimeout(() => {
        document.getElementById('tableResponsive').classList.remove('hidden');
    }, 120);
});
// function filterByDate() {
//     const dateInput = document.getElementById('datePicker').value;
//     const table = document.getElementById('employeesTable');
//     const rows = table.getElementsByTagName('tr');

//     for (let i = 1; i < rows.length; i++) {
//         const dateCell = rows[i].getElementsByTagName('td')[1];
//         if (dateCell) {
//             const cellDate = dateCell.textContent || dateCell.innerText;
//             if (dateInput === "" || cellDate === dateInput) {
//                 rows[i].style.display = "";
//             } else {
//                 rows[i].style.display = "none";
//             }
//         }
//     }
// }
async function showBasmaListForDay() {
    let dateInput = document.getElementById('datePicker').value;
    const tableBody = document.getElementById('employeesBody');
    const fp = flatpickr("#datePicker", {});

    // Set the date programmatically
    if (dateInput == "") {
        fp.setDate(new Date());
        dateInput = new Date().toISOString().split('T')[0];
    }
    const response = await fetch(`/basmaData?Day=${encodeURIComponent(dateInput)}`);
    const list = await response.json();
    console.log(list + "🔵");
    if (list.length === 0) {
        document.getElementById('noDataMessage').classList.remove('hidden');
        document.getElementById('employeesTable').classList.add('hidden');
        return;
    } else {
        document.getElementById('noDataMessage').classList.add('hidden');
        document.getElementById('employeesTable').classList.remove('hidden');
    }
    var html = "";
    globalBasmaList = list;
    list.forEach(basma => {
        html += `
                <tr id="row-${basma.Id}" class="snap-start h-12">
        <td>${basma.EmployeeName ?? ""}</td>

        <td>
            ${basma.ArrivalTime
                ? new Date(basma.ArrivalTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
                : ""}
        </td>

        <td>
            ${basma.DepartureTime
                ? new Date(basma.DepartureTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
                : ""}
        </td>

        <td>${basma.TotalHours ?? ""}</td>
        <td>${basma.LateMinutes ?? ""}</td>
        <td>${basma.EarlyLeaveMinutes ?? ""}</td>
        <td>
            ${basma.Status === 1
                ? `<span>حضور</span>`
                : basma.Status === 3
                    ? `
                    <select class="h-full border p-1 rounded focus:outline-none focus:ring focus:ring-[#7B9669]"
                            data-id="${basma.Id}">
                        <option value="Absent">غياب</option>
                        <option value="Leave">إجازة</option>
                    </select>
                    `
                    : basma.Status === 2
                        ? `<span>إجازة</span>`
                        : ""}
        </td>
        <td>
        ${basma.Ok
                ? "منتهي"
                : `<button class="col-button-dark" onclick="confirm(${basma.id})">تأكيد</button>`
            }

        </td>
        <td class="border !p-0">
            <input type="text" class="w-full -z-50 text-color3 rounded-sm focus:outline-none focus:ring focus:ring-[#7B9669] h-12 border-0">
        </td>

    </tr>
              `;
    })
    tableBody.innerHTML = html;
}
document.getElementById('datePicker').addEventListener('change', showBasmaListForDay);
async function searchForEmployee() {
    const input = document.getElementById('searchInput').value.trim();
    const tableBody = document.getElementById('employeesBody');

    const list = globalBasmaList.filter(basma => basma.EmployeeName.toLowerCase().includes(input.toLowerCase()));

    console.log(list + "🟠");
    var html = "";
    list.forEach(basma => {
        html += `
                <tr id="row-${basma.Id}" class="snap-start h-12">
        <td>${basma.EmployeeName ?? ""}</td>

        <td>
            ${basma.ArrivalTime
                ? new Date(basma.ArrivalTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
                : ""}
        </td>

        <td>
            ${basma.DepartureTime
                ? new Date(basma.DepartureTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
                : ""}
        </td>

        <td>${basma.TotalHours ?? ""}</td>
        <td>${basma.LateMinutes ?? ""}</td>
        <td>${basma.EarlyLeaveMinutes ?? ""}</td>
        <td>
            ${basma.Status === 1
                ? `<span>حضور</span>`
                : basma.Status === 3
                    ? `
                    <select class="h-full border p-1 rounded focus:outline-none focus:ring focus:ring-[#7B9669]"
                            data-id="${basma.Id}">
                        <option value="Absent">غياب</option>
                        <option value="Leave">إجازة</option>
                    </select>
                    `
                    : basma.Status === 2
                        ? `<span>إجازة</span>`
                        : ""}
        </td>
        <td>
        ${basma.Ok
                ? "منتهي"
                : `<button class="col-button-dark" onclick="takeBasma(${basma.id})">تأكيد</button>`
            }

        </td>
        <td class="border !p-0">
            <input type="text" class="w-full -z-50 text-color3 rounded-sm focus:outline-none focus:ring focus:ring-[#7B9669] h-12 border-0">
        </td>

    </tr>
              `;
    })
    tableBody.innerHTML = html;
}

function confirm(basmaId) {
    console.log("Confirming basma with ID:", basmaId);
    // eb3t elbasma ll backend hena w e3ml Ok b true lw hya 8yab 
    // bs lw agaza et2kd mn elbackend eno fe agaza hna.
    fetch(`/confirmBasma?id=${basmaId}`, {
        method: 'POST'
    }).then(response => {
        if (response.ok) {
            console.log(`Basma with ID: ${basmaId} confirmed successfully.`);
            // Update the button text to "منتهي"
            const row = document.getElementById(`row-${basmaId}`);
            const buttonCell = row.cells[7]; // Assuming the button is in the 8th cell (index 7)
            buttonCell.innerHTML = "منتهي";
        } else {
            console.error(`Failed to confirm basma with ID: ${basmaId}.`);
        }
    }).catch(error => {
        console.error('Error confirming basma:', error);
    });
}

