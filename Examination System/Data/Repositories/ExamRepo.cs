using Examination_System.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.Repositories
{
    public class ExamRepo : IExamRepo
    {
        private readonly Exam_sysContext _context;

        public ExamRepo(Exam_sysContext context)
        {
            _context = context;
        }

        public async Task<Exam> GetExamWithQuestionsAndAnswers(int examId)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Exams
                .Include(e => e.QIDs)
                    .ThenInclude(q => q.answers)
                .Include(e => e.crs)
                .FirstOrDefaultAsync(e => e.Exid == examId && e.isActive);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task SubmitExamResults(int studentId, int examId, List<Student_answer> answers)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Save student exam record
                var studentExam = new Student_Exam
                {
                    stdid = studentId,
                    ExamId = examId,
                    Grade = "0", // Temporary value
                    isActive = true
                };
                await _context.Student_Exams.AddAsync(studentExam);

                // Save student answers
                await _context.Student_answers.AddRangeAsync(answers);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Student_Exam> GetStudentExamResult(int studentId, int examId)
        {
            return await _context.Student_Exams
                .Include(se => se.Exam)
                    .ThenInclude(e => e.crs)
                .FirstOrDefaultAsync(se => se.stdid == studentId && se.ExamId == examId);
        }
    }
}
