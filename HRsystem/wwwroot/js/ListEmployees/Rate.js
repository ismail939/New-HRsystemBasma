// for rate
let idForCurEmp = 0;
function openRateModal(name, id) {
    document.getElementById("rateEmployeeId").value = id;
    document.getElementById('rateModal').classList.remove('hidden');
    document.getElementById('rateModal').classList.add('flex');
    document.getElementById("rateModalTitle").innerText = `تقييم الموظف: ${name}`;
    idForCurEmp = id;

} // rate modal

window.openRateModal = openRateModal;

document.addEventListener("DOMContentLoaded", () => {
    document.getElementById('rateModal').addEventListener('click', function (e) {
        if (e.target === this) closeRateModal();
    });

    document.getElementById("monthPicker").addEventListener("change", async () => {
    const yearMonth = document.getElementById("monthPicker").value;
    if (yearMonth == "") {
        alert("اختر شهر أولا");
    }

    // fetch request
    const rateDiv = document.getElementById("rateDiv");
    showDivFlex("rateDiv");


    let data = null;
    let [year, month] = yearMonth.split("-").map(Number);
    document.getElementById("rateMonth").value = month;
    document.getElementById("rateYear").value = year;
    await fetch(`/getRate?idForCurEmp=${idForCurEmp}&month=${month}&year=${year}`)
        .then(res => res.json())
        .then(dataJson => {
            console.log(dataJson);
            data = dataJson;
        }).catch(error => console.log(error));
    if (data.Success) {
        showRatingResult(data.Data)
        showDivFlex("ratingResult");
        hideDivFlex("ratingAdder");
    } else {
        showDivFlex("ratingAdder");
        hideDivFlex("ratingResult");
    }
})

document.getElementById("saveRateBtn").addEventListener("click", async () => {
    // post request 
    const employeeId = document.getElementById("rateEmployeeId").value;
    const rate = document.getElementById("rateValueHidden").value;
    const month = document.getElementById("rateMonth").value;
    const year = document.getElementById("rateYear").value;
    let success = false;
    if (rate > 5 || rate < 1) {
        alert("اختر تقييم اولا");
        return;
    }
    await fetch(`/addRate?rate=${rate}&employeeId=${employeeId}&month=${month}&year=${year}`, { method: "POST" })
        .then(res => res.json())
        .then(data => {
            success = data.Success;
        });
    if (success) {
        const rateDiv = document.getElementById("rateDiv");
        hideDivFlex("ratingAdder");
        showDivFlex("ratingResult");
        clearRatingAdder();
        closeRateModal();

    }
    })
});

function closeRateModal() {
    document.getElementById("monthPicker").value = "";
    document.getElementById("rateDiv").classList.add("hidden");
    clearRatingResult();
    clearRatingAdder();
    document.getElementById('rateModal').classList.remove('flex');
    document.getElementById('rateModal').classList.add('hidden');
}

window.openRateModal = openRateModal;
window.closeRateModal = closeRateModal;


// rate function
function setRating(val) {
    const box = document.querySelector('.rating');
    const stars = box.querySelectorAll('.star');

    stars.forEach((s, i) => s.textContent = i < val ? '★' : '☆');

    document.getElementById('ratingValue').textContent = `${val} / 5`;
    document.getElementById('ratingText').textContent =
        ['ضعيف', 'مقبول', 'جيد', 'جيد جدًا', 'ممتاز'][val - 1];
    document.getElementById("rateValueHidden").value = val;
}

function showRatingResult(Data) {
    console.log(Data);
    // document get text ids 
    document.getElementById("ratingResultValue").innerText = `${Data.Rate}/5`;
    document.getElementById("ratingResultText").innerText = ['ضعيف', 'مقبول', 'جيد', 'جيد جدًا', 'ممتاز'][Data.Rate - 1];
    // document get stars 
    const box = document.querySelector('#ratingResult');
    const stars = box.querySelectorAll('button');

    stars.forEach((s, i) => s.textContent = i < Data.Rate ? '★' : '☆');
}

function clearRatingAdder() {
    document.getElementById("ratingValue").innerText = "— / 5";
    document.getElementById("ratingText").innerText = "اختر تقييم";
    document.getElementById("rateValueHidden").value = "";

    document.querySelectorAll("#ratingAdder .star")
        .forEach(s => s.textContent = '☆');
}

function clearRatingResult() {
    // Reset النص
    document.getElementById("ratingResultValue").innerText = "— / 5";
    document.getElementById("ratingResultText").innerText = "لم يتم التقييم";

    // Reset النجوم
    const box = document.querySelector('#ratingResult');
    const stars = box.querySelectorAll('button');

    stars.forEach(s => s.textContent = '☆');
}