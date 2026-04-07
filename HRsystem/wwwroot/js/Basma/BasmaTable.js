document.addEventListener("DOMContentLoaded", function () {
  console.log("Table.js loaded");
  globalBasmaList = [];
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
  let dateInput = document.getElementById("datePicker").value;
  const tableBody = document.getElementById("employeesBody");
  const fp = flatpickr("#datePicker", {});

  // Set the date programmatically
  if (dateInput == "") {
    fp.setDate(new Date());
    dateInput = new Date().toLocaleDateString().split("T")[0];
  }
  console.log("Fetching basma list for date:", dateInput);
  const response = await fetch(
    `/basmaData?Day=${encodeURIComponent(dateInput)}`,
  );
  const list = await response.json();
  console.log(JSON.stringify(list) + "🔵");
  if (list.length === 0) {
    document.getElementById("noDataMessage").classList.remove("hidden");
    document.getElementById("employeesTable").classList.add("hidden");
    return;
  } else {
    document.getElementById("noDataMessage").classList.add("hidden");
    document.getElementById("employeesTable").classList.remove("hidden");
  }
  var html = "";
  globalBasmaList = list;
  list.forEach((basma) => {
    html += `
                <tr id="row-${basma.Id}" class="snap-start h-12">
        <td>${basma.EmployeeName ?? ""}</td>

        <td>
            ${
              basma.ArrivalTime
                ? new Date(basma.ArrivalTime).toLocaleTimeString([], {
                    hour: "2-digit",
                    minute: "2-digit",
                  })
                : ""
            }
        </td>

        <td>
            ${
              basma.DepartureTime
                ? new Date(basma.DepartureTime).toLocaleTimeString([], {
                    hour: "2-digit",
                    minute: "2-digit",
                  })
                : ""
            }
        </td>

        <td>${formatHours(basma.TotalHours) ?? ""}</td>
        <td>${basma.LateMinutes ?? ""}</td>
        <td>${basma.EarlyLeaveMinutes ?? ""}</td>
        <td>${basma.OvertimeMinutes ?? ""}</td>
        <td>
            ${
              basma.Status === 1
                ? `<span>حضور</span>`
                : basma.Status === 3
                  ? `
                    <select class="h-full border p-1 rounded focus:outline-none focus:ring focus:ring-[#7B9669]"
                            data-id="${basma.Id}">
                        <option value="0">غياب</option>
                        <option value="1">حضور</option>
                        <option value="2">إجازة</option>
                    </select>
                    `
                  : basma.Status === 2
                    ? `<span>إجازة</span>`
                    : `<span>غياب</span>`
            }
        </td>
        <td>
        ${
          basma.Ok
            ? `<button class="col-button-light" onclick="cancel(${basma.Id})">إلغاء</button>`
            : `<button class="col-button-dark" onclick="confirm(${basma.Id})">تأكيد</button>`
        }

        </td>
        <td class="border !p-0">
            <input type="text" value="${basma.Notes ? basma.Notes : ""}" class="w-full -z-50 text-color3 rounded-sm focus:outline-none focus:ring focus:ring-[#7B9669] h-12 border-0">
        </td>

    </tr>
              `;
  });
  tableBody.innerHTML = html;
}
function proceedShowingBasma() {
    showBasmaListForDay();
    setTimeout(() => {
      document.getElementById("tableResponsive").classList.remove("hidden");
      hideDivFlex("showBasmaModal");
    }, 120);
}
function goToEmployees(){
    hideDivFlex("showBasmaModal");
    window.location.href = `/employees`; // Redirect to the employees page
}
document.getElementById("datePicker").addEventListener("change", () => {
    showDivFlex("showBasmaModal");
  }
);
async function searchForEmployee() {
  const input = document.getElementById("searchInput").value.trim();
  const tableBody = document.getElementById("employeesBody");

  const list = globalBasmaList.filter((basma) =>
    basma.EmployeeName.toLowerCase().includes(input.toLowerCase()),
  );

  console.log(list + "🟠");
  var html = "";
  list.forEach((basma) => {
    html += `
                <tr id="row-${basma.Id}" class="snap-start h-12">
        <td>${basma.EmployeeName ?? ""}</td>

        <td>
            ${
              basma.ArrivalTime
                ? new Date(basma.ArrivalTime).toLocaleTimeString([], {
                    hour: "2-digit",
                    minute: "2-digit",
                  })
                : ""
            }
        </td>

        <td>
            ${
              basma.DepartureTime
                ? new Date(basma.DepartureTime).toLocaleTimeString([], {
                    hour: "2-digit",
                    minute: "2-digit",
                  })
                : ""
            }
        </td>

        <td>${basma.TotalHours ?? ""}</td>
        <td>${basma.LateMinutes ?? ""}</td>
        <td>${basma.EarlyLeaveMinutes ?? ""}</td>
        <td>${basma.OvertimeMinutes ?? ""}</td>
        <td>
            ${
              basma.Status === 1
                ? `<span>حضور</span>`
                : basma.Status === 3
                  ? `
                    <select class="h-full border p-1 rounded focus:outline-none focus:ring focus:ring-[#7B9669]"
                            data-id="${basma.Id}">
                        <option value="0">غياب</option>
                        <option value="1">حضور</option>
                        <option value="2">إجازة</option>
                    </select>
                    `
                  : basma.Status === 2
                    ? `<span>إجازة</span>`
                    : `<span>غياب</span>`
            }
        </td>
        <td>
        ${
          basma.Ok
            ? `<button class="col-button-light" onclick="cancel(${basma.Id})">إلغاء</button>`
            : `<button class="col-button-dark" onclick="confirm(${basma.Id})">تأكيد</button>`
        }

        </td>
        <td class="border !p-0">
            <input type="text" value="${basma.Notes ? basma.Notes : ""}" class="w-full -z-50 text-color3 rounded-sm focus:outline-none focus:ring focus:ring-[#7B9669] h-12 border-0">
        </td>

    </tr>
              `;
  });
  tableBody.innerHTML = html;
}

function confirm(basmaId) {
  console.log("Confirming basma with ID:", basmaId);
  // eb3t elbasma ll backend hena w e3ml Ok b true lw hya 8yab
  // bs lw agaza et2kd mn elbackend eno fe agaza hna.
  const type = document.querySelector(`select[data-id='${basmaId}']`)?.value;
  fetch(`/confirmBasma?id=${basmaId}&type=${type}`, {
    method: "POST",
  })
    .then((res) => res.json())
    .then((response) => {
      // edit two things 1. from select to the value, 2. button text to "منتهي"
      if (response.code == 1) {
        const row = document.getElementById(`row-${basmaId}`);
        // const buttonCell = row.cells[7]; // Assuming the button is in the 8th cell (index 7)
        // buttonCell.innerHTML = "منتهي";
        if (type == 1) {
          row.cells[7].innerHTML = "حضور";
        } else {
          row.cells[7].innerHTML = type === "2" ? "إجازة" : "غياب";
        }
        row.cells[8].innerHTML = `<button class="col-button-light" onclick="cancel(${basmaId})">إلغاء</button>`;
      } else if (response.code == 2) {
        alert("قم بتسجيل هذا اليوم كإجازة عارضة اولا");
        return;
      }
    })
    .catch((error) => {
      console.error("Error confirming basma:", error);
    });
}
function cancel(basmaId) {
  console.log("Canceling basma with ID:", basmaId);
  // eb3t elbasma ll backend hena w e3ml Ok b true lw hya 8yab
  // bs lw agaza et2kd mn elbackend eno fe agaza hna.
  const type = document.querySelector(`select[data-id='${basmaId}']`)?.value;
  fetch(`/cancelBasma?id=${basmaId}`, {
    method: "POST",
  })
    .then((res) => res.json())
    .then((response) => {
      // edit two things 1. from select to the value, 2. button text to "منتهي"
      if (response.success) {
        const row = document.getElementById(`row-${basmaId}`);
        row.cells[7].innerHTML = `
                    <select class="h-full border p-1 rounded focus:outline-none focus:ring focus:ring-[#7B9669]"
                            data-id="${basmaId}">
                        <option value="0">غياب</option>
                        <option value="1">حضور</option>
                        <option value="2">إجازة</option>
                    </select>
                    `;
        row.cells[8].innerHTML = `<button class="col-button-dark" onclick="confirm(${basmaId})">تأكيد</button>`;
      }
    })
    .catch((error) => {
      console.error("Error confirming basma:", error);
    });
}

function saveNotes() {
  const notes = [];
  const tableBody = document.getElementById("employeesBody");
  const rows = tableBody.getElementsByTagName("tr");
  for (let i = 0; i < rows.length; i++) {
    const row = rows[i];
    const basmaId = globalBasmaList[i].Id;
    const noteInput = row.cells[8].querySelector("input");
    const note = noteInput.value.trim();
    notes.push({ BasmaId: basmaId, Notes: note });
  }
  console.log("Saving notes:", notes);
  fetch("/saveBasmaNotes", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(notes),
  })
    .then((res) => res.json())
    .then((response) => {
      if (response.success) {
        alert("تم حفظ الملاحظات بنجاح!");
      } else {
        alert("حدث خطأ أثناء حفظ الملاحظات.");
      }
    })
    .catch((error) => {
      console.error("Error saving notes:", error);
    });
}
