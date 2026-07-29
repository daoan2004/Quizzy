$(document).ready(function () {
    var formData = {};
    var isSubmitting = false;
    $('#loginForm').submit(function (event) {
        event.preventDefault();
        if (isSubmitting) {
            return;
        }
        var email = $('#inputUserNameLogin').val();
        var password = $('#inputPasswordLogin').val();
        var submitButton = $('#loginModelBtn');
        var originalButtonHtml = submitButton.html();
        $('#loginErrorMessage').hide();

        if (!email || !password) {
            $('#loginErrorMessage').text('Please enter both email and password.').show();
            return;
        }

        formData = { email: email, password: password };
        isSubmitting = true;
        submitButton.prop('disabled', true).text('Logging in…');

        $.ajax({
            url: '/Account/Login',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.success) {
                    $('#buttonContainer').hide();
                    $('#userMenu').show();
                    location.reload();
                } else {
                    $('#loginErrorMessage').text(response.message).show();
                }
            },
            error: function (xhr, status, error) {
                $('#loginErrorMessage').text('An error occurred while logging in.').show();
            },
            complete: function () {
                isSubmitting = false;
                submitButton.prop('disabled', false).html(originalButtonHtml);
            }
        });
    });

    $('#registerBtnLogin').click(function () {
        window.QuizlyUi?.hideModal('#loginModal');
        window.QuizlyUi?.showModal('#registerModal');
    });
});
