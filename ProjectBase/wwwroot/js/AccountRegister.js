$(document).ready(function () {
    var isSubmitting = false; // Biến cờ để kiểm tra xem yêu cầu AJAX đã được gửi hay chưa
    var formData = {};

    $('input[name="gender"]').change(function () {
        formData.gender = $(this).val(); // Cập nhật giá trị giới tính trong formData
    });

    $('#registerForm').submit(function (event) {
        event.preventDefault(); // Ngăn chặn việc gửi yêu cầu POST tự động

        var fullname = $('#inputFullName').val().trim();
        var phone = $('#inputPhoneNumber').val().trim();
        var email = $('#inputEmail').val().trim();
        var password = $('#inputPasswordRegister').val().trim();
        var confirmPassword = $('#inputConfirmPassword').val().trim();
        var gender = $('input[name="gender"]:checked').val();

        // Xóa thông báo lỗi trước đó
        $('#registerErrorMessage').empty().hide();

        // Kiểm tra các trường nhập liệu và thêm thông báo lỗi nếu cần
        var errors = [];

        if (!fullname) {
            errors.push("Username cannot be blank");
        }
        if (!phone) {
            errors.push("Phone number cannot be blank");
        }
        if (!email) {
            errors.push("Email cannot be blank");
        }
        if (!password) {
            errors.push("Password cannot be blank");
        }
        if (password !== confirmPassword) {
            errors.push("The password and confirmation password do not match.");
        }
        if (!gender) {
            errors.push("Please select a gender");
        }

        if (errors.length > 0) {
            // Hiển thị thông báo lỗi vào phần tử registerErrorMessage
            $('#registerErrorMessage').html('<ul></ul>');
            errors.forEach(function (error) {
                $('#registerErrorMessage ul').append($('<li>').text(error));
            });
            $('#registerErrorMessage').show();
            return false;
        }

        if (isSubmitting) {
            return; // Không thực hiện thêm yêu cầu AJAX nếu đang trong quá trình gửi
        }

        // Đặt trạng thái gửi AJAX thành true
        isSubmitting = true;
        var submitButton = $('#registerModelBtn');
        var originalButtonHtml = submitButton.html();
        formData = {
            fullname: fullname,
            password: password,
            ConfirmPassword: confirmPassword,
            email: email,
            Phone: phone,
            gender: gender == "true"
        };

        // Vô hiệu hóa nút đăng ký
        submitButton.prop('disabled', true).text('Registering…');

        // Gửi yêu cầu AJAX đến action method Register trong controller AccountController
        $.ajax({
            url: '/Account/Register',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData), // Lấy dữ liệu từ form
            success: function (response) {
                // Xử lý phản hồi từ yêu cầu AJAX
                if (response.success) {
                    // Hiển thị thông báo thành công
                    $('#registerSuccessMessage').show();
                    $('#registerErrorMessage').hide();

                    // Đóng modal đăng ký (sử dụng Bootstrap)
                    setTimeout(function () {
                        window.QuizlyUi?.hideModal('#registerModal');
                        // Xóa thông tin trong các trường nhập liệu
                        $('#registerForm')[0].reset();
                    }, 2000); // 2000ms = 2s
                } else {
                    // Hiển thị thông báo lỗi với nội dung từ phản hồi của server
                    $('#registerErrorMessage').html('Registration failed. Please try again.<ul></ul>');
                    if (response.errors) {
                        response.errors.forEach(function (error) {
                            $('#registerErrorMessage ul').append($('<li>').text(error));
                        });
                    }
                    $('#registerErrorMessage').show();
                    $('#registerSuccessMessage').hide();
                }
            },
            error: function (xhr, status, error) {
                // Xử lý lỗi nếu có
                $('#registerErrorMessage').text('An error occurred: ' + error).show();
                $('#registerSuccessMessage').hide();
            },
            complete: function () {
                // Đặt trạng thái gửi AJAX thành false sau khi yêu cầu hoàn tất
                isSubmitting = false;
                // Kích hoạt lại nút đăng ký
                submitButton.prop('disabled', false).html(originalButtonHtml);
            }
        });
    });

    $('#loginBtnRegister').click(function () {
        // Hiện modal đăng nhập
        window.QuizlyUi?.showModal('#loginModal');
        // Hiển thị modal đăng ký
        window.QuizlyUi?.hideModal('#registerModal');
    });
});
