let createForm;
let createModal;

function closeModalCreate() {
    const modal = document.getElementById('createModal');
    modal.classList.add("opacity-0", "scale-95");
    modal.classList.remove("opacity-100", "scale-100");

    // wait for transition to finish
    setTimeout(() => {
        modal.classList.add("hidden");
    }, 300);
    document.getElementById('tableResponsive').classList.remove('hidden');
}

// Expose to global scope
window.closeModalCreate = closeModalCreate;

document.addEventListener("DOMContentLoaded", () => {
    createForm = document.getElementById("employeeFormCreate");
    createModal = document.getElementById("createModal");

    document.getElementById('createBtn').addEventListener('click', function (e) {
        e.preventDefault();
        document.getElementById('tableResponsive').classList.add('hidden');
        createModal.classList.remove('hidden');
        requestAnimationFrame(() => {
            createModal.classList.remove("opacity-0", "scale-95");
            createModal.classList.add("opacity-100", "scale-100", "flex");
        });
    });

    document.getElementById('employeeFormCreate').addEventListener('submit', async function (e) {
    if (e.target.id !== 'employeeFormCreate') return; // 🧱 ignore bubbled submits
    e.preventDefault();

    const formData = new FormData(this);

    const response = await fetch('/employees/add', {
        method: 'POST',
        body: formData
    });

    if (response.ok) {
        alert('تم الإضافة بنجاح ✅');
        closeModalCreate();
        this.reset()
        const newEmp = await response.json();
        console.log('New employee added:', newEmp);
        window.location.href = "/employees"

    } else {
        alert('حدث خطأ أثناء الإضافة ❌');
    }
    });

    // Optional: close modal when background is clicked
    document.getElementById('createModal').addEventListener('click', function (e) {
        if (e.target === this) closeModalCreate();
    });
});

