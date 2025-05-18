using Examination_System.Services.Interfaces;
using Examination_System.Services.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Examination_System.Controllers
{
    public class BaseController : Controller
    {
        IStudentService studentService;
        public BaseController(IStudentService studentService)
        {
            this.studentService = studentService;
        }
        public int CurrentStudentId
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    var claim =  User.FindFirst(ClaimTypes.NameIdentifier);
                    if (claim != null && int.TryParse(claim.Value, out var studentId))
                    {
                        return studentId;
                    }
                }
                throw new UnauthorizedAccessException("Student not authenticated");
               
            }
        }

        public async Task<IActionResult> Profile()
        {
            var studentId = CurrentStudentId;

            if (studentId == null)
                return RedirectToAction("Login", "Auth");

            var studentProfile = await studentService.GetStudentProfile(studentId);

            if (studentProfile == null)
                return RedirectToAction("AccessDenied", "Auth");

            return View(studentProfile);
        }
    }
}
