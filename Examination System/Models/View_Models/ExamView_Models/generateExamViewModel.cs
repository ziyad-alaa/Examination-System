using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Model.View_Models.ExamView_Models
{
    public class generateExamViewModel
    {
        [Required]
        public string ExamName { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Required]
        [Range(1, 600)]
        public int DurationMinutes { get; set; } = 120;

        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();

        // Dropdown lists
        public SelectList Courses { get; set; }
    }
    public class QuestionModel
    {
        public int QuestionOrder { get; set; }
        public string QuestionText { get; set; }
        public int Mark { get; set; }
        public string QuestionType { get; set; } // "MCQ", "TrueFalse", "Essay"
        public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>();
    }

    public class AnswerModel
    {
        public int QuestionOrder { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public int? AnswerOrder { get; set; }
    }
}
