namespace ProjectBase.Models
{
    public class QuizReviewViewModel
    {
        public IEnumerable<QuizHandleModel> QuizReviews { get; set; } = [];
        public PracticeModel? Practice { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int IncorrectCount { get; set; }
        public int UnansweredCount { get; set; }
        public double CorrectPercentage =>
            TotalQuestions == 0 ? 0 : CorrectCount * 100d / TotalQuestions;
    }

}
