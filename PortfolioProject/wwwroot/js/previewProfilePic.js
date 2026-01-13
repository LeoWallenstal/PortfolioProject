const previewBtn = document.getElementById("btn-preview");
const imgElement = document.getElementById("edit-profile-pic");
const profilePicInput = document.getElementById("profile-pic-input");

// Enable preview button when a file is selected
profilePicInput.addEventListener("change", () => {
    const file = profilePicInput.files[0];
    previewBtn.disabled = !file; // enable if a file is selected
});

// Preview the selected file when clicking preview button
previewBtn.addEventListener("click", () => {
    const file = profilePicInput.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        imgElement.setAttribute("src", e.target.result); // data URL
    }
    reader.readAsDataURL(file); // read file as base64
});
