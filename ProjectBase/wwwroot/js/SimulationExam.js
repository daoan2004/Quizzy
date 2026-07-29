$(document).ready(function () {
        var userId = null;

        function showExamMessage(message) {
            $('#simulationExamList').html(
                $('<tr>').append($('<td>', {
                    colspan: 8,
                    class: 'table-state-message',
                    text: message
                }))
            );
            $('#pagination').empty();
        }

        function GetUserDetails() {
            $.ajax({
                url: '/Account/GetUserDetails',
                method: 'GET',
                success: function (response) {
                    if (response.success) {
                        userId = response.userDetails.userID;
                        // Bắt đầu tải mục đầu tiên sau khi lấy được userId
                        GetExamPagination(1, userId);
                        loadFilter(userId);
                        
                    } else {
                        showExamMessage('Log in to view available simulation exams.');
                    }
                },
                error: function () {
                    showExamMessage('Could not retrieve simulation exams.');
                }
            });
        }
GetUserDetails();
        function GetExamPagination(page, userId, levelId = null) {
            $.ajax({
                url: '/api/SimulationExamApi/GetExamPagination/' + userId,
                type: 'GET',
                data: { page: page, pageSize: 5, levelId: levelId},
                success: function (response) {
                    $('#simulationExamList').empty();
                    if (!response.practice || response.practice.length === 0) {
                        showExamMessage('No simulation exams found.');
                        return;
                    }
                    var renderedExamCount = 0;
                    response.practice.forEach(function (item) {
                        item.subjects.exams.forEach(function (exam) {
                            var row = $('<tr>');
                            $('<td>').text(exam.id).appendTo(row);
                            $('<td>').text(item.subjects.title || '').appendTo(row);
                            $('<td>').text(exam.examName || '').appendTo(row);
                            $('<td>').text(exam.level.title || '').appendTo(row);
                            $('<td>').text(exam.number_Question).appendTo(row);
                            $('<td>').text(exam.duration || '').appendTo(row);
                            $('<td>').text(exam.passrate + '%').appendTo(row);
                            $('<td>').append(
                                $('<button>', {
                                    type: 'button',
                                    id: exam.id,
                                    class: 'details-button'
                                }).append($('<i>', { class: 'bi bi-play-circle' }), ' Do Exam')
                            ).appendTo(row);
                            $('#simulationExamList').append(row);
                            renderedExamCount++;
                        });
                    });
                    if (renderedExamCount === 0) {
                        showExamMessage('No simulation exams found.');
                        return;
                    }

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
                    showExamMessage('Could not load simulation exams.');
                }

            });
        }
        $('#pagination').on('click', 'a', function (e) {
            e.preventDefault();
            var page = $(this).data('page');
            var levelId = $('#subjectFilter').val();
            GetExamPagination(page, userId, levelId);
        });
        // hàm để tải filter
        function loadFilter(userId) {
            $.ajax({
                url: '/api/SimulationExamApi/LoadFilter/' + userId,
                method: 'GET',
                success: function (response) {
                    const seenTitles = new Set();
                   
                    response.forEach(function (item) {
                        item.subjects.exams.forEach(function (exam) {
                            if (!seenTitles.has(exam.level.title)) {
                                $('#subjectFilter').append(
                                    $('<option>', {
                                        value: exam.level.id,
                                        text: exam.level.title || ''
                                    })
                                );
                                seenTitles.add(exam.level.title);
                            }
                        });
                    });
                    
                },

            });
        }
        $('#subjectFilter').on('change', function () {
            
            var levelId = $('#subjectFilter').val();
            GetExamPagination(1, userId, levelId);
        });
        $('#simulationExamList').on('click', '.details-button', function (e) {
            e.preventDefault();
            var examId = $(this).attr('id');
            window.location.href = '/Quiz/Handle?UserID=' + userId + '&PracticeID=' + examId + '&IsPractice=false';
        });
});

