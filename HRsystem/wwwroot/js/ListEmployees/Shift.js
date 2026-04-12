// let empIdShiftModal = 0;
// document.getElementById("saveHoursShiftBtn").addEventListener("click", () => {
//     // validate the data
//     const hours = document.getElementById("hours").value;
//     if (isNumber(hours)) {
//         // send a post request
//         fetch(`/addShiftHours?EmployeeId=${empIdShiftModal}&Hours=${hours}`, {
//             method: 'POST'
//         })
//             .then(res => res.json())
//             .then(data => {
//                 console.log('Saved successfully', data);
//                 hideDivFlex("editShift");
//                 showDivFlex("currentShift");
//                 fetch(`/getShift?employeeId=${empIdShiftModal}`).then(res => res.json()).then(shift => {
//                     if (shift.success == false) {
//                         showDiv("noShift");
//                     } else {
//                         // inject the data
//                         if (shift.shift.ShiftMode == 1) {
//                             document.getElementById("hoursShiftNumber").innerText = shift.shift.RequiredHours;
//                             showDiv("hoursShiftDiv");
//                             hideDiv("fixedShiftDiv")
//                             hideDiv("variableShiftDiv");
//                         } else if (shift.shift.ShiftMode == 2) {
//                             // Assuming shift.shift.StartTime and EndTime are Date objects or ISO strings
//                             const startTime = new Date(shift.shift.StartTime);
//                             const endTime = new Date(shift.shift.EndTime);

//                             // Format time in Arabic with ص / م
//                             const options = { hour: 'numeric', minute: 'numeric', hour12: true };

//                             document.getElementById("shiftStartTime").innerText = startTime.toLocaleTimeString('ar-EG', options);
//                             document.getElementById("shiftEndTime").innerText = endTime.toLocaleTimeString('ar-EG', options);
//                             showDiv("fixedShiftDiv");
//                             hideDiv("hoursShiftDiv");
//                             hideDiv("variableShiftDiv");
//                         }
//                         else if(shift.shift.ShiftMode == 0){
//                             hideDiv("fixedShiftDiv");
//                             hideDiv("hoursShiftDiv");
//                             showDiv("variableShiftDiv");
//                         }
//                         hideDiv("noShift");
//                         showDiv("thereIsShift");
//                     }
//                 })
//             })
//             .catch(err => console.error(err));
//     }
// })
// document.getElementById("saveVariableShiftBtn").addEventListener("click", () => {
//     // validate the data

//         // send a post request
//         fetch(`/addShiftVariable?EmployeeId=${empIdShiftModal}`, {
//             method: 'POST'
//         })
//             .then(res => res.json())
//             .then(data => {
//                 console.log('Saved successfully', data);
//                 hideDivFlex("editShift");
//                 showDivFlex("currentShift");
//                 fetch(`/getShift?employeeId=${empIdShiftModal}`).then(res => res.json()).then(shift => {
//                     if (shift.success == false) {
//                         showDiv("noShift");
//                     } else {
//                         // inject the data
//                         if (shift.shift.ShiftMode == 1) {
//                             document.getElementById("hoursShiftNumber").innerText = shift.shift.RequiredHours;
//                             showDiv("hoursShiftDiv");
//                             hideDiv("variableShiftDiv");
//                             hideDiv("fixedShiftDiv")
//                         } else if (shift.shift.ShiftMode == 2) {
//                             // Assuming shift.shift.StartTime and EndTime are Date objects or ISO strings
//                             const startTime = new Date(shift.shift.StartTime);
//                             const endTime = new Date(shift.shift.EndTime);

//                             // Format time in Arabic with ص / م
//                             const options = { hour: 'numeric', minute: 'numeric', hour12: true };

//                             document.getElementById("shiftStartTime").innerText = startTime.toLocaleTimeString('ar-EG', options);
//                             document.getElementById("shiftEndTime").innerText = endTime.toLocaleTimeString('ar-EG', options);
//                             showDiv("fixedShiftDiv");
//                             hideDiv("hoursShiftDiv");
//                             hideDiv("variableShiftDiv");
//                         }
//                         else if(shift.shift.ShiftMode == 0){
//                             hideDiv("fixedShiftDiv");
//                             hideDiv("hoursShiftDiv");
//                             showDiv("variableShiftDiv");
//                         }
//                         hideDiv("noShift");
//                         showDiv("thereIsShift");
//                     }
//                 })
//             })
//             .catch(err => console.error(err));
// })
// document.getElementById("startTime").addEventListener("change", function () {
//     const endTime = document.getElementById("endTime");
//     endTime.disabled = false;
//     endTime.min = this.value;
//     // Enforce min manually
//     if (endTime.value < this.value) {
//         console.log("yeppppppppppppppp");
//         endTime.value = this.value;
//     }
// });

// function timeToMinutes(time) {
//     const [h, m] = time.split(":").map(Number);
//     return h * 60 + m;
// }

// document.getElementById("endTime").addEventListener("change", function () {
//     const startTime = document.getElementById("startTime").value;
//     if (!startTime || !this.value) return;

//     if (timeToMinutes(this.value) < timeToMinutes(startTime)) {
//         alert("وقت نهاية الشفت يجب ان يكون اكبر من وقت بدايته!");
//         this.value = startTime;
//     }
// });
// document.getElementById("saveFixedShiftBtn").addEventListener("click", () => {
//     // validate the data
//     const startTimeParts = document.getElementById("startTime").value.split(":");

//     const endTimeParts = document.getElementById("endTime").value.split(":");

//     const today = new Date(); // base date
//     const startTime = new Date(today.getFullYear(), today.getMonth(), today.getDate(), startTimeParts[0], startTimeParts[1]).toLocaleTimeString();
//     const endTime = new Date(today.getFullYear(), today.getMonth(), today.getDate(), endTimeParts[0], endTimeParts[1]).toLocaleTimeString();

//     if (startTime && endTime) {
//         // send a post request
//         fetch(`/addShiftFixed?EmployeeId=${empIdShiftModal}&StartTime=${startTime}&EndTime=${endTime}`, {
//             method: 'POST'
//         })
//             .then(res => res.json())
//             .then(data => {
//                 console.log('Saved successfully', data);
//                 hideDivFlex("editShift");
//                 showDivFlex("currentShift");
//                 fetch(`/getShift?employeeId=${empIdShiftModal}`).then(res => res.json()).then(shift => {
//                     if (shift.success == false) {
//                         showDiv("noShift");
//                     } else {
//                         // inject the data
//                         if (shift.shift.ShiftMode == 1) {
//                             document.getElementById("hoursShiftNumber").innerText = shift.shift.RequiredHours;
//                             showDiv("hoursShiftDiv");
//                             hideDiv("fixedShiftDiv");
//                             hideDiv("variableShiftDiv");
//                         } else if (shift.shift.ShiftMode == 2) {
//                             // Assuming shift.shift.StartTime and EndTime are Date objects or ISO strings
//                             const startTime = new Date(shift.shift.StartTime);
//                             const endTime = new Date(shift.shift.EndTime);

//                             // Format time in Arabic with ص / م
//                             const options = { hour: 'numeric', minute: 'numeric', hour12: true };

//                             document.getElementById("shiftStartTime").innerText = startTime.toLocaleTimeString('ar-EG', options);
//                             document.getElementById("shiftEndTime").innerText = endTime.toLocaleTimeString('ar-EG', options);
//                             showDiv("fixedShiftDiv");
//                             hideDiv("hoursShiftDiv");
//                             hideDiv("variableShiftDiv");
//                         }
//                         else if(shift.shift.ShiftMode == 0){
//                             hideDiv("fixedShiftDiv");
//                             hideDiv("hoursShiftDiv");
//                             showDiv("variableShiftDiv");
//                         }
//                         hideDiv("noShift");
//                         showDiv("thereIsShift");
//                     }
//                 })
//             })
//             .catch(err => console.error(err));
//     }
// })

// SHIFT
let employeesForShift = [];
let departmentName = "";
const days = [
  "الأحد",
  "الإتنين",
  "الثلاثاء",
  "الأربعاء",
  "الخميس",
  "الجمعة",
  "السبت",
];
const months = [
  "يناير",
  "فبراير",
  "مارس",
  "ابريل",
  "مايو",
  "يونيو",
  "يوليو",
  "اغسطس",
  "سبتمبر",
  "اكتوبر",
  "نوفمبر",
  "ديسمبر",
];
function openShiftModal() {
  const departmentId = document.getElementById("departmentFilter").value;
  if (departmentId == "") {
    alert("الرجاء اختيار قسم أولاً!");
    return;
  }

  fetch(`/getDepartmentEmployees?departmentId=${departmentId}`)
    .then((res) => res.json())
    .then((data) => {
      console.log(data);
      showDivFlex("shiftModal");
      employeesForShift = data.employees;
      departmentName = data.departmentName;
      document.getElementById("shiftModalTitle").innerText =
        `تعديل الشفت لقسم: ${data.departmentName}`;
    });
}
function buildSelect(empId, dateStr, options) {
    let html = `
        <select class="border shiftSelectF !text-color3 rounded p-1"
                data-emp="${empId}"
                data-date="${dateStr}">
            <option value="">اختر شيفت</option>
            <option value="off">راحة</option>
    `;

    options.forEach(option => {
        html += `<option value="${option.Id}">${option.Name}</option>`;
    });

    html += `</select>`;

    return html;
}
document.addEventListener("change", function (e) {
    if (e.target.classList.contains("shiftSelectF")) {

        const value = e.target.value;
        const parent = e.target.closest(".dayCard");
        // reset classes
        parent.classList.remove("bg-color1", "bg-color2", "bg-color3", "!text-color1", "!text-color3");

        if (value === "off") {
           parent.classList.add("bg-color2");
            parent.classList.add("!text-color3");
        } else{
            parent.classList.add("!bg-color3");
            parent.classList.add("text-color1");
        }
       
    }
});
function SearchWeeks() {
  const employeesRows = document.getElementById("employeesRows");

  const startDate = new Date(document.getElementById("startWeekDate").value); // أو التاريخ اللي انت مختاره
  if (startDate == "Invalid Date") {
    alert("الرجاء اختيار تاريخ بداية الأسبوع!");
    return;
  }
  options = [];
   
  employeesForShift.forEach(async (emp) => {
    let daysHtml = "";
    const res = await fetch(`/getShiftOptions`);
    const options = await res.json();
    for (let i = 0; i < 7; i++) {
      let currentDay = new Date(startDate);
      currentDay.setDate(startDate.getDate() + i);

      let dayName = days[currentDay.getDay()];
      let dateStr = `${dayName} - ${currentDay.getDate()} ${months[currentDay.getMonth()]}`;

      const dayCard = document.createElement("div");
      dayCard.className =
        "flex dayCard text-color3 flex-col gap-2 items-center bg-color3 p-2 rounded-lg shadow-sm";

      dayCard.innerHTML = `
      <div class="text-sm font-bold !text-color1">${dateStr}</div>
       `;
      const selectHtml = buildSelect(emp.Id, currentDay, options);
      dayCard.innerHTML += selectHtml;
      daysHtml += dayCard.outerHTML;
     
    }

    let row = `
        <div id="employeeDiv-${emp.Id}" 
             class="shadow-lg flex flex-col gap-3 p-3 rounded-lg">

            <span class="font-bold">${emp.Name}</span>

            <div class="grid grid-cols-7 gap-2">
                ${daysHtml}
            </div>
        </div>
        `;
    employeesRows.innerHTML += row;
  });
}

document.getElementById("shiftType").addEventListener("change", function () {
  const selectedValue = this.value;
  if (selectedValue == "1") {
    console.log("hours");
    hideDiv("shiftStartTime");
    hideDiv("shiftEndTime");
    showDiv("shiftHours");
  } else if (selectedValue == "2") {
    console.log("fixed");
    hideDiv("shiftHours");
    showDiv("shiftStartTime");
    showDiv("shiftEndTime");
  } else {
    console.log("variable");
    hideDiv("shiftHours");
    hideDiv("shiftStartTime");
    hideDiv("shiftEndTime");
  }
});

function addShiftOption() {
  const startTime = document.getElementById("shiftStartTime").value;
  const endTime = document.getElementById("shiftEndTime").value;
  const hours = document.getElementById("shiftHours").value;
  const shiftType = document.getElementById("shiftType").value;
  if (shiftType == "") {
    alert("الرجاء اختيار نوع الشفت!");
    return;
  } else if (shiftType == "1" && !isNumber(hours)) {
    alert("الرجاء إدخال عدد ساعات صحيح!");
    return;
  } else if (shiftType == "2" && (!startTime || !endTime)) {
    alert("الرجاء إدخال وقت بداية ونهاية الشفت!");
    return;
  }
  fetch(
    `/addShiftOption?StartTime=${startTime}&EndTime=${endTime}&ShiftMode=${shiftType}&Hours=${hours}`,
    {
      method: "POST",
    },
  )
    .then((res) => res.json())
    .then((data) => {
      if (data.success) {
        alert("تم إضافة خيار الشفت بنجاح!");
      } else {
        alert("حدث خطأ أثناء إضافة خيار الشفت. الرجاء المحاولة مرة أخرى.");
      }
    })
    .catch((err) => {
      console.error(err);
      alert("حدث خطأ أثناء إضافة خيار الشفت. الرجاء المحاولة مرة أخرى.");
    });
}

function closeShiftModal() {
  hideDivFlex("shiftModal");
  employeesForShift = [];
  departmentName = "";
  document.getElementById("shiftModalTitle").innerText = "";
  document.getElementById("employeesRows").innerHTML = "";
}
const shiftModal = document.getElementById("shiftModal");
shiftModal.addEventListener("click", function (e) {
  if (e.target == this) {
    closeShiftModal();
  }
});

// document.getElementById("switchBtn").addEventListener("click", function () {
//     toggleVisibility("currentShift", true);
//     toggleVisibility("editShift", true);
// })
// document.getElementById("editShiftButton").addEventListener("click", () => {
//     showDivFlex("editShift");
//     hideDivFlex("currentShift");
// })
// const hoursBtn = document.getElementById("hoursBtn");
// const fixedBtn = document.getElementById("fixedBtn");
// const variableBtn = document.getElementById("variableShift");
// variableBtn.addEventListener("click", function () {
//     hideDivFlex("newHoursShift");
//     hideDivFlex("newFixedShift");
//     showDivFlex("newVariableShift");
//     this.classList.add("outlineActive");
//     hoursBtn.classList.remove("outlineActive");
//     fixedBtn.classList.remove("outlineActive");
// });
// hoursBtn.addEventListener("click", function () {
//     hideDivFlex("newVariableShift");
//     hideDivFlex("newFixedShift");
//     showDivFlex("newHoursShift");
//     this.classList.add("outlineActive");
//     variableBtn.classList.remove("outlineActive");
//     fixedBtn.classList.remove("outlineActive");
// })
// fixedBtn.addEventListener("click", function () {
//     hideDivFlex("newVariableShift");
//     hideDivFlex("newHoursShift");
//     showDivFlex("newFixedShift");
//     this.classList.add("outlineActive");
//     variableBtn.classList.remove("outlineActive");
//     hoursBtn.classList.remove("outlineActive");
// })
