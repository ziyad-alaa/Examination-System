using Examination_System.Data.Interfaces;
using Examination_System.Data.UnitOfWorks;
using Examination_System.Model.View_Models.ExamView_Models;
using Examination_System.Models;
using Examination_System.Models.Commands;
using Examination_System.Models.View_Models.DTO;
using Examination_System.Utilities.Results;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services
{
    public interface IExamGenerationService
    {
        Task<generateExamViewModel> GetInitialViewModel(int instructorId);
        Task<ExamGenerationResult> GenerateExamAsync(generateExamViewModel model, int instructorId);
        Task<IEnumerable<Question_Bank>> GetExistingQuestionsAsync(int courseId);
    }

    public class ExamGenerationService : IExamGenerationService
    {
        private readonly UnitOfWork _unitOfWork;

        public ExamGenerationService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<generateExamViewModel> GetInitialViewModel(int instructorId)
        {
            var user = await _unitOfWork.userRepo.GetById(instructorId);
            if (user == null) throw new ArgumentException("Invalid instructor ID");

            
            var branches = await _unitOfWork.Course_Dept.GetBranchesForInstructorAsync(instructorId);
            return new generateExamViewModel
            {
                NewQuestions = new List<QuestionExamViewModel>(),
                Branches = new SelectList(branches, "branch_id", "name"),
                Departments = new SelectList(Enumerable.Empty<SelectListItem>()),
                Courses = new SelectList(Enumerable.Empty<SelectListItem>()),
                StartTime = DateTime.Now,
                DurationMinutes = 120
            };
        }
        
        //public async Task<ExamGenerationResult> GenerateExamAsync(generateExamViewModel model, int instructorId)
        //{
        //    var user = await _unitOfWork.userRepo.GetById(instructorId);
        //    if (user == null)
        //        return ExamGenerationResult.Failure("Invalid instructor ID");

        //    // Additional validation
        //    var course = await _unitOfWork.Course.GetById(model.CourseId);
        //    if (course == null || !course.course_depts.Any(cd => cd.dept_id == user.dept_id))
        //    {
        //        return ExamGenerationResult.Failure("Invalid course selection");
        //    }
        //    var command = new ExamGenerationCommand
        //    {
        //        ExamName = model.ExamName,
        //        CourseId = model.CourseId,
        //        InstructorId = instructorId,
        //        DepartmentId = user.dept_id.GetValueOrDefault(),
        //        BranchId = user.branch_id.GetValueOrDefault(),
        //        StartTime = model.StartTime,
        //        DurationMinutes = model.DurationMinutes,
        //        Questions = model.NewQuestions.Select(q => new QuestionExamViewModel
        //        {
        //            QuestionText = q.QuestionText,
        //            Mark = q.Mark,
        //            QuestionType = q.QuestionType,
        //            Answers = q.Answers.Select(a => new AnswerModel
        //            {
        //                AnswerText = a.AnswerText,
        //                IsCorrect = a.IsCorrect
        //            }).ToList()
        //        }).ToList()
        //    };

        //    try
        //    {
        //        var examId = _unitOfWork.Exam.GenerateExam(command);
        //        await _unitOfWork.CompleteAsync();
        //        return ExamGenerationResult.Success(examId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ExamGenerationResult.Failure($"Error generating exam: {ex.Message}");
        //    }
        //}

        public async Task<IEnumerable<Question_Bank>> GetExistingQuestionsAsync(int courseId)
        {
            return await _unitOfWork.Exam.GetQuestionsByCourseAsync(courseId);
        }

        public async Task<ExamGenerationResult> GenerateExamAsync(generateExamViewModel model, int instructorId)
        {
            try
            {
                // Convert new questions to the format expected by the stored procedure
                var newQuestions = model.NewQuestions?.Select(q => new QuestionExamViewModel
                {
                    QuestionText = q.QuestionText,
                    Mark = q.Mark,
                    QuestionType = q.QuestionType,
                    Answers = q.QuestionType != "Essay" ? q.Answers : new List<AnswerModel>()
                }).ToList();

                // Call the repository method that uses the stored procedure
                return await _unitOfWork.Exam.GenerateExamWithStoredProcedureAsync(
                    examName: model.ExamName,
                    crsId: model.CourseId,
                    insId: instructorId,
                    deptId: model.DepartmentId,
                    branchId: model.BranchId,
                    startAt: model.StartTime,
                    duration: model.DurationMinutes,
                    newQuestions: newQuestions,
                    existingQuestionIds: model.SelectedQuestionIds);
            }
            catch (Exception ex)
            {
                return ExamGenerationResult.Failure($"Error generating exam: {ex.Message}");
            }
        }
    }

}
