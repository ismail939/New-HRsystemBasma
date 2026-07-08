document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("employeeForm");
  const modal = document.getElementById("editModal"); // assume you have a modal container

  document
    .getElementById("uploadFileForm")
    .addEventListener("click", (e) => e.stopPropagation());
  document
    .getElementById("uploadFileForm")
    .addEventListener("submit", (e) => e.stopPropagation());

  document.getElementById("tableResponsive").addEventListener("click", (e) => {
    const btn = e.target.closest(".openModal");
    if (!btn) return;
    document.getElementById("tableResponsive").classList.add("hidden");
    const index = parseInt(btn.getAttribute("index"), 10);
    sessionStorage.setItem("editIndex", index);
    modal.classList.remove("hidden");
    modal.classList.add("flex");
    const emp = employees.find((e) => e.Id === index);
    if (!emp) return;
    document
      .getElementById("uploadFileForm")
      .querySelector('input[name="EmployeeId"]').value = emp.Id ?? "";
    // fill inputs by name attribute
    form.querySelector('[name="Id"]').value = emp.Id ?? "";
    form.querySelector('[name="Name"]').value = emp.Name ?? "";
    form.querySelector('[name="NationalId"]').value = emp.NationalId ?? "";
    form.querySelector('[name="PhoneNumber"]').value = emp.PhoneNumber ?? "";
    form.querySelector('[name="Address"]').value = emp.Address ?? "";
    form.querySelector('[name="MarriageStatus"]').value =
      emp.MarriageStatus ?? "";
    form.querySelector('[name="Religion"]').value = emp.Religion ?? "";
    form.querySelector('[name="DateOfBirth"]').value =
      emp.DateOfBirth?.split("T")[0] ?? "";
    form.querySelector('[name="InsuranceNumber"]').value =
      emp.InsuranceNumber ?? "";
    form.querySelector('[name="HireDate"]').value =
      emp.HireDate?.split("T")[0] ?? "";
    form.querySelector('[name="EndDate"]').value =
      emp.EndDate?.split("T")[0] ?? "";
    form.querySelector('[name="LeaveReason"]').value = emp.LeaveReason ?? "";
    form.querySelector('[name="HRDepartmentId"]').value =
      emp.HRDepartmentId ?? ""; // hhhere🟡
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
  document.getElementById("tableResponsive").classList.remove("hidden");
  switchTab("info");
  sessionStorage.removeItem("editIndex");
}

function switchTab(tab) {
  const infoTab = document.getElementById("tab-info");
  const filesTab = document.getElementById("tab-files");
  const infoContent = document.getElementById("tab-content-info");
  const filesContent = document.getElementById("tab-content-files");

  if (tab === "info") {
    infoTab.classList.add("border-[#404E3B]", "text-[#404E3B]");
    infoTab.classList.remove("text-[#7B9669]");
    filesTab.classList.remove("border-[#404E3B]", "text-[#404E3B]");
    filesTab.classList.add("text-[#7B9669]");
    infoContent.classList.remove("hidden");
    filesContent.classList.add("hidden");
  } else {
    filesTab.classList.add("border-[#404E3B]", "text-[#404E3B]");
    filesTab.classList.remove("text-[#7B9669]");
    infoTab.classList.remove("border-[#404E3B]", "text-[#404E3B]");
    infoTab.classList.add("text-[#7B9669]");
    filesContent.classList.remove("hidden");
    infoContent.classList.add("hidden");
    console.log(sessionStorage.getItem("editIndex"));
    getEmployeeFiles(
      employees.find(
        (e) => e.Id == parseInt(sessionStorage.getItem("editIndex"), 10),
      ),
    );
  }
}
// ✅ expose functions to global scope so onclick can find them
window.switchTab = switchTab;
window.closeModal = closeModal;
window.deleteFile = deleteFile;
window.openImage = openImage;
window.openPdf = openPdf;
window.closePdf = closePdf;
// ...existing code...

async function getEmployeeFiles(emp) {
  console.log("🔍 getEmployeeFiles called for employee:", emp?.Id, emp?.Name);
  const filesList = document.querySelector("#tab-content-files ul");
  if (!filesList) {
    console.error("❌ Files list not found!");
    return;
  }
  
  filesList.innerHTML = `
    <div class="flex flex-col justify-center items-center py-4">
      <div class="loader border-4 border-blue-500 border-t-transparent rounded-full w-8 h-8 animate-spin mb-2"></div>
      <p class="text-gray-600 text-sm">جارٍ تحميل الملفات...</p>
    </div>
  `;

  try {
    const res = await fetch(`/employees/files/${emp.Id}`);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const files = await res.json();
    console.log("✅ Fetched files:", files);

    if (files.length === 0) {
      filesList.innerHTML = "<p class='text-gray-500'>لا توجد ملفات لهذا الموظف.</p>";
      return;
    }

    let html = '';
    files.forEach((file) => {
      const isImage = /\.(jpg|jpeg|png|gif|webp)$/i.test(file.FileName);
      const isPdf = /\.pdf$/i.test(file.FileName);
      
      if (isPdf) {
        const encodedUrl = encodeURI(file.Url);
        const encodedName = encodeURIComponent(file.FileName);
        html += `
          <li class="file-item flex flex-col items-center p-2 border rounded hover:shadow-md transition-shadow">
            <label class="max-w-[280px] truncate block text-sm mb-2">${file.FileName}</label>
            <div onclick="window.openPdf('${encodedUrl}', '${encodedName}')" 
                 style="cursor: pointer; padding: 10px; background: #fee; border-radius: 8px; text-align: center; min-width: 150px;"
                 title="Click to preview PDF">
              <i class="bi bi-file-earmark-pdf-fill text-red-600" style="font-size: 48px;"></i>
              <div style="margin-top: 8px; font-size: 12px; color: #666;">👆 معاينة</div>
            </div>
            <button onclick="event.stopPropagation(); deleteFile(${file.Id})" 
                    style="margin-top: 8px;"
                    class="btn btn-danger btn-sm">✕ حذف</button>
          </li>`;
      } else if (isImage) {
        html += `
          <li class="file-item flex flex-col items-center p-2 border rounded hover:shadow-md transition-shadow">
            <label class="max-w-[280px] truncate block text-sm mb-2">${file.FileName}</label>
            <img src="${file.Url}" alt="${file.FileName}" 
                 onclick="window.openImage(this.src)"
                 style="cursor: pointer; max-width: 150px; max-height: 150px; border-radius: 8px;"
                 class="imgFile">
            <button onclick="event.stopPropagation(); deleteFile(${file.Id})" 
                    style="margin-top: 8px;"
                    class="btn btn-danger btn-sm">✕ حذف</button>
          </li>`;
      } else {
        html += `
          <li class="file-item flex flex-col items-center p-2 border rounded">
            <label class="max-w-[280px] truncate block text-sm mb-2">${file.FileName}</label>
            <div style="padding: 20px; background: #f5f5f5; border-radius: 8px; text-align: center; min-width: 150px;">
              <i class="bi bi-file-earmark-text-fill text-gray-400" style="font-size: 48px;"></i>
            </div>
            <button onclick="deleteFile(${file.Id})" 
                    style="margin-top: 8px;"
                    class="btn btn-danger btn-sm">✕ حذف</button>
            <a href="${file.Url}" download class="col-button-light btn-sm mt-2 px-3 py-1 rounded">تحميل</a>
          </li>`;
      }
    });
    
    filesList.innerHTML = html;
    console.log("✅ Files rendered to DOM");

  } catch (err) {
    console.error("❌ Error fetching files:", err);
    filesList.innerHTML = `<p class='text-red-600'>حدث خطأ أثناء تحميل الملفات: ${err.message}</p>`;
  }
}
document
  .getElementById("submitBtn")
  .addEventListener("click", async function uploadFile(event) {
    event.preventDefault();
    event.stopPropagation(); // 🧱 stop the event from closing the modal
    console.log("Uploading files...");
    console.log("********************* this is file upload submit action");
    const form = document.getElementById("uploadFileForm");
    const formData = new FormData(form); // mfesh form data ezaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaay!
    const files = form.querySelector("#fileUpload").files;
    Array.from(files).forEach((f) => {
      if (f.name.length > 100) {
        f.name = f.name.substring(0, 50);
        console.log(`File name truncated to 100 chars: ${f.name}`);
      }
    });
    Array.from(files).forEach((f) => {
      console.log(`File name: ${f.name}`);
    });
    if (!files || files.length === 0) {
      alert("⚠️ الرجاء اختيار ملف قبل الرفع.");
      return;
    }

    console.log("FormData prepared:", formData);

    try {
      const response = await fetch("/employees/uploadImage", {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        throw new Error("فشل رفع الملفات");
      }

      const result = await response.json();
      alert("✅ تم رفع الملفات بنجاح!");
      document.getElementById("fileUpload").value = ""; // clear file

      console.log("Server response:", result);

      const empId = parseInt(sessionStorage.getItem("editIndex"), 10);
      const emp = employees.find((e) => e.Id === empId);
      if (emp) await getEmployeeFiles(emp);
    } catch (err) {
      console.error(err);
      console.log(err);
      alert("❌ حدث خطأ أثناء رفع الملفات.");
    }
  });
async function deleteFile(fileId) {
  if (!confirm("هل أنت متأكد من حذف هذا الملف؟")) return;

  try {
    const response = await fetch(`/employees/deleteFile/${fileId}`, {
      method: "POST",
    });

    if (!response.ok) {
      throw new Error("فشل حذف الملف");
    }

    const empId = parseInt(sessionStorage.getItem("editIndex"), 10);
    const emp = employees.find((e) => e.Id === empId);
    if (emp) await getEmployeeFiles(emp);
  } catch (err) {
    console.error(err);
    alert("❌ حدث خطأ أثناء حذف الملف.");
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

document
  .getElementById("employeeForm")
  .addEventListener("submit", async function (e) {
    if (e.target.id !== "employeeForm") return; // 🧱 ignore bubbled submits
    e.preventDefault();

    const formData = new FormData(this);

    const response = await fetch("/Employees/Edit", {
      method: "POST",
      body: formData,
    });

    if (response.ok) {
      const index = sessionStorage.getItem("editIndex");
      const row = document.getElementById("row-" + index);
      // row.querySelectorAll('td')[0].innerText = formData.get('Name');
      // row.querySelectorAll('td')[1].innerText = formData.get('NationalId');
      // row.querySelectorAll('td')[2].innerText = formData.get('PhoneNumber');
      // row.querySelectorAll('td')[3].innerText = formData.get('Address');
      // row.querySelectorAll('td')[4].innerText = formData.get('MarriageStatus');
      // row.querySelectorAll('td')[5].innerText = formData.get('Religion');
      // row.querySelectorAll('td')[6].innerText = formData.get('DateOfBirth');
      // row.querySelectorAll('td')[7].innerText = formData.get('InsuranceNumber');
      // row.querySelectorAll('td')[8].innerText = formData.get('HireDate');
      // row.querySelectorAll('td')[9].innerText = formData.get('EndDate');
      // row.querySelectorAll('td')[10].innerText = formData.get('Department');
      // row.querySelectorAll('td')[11].innerText = formData.get('JobName');
      // row.querySelectorAll('td')[12].innerText = formData.get('ContractType');
      closeModal();
      setTimeout(() => {
        alert("تم الحفظ بنجاح ✅");
        window.location.href = "/employees";
      }, 200);
    } else {
      alert("حدث خطأ أثناء الحفظ ❌");
    }
  });

function addEmployeeRow(emp) {
  employees = [...employees, emp]; // update local employees array
  const tbody = document.getElementById("employeesBody");
  console.log("employee dateofbirth:", emp);
  console.log("employee dateofbirth:", emp.id);
  const row = document.createElement("tr");
  row.innerHTML = `
        <td>${emp.Name}</td>
        <td>${emp.NationalId}</td>
        <td>${emp.PhoneNumber}</td>
        <td>${emp.Address}</td>
        <td>${emp.MarriageStatus}</td>
        <td>${emp.Religion}</td>
        <td>${emp.DateOfBirth ? new Date(emp.DateOfBirth).toLocaleDateString().split("T")[0] : ""}</td>
        <td>${emp.InsuranceNumber}</td>
        <td>${emp.HireDate ? new Date(emp.HireDate).toLocaleDateString().split("T")[0] : ""}</td>
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

  row.setAttribute("id", "row-" + emp.Id);
  row.classList.add("snap-start");
  console.log(row);
  tbody.appendChild(row);
}

function openImage(src) {
  const lightbox = document.getElementById("imageLightbox");
  const img = document.getElementById("lightboxImage");

  img.src = src;
  lightbox.classList.remove("hidden");
  lightbox.classList.add("flex");
  setTimeout(() => img.classList.add("scale-100"), 10); // smooth zoom-in
}

function openPdf(url, fileName) {
  console.log("openPdf called with:", url, fileName);
  const pdfModal = document.getElementById("pdfLightbox");
  const iframe = document.getElementById("pdfViewer");
  const title = document.getElementById("pdfTitle");

  iframe.src = url;
  title.textContent = fileName || "معاينة PDF";
  pdfModal.classList.remove("hidden");
  pdfModal.classList.add("flex");
}

function closePdf() {
  const pdfModal = document.getElementById("pdfLightbox");
  const iframe = document.getElementById("pdfViewer");

  pdfModal.classList.add("hidden");
  pdfModal.classList.remove("flex");
  iframe.src = ""; // unload PDF to free memory
}

document.getElementById("imageLightbox").addEventListener("click", function () {
  const img = document.getElementById("lightboxImage");
  img.classList.remove("scale-100");
  setTimeout(() => {
    this.classList.add("hidden");
    this.classList.remove("flex");
  }, 200); // fade out
});
["submit", "click", "change"].forEach((evt) => {
  document.addEventListener(
    evt,
    function (e) {
      console.log(
        `🔹 EVENT: ${evt} | TARGET: ${e.target.tagName}#${e.target.id || "(no id)"} | CLASS: ${e.target.className}`,
      );
    },
    true,
  ); // <-- use capture phase
});
const modal = document.getElementById("editModal");
// Optional: close modal when background is clicked
modal.addEventListener("click", function (e) {
  if (e.target === this) closeModal();
});
