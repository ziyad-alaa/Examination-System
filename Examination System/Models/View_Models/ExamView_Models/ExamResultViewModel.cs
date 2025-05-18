namespace Examination_System.Models.View_Models.ExamView_Models
{
    public class ExamResultViewModel
    {
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public int? TotalScore { get; set; }
        public int? MaxScore { get; set; }
        public string? CourseName { get; set; }
        public DateTime? SubmissionTime { get; set; }
        public decimal? Percentage => Math.Round((decimal)(TotalScore * 100.0 / MaxScore), 2);
    }
}
