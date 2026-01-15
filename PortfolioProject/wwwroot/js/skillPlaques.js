(() => {

    document.querySelectorAll(".skill-section").forEach(section => {

        const addSkillBtn = section.querySelector(".btn-add-skill");
        const skillTitleInput = section.querySelector(".input-skill-name");
        const skillPlaqueSection = section.querySelector(".skill-plaque-section");

        const resetError = () => {
            const errorSpan = section.querySelector(".skill-name-error");
            if (!errorSpan) return;
            errorSpan.innerText = "";
            errorSpan.style.display = "none";
        };

        const showError = (msg) => {
            const errorSpan = section.querySelector(".skill-name-error");
            if (!errorSpan) return;
            errorSpan.innerText = msg;
            errorSpan.style.display = "block";
        };

        skillTitleInput.addEventListener("input", () => {
            resetError();
            addSkillBtn.disabled = skillTitleInput.value.trim() === "";
        });

        addSkillBtn.addEventListener("click", () => {

            const index = skillPlaqueSection.querySelectorAll(".plaque").length;

            const exists = Array.from(skillPlaqueSection.children).some(plaque =>
                plaque.querySelector("p")?.innerText.trim() === skillTitleInput.value.trim()
            );

            if (exists) {
                showError("Den färdigheten finns redan!");
                return;
            }

            const plaque = document.createElement("div");
            plaque.className = "plaque";

            const removeBtn = document.createElement("button");
            removeBtn.type = "button";
            removeBtn.className = "remove-btn";
            removeBtn.innerText = "X";

            const title = document.createElement("p");
            title.innerText = skillTitleInput.value.trim();

            plaque.append(
                removeBtn,
                title,
                createHidden("Skills", index, "Name", skillTitleInput.value)
            );

            skillPlaqueSection.appendChild(plaque);

            skillTitleInput.value = "";
            addSkillBtn.disabled = true;
        });

        skillPlaqueSection.addEventListener("click", e => {
            if (!e.target.classList.contains("remove-btn")) return;
            e.target.closest(".plaque").remove();
            reindexSkills(skillPlaqueSection);
        });
    });

    function reindexSkills(container) {
        container.querySelectorAll(".plaque").forEach((plaque, index) => {
            plaque.querySelector('input[name$=".Name"]').name = `Skills[${index}].Name`;
        });
    }

    function createHidden(collection, index, prop, value) {
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = `${collection}[${index}].${prop}`;
        input.value = value.trim();
        return input;
    }

})();
