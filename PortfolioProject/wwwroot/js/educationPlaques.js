(() => {

    document.querySelectorAll(".education-section").forEach(section => {

        const addEducationBtn = section.querySelector(".btn-add-education");
        const schoolInput = section.querySelector(".school-input");
        const degreeInput = section.querySelector(".degree-input");
        const fromDateInput = section.querySelector(".from-year-education-input");
        const toDateInput = section.querySelector(".to-year-education-input");
        const educationPlaqueSection = section.querySelector(".education-plaque-section");

        const inputs = [schoolInput, degreeInput, fromDateInput, toDateInput];

        const resetError = (errorClass) => {
            const errorSpan = section.querySelector(`.${errorClass}`);
            if (!errorSpan) return;
            errorSpan.innerText = "";
            errorSpan.style.display = "none";
        };

        const showError = (errorClass, msg) => {
            const errorSpan = section.querySelector(`.${errorClass}`);
            if (!errorSpan) return;
            errorSpan.innerText = msg;
            errorSpan.style.display = "block";
        };

        inputs.forEach(input => {
            input.addEventListener("input", () => {
                resetError("school-name-error");
                resetError("education-name-error");
                addEducationBtn.disabled = !inputs.every(i => i.value.trim() !== "");
            });
        });

        addEducationBtn.addEventListener("click", () => {
            const school = schoolInput.value.trim();
            const degree = degreeInput.value.trim();
            const startYear = fromDateInput.value;
            const endYear = toDateInput.value;

            // Check for duplicates
            const exists = Array.from(educationPlaqueSection.children).some(plaque => {
                const pSchool = plaque.querySelector("p.school")?.innerText.trim();
                const pDegree = plaque.querySelector("p.degree")?.innerText.trim();
                return pSchool === school && pDegree === degree;
            });

            if (exists) {
                showError("school-name-error", "Den skolan finns redan!");
                return;
            }

            const index = educationPlaqueSection.querySelectorAll(".plaque").length;

            const plaque = document.createElement("div");
            plaque.className = "plaque";

            const removeBtn = document.createElement("button");
            removeBtn.type = "button";
            removeBtn.className = "remove-btn";
            removeBtn.innerText = "X";

            const schoolP = document.createElement("p");
            schoolP.className = "school";
            schoolP.innerText = school;

            const degreeP = document.createElement("p");
            degreeP.className = "degree";
            degreeP.innerText = degree;

            const yearsP = document.createElement("p");
            yearsP.className = "years";
            yearsP.innerText = `${startYear} - ${endYear}`;

            plaque.append(
                removeBtn,
                schoolP,
                degreeP,
                yearsP,
                createHidden("Educations", index, "School", school),
                createHidden("Educations", index, "Degree", degree),
                createHidden("Educations", index, "StartYear", startYear),
                createHidden("Educations", index, "EndYear", endYear)
            );

            educationPlaqueSection.appendChild(plaque);

            // Reset inputs
            inputs.forEach(i => i.value = "");
            addEducationBtn.disabled = true;
        });

        educationPlaqueSection.addEventListener("click", e => {
            if (!e.target.classList.contains("remove-btn")) return;
            e.target.closest(".plaque").remove();
            reindexEducation(educationPlaqueSection);
        });

    });

    function reindexEducation(container) {
        container.querySelectorAll(".plaque").forEach((plaque, index) => {
            plaque.querySelector('input[name$=".School"]').name = `Educations[${index}].School`;
            plaque.querySelector('input[name$=".Degree"]').name = `Educations[${index}].Degree`;
            plaque.querySelector('input[name$=".StartYear"]').name = `Educations[${index}].StartYear`;
            plaque.querySelector('input[name$=".EndYear"]').name = `Educations[${index}].EndYear`;
        });
    }

    function createHidden(collection, index, prop, value) {
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = `${collection}[${index}].${prop}`;
        input.value = value;
        return input;
    }

})();
