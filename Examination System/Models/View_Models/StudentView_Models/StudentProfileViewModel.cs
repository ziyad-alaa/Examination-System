namespace Examination_System.Models.View_Models.StudentView_Models
{
    public class StudentProfileViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<StudentCourseViewModel> Courses { get; set; }= new List<StudentCourseViewModel>();
        public List<ExamViewModel> AvailableExams { get; set; }=new List<ExamViewModel>();
    }
}
