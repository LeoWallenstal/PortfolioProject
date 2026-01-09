(() => {

    const addEducationBtn = document.getElementById("btn-add-education")

    //Inputs
    const schoolInput = document.getElementById("school-input")
    const degreeInput = document.getElementById("degree-input")
    const fromDateInput = document.getElementById("from-year-education-input")
    const toDateInput = document.getElementById("to-year-education-input")

    const inputs = [schoolInput, degreeInput, fromDateInput, toDateInput]
    inputs.forEach((input) => {
        input.addEventListener("input", () => {
            const allFilled = inputs.every(inputIsntEmpty)

            addEducationBtn.disabled = !allFilled

        })
    })

    //Plaque Section
    const educationPlaqueSection = document.getElementById("education-plaque-section")

    addEducationBtn.addEventListener("click", () => {
        const index = educationPlaqueSection.children.length;
        const educationPlaque = document.createElement("div")
        educationPlaque.className = "plaque"

        //Kollar efter dubletter
        const schoolAlreadyExists = Array.from(educationPlaqueSection.children).some(plaque => {
            const schoolName = plaque.children[1]?.innerText.trim();
            return schoolName === schoolInput.value.trim()
        });

        const educationAlreadyExists = Array.from(educationPlaqueSection.children).some(plaque => {
            const educationName = plaque.children[2]?.innerText.trim();
            return educationName === degreeInput.value.trim()
        });

        const datesAreValid = validDate(fromDateInput.value.trim()) && validDate(toDateInput.value.trim())

        const validityChecksPassed = !schoolAlreadyExists && !educationAlreadyExists && datesAreValid

        if (schoolAlreadyExists) {
            showError("school-name-error", "Den skolan finns redan!")
        }
        if (educationAlreadyExists) {
            showError("education-name-error", "Den utbildningen finns redan!")
        }
        if (!datesAreValid) {
            showError("education-year-error", "Ogiltigt datumformat! (yyyy-mm-dd)")
        }

        if (!validityChecksPassed) {
            return
        }

        //Bygger ihop plaquen här

        const schoolName = document.createElement("p")
        schoolName.innerText = schoolInput.value.trim()

        const degreeName = document.createElement("p")
        degreeName.innerText = degreeInput.value.trim()

        const schoolYears = document.createElement("p")
        schoolYears.innerText = fromDateInput.value.trim() + " - " + toDateInput.value.trim()

        const removeBtn = document.createElement("button")
        removeBtn.className = "remove-btn"
        removeBtn.type = "button"
        removeBtn.innerText = "X"

        //Mappers
        const mappers = [
            getInputMapper(schoolInput, "School", index),
            getInputMapper(degreeInput, "Degree", index),
            getInputMapper(fromDateInput, "FromYear", index),
            getInputMapper(toDateInput, "ToYear", index)
        ]

        mappers.forEach((mapper) => {
            educationPlaque.appendChild(mapper)
        })

        educationPlaque.appendChild(removeBtn)
        educationPlaque.appendChild(schoolName)
        educationPlaque.appendChild(degreeName)
        educationPlaque.appendChild(schoolYears)

        educationPlaqueSection.appendChild(educationPlaque)
        addEducationBtn.disabled = true
        resetInputs(inputs)
    })

    educationPlaqueSection.addEventListener("click", (e) => {
        if (e.target.classList.contains("remove-btn")) {
            const plaque = e.target.closest(".plaque")
            plaque.remove()
            reindexEducations()
        }
    });

    const inputIsntEmpty = (anInput) => anInput.value.trim().length > 0

    const validDate = (aString) => {
        const regex = /^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$/
        return regex.test(aString)
    } 

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

    const reindexEducations = () => {
        const plaques = educationPlaqueSection.querySelectorAll(".plaque");

        plaques.forEach((plaque, index) => {
            plaque.querySelector('input[name$=".School"]').name =
                `Educations[${index}].School`

            plaque.querySelector('input[name$=".Degree"]').name =
                `Educations[${index}].Degree`

            plaque.querySelector('input[name$=".FromYear"]').name =
                `Educations[${index}].FromYear`

            plaque.querySelector('input[name$=".ToYear"]').name =
                `Educations[${index}].ToYear`
        });
    }

    const getInputMapper = (inputElement, nameOfProperty, index) => {
        const inputMapper = document.createElement("input")
        inputMapper.type = "hidden"
        inputMapper.name = `Educations[${index}].${nameOfProperty}`
        inputMapper.value = inputElement.value.trim()

        return inputMapper
    }

})()

