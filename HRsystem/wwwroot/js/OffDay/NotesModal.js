function openNotesModal() {
    const empId = document.getElementById("empId").value;
    showDivFlex('notesModal');
    fetch(`/offdays/balance/notes?employeeId=${empId}`).then(res => res.json()).then(data => {
        document.getElementById('notesTextarea').value = data.notes || "";
    });
    
}
function closeNotesModal() {
    hideDivFlex('notesModal');
}

const notesModal = document.getElementById('notesModal');
notesModal.addEventListener('click', (event) => {
    if (event.target === notesModal) {
        closeNotesModal();
    }
});

function saveNotes() {
    const notes = document.getElementById('notesTextarea').value;
    const employeeId = document.getElementById("empId").value;

    const url = `/offdays/balance/notes/edit?employeeId=${employeeId}`; // must start with / for MVC
    const body = {
        notes: notes
    };
    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    }).then(res => {
        if (res.ok) {
            console.log("Notes saved successfully");
            closeNotesModal();
        } else {
            console.log("حدث خطأ أثناء الحفظ ❌");
            res.text().then(text => console.log(text));
        }
    });
}