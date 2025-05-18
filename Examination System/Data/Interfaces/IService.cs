using System.Threading.Tasks;

namespace Examination_System.Data.Interfaces
{
    public interface IService2<T>
    {
        public Task<List<T>> GetAll();
        public Task<List<T>> GetAllNotActive();
        public Task<T> GetById(int id);
        public Task<T> GetNotactiveById(int id);
        Task<bool> HasStudentTakenCourseExam(int studentId, int courseId);
        public Task  Create(T entity);
        public Task  Delete(int id);
        public Task Update(int id, T entity);
        public Task< List<Exam>> GetAvailableExams(int studentId);
        public Task<Student_course> GetStudentCourse(int studentId, int courseId);
        public Task UpdateStudentCourse(Student_course studentCourse);
        public Task<List<Student_course>> GetStudentCourses(int studentId);

    }
}
