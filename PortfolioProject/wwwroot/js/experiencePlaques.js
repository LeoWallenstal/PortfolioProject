(() => {
    //Plaque Section
    const experiencePlaqueSection = document.getElementById("experience-plaque-section")

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

    //Button to add
    const addExperienceBtn = document.getElementById("btn-add-experience")
    
    
    addExperienceBtn.addEventListener("click", () => {
        const index = educationPlaqueSection.querySelectorAll(".plaque").length;
        const experiencePlaque = document.createElement("div")
        experiencePlaque.className = "plaque"

        //Skapar htmlelement
        const companyName = document.createElement("p")
        companyName.innerText = companyInput.value.trim()

        const roleName = document.createElement("p")
        roleName.innerText = roleInput.value.trim()

        const workYears = document.createElement("p")
        workYears.innerText = fromDateInput.value.trim() + " - " + toDateInput.value.trim()


        //Skapar inputmappers
        const mappers = [
            getInputMapper(companyInput, "Company", index),
            getInputMapper(roleInput, "Role", index),
            getInputMapper(fromDateInput, "StartYear", index),
            getInputMapper(toDateInput, "EndYear", index)
        ]

        //Remove-knapp på plaque
        const removeBtn = document.createElement("button")
        removeBtn.className = "remove-btn"
        removeBtn.type = "button"
        removeBtn.innerText = "X"


        //----Bygger ihop en plaque----
        //Lägg till gömda inputmapprs
        mappers.forEach(mapper => {
            experiencePlaque.appendChild(mapper)
        })

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
            const plaque = e.target.closest(".plaque")

            plaque.remove()
            reindexExperiences()
        }
    })

    const getInputMapper = (inputElement, nameOfProperty, index) => {
        const inputMapper = document.createElement("input")
        inputMapper.type = "hidden"
        inputMapper.name = `Experiences[${index}].${nameOfProperty}`
        inputMapper.value = inputElement.value.trim()

        return inputMapper
    }

    const inputIsntEmpty = (anInput) => anInput.value.trim().length > 0

    const resetInputs = (inputs) => {
        inputs.forEach((input) => {
            input.value = ""
        })
    }

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

    const reindexExperiences = () => {
        const plaques = experiencePlaqueSection.querySelectorAll(".plaque");

        plaques.forEach((plaque, index) => {
            plaque.querySelector('input[name$=".Company"]').name =
                `Experiences[${index}].Company`

            plaque.querySelector('input[name$=".Role"]').name =
                `Experiences[${index}].Role`

            plaque.querySelector('input[name$=".StartYear"]').name =
                `Experiences[${index}].StartYear`

            plaque.querySelector('input[name$=".EndYear"]').name =
                `Experiences[${index}].EndYear`
        });
    }

})()