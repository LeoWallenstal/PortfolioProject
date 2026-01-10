(() => { 
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
        const index = educationPlaqueSection.querySelectorAll(".plaque").length;

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
        removeBtn.type = "button"   // För att inte knappen ska 'submitta'
        removeBtn.className = "remove-btn"
        removeBtn.innerText = "X"

        const thumbnail = document.createElement("img")
        thumbnail.className = "thumbnail"
    
        thumbnail.setAttribute("onerror", "this.src='../images/default-img.png'");
        if (skillImgUrl.value.trim() !== "") {
            thumbnail.setAttribute("src", skillImgUrl.value.trim())
        }

        //Inputmapping
        const mappers = [
            getInputMapper(skillTitleInput, "Name", index),
            getInputMapper(skillImgUrl, "ImageUrl", index)
        ]

        //Sätter ihop plaquen
        skillPlaque.appendChild(removeBtn)
        skillPlaque.appendChild(thumbnail)
        skillPlaque.appendChild(skillTitle)
        //Nedanstående två är gömda, och används för att mappa tillbaka mot VM
        mappers.forEach((mapper) => {
            skillPlaque.appendChild(mapper)
        })

        skillPlaqueSection.appendChild(skillPlaque)

        skillTitleInput.value = ""
        skillImgUrl.value = ""
        addSkillBtn.disabled = true
    })

    skillPlaqueSection.addEventListener("click", (e) => {
        if (e.target.classList.contains("remove-btn")) {
            const plaque = e.target.closest(".plaque")
            plaque.remove() 
            reindexSkills()
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

    const reindexSkills = () => {
        const plaques = skillPlaqueSection.querySelectorAll(".plaque");

        plaques.forEach((plaque, index) => {
            plaque.querySelector('input[name$=".Name"]').name =
                `Skills[${index}].Name`

            plaque.querySelector('input[name$=".ImageUrl"]').name =
                `Skills[${index}].ImageUrl`
        });
    }

    const getInputMapper = (inputElement, nameOfProperty, index) => {
        const inputMapper = document.createElement("input")
        inputMapper.type = "hidden"
        inputMapper.name = `Skills[${index}].${nameOfProperty}`
        inputMapper.value = inputElement.value.trim()

        return inputMapper
    }

})()