function menuToggle() {
    const toggleMenu = document.querySelector(".menu");
    if (toggleMenu) {
        toggleMenu.classList.toggle("active");
    }
}

window.menuToggle = menuToggle;

$(document).ready(function () {
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
