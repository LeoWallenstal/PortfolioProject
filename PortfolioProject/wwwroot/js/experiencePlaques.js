(() => {
    const addExperienceBtn = document.getElementById("btn-add-experience")

    //Inputs
    const companyInput = document.getElementById("company-input")
    const roleInput = document.getElementById("role-input")
    const fromDateInput = document.getElementById("from-year-experience-input")
    const toDateInput = document.getElementById("to-year-experience-input")

    const inputs = [companyInput, roleInput, fromDateInput, toDateInput]

    inputs.forEach((input) => {
        input.addEventListener("input", () => {
            const allFilled = inputs.every(inputIsntEmpty)
            addExperienceBtn.disabled = !allFilled
        })
    })

    //Plaque Section
    const experiencePlaqueSection = document.getElementById("experience-plaque-section")

    addExperienceBtn.addEventListener("click", () => {
        const experiencePlaque = document.createElement("div")
        experiencePlaque.className = "plaque"

        const datesAreValid = validDate(fromDateInput.value.trim()) && validDate(toDateInput.value.trim())


        if (!datesAreValid) {
            showError("experience-year-error", "Ogiltigt datumformat! (yyyy-mm-dd)")
            return
        }

        //Bygger ihop plaquen här

        const companyName = document.createElement("p")
        companyName.innerText = companyInput.value.trim()

        const roleName = document.createElement("p")
        roleName.innerText = roleInput.value.trim()

        const workYears = document.createElement("p")
        workYears.innerText = fromDateInput.value.trim() + " - " + toDateInput.value.trim()


        const removeBtn = document.createElement("button")
        removeBtn.className = "remove-btn"
        removeBtn.innerText = "X"

        experiencePlaque.appendChild(removeBtn)
        experiencePlaque.appendChild(companyName)
        experiencePlaque.appendChild(roleName)
        experiencePlaque.appendChild(workYears)

        experiencePlaqueSection.appendChild(experiencePlaque)
        addExperienceBtn.disabled = true
        resetInputs(inputs)
    })

    experiencePlaqueSection.addEventListener("click", (e) => {
        if (e.target.classList.contains("remove-btn")) {
            const plaque = e.target.closest(".skill-plaque");
            plaque.remove();
        }
    })

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

    const resetInputs = (inputs) => {
        inputs.forEach((input) => {
            input.value = ""
        })
    }

})()