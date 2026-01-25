
let popupTarget = null; // the card or day being edited

function openTypePopup(dateValue) {
    sessionStorage.setItem("curCardDate", JSON.stringify(dateValue));
    document.getElementById("typeModal").classList.remove("hidden");
    document.getElementById("typeModal").classList.add("flex");
}
document.getElementById('popupSelect').addEventListener('input', () => {
    const select = document.getElementById('popupSelect');
    const otherInput = document.getElementById('otherInput');

    if (select.value === "أخرى") {
        otherInput.classList.remove("hidden");
    } else {
        otherInput.classList.add("hidden");
        otherInput.value = ""; // optional: clear previous text
    }
});


function closeTypePopup() {
    document.getElementById('typePopup').reset();
    if (!document.getElementById('otherInput').classList.contains('hidden')) {
        document.getElementById('otherInput').classList.add('hidden');
    }
    if (!document.getElementById('error').classList.contains('hidden')) {
        document.getElementById('error').classList.add('hidden');
    }
    const popup = document.getElementById("typeModal");
    popup.classList.add("hidden");
}

function highlightCard(id, typeValue) {
    const card = document.getElementById(id);
    card.classList.remove("border-transparent");
    card.classList.add("border-[6px]", "border-[#404E3B]", "box-border");
    setTimeout(() => {
        const type = document.getElementById(`type-${id}`);
        type.innerText = typeValue;
        type.classList.remove('hidden');
    }, 10);
}

function unHighlightCard(id) {
    const card = document.getElementById(id)
    card.classList.remove("border-[#404E3B]");
    card.classList.add("border-transparent");
}

const modalTypePopupBackdrop = document.getElementById("typeModal");
// Optional: close modal when background is clicked
modalTypePopupBackdrop.addEventListener('click', function (e) {
    if (e.target === this) closeTypePopup();
});
