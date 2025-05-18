using Examination_System.Models;

namespace Examination_System.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentProfileViewModel> GetStudentProfile(int studentId);
        Task<List<StudentCourseViewModel>> GetStudentCourses(int studentId);
        Task<List<Exam>> GetAvailableExams(int studentId);
    }
}
