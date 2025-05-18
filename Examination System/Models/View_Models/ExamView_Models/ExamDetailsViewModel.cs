namespace Examination_System.Models.View_Models.ExamView_Models
{
    public class ExamDetailsViewModel
    {
        public Exam Exam { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public int TotalMarks { get; set; }
        public int QuestionCount { get; set; }
        public bool CanEdit => Exam.isActive && Exam.startat > DateTime.Now;
    }
}
