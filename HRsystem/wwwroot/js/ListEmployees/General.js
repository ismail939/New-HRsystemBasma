window.addEventListener("beforeunload", () => {
    console.log("🚨 PAGE IS RELOADING or navigating away!");
});

function searchForEmployee() {
    const input = document.getElementById('searchInput');
    const query = input.value.toLowerCase();
    const rows = document.querySelectorAll('#employeesBody tr');

    rows.forEach(row => {
        const name = row.cells[0].innerText.toLowerCase();
        const nationalId = row.cells[1].innerText.toLowerCase();
        if (name.includes(query) || nationalId.includes(query)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}
function showAllEmployees() {
    const rows = document.querySelectorAll('#employeesBody tr');
    rows.forEach(row => row.style.display = '');
}

document.getElementById("departmentFilter").addEventListener("change", function () {
    const selectedDeptId = this.value;
    const rows = document.querySelectorAll('#employeesBody tr');
    rows.forEach(row => {
        const deptId = row.getAttribute('data-dept-id');
        if (selectedDeptId === '' || deptId === selectedDeptId) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
});


function openLeaveReasonModal(LeaveReason, date) {
    if (date != "" && LeaveReason != "") {
        document.getElementById("LeaveReasonDateText").innerText = formatDate(date);
        document.getElementById("LeaveReasonModalDescription").innerText = LeaveReason;
        showDiv("reasonLabel");
        showDivFlex("LeaveReasonModal");
    }
}
function closeLeaveReasonModal() {
    hideDivFlex("LeaveReasonModal");
    document.getElementById("LeaveReasonDateText").innerText = "";
    hideDiv("reasonLabel");
}
const LeaveReasonModal = document.getElementById("LeaveReasonModal");
// Optional: close modal when background is clicked
LeaveReasonModal.addEventListener('click', function (e) {
    if (e.target === this) closeLeaveReasonModal();
});