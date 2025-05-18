using Examination_System.Models;
using Examination_System.Models.View_Models.ExamView_Models;

namespace Examination_System.Services.Interfaces
{
    public interface IExamService
    {
        Task<ExamViewModel> GetExamDetails(int examId);
        Task<ExamResultViewModel> SubmitExam(int studentId, int examId, ExamSubmissionViewModel submission);
        Task<ExamResultViewModel> GetExamResult(int studentId, int examId);
    }
}
