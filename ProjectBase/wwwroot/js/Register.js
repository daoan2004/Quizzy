$(function () {
    $('.register_button').on('click', function (e) {
        e.preventDefault();
        console.log("Register Now button clicked!"); // Log message to console

        var subjectId = $(this).data('subject-id');
        var userId = $('#userId').val();

        // Set userId to 0 if it is null or empty
        if (!userId) {
            userId = 0;
        }

        console.log(userId);

        $.ajax({
            url: '/Subjects/GetSubjectData',
            type: 'POST',
            data: { subjectId: subjectId, userId: userId },
            success: function (data) {
                console.log(data); // Log the data received
                $('#subjectPopupContent').html(data);
                $('#subjectPopup').modal('show'); // Show the modal
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error:', status, error); // Log the error details
                alert('Error loading subject data. See console for details.');
            }
        });
    });
});