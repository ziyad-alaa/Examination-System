using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    public class StudentApproveController : Controller
    {
        UnitOfWork _unit;
        public StudentApproveController(UnitOfWork unit)
        {
            _unit = unit;
        }
        public async  Task<IActionResult> Index()
        {
            List<Student> StudentsToApprove =await _unit.StdRepo.GetAllNotActive();
            return View("NotActivatedStudents", StudentsToApprove);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            Student NotActiveStd=await _unit.StdRepo.GetNotactiveById(id);
            if (NotActiveStd == null)
                return NotFound();
            else
            {
                NotActiveStd.std.isActive= true;
                _unit.Save();
                return RedirectToAction("Index");
            }
        }
    }
}
