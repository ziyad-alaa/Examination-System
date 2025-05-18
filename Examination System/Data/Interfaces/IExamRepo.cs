namespace Examination_System.Data.Interfaces
{
    public interface IExamRepo
    {
        Task<Exam> GetExamWithQuestionsAndAnswers(int examId);
        Task SubmitExamResults(int studentId, int examId, List<Student_answer> answers);
        Task<Student_Exam> GetStudentExamResult(int studentId, int examId);
    }
}
