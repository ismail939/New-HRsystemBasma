document.addEventListener('DOMContentLoaded', function () {
    // Optional: close modal when background is clicked
    document.getElementById('editModal').addEventListener('click', function (e) {
        if (e.target === this) closeModal();
    });
});
let openedModalEmployeeId = 0;

function openModal(employeeId) {
    sessionStorage.setItem("currentEmployeeId", employeeId);
    openedModalEmployeeId = employeeId;
    document.getElementById('editModal').classList.remove('hidden');
    document.getElementById('editModal').classList.add('flex');
    const form = document.querySelector("#addPenaltyForm");
    form.elements["employeeId"].value = employeeId;
    getPenaltiesForEmployee();
}
function closeModal() {
    document.getElementById('editModal').classList.remove('flex');
    document.getElementById('editModal').classList.add('hidden');
    const form = document.querySelector("#contentAdd form");
    form.reset();
    clearFlatpickrInModal('addPenaltyForm');
    activateTab(contentAdd, contentList, tabAdd, tabList);
}

const tabAdd = document.getElementById("tabAdd");
const tabList = document.getElementById("tabList");

const contentAdd = document.getElementById("contentAdd");
const contentList = document.getElementById("contentList");

function activateTab(show, hide, activeTab, inactiveTab) {
    show.classList.remove("hidden");
    hide.classList.add("hidden");

    activeTab.classList.add("border-color3", "text-color3");
    activeTab.classList.remove("border-transparent", "text-color5");

    inactiveTab.classList.add("border-transparent", "text-color5");
    inactiveTab.classList.remove("border-color3", "text-color3");
}

tabAdd.addEventListener("click", () => {
    activateTab(contentAdd, contentList, tabAdd, tabList);
});

tabList.addEventListener("click", () => {
    activateTab(contentList, contentAdd, tabList, tabAdd);
});

async function addPenalty() {
    const form = document.querySelector("#addPenaltyForm");
    console.log("******************");
    console.log(document.querySelector('input[name="penalty"]').value)
    console.log(document.querySelector('input[name="dayDate"]').value)
    console.log(document.querySelector('textarea[name="reason"]').value)
    console.log("******************");

    if (!form) {
        console.log("Form not found!");
        return;
    }
    document.getElementById("employeeId").value = openedModalEmployeeId;
    const formData = new FormData(form);
    // check every input field is filled
    for (let [key, value] of formData.entries()) {
        console.log(key, value, "❎");
        if (!value) {
            alert("يرجى ملء جميع المدخلات.");
            return;
        }
    }
    const response = await fetch('/employee/addPenalty', {
        method: 'POST',
        body: formData
    });
    if (!response.ok) {
        alert("حدث خطأ أثناء إضافة الجزاء.");
        return;
    }
    // Implement the logic to add a penalty
    alert("تم إضافة الجزاء بنجاح!");
    getPenaltiesForEmployee();
    clearFlatpickrInModal('addPenaltyForm');
    activateTab(contentList, contentAdd, tabList, tabAdd);
    return;
}
async function toggleActive(isActive, penaltyId) {
    const button = document.getElementById(`penalty-${penaltyId}`);
    const newStatus = !isActive;
    console.log("Toggling penalty ID", penaltyId, "to", newStatus);
    await fetch(`/employee/togglePenaltyActive`, {
        body: JSON.stringify({
            penaltyId: penaltyId,
            isActive: newStatus
        }),
        method: 'POST',
        headers: { "Content-Type": "application/json" }
    }).then(response => {
        if (response.ok) {
            button.classList.toggle('active', newStatus);
            button.classList.toggle('inactive', !newStatus);
            button.textContent = newStatus ? 'مفعّل' : 'غير مفعّل';
            getPenaltiesForEmployee();
        } else {
            alert("حدث خطأ أثناء تحديث حالة الجزاء.");
        }
    }).catch(error => {
        console.error("Error toggling penalty status:", error);
        alert("حدث خطأ أثناء تحديث حالة الجزاء.");
    });
}
async function getPenaltiesForEmployee() {
    const employeeId = sessionStorage.getItem("currentEmployeeId");;
    console.log("Fetching penalties for employee ID:", employeeId); // ✔ correct logging
    try {
        const response = await fetch(`/employee/penalties/${employeeId}`);
        const data = await response.json();
        if (data == "") {
            document.getElementById("contentBody").innerHTML = `
                <div class="p-2 mt-2 fs-4">لا توجد جزاءات لهذا الموظف</div>
                `;
            return;
        }
        document.getElementById("contentBody").innerHTML =
            `<table class="table-base w-full border">
                    <thead class="table-dark col-table-header">
                        <tr class="bg-color3 text-white">
                            <th class="p-2">سبب الجزاء</th>
                            <th class="p-2">نقاط الجزاء</th>
                            <th class="p-2">القرار</th>
                            <th class="p-2">التاريخ</th>
                            <th class="p-2">الحالة</th>
                        </tr>
                    </thead>
                    <tbody id="penaltiesTable">
                    </tbody>
                </table>
                `;

        const penaltiesTable = document.getElementById("penaltiesTable");
        penaltiesTable.innerHTML = "";

        data.forEach(penalty => {
            console.log("Penalty:", penalty); // ✔ correct per-penalty logging

            const row = `
        <tr>
            <td class="border p-2">${penalty.Reason}</td>
            <td class="border p-2">${penalty.PenaltyPoints}</td>
            <td class="border p-2">${penalty.Decision}</td>
            <td class="border p-2">${penalty.PenaltyDate.split('T')[0]}</td>
            <td class="border p-2">
                <button 
                    id="penalty-${penalty.Id}" 
                    onclick="toggleActive(${penalty.IsActive}, ${penalty.Id})"
                    class="rounded-lg p-2 ${penalty.IsActive ? 'active' : 'inactive'}">
                    ${penalty.IsActive ? 'مفعّل' : 'غير مفعّل'}
                </button>
            </td>
        </tr>`;

            penaltiesTable.insertAdjacentHTML('beforeend', row);
        });

    } catch (error) {
        console.error("Error fetching penalties:", error);
    }
}

