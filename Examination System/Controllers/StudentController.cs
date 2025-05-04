using Examination_System.Data.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    public class StudentController : Controller
    {
        UnitOfWork _unit;
        public StudentController(UnitOfWork unit)
        {
            _unit = unit;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
