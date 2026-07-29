$(document).ready(function () {
    $('.home-side-post-card').each(function () {
        var userId = $(this).find('.author').data('user-id');
        loadLatestPostInfo(userId);

    });

});



// gọi yêu cầu AJAX tới BlogApi.cs để thực hiện function GetBlogUser cho lastest post
function loadLatestPostInfo(userId) {
    $.ajax({
        url: '/api/BlogApi/GetBlogUser/' + userId,
        type: 'GET',
        success: function (data) {
            if (!data || !data.length) {
                return;
            }
            var user = data[0];
            $('.author-user-' + userId)
                .empty()
                .append($('<i>', { class: 'bi bi-person' }))
                .append(document.createTextNode(' ' + (user.fullname || '')));

        },
        error: function (error) {
            $('.author-user-' + userId).html('<i class="bi bi-person"></i> Quizly');
        }
    });
}
