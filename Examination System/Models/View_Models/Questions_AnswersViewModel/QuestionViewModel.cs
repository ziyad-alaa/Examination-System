namespace Examination_System.Models.View_Models.Questions_AnswersViewModel
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public int? Marks { get; set; }
        public string? QuestionType { get; set; }
        public List<AnswerViewModel>? Answers { get; set; }
    }
}
