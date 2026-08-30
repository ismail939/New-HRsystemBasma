document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('editModal').addEventListener('click', function (e) {
        if (e.target === this) closeModal();
    });
    loadRules();
});

let openedModalEmployeeId = 0;

const DEDUCTION_LABELS = { 0: 'مبلغ', 1: 'ساعة', 2: 'يوم', 3: 'نسبة %', 4: 'إنذار فقط' };
const STATUS_LABELS = { 0: 'مسودة', 1: 'بانتظار المراجعة', 2: 'معتمد', 3: 'مرفوض' };

async function loadRules() {
    try {
        const res = await fetch('/penalty/rules');
        if (!res.ok) return;
        const rules = await res.json();
        const select = document.getElementById('ruleId');
        rules.forEach(r => {
            const opt = document.createElement('option');
            opt.value = r.Id;
            opt.textContent = r.NameAr + ' (' + r.Name + ')';
            select.appendChild(opt);
        });
    } catch (e) {
        console.error('Error loading rules:', e);
    }
}

function openModal(employeeId) {
    sessionStorage.setItem("currentEmployeeId", employeeId);
    openedModalEmployeeId = employeeId;
    document.getElementById('editModal').classList.remove('hidden');
    document.getElementById('editModal').classList.add('flex');
    document.getElementById("employeeId").value = employeeId;
    getPenaltiesForEmployee();
}
function closeModal() {
    document.getElementById('editModal').classList.remove('flex');
    document.getElementById('editModal').classList.add('hidden');
    document.getElementById('addPenaltyForm').reset();
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

tabAdd.addEventListener("click", () => activateTab(contentAdd, contentList, tabAdd, tabList));
tabList.addEventListener("click", () => activateTab(contentList, contentAdd, tabList, tabAdd));

async function addPenalty() {
    const employeeId = document.getElementById("employeeId").value;
    const ruleId = document.getElementById("ruleId").value;
    const incidentDate = document.querySelector('input[name="incidentDate"]').value;
    const notes = document.querySelector('textarea[name="notes"]').value;
    const deductionUnit = document.getElementById("deductionUnit").value;
    const deductionValue = document.getElementById("deductionValue").value || '0';

    if (!employeeId || !ruleId || !incidentDate) {
        alert("يرجى اختيار نوع الجزاء وتاريخ الواقعة.");
        return;
    }

    const formData = new FormData();
    formData.append('employeeId', employeeId);
    formData.append('ruleId', ruleId);
    formData.append('incidentDate', incidentDate);
    formData.append('deductionUnit', deductionUnit);
    formData.append('deductionValue', deductionValue);
    formData.append('notes', notes);

    const response = await fetch('/employee/addPenalty', { method: 'POST', body: formData });
    if (!response.ok) {
        alert("حدث خطأ أثناء إضافة الجزاء.");
        return;
    }
    alert("تم إضافة الجزاء بنجاح!");
    getPenaltiesForEmployee();
    document.getElementById('addPenaltyForm').reset();
    activateTab(contentList, contentAdd, tabList, tabAdd);
}

async function toggleActive(isActive, penaltyId) {
    const newStatus = !isActive;
    const res = await fetch(`/employee/togglePenaltyActive`, {
        method: 'POST',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ penaltyId: penaltyId, isActive: newStatus })
    });
    if (res.ok) {
        getPenaltiesForEmployee();
    } else {
        alert("حدث خطأ أثناء تحديث حالة الجزاء.");
    }
}

async function getPenaltiesForEmployee() {
    const employeeId = sessionStorage.getItem("currentEmployeeId");
    try {
        const response = await fetch(`/employee/penalties/${employeeId}`);
        const data = await response.json();
        const body = document.getElementById("contentBody");
        if (!data || data.length === 0) {
            body.innerHTML = '<div class="p-2 mt-2 fs-4">لا توجد جزاءات لهذا الموظف</div>';
            return;
        }
        body.innerHTML = `<table class="table-base w-full border">
            <thead class="table-dark col-table-header">
                <tr class="bg-color3 text-white">
                    <th class="p-2">نوع الجزاء</th>
                    <th class="p-2">تاريخ الواقعة</th>
                    <th class="p-2">الخصم</th>
                    <th class="p-2">الحالة</th>
                    <th class="p-2">إجراء</th>
                </tr>
            </thead>
            <tbody></tbody>
        </table>`;
        const tbody = body.querySelector('tbody');
        data.forEach(p => {
            const unitLabel = DEDUCTION_LABELS[p.DeductionUnit] || p.DeductionUnit;
            const statusText = STATUS_LABELS[p.Status] || p.Status;
            const processed = p.PayrollItemId != null;
            const amountText = (p.DeductionAmount != null && Number(p.DeductionAmount) > 0)
                ? ` (≈ ${Number(p.DeductionAmount).toFixed(2)} ر.س)`
                : '';

            const row = `
                <tr>
                    <td class="border p-2">${p.RuleNameAr || p.RuleName}</td>
                    <td class="border p-2">${String(p.IncidentDate).split('T')[0]}</td>
                    <td class="border p-2">${p.DeductionValue} ${unitLabel}${amountText}</td>
                    <td class="border p-2">${statusText}</td>
                    <td class="border p-2">
                        ${processed
                            ? '<span class="text-green-700 font-semibold">تم خصمه في الراتب</span>'
                            : `<button onclick="toggleActive(${p.IsActive}, ${p.Id})" class="rounded-lg p-2 ${p.IsActive ? 'active' : 'inactive'}">
                                ${p.IsActive ? 'تعطيل' : 'تفعيل'}
                            </button>`}
                    </td>
                </tr>`;
            tbody.insertAdjacentHTML('beforeend', row);
        });
    } catch (error) {
        console.error("Error fetching penalties:", error);
    }
}
