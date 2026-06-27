function menuToggle() {
    const toggleMenu = document.querySelector(".menu");
    const profileButton = document.querySelector(".profile");
    if (toggleMenu) {
        const isOpen = toggleMenu.classList.toggle("active");
        if (profileButton) {
            profileButton.setAttribute("aria-expanded", String(isOpen));
        }
    }
}

window.menuToggle = menuToggle;

function closeUserMenu() {
    const toggleMenu = document.querySelector(".menu");
    const profileButton = document.querySelector(".profile");
    if (toggleMenu) {
        toggleMenu.classList.remove("active");
    }
    if (profileButton) {
        profileButton.setAttribute("aria-expanded", "false");
    }
}

function userPrefersReducedMotion() {
    return window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
}

function loadStylesheetOnce(href, marker) {
    if (document.querySelector("link[data-quizly-lib='" + marker + "']")) {
        return;
    }

    const stylesheet = document.createElement("link");
    stylesheet.rel = "stylesheet";
    stylesheet.href = href;
    stylesheet.dataset.quizlyLib = marker;
    document.head.appendChild(stylesheet);
}

function loadScriptOnce(src, marker) {
    const existing = document.querySelector("script[data-quizly-lib='" + marker + "']");
    if (existing) {
        return Promise.resolve();
    }

    return new Promise(function (resolve, reject) {
        const script = document.createElement("script");
        script.src = src;
        script.dataset.quizlyLib = marker;
        script.onload = resolve;
        script.onerror = reject;
        document.body.appendChild(script);
    });
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

        const options = {
            loop: container.dataset.loop !== "false",
            speed: 650,
            grabCursor: true,
            slidesPerView: 1,
            spaceBetween: 18,
            breakpoints: {
                768: {
                    slidesPerView: Number(container.dataset.tabletSlides || 2)
                },
                1100: {
                    slidesPerView: Number(container.dataset.desktopSlides || 3)
                }
            }
        };

        if (pagination) {
            options.pagination = { el: pagination, clickable: true };
        }

        if (nextButton && prevButton) {
            options.navigation = {
                nextEl: nextButton,
                prevEl: prevButton
            };
        }

        new window.Swiper(container, options);

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

function loadDeferredEffects() {
    if (userPrefersReducedMotion()) {
        return;
    }

    loadStylesheetOnce("/lib/aos/dist/aos.css", "aos-css");
    loadScriptOnce("/lib/aos/dist/aos.js", "aos-js").then(initAos).catch(function () {});
    loadScriptOnce("/lib/vanilla-tilt/vanilla-tilt.min.js", "vanilla-tilt").then(initTiltCards).catch(function () {});
}

function showModal(selector) {
    const element = document.querySelector(selector);
    if (!element) {
        return;
    }

    if (window.bootstrap && window.bootstrap.Modal) {
        window.bootstrap.Modal.getOrCreateInstance(element).show();
        return;
    }

    if (window.jQuery && window.jQuery.fn.modal) {
        window.jQuery(element).modal("show");
    }
}

function hideModal(selector) {
    const element = document.querySelector(selector);
    if (!element) {
        return;
    }

    if (window.bootstrap && window.bootstrap.Modal) {
        window.bootstrap.Modal.getOrCreateInstance(element).hide();
        return;
    }

    if (window.jQuery && window.jQuery.fn.modal) {
        window.jQuery(element).modal("hide");
    }
}

let datepickerPromise;

function ensureDatepicker(callback) {
    if (window.jQuery && window.jQuery.fn && window.jQuery.fn.datepicker) {
        if (callback) {
            callback();
        }
        return Promise.resolve();
    }

    if (!datepickerPromise) {
        datepickerPromise = new Promise(function (resolve, reject) {
            if (!document.querySelector("link[data-quizly-datepicker]")) {
                const stylesheet = document.createElement("link");
                stylesheet.rel = "stylesheet";
                stylesheet.href = "/lib/jquery-ui/jquery-ui.css";
                stylesheet.dataset.quizlyDatepicker = "true";
                document.head.appendChild(stylesheet);
            }

            const script = document.createElement("script");
            script.src = "/lib/jquery-ui/jquery-ui.js";
            script.onload = resolve;
            script.onerror = reject;
            document.body.appendChild(script);
        });
    }

    return datepickerPromise.then(function () {
        if (callback) {
            callback();
        }
    });
}

window.QuizlyUi = {
    init: initUiLibraries,
    initAos: initAos,
    initSwipers: initSwipers,
    initTiltCards: initTiltCards,
    loadDeferredEffects: loadDeferredEffects,
    showModal: showModal,
    hideModal: hideModal,
    ensureDatepicker: ensureDatepicker
};

$(document).ready(function () {
    initUiLibraries();

    window.addEventListener("load", function () {
        window.setTimeout(loadDeferredEffects, 120);
    });

    $(document).on("click", ".profile", function () {
        menuToggle();
    });

    $(document).on("click", function (event) {
        const menu = document.querySelector(".menu");
        const profileButton = document.querySelector(".profile");
        if (!menu || !profileButton) {
            return;
        }

        const target = event.target;
        if (!menu.contains(target) && !profileButton.contains(target)) {
            closeUserMenu();
        }
    });

    $(document).on("keydown", function (event) {
        if (event.key === "Escape") {
            closeUserMenu();
        }
    });

    $(document).on("click", "#changePasswordBtn", function (event) {
        event.preventDefault();
        showModal("#changePasswordModal");
    });

    $(document).on("click", "#loginBtn", function () {
        showModal("#loginModal");
    });

    $(document).on("click", "#registerBtn", function () {
        showModal("#registerModal");
    });

    $(document).on("click", "#profileBtn", function (event) {
        event.preventDefault();
        showModal("#profileModal");
    });
});
