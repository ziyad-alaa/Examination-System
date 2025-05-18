using Examination_System.Data.Interfaces;
using Examination_System.Model.View_Models.ExamView_Models;
using Examination_System.Models;
using Examination_System.Models.Commands;
using Examination_System.Utilities.Results;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;





public class ExamRepository : IExamRepository
{
    private readonly Exam_sysContext _context;

    public ExamRepository(Exam_sysContext context)
    {
        _context = context;
    }

    //public int GenerateExam(ExamGenerationCommand command)
    //{
    //    // Create DataTables for table-valued parameters
    //    var questionsTable = new DataTable();
    //    questionsTable.Columns.Add("QuestionOrder", typeof(int));
    //    questionsTable.Columns.Add("QuestionText", typeof(string));
    //    questionsTable.Columns.Add("Mark", typeof(int));
    //    questionsTable.Columns.Add("QuestionType", typeof(string));

    //    var answersTable = new DataTable();
    //    answersTable.Columns.Add("QuestionOrder", typeof(int));
    //    answersTable.Columns.Add("AnswerText", typeof(string));
    //    answersTable.Columns.Add("IsCorrect", typeof(bool));
    //    answersTable.Columns.Add("AnswerOrder", typeof(int));

    //    // Populate tables
    //    for (int i = 0; i < command.Questions.Count; i++)
    //    {
    //        var question = command.Questions[i];
    //        questionsTable.Rows.Add(i + 1, question.QuestionText, question.Mark, question.QuestionType);

    //        for (int j = 0; j < question.Answers.Count; j++)
    //        {
    //            var answer = question.Answers[j];
    //            answersTable.Rows.Add(i + 1, answer.AnswerText, answer.IsCorrect, j + 1);
    //        }
    //    }

    //    // Call stored procedure
    //    var examIdParam = new SqlParameter("@exam_id", SqlDbType.Int) { Direction = ParameterDirection.Output };

    //    _context.Database.ExecuteSqlRawAsync(
    //        "EXEC GenerateExam " +
    //        "@exam_name, @crs_id, @ins_id, @dept_id, @branch_id, " +
    //        "@start_at, @end_at, @duration, @Questions, @Answers, @exam_id OUTPUT",
    //        new SqlParameter("@exam_name", command.ExamName),
    //        new SqlParameter("@crs_id", command.CourseId),
    //        new SqlParameter("@ins_id", command.InstructorId),
    //        new SqlParameter("@dept_id", command.DepartmentId),
    //        new SqlParameter("@branch_id", command.BranchId),
    //        new SqlParameter("@start_at", command.StartTime),
    //        new SqlParameter("@end_at", command.StartTime.AddMinutes(command.DurationMinutes)),
    //        new SqlParameter("@duration", command.DurationMinutes),
    //        new SqlParameter("@Questions", questionsTable) { TypeName = "QuestionList" },
    //        new SqlParameter("@Answers", answersTable) { TypeName = "AnswerList" },
    //    examIdParam
    //    );

    //    return (int)examIdParam.Value;
    //}


    public async Task<IEnumerable<course>> GetAvailableCoursesForInstructor(int instructorId)
    {
        // First operation - properly awaited
        var departmentId = await _context.Users
            .AsNoTracking()
            .Where(u => u.id == instructorId)
            .Select(u => u.dept_id)
            .FirstOrDefaultAsync();

        if (departmentId == 0)
            return Enumerable.Empty<course>();

        // Second operation - properly awaited
        return await _context.course_depts
            .AsNoTracking() 
            .Where(cd => cd.dept_id == departmentId && cd.isActive &&cd.insid==instructorId)
            .Select(cd => cd.crs)
            .Where(c => c.isActive)
            .ToListAsync();
    }
    public async Task<IEnumerable<Question_Bank>> GetQuestionsByCourseAsync(int courseId)
    {
        return await _context.Question_Banks
            .Where(q => q.crsid == courseId && q.isActive)
            .Include(q => q.answers)
            .ToListAsync();
    }


    public async Task<ExamGenerationResult> GenerateExamWithStoredProcedureAsync(
    string examName,
    int crsId,
    int insId,
    int deptId,
    int branchId,
    DateTime startAt,
    int duration,
    List<QuestionExamViewModel> newQuestions,
    List<int> existingQuestionIds)
    {
        try
        {
            // Prepare the table-valued parameters
            var questionsTable = new DataTable();
            questionsTable.Columns.Add("QuestionText", typeof(string));
            questionsTable.Columns.Add("Mark", typeof(int));
            questionsTable.Columns.Add("QuestionType", typeof(string));

            var answersTable = new DataTable();
            answersTable.Columns.Add("QuestionOrder", typeof(int));
            answersTable.Columns.Add("AnswerText", typeof(string));
            answersTable.Columns.Add("IsCorrect", typeof(bool));

            // Populate questions and answers tables
            int questionOrder = 0;
            foreach (var question in newQuestions)
            {
                questionsTable.Rows.Add(question.QuestionText, question.Mark, question.QuestionType);

                if (question.QuestionType != "Essay")
                {
                    foreach (var answer in question.Answers)
                    {
                        answersTable.Rows.Add(questionOrder, answer.AnswerText, answer.IsCorrect);
                    }
                }
                questionOrder++;
            }

            var existingQuestionsTable = new DataTable();
            existingQuestionsTable.Columns.Add("Value", typeof(int));
            foreach (var id in existingQuestionIds)
            {
                existingQuestionsTable.Rows.Add(id);
            }

            // Calculate end time
            var endAt = startAt.AddMinutes(duration);

            // Execute the stored procedure
            var parameters = new[]
            {
            new SqlParameter("@exam_name", examName),
            new SqlParameter("@crs_id", crsId),
            new SqlParameter("@ins_id", insId),
            new SqlParameter("@dept_id", deptId),
            new SqlParameter("@branch_id", branchId),
            new SqlParameter("@start_at", startAt),
            new SqlParameter("@end_at", endAt),
            new SqlParameter("@duration", duration),
            new SqlParameter("@Questions", questionsTable) { SqlDbType = SqlDbType.Structured, TypeName = "QuestionList" },
            new SqlParameter("@Answers", answersTable) { SqlDbType = SqlDbType.Structured, TypeName = "AnswerList" },
            new SqlParameter("@ExistingQuestionIDs", existingQuestionsTable) { SqlDbType = SqlDbType.Structured, TypeName = "IntList" },
            new SqlParameter("@ExamID", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@TotalMarks", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@QuestionCount", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC GenerateExam @exam_name, @crs_id, @ins_id, @dept_id, @branch_id, " +
                "@start_at, @end_at, @duration, @Questions, @Answers, @ExistingQuestionIDs, " +
                "@ExamID OUTPUT, @TotalMarks OUTPUT, @QuestionCount OUTPUT",
                parameters);

            var examId = (int)parameters.First(p => p.ParameterName == "@ExamID").Value;
            var totalMarks = (int)parameters.First(p => p.ParameterName == "@TotalMarks").Value;
            var questionCount = (int)parameters.First(p => p.ParameterName == "@QuestionCount").Value;

            return ExamGenerationResult.Success(examId, totalMarks, questionCount);
        }
        catch (Exception ex)
        {
            return ExamGenerationResult.Failure(ex.Message);

        }
    }

   
}