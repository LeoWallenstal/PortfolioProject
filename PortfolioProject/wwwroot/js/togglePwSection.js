const pwBtn = document.getElementById("change-pw-btn")
const pwSection = document.getElementById("pw-section")

pwBtn.addEventListener("click", () => {
    const currentDisplay = window.getComputedStyle(pwSection).display;

    if (currentDisplay === "none") {
        pwSection.style.display = "flex"
        pwBtn.innerText = "Göm"
    }
    else {
        pwSection.style.display = "none"
        pwBtn.innerText = "Ändra Lösenord"
    }
})
