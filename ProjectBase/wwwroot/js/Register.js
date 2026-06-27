$(function () {
    $('.register_button').on('click', function (e) {
        e.preventDefault();

        var subjectId = $(this).data('subject-id');
        var userId = $('input[name="userId"]').first().val();

        // Set userId to 0 if it is null or empty
        if (!userId) {
            userId = 0;
        }

        $.ajax({
            url: '/Subjects/GetSubjectData',
            type: 'POST',
            data: { subjectId: subjectId, userId: userId },
            success: function (data) {
                $('#subjectPopupContent').html(data);
                window.QuizlyUi?.showModal('#subjectPopup');
            },
            error: function (xhr, status, error) {
                $('#subjectPopupContent').html('<div class="form-state-message">Could not load subject registration. Please try again.</div>');
                window.QuizlyUi?.showModal('#subjectPopup');
            }
        });
    });
});
