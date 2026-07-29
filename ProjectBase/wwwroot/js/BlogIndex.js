$(document).ready(function () {
    $('.blog-card').each(function () {
        var blogId = $(this).find('.blog-category').attr('id').split('-')[2];
        var userId = $(this).find('.blog-user-meta').data('user-id');
        loadUserInfo(userId);
        loadCategories(blogId);
    });

    $('.home-side-post-card').each(function () {
        var userId = $(this).find('.author').data('user-id');
        loadLatestPostInfo(userId);
    });
});

function loadCategories(blogId) {
    $.ajax({
        url: '/api/BlogApi/GetBlogCategory/' + blogId,
        type: 'GET',
        success: function (data) {
            var container = $('#blog-category-' + blogId).empty();
            data.forEach(function (category) {
                $('<span>').text(category.title || '').appendTo(container);
                container.append(document.createTextNode(' '));
            });
        },
        error: function () {
            $('#blog-category-' + blogId).html('');
        }
    });
}

function loadUserInfo(userId) {
    $.ajax({
        url: '/api/BlogApi/GetBlogUser/' + userId,
        type: 'GET',
        success: function (data) {
            if (!data || !data.length) {
                return;
            }
            var user = data[0];
            var container = $('.blog-user-' + userId).empty();
            $('<span>').append(
                $('<i>', { class: 'bi bi-person' }),
                document.createTextNode(' By '),
                $('<a>', { href: '#', text: user.fullname || '' })
            ).appendTo(container);
            $('<span>').append(
                $('<i>', { class: 'bi bi-tag-fill' }),
                document.createTextNode(' Blogs')
            ).appendTo(container);
        },
        error: function () {
            $('.blog-user-' + userId).html('<span><i class="bi bi-person"></i> By Quizly</span>');
        }
    });
}

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
        error: function () {
            $('.author-user-' + userId).html('<i class="bi bi-person"></i> Quizly');
        }
    });
}

document.querySelectorAll('.blog-category').forEach(function (container) {
    container.addEventListener('wheel', function (event) {
        this.scrollLeft += event.deltaY > 0 ? 30 : -30;
        event.preventDefault();
    });
});
