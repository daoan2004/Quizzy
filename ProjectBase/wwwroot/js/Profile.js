$(document).ready(function () {
    var isUpdating = false;
    $('#profileBtn').click(function () {
        fetchUserDetails();
    });

    function fetchUserDetails() {
        $.ajax({
            url: '/Account/GetUserDetails', // Adjust API URL for fetching user details
            method: 'GET',
            success: function (response) {
                if (response.success) {
                    var data = response.userDetails;
                    $('#profileAvatar').attr('src', data.avatarUrl || '/picture/DropList/user.png'); // Set default avatar if none
                    $('#profileName').val(data.fullName);
                    $('#profileRole').val(data.role);
                    $('input[name="gender"][value="' + data.gender + '"]').prop('checked', true);
                    $('#address').val(data.address || '');
                    $('#email').val(data.email);
                    $('#phoneNumber').val(data.phone);
                    $('#dob').val(data.dob || ''); // Set the date of birth value

                    $('#description').val(data.description || '');
                    window.QuizlyUi.ensureDatepicker(function () {
                        // Initialize datepicker with the correct value
                        $('#dob').datepicker({
                            dateFormat: 'yy-mm-dd',
                            changeMonth: true,
                            changeYear: true,
                            yearRange: '1900:2030',
                            onSelect: function (dateText, inst) {
                                $(this).val(dateText);
                            }
                        }).datepicker('setDate', data.dob || ''); // Set the date

                        window.QuizlyUi?.showModal('#profileModal');
                    });
                } else {
                    $('#errorMessage').text(response.message || 'Could not load profile details.').fadeIn();
                }
            },
            error: function (error) {
                $('#errorMessage').text('An error occurred while fetching user details.').fadeIn();
            }
        });
    }

    $('#editProfileBtn').click(function () {
        // Enable editing of fields
        $('#profileName').prop('readonly', false);
        $('#profileRole').prop('readonly', true); // Adjust if role should be editable
        $('#email').prop('readonly', true);      // Adjust if email should be editable
        $('#address').prop('readonly', false);
        $('#phoneNumber').prop('readonly', false);
        $('#description').prop('readonly', false);
        $('#profileForm input[type="radio"]').prop('disabled', false);
        $('#avatarUpload').css('display', 'block'); // Show avatar upload
        $('#editProfileBtn').hide();
        $('#updateProfileBtn').show();
        $('#dob').datepicker('option', 'disabled', false);  // Enable DatePicker
        $('#successMessage').hide();  // Hide success message when editing
        $('#errorMessage').hide();    // Hide error message when editing
    });

    $('#profileForm').submit(function (event) {
        event.preventDefault();
        if (isUpdating) {
            return;
        }
        var formData = new FormData(this);
        var form = $('#profileForm');
        var submitButton = $('#updateProfileBtn');
        var originalButtonHtml = submitButton.html();

        // Get gender value and convert to true (Male) or false (Female)
        var genderValue = $('input[name="gender"]:checked').val() === "true";
        formData.append("Gender", genderValue);
        isUpdating = true;
        form.attr('aria-busy', 'true');
        submitButton.prop('disabled', true).text('Updating…');

        $.ajax({
            url: '/Account/UpdateUserProfile',
            method: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('#successMessage').fadeIn();
                    $('#errorMessage').hide();

                    // Update the avatar image immediately after a successful update
                    var updatedAvatarUrl = response.updatedAvatarUrl || $('#profileAvatar').attr('src');
                    $('#profileAvatar').attr('src', updatedAvatarUrl);
                    // Re-fetch user details and reopen modal without closing it first
                    fetchUserDetails();
                    setTimeout(function () {
                        $('#successMessage').fadeOut();
                        window.QuizlyUi?.hideModal('#profileModal');
                    }, 2000);

                    // Disable fields and revert buttons
                    $('#profileName').prop('readonly', true);
                    $('#profileRole').prop('readonly', true);
                    $('#email').prop('readonly', true);
                    $('#address').prop('readonly', true);
                    $('#phoneNumber').prop('readonly', true);
                    $('#description').prop('readonly', true);
                    $('#profileForm input[type="radio"]').prop('disabled', true);
                    $('#avatarUpload').hide();
                    $('#editProfileBtn').show();
                    $('#updateProfileBtn').hide();
                    $('#dob').datepicker('option', 'disabled', true); // Disable DatePicker

                } else {
                    $('#errorMessage').text(response.message || 'Could not update profile.').fadeIn();
                    $('#successMessage').hide();
                }
            },
            error: function (error) {
                $('#errorMessage').text('An error occurred while updating your profile.').fadeIn();
                $('#successMessage').hide();
            },
            complete: function () {
                isUpdating = false;
                form.attr('aria-busy', 'false');
                submitButton.prop('disabled', false).html(originalButtonHtml);
            }
        });
    });
});

