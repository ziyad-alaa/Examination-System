using Examination_System.Model.View_Models.ExamView_Models;

namespace Examination_System.Models.Commands
{
    public class ExamGenerationCommand
    {
        public string ExamName { get; set; }
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public int DepartmentId { get; set; }
        public int BranchId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public List<QuestionExamViewModel> Questions { get; set; } = new List<QuestionExamViewModel>();
    }
}
