$(document).ready(function () {
    var isSubmitting = false;
    var shouldRestoreSubmitState = true;
    $('#changePasswordForm').submit(function (event) {
        event.preventDefault(); // Ngăn chặn việc submit form mặc định
        if (isSubmitting) {
            return;
        }

        var formData = {
            CurrentPassword: $('#currentPassword').val(),
            NewPassword: $('#newPassword').val(),
            ConfirmNewPassword: $('#confirmNewPassword').val()
        };
        var form = $('#changePasswordForm');
        var submitButton = $('#changePassword');
        var originalButtonHtml = submitButton.html();
        isSubmitting = true;
        shouldRestoreSubmitState = true;
        form.attr('aria-busy', 'true');
        submitButton.prop('disabled', true).text('Changing…');

        // Gửi yêu cầu AJAX đến action method ChangePassword trong controller AccountController
        $.ajax({
            url: '/Account/ChangePassword',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData), // Lấy dữ liệu từ form
            success: function (response) {
                // Xử lý phản hồi từ yêu cầu AJAX
                if (response.success) {
                    shouldRestoreSubmitState = false;
                    $('#changePasswordErrorMessage').hide(); // Ẩn thông báo lỗi nếu có
                    $('#changePasswordSuccessMessage').text(response.message).show(); // Hiển thị thông báo thành công
                    setTimeout(function () {
                        $('#changePasswordSuccessMessage').hide(); // Ẩn thông báo thành công
                        window.QuizlyUi?.hideModal('#changePasswordModal');
                        // Tải lại trang hiện tại
                        location.reload(); // Tải lại trang hiện tại
                    }, 2000);

                } else {
                    $('#changePasswordSuccessMessage').hide(); // Ẩn thông báo thành công nếu có
                    $('#changePasswordErrorMessage').text(response.message).show(); // Hiển thị thông báo lỗi
                }
            },
            error: function (xhr, status, error) {
                // Xử lý lỗi nếu có
                $('#changePasswordSuccessMessage').hide();
                $('#changePasswordErrorMessage').text('An error occurred while changing password.').show();
            },
            complete: function () {
                if (shouldRestoreSubmitState) {
                    isSubmitting = false;
                    form.attr('aria-busy', 'false');
                    submitButton.prop('disabled', false).html(originalButtonHtml);
                }
            }
        });
    });
});
