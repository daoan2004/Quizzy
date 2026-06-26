function menuToggle() {
    const toggleMenu = document.querySelector(".menu");
    if (toggleMenu) {
        toggleMenu.classList.toggle("active");
    }
}

window.menuToggle = menuToggle;

function userPrefersReducedMotion() {
    return window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
}

function initAos() {
    if (!window.AOS || userPrefersReducedMotion()) {
        return;
    }

    window.AOS.init({
        duration: 650,
        easing: "ease-out-cubic",
        once: true,
        offset: 80
    });
}

function initSwipers() {
    if (!window.Swiper) {
        return;
    }

    document.querySelectorAll(".js-swiper").forEach(function (container) {
        if (container.dataset.swiperReady === "true") {
            return;
        }

        const pagination = container.querySelector(".swiper-pagination");
        const nextButton = container.querySelector(".swiper-button-next");
        const prevButton = container.querySelector(".swiper-button-prev");

        new window.Swiper(container, {
            loop: container.dataset.loop !== "false",
            speed: 650,
            grabCursor: true,
            slidesPerView: 1,
            spaceBetween: 18,
            pagination: pagination ? { el: pagination, clickable: true } : undefined,
            navigation: nextButton && prevButton ? {
                nextEl: nextButton,
                prevEl: prevButton
            } : undefined,
            breakpoints: {
                768: {
                    slidesPerView: Number(container.dataset.tabletSlides || 2)
                },
                1100: {
                    slidesPerView: Number(container.dataset.desktopSlides || 3)
                }
            }
        });

        container.dataset.swiperReady = "true";
    });
}

function initTiltCards() {
    if (!window.VanillaTilt || userPrefersReducedMotion()) {
        return;
    }

    const tiltCards = document.querySelectorAll(".js-tilt-card:not([data-tilt-ready='true'])");
    if (!tiltCards.length) {
        return;
    }

    window.VanillaTilt.init(tiltCards, {
        max: 6,
        speed: 450,
        glare: true,
        "max-glare": 0.16,
        scale: 1.01
    });

    tiltCards.forEach(function (card) {
        card.dataset.tiltReady = "true";
    });
}

function initUiLibraries() {
    initAos();
    initSwipers();
    initTiltCards();
}

window.QuizlyUi = {
    init: initUiLibraries,
    initAos: initAos,
    initSwipers: initSwipers,
    initTiltCards: initTiltCards
};

$(document).ready(function () {
    initUiLibraries();

    $(document).on("click", ".profile", function () {
        menuToggle();
    });

    $(document).on("click", "#changePasswordBtn", function (event) {
        event.preventDefault();
        $("#changePasswordModal").modal("show");
    });

    $(document).on("click", "#loginBtn", function () {
        $("#loginModal").modal("show");
    });

    $(document).on("click", "#registerBtn", function () {
        $("#registerModal").modal("show");
    });

    $(document).on("click", "#profileBtn", function (event) {
        event.preventDefault();
        $("#profileModal").modal("show");
    });
});
