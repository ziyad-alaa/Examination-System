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
        public IActionResult Index()
        {
            List<Student> StudentsToApprove = _unit.StdRepo.GetAllNotActive();
            return View("NotActivatedStudents", StudentsToApprove);
        }
        [HttpPost]
        public IActionResult Approve(int id)
        {
            Student NotActiveStd=_unit.StdRepo.GetNotactiveById(id);
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
