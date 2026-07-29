function startTimer(deadline, display) {
    var finishAttemptButton = $('.finish-attempt');
    var timerInterval = setInterval(function () {
var timer = Math.max(0, Math.floor((deadline.getTime() - Date.now()) / 1000));
var hours = parseInt(timer / 3600, 10);
var minutes = parseInt((timer % 3600) / 60, 10);
var seconds = parseInt(timer % 60, 10);

hours = hours < 10 ? "0" + hours : hours;
minutes = minutes < 10 ? "0" + minutes : minutes;
seconds = seconds < 10 ? "0" + seconds : seconds;

display.text("Time remaining: " + hours + ":" + minutes + ":" + seconds);

if (timer <= 0) {
    clearInterval(timerInterval);
    finishAttemptButton.click();
}
    }, 1000);
}
var userId = 0;
var practiceId = 0;
var attemptEndsAt = '';
var currentQuestionID = 0;
var currentQuestionIndex = 0;
var isFinishing = false;

function escapeQuizText(value) {
    return window.QuizlyUi?.escapeHtml(value) ?? '';
}

function showQuizFeedback(message) {
    $('#quizFeedback').text(message || '').prop('hidden', !message);
}

function setQuizBusy(isBusy) {
    $('.main-detail').attr('aria-busy', isBusy ? 'true' : 'false');
}
// This function will be executed when the DOM is fully loaded
$(document).ready(function () {
    var quizPage = document.querySelector('.quiz-page');
    userId = Number(quizPage.dataset.userId);
    practiceId = Number(quizPage.dataset.practiceId);
    attemptEndsAt = quizPage.dataset.attemptEndsAt;
    loadQuestionsList();
    $('.finish-attempt').on('click', function() {
        if (isFinishing) {
            return;
        }
        var finishButton = $(this);
        var originalButtonHtml = finishButton.html();
        isFinishing = true;
        finishButton.prop('disabled', true).text('Finishing…');
        showQuizFeedback('');
        $.ajax({
            url: '/api/QuizApi/finishAttempt', // Thay đổi URL này thành endpoint API của bạn
            method: 'POST',
            data: {
                UserID: userId,
                PracticeID: practiceId,

            },
            success: function(response) {
                window.location.href = '/Practice';
                // Bạn có thể thêm mã để xử lý sau khi hoàn thành bài kiểm tra, ví dụ: chuyển hướng đến trang kết quả
            },
            error: function(xhr, status, error) {
                showQuizFeedback('Could not finish the quiz. Please try again.');
            },
            complete: function () {
                isFinishing = false;
                finishButton.prop('disabled', false).html(originalButtonHtml);
            }
        });
    });
    var display = $('.timer');
    var serverDeadline = new Date(attemptEndsAt);
    startTimer(serverDeadline, display);
    
});

//loadquestionlist
function loadQuestionsList() {
        $.ajax({
            url: '/api/QuizApi/getQuestionsList', // Endpoint của bạn để lấy danh sách câu hỏi
            method: 'GET',
            data: {
                UserID: userId,
                PracticeID: practiceId
            },
            success: function(response) {
                var questionsListHtml = '';
                $.each(response, function(index, question) {
                    questionsListHtml += '<button type="button" class="question-button" data-question-id="' + question.id + '" data-question-index="'+(index+1)+'">' + (index + 1) + '</button>';
                   
                });
                $('#quiz-number').html(questionsListHtml);

                // Attach click event handler after buttons are added to DOM
                $('.question-button').on('click', function() {
                    var questionId = $(this).data('question-id');
                    currentQuestionIndex = $(this).data('question-index');
                    currentQuestionID = questionId;
                    loadQuestion(questionId);
                });
                // Attach click event for next and previous button
                $('.prev-button').off('click').on('click', function() {
                    if (currentQuestionIndex > 1) {
                        currentQuestionIndex--;
                        currentQuestionID = response[currentQuestionIndex - 1].id;
                        loadQuestion(currentQuestionID);
                    }
                });
                $('.next-button').off('click').on('click', function() {
                    if (currentQuestionIndex < response.length) {
                        currentQuestionIndex++;
                        currentQuestionID = response[currentQuestionIndex - 1].id;
                        loadQuestion(currentQuestionID);
                    }
                });
                // Load the first question
                if (response.length > 0) {
                    loadQuestion(response[0].id);
                    currentQuestionID = response[0].id;
                    currentQuestionIndex = 1;
                } else {
                    showQuizFeedback('No questions are available for this attempt.');
                }
                
            

            },
            error: function() {
                showQuizFeedback('Could not load the question list.');
            }
        });

}
function loadQuestion(questionId) {
    setQuizBusy(true);
    showQuizFeedback('');
    $.ajax({
        url: '/api/QuizApi/loadQuestion/'+questionId, // Endpoint của bạn để lấy danh sách câu hỏi
        method: 'GET',      
        success: function(response) {
            $.ajax({
                url: '/api/QuizApi/getQuestionsList',
                method: 'GET',
                data: {
                    UserID: userId,
                    PracticeID: practiceId
                },
                success: function(response) {
                    var totalQuestions = response.length;
                    var answeredCount = 0;

                    // Iterate through questions to count answered ones
                    $.each(response, function(index, question) {
                        if (question.status == 1) { // Assuming 'status' indicates answered or not
                            answeredCount++;
                        }
                    });

                    // Update progress bar
                    updateProgressBar(answeredCount, totalQuestions);

                    // Rest of your code...
                },
                error: function() {
                    showQuizFeedback('Could not update quiz progress.');
                }
            });
            

           if (response && typeof response.status !== 'undefined') {
            var status = response.status == 1 ? 'Status: Answered' : 'Status: Not yet answered';
                $('.status').html(status);
           } else {
                $('.status').html('Status: Unknown');
           }
            $('.quiz-title').text(response.quizBank.title || '');
            $('.question-index').html('Question No.'+ currentQuestionIndex);
                    var mark = response.isMark == 1 ? '<button type="button" class="unmark-button"><i class="bi bi-bookmark-x"></i> Unmark</button>' : '<button type="button" class="mark-button"><i class="bi bi-bookmark"></i> Mark</button>';
                $('.ismark').html(mark);
                $('.ismark').off('click', '.mark-button, .unmark-button')
                    .on('click', '.mark-button, .unmark-button', function () {
                        var shouldMark = $(this).hasClass('mark-button');
                        $.ajax({
                            url: '/api/QuizApi/toggleMark',
                            method: 'POST',
                            data: {
                                questionId: questionId,
                                PracticeID: practiceId,
                                isMarked: shouldMark
                            },
                            success: function () {
                                loadQuestion(questionId);
                            },
                            error: function () {
                                showQuizFeedback('Could not update the bookmark.');
                            }
                        });
                    });
            var quizanswer = '';
            if (response.quizBank.groupID == "1" || response.quizBank.groupID == "2") {
                var selectedAnswer = response.qAnswer || "";
                quizanswer += '<div class="answer-label">Choose one</div><form class="answer-form">';
                if (response.quizBank.qa && response.quizBank.qa.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="radio" name="answer" value="A" ' + (selectedAnswer == "A" ? ' checked' : '') + '> <span>A.</span> ' + escapeQuizText(response.quizBank.qa) + '</label>';
                }
                if (response.quizBank.qb && response.quizBank.qb.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="radio" name="answer" value="B" ' + (selectedAnswer == "B" ? ' checked' : '') + '> <span>B.</span> ' + escapeQuizText(response.quizBank.qb) + '</label>';
                }
                if (response.quizBank.qc && response.quizBank.qc.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="radio" name="answer" value="C" ' + (selectedAnswer == "C" ? ' checked' : '') + '> <span>C.</span> ' + escapeQuizText(response.quizBank.qc) + '</label>';
                }
                if (response.quizBank.qd && response.quizBank.qd.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="radio" name="answer" value="D" ' + (selectedAnswer == "D" ? ' checked' : '') + '> <span>D.</span> ' + escapeQuizText(response.quizBank.qd) + '</label>';
                }
                if (response.quizBank.qe && response.quizBank.qe.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="radio" name="answer" value="E" ' + (selectedAnswer == "E" ? ' checked' : '') + '> <span>E.</span> ' + escapeQuizText(response.quizBank.qe) + '</label>';
                }
                if (response.quizBank.qf && response.quizBank.qf.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="radio" name="answer" value="F" ' + (selectedAnswer == "F" ? ' checked' : '') + '> <span>F.</span> ' + escapeQuizText(response.quizBank.qf) + '</label>';
                }
                
                quizanswer += '</form>';
            }
            else if (response.quizBank.groupID == "3") {
                var length = response.quizBank.selectionLimit;
                var userChoices = response.qanswer ? response.qanswer.split(';') : [];

                // Duyệt qua từng checkbox và đánh dấu nếu giá trị checkbox có trong mảng userChoices
                $('.answer-checkbox input[type="checkbox"]').each(function() {
                    var checkboxValue = $(this).val();

                    // Nếu giá trị của checkbox có trong mảng userChoices, đánh dấu checkbox
                    if (userChoices.includes(checkboxValue)) {
                        $(this).prop('checked', true);
                    }
                });
                quizanswer += '<div class="answer-label">Choose '+length+' answers</div><form class="answer-checkbox">';
                if (response.quizBank.qa && response.quizBank.qa.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="checkbox" name="answer" value="A"> <span>A.</span> ' + escapeQuizText(response.quizBank.qa) + '</label>';
                }
                if (response.quizBank.qb && response.quizBank.qb.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="checkbox" name="answer" value="B"> <span>B.</span> ' + escapeQuizText(response.quizBank.qb) + '</label>';
                }
                if (response.quizBank.qc && response.quizBank.qc.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="checkbox" name="answer" value="C"> <span>C.</span> ' + escapeQuizText(response.quizBank.qc) + '</label>';
                }
                if (response.quizBank.qd && response.quizBank.qd.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="checkbox" name="answer" value="D"> <span>D.</span> ' + escapeQuizText(response.quizBank.qd) + '</label>';
                }
                if (response.quizBank.qe && response.quizBank.qe.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="checkbox" name="answer" value="E"> <span>E.</span> ' + escapeQuizText(response.quizBank.qe) + '</label>';
                }
                if (response.quizBank.qf && response.quizBank.qf.trim() !== '') {
                    quizanswer += '<label class="answer-option"><input type="checkbox" name="answer" value="F"> <span>F.</span> ' + escapeQuizText(response.quizBank.qf) + '</label>';
                }
                
                quizanswer += '</form>';
            }
            else if (response.quizBank.groupID == "4") {
                quizanswer += '<div class="answer-label">Input answer here</div>';
                var selectedAnswer = response.qAnswer || "";
                quizanswer += '<textarea name="answer" rows="4" cols="50">'+escapeQuizText(selectedAnswer)+'</textarea>';
            }
            $('.quiz-answer').html(quizanswer);
            $('.answer-form input').on('change', function() {
                submitAnswer(questionId, $(this).val());
            });
            $('textarea[name="answer"]').on('blur', function() {
                var answerValue = $(this).val().trim();
                if (answerValue !== '') { 
                submitAnswer(questionId, $(this).val());
                }
            });
            $('.answer-checkbox input').on('change', function() {
                var selectedAnswers = $('.answer-checkbox input:checked')
                    .map(function () { return $(this).val(); })
                    .get();
                if (selectedAnswers.length == response.quizBank.selectionLimit) {
                    var answerString = selectedAnswers.join(';');
                    submitAnswer(currentQuestionID, answerString);
                }
            });
        },
        error: function(xhr, status, error) {
            showQuizFeedback('Could not load this question.');
        },
        complete: function () {
            setQuizBusy(false);
        }
    });

    
}
// Function to update progress bar
function updateProgressBar(answeredCount, totalCount) {
    var progress = (answeredCount / totalCount) * 100; // Tính phần trăm tiến độ
    $('#progress-bar').css('width', progress + '%'); // Cập nhật chiều rộng của thanh tiến độ
    $('#progress-bar').attr('aria-valuenow', progress); // Cập nhật giá trị hiện tại của thanh tiến độ
}
//function to submit answer
function submitAnswer(currentQuestionID, answer) {
    $('.quiz-answer :input').prop('disabled', true);
    showQuizFeedback('');
    $.ajax({
    url: '/api/QuizApi/submitAnswer', // Endpoint của bạn để lưu câu trả lời
    method: 'POST',
    data: {
        questionId: currentQuestionID,
        answer: answer,
         PracticeID: practiceId
    },
    success: function(response) {
        // Call the function to update progress bar with the new status
        loadQuestion(currentQuestionID); // Reload the questions list to update progress bar
    },
    error: function(xhr, status, error) {
        showQuizFeedback('Could not save your answer. Please try again.');
    },
    complete: function () {
        $('.quiz-answer :input').prop('disabled', false);
    }
    });
}

