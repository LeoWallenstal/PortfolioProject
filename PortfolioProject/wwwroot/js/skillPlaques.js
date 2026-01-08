//Skills
const addSkillBtn = document.getElementById("btn-add-skill")
const skillTitleInput = document.getElementById("input-skill-name")
const skillImgUrl = document.getElementById("skill-img-url")
const skillPlaqueSection = document.getElementById("skill-plaque-section")

skillTitleInput.addEventListener("input", () => {
    resetError("skill-name-error")
    addSkillBtn.disabled = skillTitleInput.value.trim() === ""
})

addSkillBtn.addEventListener("click", () => {
    const skillPlaque = document.createElement("div")
    skillPlaque.className = "plaque"

    //Kollar efter dubletter
    const exists = Array.from(skillPlaqueSection.children).some(plaque => {
        const titleText = plaque.querySelector("p")?.innerText.trim();
        return titleText === skillTitleInput.value.trim();
    });

    if (exists) {
        showError("skill-name-error", "Den färdigheten finns redan!")
        return; 
    }

    const skillTitle = document.createElement("p")
    skillTitle.innerText = skillTitleInput.value.trim()



    const removeBtn = document.createElement("button")
    removeBtn.className = "remove-btn"
    removeBtn.innerText = "X"


    const thumbnail = document.createElement("img")
    thumbnail.className = "thumbnail"
    thumbnail.setAttribute("onerror", "../images/default-img.png")
    if (skillImgUrl.value.trim() !== "") {
        thumbnail.setAttribute("src", skillImgUrl.value.trim())
    }

    skillPlaque.appendChild(removeBtn)
    skillPlaque.appendChild(thumbnail)
    skillPlaque.appendChild(skillTitle)

    skillPlaqueSection.appendChild(skillPlaque)
    skillTitleInput.value = ""
    skillImgUrl.value = ""
    addSkillBtn.disabled = true
})

skillPlaqueSection.addEventListener("click", (e) => {
    if (e.target.classList.contains("remove-btn")) {
        const plaque = e.target.closest(".skill-plaque");
        plaque.remove(); 
    }
});

const showError = (errorSpanId, errorMsg) => {
    const errorSpan = document.getElementById(errorSpanId)
    errorSpan.style.display = "block"
    errorSpan.innerText = errorMsg
}

const resetError = (errorSpanId) => {
    const errorSpan = document.getElementById(errorSpanId)
    errorSpan.innerText = ""
    errorSpan.style.display = "none"
}

