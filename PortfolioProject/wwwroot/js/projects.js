const showOnScroll = () => {
    const cards = document.querySelectorAll('.project-card');
    const triggerBottom = window.innerHeight * 1.05;

    cards.forEach(card => {
        const cardTop = card.getBoundingClientRect().top;

        if (cardTop < triggerBottom) {
            card.classList.add('show');
        }
    })
}

const controlButton = () => {
    const titleInput = document.querySelector('input[name="Project.Title"]');
    const descInput = document.querySelector('textarea[name="Project.Description"]');
    const saveBtn = document.querySelector('.save-changes-btn');

    if (!titleInput || !descInput || !saveBtn) return;

    const originalTitle = titleInput.value.trim();
    const originalDesc = descInput.value.trim();

    const checkChanges = () => {
        const currentTitle = titleInput.value.trim();
        const currentDesc = descInput.value.trim();

        const isUnchanged = currentTitle === originalTitle && currentDesc === originalDesc;

        saveBtn.disabled = isUnchanged;
    };

    titleInput.addEventListener('input', checkChanges);
    descInput.addEventListener('input', checkChanges);

    checkChanges();
}

document.addEventListener('DOMContentLoaded', () => {
    showOnScroll();
    controlButton();
});

window.addEventListener('scroll', showOnScroll);