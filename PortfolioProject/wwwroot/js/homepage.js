document.addEventListener("DOMContentLoaded", init);
function init() {
    const name = document.getElementById("nameSearch");
    const skill = document.getElementById("skillSearch");
    name.addEventListener("input", searchCvs)
    skill.addEventListener("change", searchCvs)

    window.addEventListener("scroll", onScrollEventProjects);

    toggleButtons();

}

function searchCvs() {
    const name = document.getElementById("nameSearch").value.toLowerCase();
    const skill = document.getElementById("skillSearch").value;

    //kör metoden Search i HomeController och skickar med name och skill som parametrar
    //encodeURIComponent används för att skicka med specialtecken såsom #. Den omvandlar specialtecken till säkra URL-tecken som servern kan läsa.
    fetch(`/Home/Search?name=${encodeURIComponent(name)}&skill=${encodeURIComponent(skill)}`)
        //väntar på svaret och konverterar det till text
        .then(response => response.text())
        //väntar på texten (som är html-kod från search-metoden) och lägger in den i cvSection
        .then(html => {
            const carousel = document.getElementById("cvCarousel");
            const inner = carousel.querySelector(".carousel-inner");
            inner.innerHTML = html;

            //skapar en js kontroll för att kunna styra carouselen som skapas med bootstrap-klasser
            //stänger av intervall som byter slide automatiskt
            const carouselInstance = new bootstrap.Carousel(carousel, {
                interval: false
            });
            //visar slide 0 (första sliden)
            carouselInstance.to(0);

            toggleButtons();
        });
}
function animateProjectCards() {

    const cards = document.querySelectorAll(".project-card");
    cards.forEach((card, index) => {
        setTimeout(() => {
            card.classList.add("show");
        }, index * 1000);
    });
}

let projectsAnimated = false;
function onScrollEventProjects() {
    if (projectsAnimated) {
        return;
    }
    const projectsection = document.getElementById("recentProjects");
    const rect = projectsection.getBoundingClientRect();
    const windowHeight = window.innerHeight;

    //event triggas när sektionen är 30% synlig.
    const triggerPoint = windowHeight * 0.7;
    if (rect.top < triggerPoint) {
        animateProjectCards();
        projectsAnimated = true;
        window.removeEventListener("scroll", onScrollAnimateProjects);
    }
}


function toggleButtons() {
    const carousel = document.getElementById("cvCarousel");
    const slides = carousel.querySelectorAll(".carousel-item");

    const prevBtn = carousel.querySelector(".carousel-control-prev");
    const nextBtn = carousel.querySelector(".carousel-control-next");

    if (slides.length > 1) {
        prevBtn.style.display = "block";
        nextBtn.style.display = "block";
    } else {
        prevBtn.style.display = "none";
        nextBtn.style.display = "none";
    }
}