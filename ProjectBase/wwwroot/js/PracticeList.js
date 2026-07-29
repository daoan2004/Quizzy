    $(document).ready(function () {
        var userId = null;

        function showPracticeMessage(message) {
            $('#PracticeList').html(
                $('<tr>').append($('<td>', {
                    colspan: 7,
                    class: 'table-state-message',
                    text: message
                }))
            );
            $('#pagination').empty();
        }

        // Hàm để lấy userId từ API
        function GetUserDetails() {
            $.ajax({
                url: '/Account/GetUserDetails',
                method: 'GET',
                success: function (response) {
                    if (response.success) {
                        userId = response.userDetails.userID;
                        // Bắt đầu tải mục đầu tiên sau khi lấy được userId    
                        GetPracticePagination(1, userId);
                        loadFilter(userId);
                    } else {
                        showPracticeMessage('Log in to view your practice history.');
                    }
                },
                error: function () {
                    showPracticeMessage('Could not retrieve your practice history.');
                }
            });
        }

        // Gọi hàm GetUserDetails để khởi tạo userId và tải mục đầu tiên
        GetUserDetails();

        // Hàm để tải các mục phân trang
        function GetPracticePagination(page, userId,subjectId=null,levelId=null) {
            $.ajax({
                url: '/api/PracticeApi/GetPracticePagination/' + userId,
                type: 'GET',
                data: { page: page, pageSize: 5, subjectId: subjectId, levelId : levelId},
                success: function (response) {
                    $('#PracticeList').empty();
                    if (!response.practice || response.practice.length === 0) {
                        showPracticeMessage('No practice attempts found.');
                        return;
                    }
                    response.practice.forEach(function (item) {
                        var takenDate = new Date(item.taken_date).toLocaleDateString();
                        var correctRate = item.number_quest > 0
                            ? Math.round((item.number_correct / item.number_quest) * 100)
                            : 0;
                        var row = $('<tr>');
                        $('<td>').text(item.subject.title || '').appendTo(row);
                        $('<td>').text(item.title || '').appendTo(row);
                        $('<td>').text(item.number_quest).appendTo(row);
                        $('<td>').text(correctRate + '%').appendTo(row);
                        $('<td>').text(item.level.title || '').appendTo(row);
                        $('<td>').text(takenDate).appendTo(row);
                        $('<td>').append(
                            $('<button>', {
                                type: 'button',
                                id: item.id,
                                class: 'details-button'
                            }).append($('<i>', { class: 'bi bi-eye' }), ' View')
                        ).appendTo(row);
                        $('#PracticeList').append(row);
                    });

                    $('#pagination').empty();
                    for (var i = 1; i <= response.totalPages; i++) {
                        if (i === response.currentPage) {
                            $('#pagination').append('<span aria-current="page">' + i + '</span>');
                        } else {
                            $('#pagination').append('<a href="#" data-page="' + i + '">' + i + '</a>');
                        }
                    }

                },
                error: function () {
                    showPracticeMessage('Could not load practice attempts.');
                }
            });
        }
        //thêm sự kiện pagination
        $('#pagination').on('click', 'a', function (e) {
            e.preventDefault();
            var page = $(this).data('page');
            var subjectId = $('#subjectFilter').val();
            var levelId = $('#levelFilter').val();
            GetPracticePagination(page, userId,subjectId,levelId);
        });
        //thêm sự kiện ấn button detail:
        $('#PracticeList').on('click', '.details-button', function () {
            var id = $(this).attr('id');
            window.location.href = '/QuizReview/Detail?id=' + id;
        });

        // Sự kiện thay đổi bộ lọc
        
        $('#subjectFilter, #levelFilter').on('change', function () {
            var subjectId = $('#subjectFilter').val();
            var levelId = $('#levelFilter').val();
            GetPracticePagination(1, userId, subjectId, levelId);
        });
        
        // hàm để tải filter
            function loadFilter(userId) { 
                $.ajax({
                    url: '/api/PracticeApi/LoadFilter/' + userId,
                    method: 'GET',
                    success: function (response) {
                        const seenTitles = new Set();
                        const seenLevel = new Set();
                        response.forEach(function (item) {
                            if (!seenTitles.has(item.subject.title)) {
                                $('#subjectFilter').append(
                                    $('<option>').val(item.subject.id).text(item.subject.title || '')
                                );
                                seenTitles.add(item.subject.title);
                            }
                        });
                        response.forEach(function (item) {
                            if (!seenLevel.has(item.level.title)) {
                                $('#levelFilter').append(
                                    $('<option>').val(item.level.id).text(item.level.title || '')
                                );
                                seenLevel.add(item.level.title);
                            }
                        });
                    },
                    
                });
            }
    });
    
