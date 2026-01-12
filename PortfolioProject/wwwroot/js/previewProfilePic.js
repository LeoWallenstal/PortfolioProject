const previewBtn = document.getElementById("btn-preview")
const imgElement = document.getElementById("edit-profile-pic")
const profilePicUrlInput = document.getElementById("profile-pic-input")

if (profilePicUrlInput.value === "/images/default-profile2.png") {
    profilePicUrlInput.value = ""
}

profilePicUrlInput.addEventListener("input", () => {
    previewBtn.disabled = !profilePicUrlInput.value.trim() > 0
})

previewBtn.addEventListener("click", () => {
    imgElement.setAttribute("src", profilePicUrlInput.value.trim())
})