namespace Examination_System.Models.View_Models.ExamView_Models
{
    public class ExamSubmissionViewModel
    {
        public int ExamId { get; set; }
        public List<QuestionSubmission> Answers { get; set; } = new List<QuestionSubmission>();
    }

}
