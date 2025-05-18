using Examination_System.Models.View_Models.Questions_AnswersViewModel;

namespace Examination_System.Models.View_Models.StudentView_Models
{
    public class ExamViewModel
    {
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Duration { get; set; }
        public List<QuestionViewModel>? Questions { get; set; }
    }
}
