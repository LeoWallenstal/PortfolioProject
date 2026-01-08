const pwBtn = document.getElementById("reavel-pw-btn")
const pwSection = document.GetElementById("pw-section")

pwBtn.addEventListener("click", () => {
    if (pwSection.style.display == "none") {
        pwSection.style.display == "block"
    }
    else {
        pwSection.style.display == "none"
    }
})