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
function openShiftModal() {

    const departmentId = document.getElementById("departmentFilter").value;
    if(departmentId == ""){
        alert("الرجاء اختيار قسم أولاً!");
        return;
    }

    fetch(`/getDepartmentEmployees?departmentId=${departmentId}`).then(res => res.json()).then(data => {
        console.log(data);
        showDivFlex("shiftModal");
        console.log(data);
        document.getElementById("shiftModalTitle").innerText = `تعديل الشفت لقسم: ${data.departmentName}`;
    })

}
function closeShiftModal() {
    hideDivFlex("shiftModal");
    hideDiv("noShift");
    hideDiv("thereIsShift");
    hideDivFlex("editShift");
    showDivFlex("currentShift");
}
const shiftModal = document.getElementById("shiftModal");
shiftModal.addEventListener("click", function (e) {
    if (e.target == this) {
        closeShiftModal();
    }
})

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