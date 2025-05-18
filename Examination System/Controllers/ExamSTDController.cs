using Examination_System.Models.View_Models.ExamView_Models;
using Examination_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    public class ExamSTDController : BaseController
    {
        private readonly IExamService _examService;
        private readonly IStudentService _std;

        public ExamSTDController(IExamService examService, IStudentService std, UnitOfWork unit)
    : base( std) // ✅ Pass both dependencies to base
        {
            _examService = examService;
            _std = std;
        }

        public async Task<IActionResult> TakeExam(int examId)
        {
            try
            {
                var exam = await _examService.GetExamDetails(examId);
                return View(exam);
            }
            catch (KeyNotFoundException)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam(int examId, ExamSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("TakeExam", new { examId });
            }

            try
            {
                var result = await _examService.SubmitExam(CurrentStudentId, examId, model);
                return RedirectToAction("Result", new { examId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("TakeExam", new { examId });
            }
        }

        public async Task<IActionResult> Result(int examId)
        {
            var result = await _examService.GetExamResult(CurrentStudentId, examId);
            return View(result);
        }
    }
}
