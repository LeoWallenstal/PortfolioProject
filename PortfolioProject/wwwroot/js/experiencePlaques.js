(() => {

    document.querySelectorAll(".experience-section").forEach(section => {

        const addExperienceBtn = section.querySelector(".btn-add-experience");
        const companyInput = section.querySelector(".company-input");
        const roleInput = section.querySelector(".role-input");
        const fromDateInput = section.querySelector(".from-year-experience-input");
        const toDateInput = section.querySelector(".to-year-experience-input");
        const experiencePlaqueSection = section.querySelector(".experience-plaque-section");

        const inputs = [companyInput, roleInput, fromDateInput, toDateInput];

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
                resetError("company-error");
                resetError("role-error");
                addExperienceBtn.disabled = !inputs.every(i => i.value.trim() !== "");
            });
        });

        addExperienceBtn.addEventListener("click", () => {
            const company = companyInput.value.trim();
            const role = roleInput.value.trim();
            const startYear = fromDateInput.value;
            const endYear = toDateInput.value;

            // Check for duplicates
            const exists = Array.from(experiencePlaqueSection.children).some(plaque => {
                const pCompany = plaque.querySelector("p.company")?.innerText.trim();
                const pRole = plaque.querySelector("p.role")?.innerText.trim();
                return pCompany === company && pRole === role;
            });

            if (exists) {
                showError("company-error", "Den erfarenheten finns redan!");
                return;
            }

            const index = experiencePlaqueSection.querySelectorAll(".plaque").length;

            const plaque = document.createElement("div");
            plaque.className = "plaque";

            const removeBtn = document.createElement("button");
            removeBtn.type = "button";
            removeBtn.className = "remove-btn";
            removeBtn.innerText = "X";

            const companyP = document.createElement("p");
            companyP.className = "company";
            companyP.innerText = company;

            const roleP = document.createElement("p");
            roleP.className = "role";
            roleP.innerText = role;

            const yearsP = document.createElement("p");
            yearsP.className = "years";
            yearsP.innerText = `${startYear} - ${endYear}`;

            plaque.append(
                removeBtn,
                companyP,
                roleP,
                yearsP,
                createHidden("Experiences", index, "Company", company),
                createHidden("Experiences", index, "Role", role),
                createHidden("Experiences", index, "StartYear", startYear),
                createHidden("Experiences", index, "EndYear", endYear)
            );

            experiencePlaqueSection.appendChild(plaque);

            inputs.forEach(i => i.value = "");
            addExperienceBtn.disabled = true;
        });

        experiencePlaqueSection.addEventListener("click", e => {
            if (!e.target.classList.contains("remove-btn")) return;
            e.target.closest(".plaque").remove();
            reindexExperience(experiencePlaqueSection);
        });

    });

    function reindexExperience(container) {
        container.querySelectorAll(".plaque").forEach((plaque, index) => {
            plaque.querySelector('input[name$=".Company"]').name = `Experiences[${index}].Company`;
            plaque.querySelector('input[name$=".Role"]').name = `Experiences[${index}].Role`;
            plaque.querySelector('input[name$=".StartYear"]').name = `Experiences[${index}].StartYear`;
            plaque.querySelector('input[name$=".EndYear"]').name = `Experiences[${index}].EndYear`;
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
