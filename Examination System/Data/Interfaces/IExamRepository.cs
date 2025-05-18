using Examination_System.Model.View_Models.ExamView_Models;
using Examination_System.Models;
using Examination_System.Models.Commands;
using Examination_System.Utilities.Results;

namespace Examination_System.Data.Interfaces
{
    public interface IExamRepository
    {
         Task<ExamGenerationResult> GenerateExamWithStoredProcedureAsync(
        string examName,
        int crsId,
        int insId,
        int deptId,
        int branchId,
        DateTime startAt,
        int duration,
        List<QuestionExamViewModel> newQuestions,
        List<int> existingQuestionIds);

        Task<IEnumerable<course>> GetAvailableCoursesForInstructor(int instructorId);
    }
}
