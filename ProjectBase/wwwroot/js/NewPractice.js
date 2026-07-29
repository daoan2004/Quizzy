    $(document).ready(function () {
        function GetUserDetails() {
            $.ajax({
                url: '/Account/GetUserDetails',
                method: 'GET',
                success: function (response) {
                    if (response.success) {
                        userId = response.userDetails.userID;
                        // Bắt đầu tải mục đầu tiên sau khi lấy được userId
                        loadSubject(userId);
                    } else {
                        $('.scroll-list-wrapper').html('<div class="form-state-message">Failed to load your subjects.</div>');
                    }
                },
                error: function () {
                    $('.scroll-list-wrapper').html('<div class="form-state-message">Error retrieving user details.</div>');
                }
            });
        }
        GetUserDetails();
        function loadSubject(userId) {
            $.ajax({
                url: '/api/PracticeApi/LoadSubject/' + userId,
                method: 'GET',
                success: function (response) {
                    const seenSubjects = new Set();
                    if (!response || !response.length) {
                        $('.scroll-list-wrapper').html('<div class="form-state-message">No registered subjects found.</div>');
                        return;
                    }
                    response.forEach(function (item) {
                        if (!seenSubjects.has(item.subjects.title)) {
                            $('.scroll-list-wrapper').append(
                                $('<div>', {
                                    class: 'scroll-list-item glass-card',
                                    'data-subject': item.subjects.id
                                }).append(
                                    $('<div>', { class: 'practice-scroll-title' })
                                        .append($('<h2>').text(item.subjects.title || ''))
                                )
                            );
                            seenSubjects.add(item.subjects.title);
                        }
                    });
                    if (window.QuizlyUi) {
                        window.QuizlyUi.initTiltCards();
                    }
                },
                 error: function () {
                            $('.scroll-list-wrapper').html('<div class="form-state-message">Error retrieving subjects.</div>');
                 }

            });
        }


        var selectedSubject = null;

        $(document).on('click', '.scroll-list-item', function () {
            // Loại bỏ lớp 'selected' khỏi tất cả các mục
            $(".scroll-list-item").removeClass("selected");

            // Thêm lớp 'selected' vào mục được chọn
            $(this).addClass("selected");

            // Lưu lại giá trị của mục được chọn
            selectedSubject = $(this).data("subject");
        });

        $(".submit-button").click(function () {
            var submitButton = $(this);
            var title = $('#test-name').val();
            var noOfQuestions = $('#no-of-questions').val();
            var questionGroup = $('#Quest-group').val();
            var testDuration = $('#test-duration').val();
            var difficultyLevel = $('input[name="difficulty"]:checked').val(); // Assuming a button will be selected
            var practiceID = 0;
            var isPractice = true;
            if (!title) {
                $('.alerttestname').empty();
                $('.alerttestname').append('Please input Practice name!');
                return;
            }
            if (!noOfQuestions) {
                $('.alertnoofquest').empty();
                $('.alertnoofquest').append('Please input number of question!');
                return;
            }
            if (noOfQuestions <= 0) {
                $('.alertnoofquest').empty();
                $('.alertnoofquest').append('Input can not be negative!');
                return;
            }
            if (!testDuration) {
                $('.alertduration').empty();
                $('.alertduration').append('Duration is required!');
                return;
            }
            if (testDuration <= 0) {
                $('.alertduration').empty();
                $('.alertduration').append('Input can not be negative!');
                return;
            }

            if (selectedSubject) {
                var hours = Math.floor(testDuration / 60);
                var minutes = testDuration % 60;
                var durationFormatted = hours.toString().padStart(2, '0') + ':' + minutes.toString().padStart(2, '0') + ':00';

                $.ajax({
                    type: "POST",
                    url: "/api/PracticeApi/AddPractice",
                    contentType: "application/x-www-form-urlencoded",
                    data: ({
                        UserID : userId,
                        SubjectID : selectedSubject,
                        title: title,
                        number_quest: noOfQuestions,
                        Quest_group: questionGroup,
                        duration: durationFormatted,
                        levelID: difficultyLevel
                        // Thêm các thuộc tính khác tương ứng trong data object
                    }),
                    beforeSend: function () {
                        submitButton.prop('disabled', true).text('Creating…');
                    },
                    success: function (response) {
                        practiceID = response
                        window.location.href = '/Quiz/Handle?UserID=' + userId + '&PracticeID=' + practiceID + '&IsPractice=' + isPractice;
                    },
                    error: function (xhr, status, error) {
                        $('.alertbottom').empty();
                        $('.alertbottom').text(
                            xhr.responseJSON?.message || 'Could not create practice. Please try again.'
                        );
                    },
                    complete: function () {
                        submitButton.prop('disabled', false).text('Create Practice');
                    }
                });
                

            } else {
                $('.alertbottom').empty();
                $('.alertbottom').append('Please choose subject first');
            }
        });          


    });

