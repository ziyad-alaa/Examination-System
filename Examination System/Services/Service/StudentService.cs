using Examination_System.Data.Repositories;
using Examination_System.Models;
using Examination_System.Models.View_Models.Questions_AnswersViewModel;
using Examination_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.Service
{
    public class StudentService : IStudentService
    {
        private readonly StudentRepo _studentRepository;
        private readonly Exam_sysContext _sysContext;

        public StudentService(Exam_sysContext sysContext, StudentRepo studentRepository)
        {
            _sysContext = sysContext;
            _studentRepository = studentRepository;
        }

        public async Task<List<Exam>> GetAvailableExams(int studentId)
        {
            return await _studentRepository.GetAvailableExams(studentId);
        }

        public async Task<List<StudentCourseViewModel>> GetStudentCourses(int studentId)
        {
            var studentCourses = await _studentRepository.GetStudentCourses(studentId);
            var result = new List<StudentCourseViewModel>();

            foreach (var sc in studentCourses)
            {
                var course = await _sysContext.courses.FirstOrDefaultAsync(c => c.crsid == sc.crsid);
                var hasTakenExam = await _studentRepository.HasStudentTakenCourseExam(studentId, sc.crsid);

                result.Add(new StudentCourseViewModel
                {
                    CourseId = course?.crsid ?? sc.crsid,
                    CourseName = course?.crsname ?? "Unknown",
                    IsActive = hasTakenExam,
                    Grade = sc.grade,
                    Comments = sc.comments
                });
            }

            return result;
        }

        public async Task<StudentProfileViewModel> GetStudentProfile(int studentId)
        {
            // Fetch student entity
            var student = await _studentRepository.GetById(studentId);
            if (student == null) return null;

            // Fetch student courses
            var courses = await _studentRepository.GetStudentCourses(studentId);

            // Fetch available exams
            var exams = await _studentRepository.GetAvailableExams(studentId);

            // Map courses
            var courseViewModels = courses.Select(c => new StudentCourseViewModel
            {
                CourseId = c.crsid,
                CourseName = c.crs?.crsname ?? "Unknown Course",
                IsActive = c.isActive,
                Grade = c.grade,
                Comments = c.comments
            }).ToList();

            // Map exams
            var examViewModels = exams.Select(exam => new ExamViewModel
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
                    }).ToList() ?? new List<AnswerViewModel>()
                }).ToList() ?? new List<QuestionViewModel>()
            }).ToList();

            return new StudentProfileViewModel
            {
                StudentId = studentId,
                StudentName = student.std?.name ?? "Unknown",
                Courses = courseViewModels,
                AvailableExams = examViewModels
            };
        }
    }
}
