using Examination_System.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

public class ExamGenerationCommand
{
    public string ExamName { get; set; }
    public int CourseId { get; set; }
    public int InstructorId { get; set; }
    public int DepartmentId { get; set; }
    public int BranchId { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public List<QuestionExamViewModel> Questions { get; set; } = new List<QuestionExamViewModel>();
}
public interface IExamRepository
{
    int GenerateExam(ExamGenerationCommand command);
    IEnumerable<course> GetAvailableCoursesForInstructor(int instructorId);
}

public class ExamRepository : IExamRepository
{
    private readonly Exam_sysContext _context;

    public ExamRepository(Exam_sysContext context)
    {
        _context = context;
    }

    public int GenerateExam(ExamGenerationCommand command)
    {
        // Create DataTables for table-valued parameters
        var questionsTable = new DataTable();
        questionsTable.Columns.Add("QuestionOrder", typeof(int));
        questionsTable.Columns.Add("QuestionText", typeof(string));
        questionsTable.Columns.Add("Mark", typeof(int));
        questionsTable.Columns.Add("QuestionType", typeof(string));

        var answersTable = new DataTable();
        answersTable.Columns.Add("QuestionOrder", typeof(int));
        answersTable.Columns.Add("AnswerText", typeof(string));
        answersTable.Columns.Add("IsCorrect", typeof(bool));
        answersTable.Columns.Add("AnswerOrder", typeof(int));

        // Populate tables
        for (int i = 0; i < command.Questions.Count; i++)
        {
            var question = command.Questions[i];
            questionsTable.Rows.Add(i + 1, question.QuestionText, question.Mark, question.QuestionType);

            for (int j = 0; j < question.Answers.Count; j++)
            {
                var answer = question.Answers[j];
                answersTable.Rows.Add(i + 1, answer.AnswerText, answer.IsCorrect, j + 1);
            }
        }

        // Call stored procedure
        var examIdParam = new SqlParameter("@exam_id", SqlDbType.Int) { Direction = ParameterDirection.Output };

        _context.Database.ExecuteSqlRawAsync(
            "EXEC GenerateExam " +
            "@exam_name, @crs_id, @ins_id, @dept_id, @branch_id, " +
            "@start_at, @end_at, @duration, @Questions, @Answers, @exam_id OUTPUT",
            new SqlParameter("@exam_name", command.ExamName),
            new SqlParameter("@crs_id", command.CourseId),
            new SqlParameter("@ins_id", command.InstructorId),
            new SqlParameter("@dept_id", command.DepartmentId),
            new SqlParameter("@branch_id", command.BranchId),
            new SqlParameter("@start_at", command.StartTime),
            new SqlParameter("@end_at", command.StartTime.AddMinutes(command.DurationMinutes)),
            new SqlParameter("@duration", command.DurationMinutes),
            new SqlParameter("@Questions", questionsTable) { TypeName = "QuestionList" },
            new SqlParameter("@Answers", answersTable) { TypeName = "AnswerList" },
        examIdParam
        );

        return (int)examIdParam.Value;
    }

    public IEnumerable<course> GetAvailableCoursesForInstructor(int instructorId)
    {
        var departmentId = _context.Users
            .Where(u => u.id == instructorId)
            .Select(u => u.dept_id)
            .FirstOrDefault();

        return _context.courses
            .Where(c => 1 == departmentId && c.isActive)
            .ToList();
    }
}