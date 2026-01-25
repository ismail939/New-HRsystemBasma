// ⭐ Open Modal 
function openModal(empId) {
    const editModal = document.getElementById('editModal');
    document.getElementById('tableResponsive').classList.add('hidden');
    editModal.classList.remove('hidden');
    requestAnimationFrame(() => {
        editModal.classList.remove("opacity-0", "scale-95");
        editModal.classList.add("opacity-100", "scale-100", "flex");
    });
    document.getElementById("empId").value = empId;
}

// ⭐ Close Modal
function closeModal() {
    // empty or reset all inputs
    document.getElementById('from').value = "";
    document.getElementById('to').value = "";
    clearFlatpickrInModal('dateInputs');
    const modal = document.getElementById('editModal');
    modal.classList.add("opacity-0", "scale-95");
    modal.classList.remove("opacity-100", "scale-100");

    // wait for transition to finish
    setTimeout(() => {
        modal.classList.add("hidden");
    }, 300);
    document.getElementById('tableResponsive').classList.remove('hidden');
    document.getElementById("daysContainer").innerHTML = "";
    document.getElementById("daysContainer").classList.add("hidden");
    hideSaveButton();
}

function makeOffDay() {
    const form = document.getElementById('typePopup');
    const daysArray = loadDaysFromSession();
    let dateValue = JSON.parse(sessionStorage.getItem("curCardDate"));
    console.log(dateValue + "   💕\n" + daysArray);
    let item = daysArray.find(day => day.OffDayDate === dateValue);
    console.log(JSON.stringify(item) + " 💕before");
    item.isOffDay = true;
    let selectValue = document.getElementById('popupSelect').value;
    let otherValue = document.getElementById('otherInput').value;
    item.OffDayType = (selectValue === "أخرى" ? otherValue : selectValue);
    if (item.OffDayType.trim() == "") { // show error div if there is no value
        document.getElementById('error').classList.remove('hidden');
        return;
    } else { // 
        if (!document.getElementById('error').classList.contains('hidden')) {
            document.getElementById('error').classList.add('hidden');
        }
    }

    saveDaysToSession(daysArray);
    console.log(JSON.stringify(item) + "  💕after");
    highlightCard(dateValue, item.OffDayType);
    closeTypePopup();
}


const fromInput = document.getElementById("from");
const toInput = document.getElementById("to");

fromInput.addEventListener("change", () => {
    if (fromInput.value) {
        toInput.disabled = false;
        toInput.min = fromInput.value;
        toInput.value = ""; // reset previous value
    } else {
        toInput.disabled = true;
        toInput.value = "";
    }
});

// ⭐ Load Days via GET /employees/offdays/{id}?from=...&to=...
async function loadDays() {
    const id = document.getElementById("empId").value;
    const from = document.getElementById("from").value;
    const to = document.getElementById("to").value;

    if (!from || !to) {
        alert("من فضلك أدخل تاريخ البداية والنهاية");
        return;
    }
    console.log(`to: ${to} and from: ${from}`)
    const url = `/employees/offdays/${id}?from=${from}&to=${to}`;
    console.log("GET:", url);

    let days = [];

    try {
        const res = await fetch(url);
        days = await res.json();
        console.log(days);
    } catch {
        // fallback
        days = [
            { id: 1, name: "الأحد", date: "10 نوفمبر 2025", type: "" },
            { id: 2, name: "الاثنين", date: "11 نوفمبر 2025", type: "" },
            { id: 3, name: "الثلاثاء", date: "12 نوفمبر 2025", type: "" }
        ];
    }

    console.log(`FROM: ${from} to: ${to}`)
    showSaveButton();
    renderDays(days, from, to);
}


function showSaveButton() {
    document.getElementById('saveBtn').classList.remove('hidden');
    document.getElementById('saveBtn').classList.add('flex');
}
function hideSaveButton() {
    document.getElementById('saveBtn').classList.remove('flex');
    document.getElementById('saveBtn').classList.add('hidden');
}
// ⭐ Render selectable cards
let selectedDays = [];

function renderDays(existingDays, from, to) {
    const container = document.getElementById("daysContainer");
    container.innerHTML = "";
    container.classList.remove("hidden");
    container.classList.add("grid", "grid-cols-7", "place-items-stretch")
    selectedDays = [];

    let start = new Date(from);
    let end = new Date(to);
    var currentDate = new Date(start);
    let daysArray = []
    // loop each day
    for (let index = 1; currentDate <= end; index++) {
        console.log(`currentdate: ${currentDate.getDate()} and end is : ${end.getDate()}`);
        let iso = currentDate.toISOString().split("T")[0]; // yyyy-mm-dd
        console.log(iso + "after getting the current date 😢")

        // check if this day exists in backend list
        let match = existingDays.find(d => d.OffDayDate.split("T")[0] === iso);
        console.log(`match value is : ${JSON.stringify(match)} 🟢`)
        let dayName = currentDate.toLocaleDateString("ar-EG", { weekday: "long" });
        let dayDateText = currentDate.toLocaleDateString("ar-EG");

        let card = document.createElement("div");
        card.className =
            "p-1 text-center box-border border-[6px] border-transparent h-[60px] rounded-xl shadow transition cursor-pointer bg-white hover:shadow-lg";
        card.id = iso;
        card.innerHTML = `
            <div class="flex flex-col xl:flex-row items-center justify-center px-3 text-center" id="dayDateText">
                        <div class="font-bold flex flex-col">${dayName}<br><span class="text-xs xl:text-sm text-[#6C8480]">${dayDateText}</span></div>
                        <div id="type-${iso}" class="xl:mr-1  bg-red-100 p-1 rounded-lg hidden text-xs xl:text-sm"></div>
            </div>
        `;
        // click toggle
        card.onclick = (event) => {
            event.stopPropagation(); // prevent clicks inside from closing modal
            const daysArray = loadDaysFromSession();
            let item = daysArray.find(d => d.OffDayDate === iso);
            console.log(item + "🔴");
            if (item.isOffDay) {
                item.isOffDay = false;
                item.OffDayType = "";
                saveDaysToSession(daysArray);
                document.getElementById(`type-${iso}`).classList.add('hidden');
                unHighlightCard(iso);
            } else {
                openTypePopup(iso); // iso = date string
            }
        };
        container.appendChild(card);
        // highlight if already offday
        if (match) {
            console.log(`i entered if condition to highlight the card`);
            daysArray.push({ OffDayDate: iso, isOffDay: true, OffDayType: match["OffDayType"] });
            console.log(`type-${currentDate.toISOString().split("T")[0]}` + "⭐");
            highlightCard(iso, match["OffDayType"]);
        }
        else {
            daysArray.push({ OffDayDate: iso, isOffDay: false });
        }
        // calculate current date
        currentDate = new Date(start);
        currentDate.setDate(start.getDate() + index);
    }
    saveDaysToSession(daysArray);
}

function saveDaysToSession(daysArray) {
    sessionStorage.setItem("daysList", JSON.stringify(daysArray));
}

function loadDaysFromSession() {
    let data = sessionStorage.getItem("daysList");
    return data ? JSON.parse(data) : [];
}

// ⭐ Submit Vacation
async function submitVacation() {

    const employeeId = document.getElementById("empId").value;
    const daysArray = loadDaysFromSession(); // your stored list

    const url = "/employees/offdays/edit"; // must start with / for MVC

    const body = {
        employeeId: employeeId,
        days: daysArray
    };

    const res = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });

    if (res.ok) {
        alert("تم تعديل الإجازة بنجاح!");
        closeModal();
    } else {
        alert("حدث خطأ أثناء الحفظ ❌");
        console.log(await res.text());
    }
}
const editModal = document.getElementById("editModal");
// Optional: close modal when background is clicked
editModal.addEventListener('click', function (e) {
    if (e.target === this) closeModal();
});


