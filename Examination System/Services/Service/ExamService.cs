using AutoMapper;
using Examination_System.Data.Interfaces;
using Examination_System.Data.Repositories;
using Examination_System.Models.View_Models.ExamView_Models;
using Examination_System.Models.View_Models.Questions_AnswersViewModel;
using Examination_System.Services.Interfaces;

namespace Examination_System.Services.Service
{
    public class ExamService : IExamService
    {
        private readonly IExamRepo _examRepo;
        private readonly StudentRepo _studentRepo;
        private readonly IMapper _mapper;

        public ExamService(IExamRepo examRepo, StudentRepo studentRepo, IMapper mapper)
        {
            _examRepo = examRepo;
            _studentRepo = studentRepo;
            _mapper = mapper;
        }

        public async Task<ExamViewModel> GetExamDetails(int examId)
        {
            var exam = await _examRepo.GetExamWithQuestionsAndAnswers(examId)
                ?? throw new KeyNotFoundException("Exam not found");

            var examViewModel = new ExamViewModel
            {
                ExamId = exam.Exid,
                ExamName = exam.name,
                StartTime = exam.startat,
                EndTime = exam.endat,
                Duration = exam.duration,
                Questions = exam.QIDs?.Select(q => new QuestionViewModel
                {
                    QuestionId = q.QID,
                    QuestionText = q.title,
                    Marks = q.mark,
                    QuestionType = q.type,
                    Answers = q.answers?.Select(a => new AnswerViewModel
                    {
                        AnswerId = a.ansid,
                        AnswerText = a.title
                    }).ToList()
                }).ToList()
            };

            return examViewModel;
        }

        public async Task<ExamResultViewModel> SubmitExam(int studentId, int examId, ExamSubmissionViewModel submission)
        {
            var exam = await _examRepo.GetExamWithQuestionsAndAnswers(examId);
            var student = await _studentRepo.GetById(studentId);

            ValidateExamSubmission(exam, student);

            var (answers, totalScore) = ProcessAnswers(studentId, examId, submission, exam);

            await _examRepo.SubmitExamResults(studentId, examId, answers);
            await UpdateCourseGrade(studentId, exam.crsid.Value, totalScore);

            return new ExamResultViewModel
            {
                ExamId = examId,
                TotalScore = totalScore,
                MaxScore = exam.QIDs.Sum(q => q.mark),
                CourseName = exam.crs?.crsname ?? "Unknown Course"
            };
        }

        private void ValidateExamSubmission(Exam exam, Student student)
        {
            if (DateTime.Now < exam.startat || DateTime.Now > exam.endat)
                throw new InvalidOperationException("Exam is not active");

            if (student.std.dept_id != exam.deptid || student.std.branch_id != exam.branchid)
                throw new UnauthorizedAccessException("Not authorized for this exam");
        }

        private (List<Student_answer>, int) ProcessAnswers(int studentId, int examId,
            ExamSubmissionViewModel submission, Exam exam)
        {
            var answers = new List<Student_answer>();
            var totalScore = 0;

            foreach (var question in exam.QIDs)
            {
                var submitted = submission.Answers.FirstOrDefault(a => a.QuestionId == question.QID);
                var correctAnswers = question.answers.Where(a => a.isCorrect).Select(a => a.ansid).ToList();

                if (submitted?.SelectedAnswerIds.SequenceEqual(correctAnswers) == true)
                {
                    totalScore += (int)question.mark;
                }

                answers.AddRange(submitted.SelectedAnswerIds.Select(answerId => new Student_answer
                {
                    stdid = studentId,
                    examId = examId,
                    qid = question.QID,
                    ansid = answerId,
                    isActive = true
                }));
            }

            return (answers, totalScore);
        }

        private async Task UpdateCourseGrade(int studentId, int courseId, int score)
        {
            var course = await _studentRepo.GetStudentCourse(studentId, courseId);
            if (course != null)
            {
                course.grade = score.ToString();
                course.isActive = true; // ✅ Ensure it's marked as active
                await _studentRepo.UpdateStudentCourse(course);
            }
        }

        public async Task<ExamResultViewModel> GetExamResult(int studentId, int examId)
        {
            var result = await _examRepo.GetStudentExamResult(studentId, examId);
            return _mapper.Map<ExamResultViewModel>(result);
        }
    }
}
