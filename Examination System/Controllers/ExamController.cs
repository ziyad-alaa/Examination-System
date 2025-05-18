using Examination_System.Data.UnitOfWorks;
using Examination_System.Model.View_Models.ExamView_Models;
using Examination_System.Models;
using Examination_System.Models.View_Models.ExamView_Models;
using Examination_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Controllers
{
    public class ExamController : Controller
    {
        private readonly IExamGenerationService _examService;
        Exam_sysContext _context;

        private UnitOfWork _unitOfWork ;
        public ExamController(IExamGenerationService examService, Exam_sysContext userContext)
        {
            _examService = examService;
            _context = userContext;
            _unitOfWork= new UnitOfWork(_context);
        }

        [HttpGet]
        public async Task<ActionResult> Generate()
        {
            var instructorId = 2; // Should come from authentication
            try
            {
                var model = await _examService.GetInitialViewModel(instructorId);
                foreach (var item in model.Courses) {

                    Console.WriteLine("1");
                }
                
                return View("~/Views/Exam/Generate.cshtml", model);
            }
            catch (Exception ex)
            {
                // Log error
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDepartments(int branchId)
        {
            var instructorId = 2;
            var departments = await _unitOfWork.Course_Dept
                .GetDepartmentsForInstructorAsync(instructorId, branchId);
            return Json(new SelectList(departments, "dept_id", "name"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses(int branchId, int deptId)
        {
            var instructorId = 2;
            var courses = await _unitOfWork.Course_Dept
                .GetCoursesForInstructorAsync(instructorId, branchId, deptId);
            return Json(new SelectList(courses, "crsid", "crsname"));
        }

        [HttpGet]
        public async Task<IActionResult> GetExistingQuestions(int courseId)
        {
            var questions = await _examService.GetExistingQuestionsAsync(courseId);

            foreach(var question in questions)
            {
                Console.WriteLine(question.QID);
            }
            return PartialView("_ExistingQuestionsPartial", questions);
        }

        

        [HttpPost]
        public async Task<IActionResult> Generate(generateExamViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Repopulate dropdowns if model is invalid
            //    model.Branches = (await _instructorRepo.GetBranchesByInstructor(GetCurrentInstructorId()))
            //        .Select(b => new SelectListItem
            //        {
            //            Value = b.branchId.ToString(),
            //            Text = b.Name
            //        }).ToList();

            //    if (model.BranchId > 0)
            //    {
            //        model.Departments = (await _examService.GetDepartmentsByBranchAsync(model.BranchId))
            //            .Select(d => new SelectListItem
            //            {
            //                Value = d.dept_id.ToString(),
            //                Text = d.Name
            //            }).ToList();
            //    }

            //    if (model.DepartmentId > 0)
            //    {
            //        model.Courses = (await _examService.GetCoursesByDepartmentAndBranchAsync(model.DepartmentId, model.BranchId))
            //            .Select(c => new SelectListItem
            //            {
            //                Value = c.crsid.ToString(),
            //                Text = c.Name
            //            }).ToList();
            //    }

            //    return View(model);
            //}

            var result = await _examService.GenerateExamAsync(model, 2);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Exam generated successfully with {result.QuestionCount} questions and total marks of {result.TotalMarks}.";
                return RedirectToAction("Details", new { id = result.ExamId });
            }

            ModelState.AddModelError("", result.ErrorMessage);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Get exam with related data
                var exam = await _context.Exams
                    .Include(e => e.crs)
                    .Include(e => e.ins)
                    .Include(e => e.department)
                    .Include(e => e.branch)
                    .Include(e => e.QIDs)
                        .ThenInclude(q => q.answers)
                    .FirstOrDefaultAsync(e => e.Exid == id);

                if (exam == null)
                {
                    return NotFound();
                }

                // Calculate time remaining if exam is active
                var timeRemaining = TimeSpan.Zero;
                if (exam.isActive && exam.endat > DateTime.Now)
                {
                    timeRemaining = exam.endat.Value - DateTime.Now;
                }

                // Prepare view model
                var viewModel = new ExamDetailsViewModel
                {
                    Exam = exam,
                    TimeRemaining = timeRemaining,
                    TotalMarks = exam.QIDs?.Sum(q => q.mark) ?? 0,
                    QuestionCount = exam.QIDs?.Count ?? 0
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
               
                TempData["ErrorMessage"] = "An error occurred while loading exam details.";
                return RedirectToAction("Index");
            }
        }
    }
}
