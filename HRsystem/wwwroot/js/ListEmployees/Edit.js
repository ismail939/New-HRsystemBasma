document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("employeeForm");
    const modal = document.getElementById("editModal"); // assume you have a modal container

    document.getElementById('uploadFileForm').addEventListener('click', e => e.stopPropagation());
    document.getElementById('uploadFileForm').addEventListener('submit', e => e.stopPropagation());

    document.getElementById("tableResponsive").addEventListener("click", e => {
        const btn = e.target.closest(".openModal");
        if (!btn) return;
        document.getElementById('tableResponsive').classList.add('hidden');
        const index = parseInt(btn.getAttribute("index"), 10);
        sessionStorage.setItem("editIndex", index);
        modal.classList.remove('hidden');
        modal.classList.add("flex");
        const emp = employees.find(e => e.Id === index);
        if (!emp) return;
        document.getElementById("uploadFileForm").querySelector('input[name="EmployeeId"]').value = emp.Id ?? "";
        // fill inputs by name attribute
        form.querySelector('[name="Id"]').value = emp.Id ?? "";
        form.querySelector('[name="Name"]').value = emp.Name ?? "";
        form.querySelector('[name="NationalId"]').value = emp.NationalId ?? "";
        form.querySelector('[name="PhoneNumber"]').value = emp.PhoneNumber ?? "";
        form.querySelector('[name="MarriageStatus"]').value = emp.MarriageStatus ?? "";
        form.querySelector('[name="Religion"]').value = emp.Religion ?? "";
        form.querySelector('[name="DateOfBirth"]').value = emp.DateOfBirth?.split('T')[0] ?? "";
        form.querySelector('[name="InsuranceNumber"]').value = emp.InsuranceNumber ?? "";
        form.querySelector('[name="HireDate"]').value = emp.HireDate?.split('T')[0] ?? "";
        form.querySelector('[name="EndDate"]').value = emp.EndDate?.split('T')[0] ?? "";
        form.querySelector('[name="LeaveReason"]').value = emp.LeaveReason ?? "";
        form.querySelector('[name="HRDepartmentId"]').value = emp.HRDepartmentId ?? "";// hhhere🟡
        form.querySelector('[name="JobName"]').value = emp.JobName ?? "";
        form.querySelector('[name="ContractType"]').value = emp.ContractType ?? "";
        form.querySelector('[name="BasmaId"]').value = emp.BasmaId ?? "";

        // open modal (Bootstrap, Tailwind, or your custom)
        modal.classList.remove("hidden");
    });
});

function closeModal() {
    const modal = document.getElementById("editModal");
    
    modal.classList.add("hidden");
    document.getElementById('tableResponsive').classList.remove('hidden');
    switchTab('info');
    sessionStorage.removeItem("editIndex");
}

function switchTab(tab) {
    const infoTab = document.getElementById('tab-info');
    const filesTab = document.getElementById('tab-files');
    const infoContent = document.getElementById('tab-content-info');
    const filesContent = document.getElementById('tab-content-files');

    if (tab === 'info') {
        infoTab.classList.add('border-[#404E3B]', 'text-[#404E3B]');
        infoTab.classList.remove('text-[#7B9669]');
        filesTab.classList.remove('border-[#404E3B]', 'text-[#404E3B]');
        filesTab.classList.add('text-[#7B9669]');
        infoContent.classList.remove('hidden');
        filesContent.classList.add('hidden');
    } else {
        filesTab.classList.add('border-[#404E3B]', 'text-[#404E3B]');
        filesTab.classList.remove('text-[#7B9669]');
        infoTab.classList.remove('border-[#404E3B]', 'text-[#404E3B]');
        infoTab.classList.add('text-[#7B9669]');
        filesContent.classList.remove('hidden');
        infoContent.classList.add('hidden');
        console.log(sessionStorage.getItem("editIndex"))
        getEmployeeFiles(employees.find(e => e.Id == parseInt(sessionStorage.getItem("editIndex"), 10)));
    };
}
// ✅ expose functions to global scope so onclick can find them
window.switchTab = switchTab;
window.closeModal = closeModal;
window.closeModalCreate = closeModalCreate;
window.deleteFile = deleteFile;
// ...existing code...

async function getEmployeeFiles(emp) {
    console.log('Getting files for employee:', emp);
    // ✅ Fetch employee files dynamically
    const filesList = document.querySelector("#tab-content-files ul");
    filesList.innerHTML = `
            <div class="flex flex-col justify-center items-center py-4">
                <div class="loader border-4 border-blue-500 border-t-transparent rounded-full w-8 h-8 animate-spin mb-2"></div>
                <p class="text-gray-600 text-sm">جارٍ تحميل الملفات...</p>
            </div>
        `;

    try {
        const res = await fetch(`/employees/files/${emp.Id}`);
        const files = await res.json();
        console.log('Fetched files:');
        console.log(files);

        if (files.length === 0) {
            filesList.innerHTML = "<p class='text-gray-500'>لا توجد ملفات لهذا الموظف.</p>";
        } else {
            filesList.innerHTML = files.map(file => `
            <li class="file-item flex flex-col justify-center align-items-center">
                <label class="max-w-[280px] truncate block">${file.FileName}</label>
                <div class="img-wrapper position-relative">
                    <img src="${file.Url}" alt="${file.FileName}" class="imgFile rounded transition-opacity" onclick="openImage(this.src)">
                    <button class="hover-button btn btn-danger btn-sm" onclick="deleteFile(${file.Id})">✕</button>
                </div>
                <a href="${file.Url}" download class="col-button-dark m-1 py-1">تحميل</a>
            </li>
        `).join('');
        }
    } catch (err) {
        console.error(err);
        filesList.innerHTML = "<p class='text-red-600'>حدث خطأ أثناء تحميل الملفات.</p>";
    }
}
document.getElementById('submitBtn').addEventListener('click', async function uploadFile(event) {
    event.preventDefault();
    event.stopPropagation(); // 🧱 stop the event from closing the modal
    console.log('Uploading files...');
    console.log("********************* this is file upload submit action")
    const form = document.getElementById('uploadFileForm');
    const formData = new FormData(form); // mfesh form data ezaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaay!
    const files = form.querySelector('#fileUpload').files;
    Array.from(files).forEach(
        (f)=>{
            if(f.name.length > 100){
                f.name = f.name.substring(0, 50);
                console.log(`File name truncated to 100 chars: ${f.name}`);
            }
        }
    );
    Array.from(files).forEach(
        (f)=>{
            console.log(`File name: ${f.name}`);
            
        }
    );
    if (!files || files.length === 0) {
        alert('⚠️ الرجاء اختيار ملف قبل الرفع.');
        return;
    }

    console.log('FormData prepared:', formData);

    try {
        const response = await fetch('/employees/uploadImage', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            throw new Error('فشل رفع الملفات');
        }

        const result = await response.json();
        alert('✅ تم رفع الملفات بنجاح!');
        document.getElementById("fileUpload").value = ""; // clear file

        console.log('Server response:', result);

        await getEmployeeFiles(employees[sessionStorage.getItem("editIndex") - 1]);


    } catch (err) {
        console.error(err);
        console.log(err);
        alert('❌ حدث خطأ أثناء رفع الملفات.');
    }
});
async function deleteFile(fileId) {
    if (!confirm('هل أنت متأكد من حذف هذا الملف؟')) return;

    try {
        const response = await fetch(`/employees/deleteFile/${fileId}`, {
            method: 'POST'
        });

        if (!response.ok) {
            throw new Error('فشل حذف الملف');
        }



        await getEmployeeFiles(employees[sessionStorage.getItem("editIndex") - 1]);

    } catch (err) {
        console.error(err);
        alert('❌ حدث خطأ أثناء حذف الملف.');
    }
}

async function downloadAllFiles() {
    const empId = sessionStorage.getItem("editIndex");

    const res = await fetch(`/employees/files/zip/${empId}`);
    const blob = await res.blob();

    const a = document.createElement("a");
    a.href = URL.createObjectURL(blob);
    const form = document.getElementById("employeeForm");
    const name = form.querySelector('[name="Name"]').value;
    a.download = `ملفات_${name}.zip`; // backend sets filename
    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(a.href);


}

document.getElementById('employeeForm').addEventListener('submit', async function (e) {
    if (e.target.id !== 'employeeForm') return; // 🧱 ignore bubbled submits
    e.preventDefault();

    const formData = new FormData(this);

    const response = await fetch('/Employees/Edit', {
        method: 'POST',
        body: formData
    });

    if (response.ok) {
        const index = sessionStorage.getItem("editIndex");
        const row = document.getElementById('row-' + index);
        row.querySelectorAll('td')[0].innerText = formData.get('Name');
        row.querySelectorAll('td')[1].innerText = formData.get('NationalId');
        row.querySelectorAll('td')[2].innerText = formData.get('PhoneNumber');
        row.querySelectorAll('td')[3].innerText = formData.get('MarriageStatus');
        row.querySelectorAll('td')[4].innerText = formData.get('Religion');
        row.querySelectorAll('td')[5].innerText = formData.get('DateOfBirth');
        row.querySelectorAll('td')[6].innerText = formData.get('InsuranceNumber');
        row.querySelectorAll('td')[7].innerText = formData.get('HireDate');
        row.querySelectorAll('td')[8].innerText = formData.get('EndDate');
        row.querySelectorAll('td')[9].innerText = formData.get('Department');
        row.querySelectorAll('td')[10].innerText = formData.get('JobName');
        row.querySelectorAll('td')[11].innerText = formData.get('ContractType');
        closeModal();
        setTimeout(() => {
            alert('تم الحفظ بنجاح ✅');
            window.location.href = "/employees";
        }, 200);

    } else {
        alert('حدث خطأ أثناء الحفظ ❌');
    }
});

function addEmployeeRow(emp) {
    employees = [...employees, emp]; // update local employees array
    const tbody = document.getElementById("employeesBody");
    console.log('employee dateofbirth:', emp);
    console.log('employee dateofbirth:', emp.id);
    const row = document.createElement("tr");
    row.innerHTML = `
        <td>${emp.Name}</td>
        <td>${emp.NationalId}</td>
        <td>${emp.PhoneNumber}</td>
        <td>${emp.MarriageStatus}</td>
        <td>${emp.Religion}</td>
        <td>${emp.DateOfBirth ? new Date(emp.DateOfBirth).toLocaleDateString().split('T')[0] : ''}</td>
        <td>${emp.InsuranceNumber}</td>
        <td>${emp.HireDate ? new Date(emp.HireDate).toLocaleDateString().split('T')[0] : ''}</td>
        <td></td>
        <td>${emp.Department}</td>
        <td>${emp.JobName}</td>
        <td>${emp.ContractType}</td>
        <td>
            <!-- Button to open modal -->
            <button class="openModal col-button-dark m-1  rounded "
                index="${emp.Id}">
                تعديل
            </button>
        </td>
                                `;
    // add 1 to the index

    row.setAttribute('id', 'row-' + emp.Id);
    row.classList.add('snap-start');
    console.log(row);
    tbody.appendChild(row);
}

function openImage(src) {
    const lightbox = document.getElementById('imageLightbox');
    const img = document.getElementById('lightboxImage');

    img.src = src;
    lightbox.classList.remove('hidden');
    lightbox.classList.add('flex');
    setTimeout(() => img.classList.add('scale-100'), 10); // smooth zoom-in
}

document.getElementById('imageLightbox').addEventListener('click', function () {
    const img = document.getElementById('lightboxImage');
    img.classList.remove('scale-100');
    setTimeout(() => {
        this.classList.add('hidden');
        this.classList.remove('flex');
    }, 200); // fade out
});
['submit', 'click', 'change'].forEach(evt => {
    document.addEventListener(evt, function (e) {
        console.log(
            `🔹 EVENT: ${evt} | TARGET: ${e.target.tagName}#${e.target.id || '(no id)'} | CLASS: ${e.target.className}`
        );
    }, true); // <-- use capture phase
});
const modal = document.getElementById("editModal");
// Optional: close modal when background is clicked
modal.addEventListener('click', function (e) {
    if (e.target === this) closeModal();
});
