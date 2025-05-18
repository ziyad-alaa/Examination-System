using Examination_System.Models;
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
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }


        [Required]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Required]
        [Range(1, 600)]
        public int DurationMinutes { get; set; } = 120;

        public List<QuestionExamViewModel> NewQuestions { get; set; } = new List<QuestionExamViewModel>();

        // Dropdown lists
        public SelectList Courses { get; set; }
        public SelectList Branches { get; set; }
        public SelectList Departments { get; set; }

        public List<Question_Bank> ExistingQuestions { get; set; } = new List<Question_Bank>();
        public List<int> SelectedQuestionIds { get; set; } = new List<int>();
    }


    public class QuestionExamViewModel
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
