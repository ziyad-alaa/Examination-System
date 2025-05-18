namespace Examination_System.Models.View_Models.ExamView_Models
{
    public class QuestionSubmission
    {
        public int QuestionId { get; set; }
        public List<int> SelectedAnswerIds { get; set; } = new List<int>();
    }
}
