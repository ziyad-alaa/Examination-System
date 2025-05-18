using Examination_System.Data.UnitOfWorks;
using Examination_System.Models.View_Models.StudentView_Models;
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
            var students = _unit.StdRepo.GetAll();
            return View("AllStudents", students);
        }
        [HttpGet]
        public IActionResult Create()
        {

            studentcreate studentcreate = new studentcreate()
            {
                Departments = _unit.DeptRepo.GetAll(),
                Branches = _unit.brancRepo.GetAll()
            };
            return View(studentcreate);
        }
        [HttpPost]
        public IActionResult Create(studentcreate std)
        {
            if (std == null)
                return BadRequest();
            if (ModelState.IsValid)
            {
                User newUser = new User
                {
                    
                    name = std.name,
                    email = std.email,
                    phone = std.phone,
                    st_city = std.st_city,
                    password = std.Password,
                    dept_id = std.dept_id,
                    branch_id = std.branch_id,
                    isActive=true,
                    
                };

                Student student = new Student
                {
                    std = newUser
                };

                _unit.StdRepo.Create(student);
                _unit.Save();
                return RedirectToAction("Index");
            }


            std.Departments = _unit.DeptRepo.GetAll();
            std.Branches = _unit.brancRepo.GetAll();
            return View(std);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Student student = _unit.StdRepo.GetById(id);
            studentupdate studentview = new studentupdate()
            {
                Id = student.std.id,
                name = student.std.name,
                email = student.std.email,
                phone = student.std.phone,
                Password = student.std.password,
                st_city = student.std.st_city,
                Departments = _unit.DeptRepo.GetAll(),
                Branches = _unit.brancRepo.GetAll(),


            };
            if (studentview == null)
                return NotFound();
            return View(studentview);
        }

        [HttpPost]
        public IActionResult Edit(int id, studentupdate student)
        {
            student.Branches = _unit.brancRepo.GetAll();
            student.Departments = _unit.DeptRepo.GetAll();
            Student std = _unit.StdRepo.GetById(id);
            if (std == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                std.std.id = student.Id;
                std.std.name = student.name;
                std.std.email = student.email;
                std.std.phone = student.phone;
                std.std.password = student.Password;
                std.std.st_city = student.st_city;
                std.std.phone = student.phone;
                std.std.branch_id = student.branch_id;
                std.std.dept_id = student.dept_id;


                _unit.StdRepo.Update(id, std);
                _unit.Save();
                return RedirectToAction("Index");

            }
            else
                return View(student);


        }

        public IActionResult Delete(int id)
        {
            Student std = _unit.StdRepo.GetById(id);
            if (std == null) return NotFound();
            else
            {
                _unit.StdRepo.Delete(id);
                _unit.Save();
                return RedirectToAction("Index");
            }
        }

        public IActionResult details(int id)
        {
            Student student = _unit.StdRepo.GetById(id);
            if (student == null) return NotFound();
            else
            {
                StudentDisplay std = new StudentDisplay()
                {
                    name = student.std.name,
                    phone = student.std.phone,
                    email = student.std.email,
                    st_city = student.std.st_city,
                    Branch = student.std.branch.name,
                    Department = student.std.dept.name,

                };
                return View(std);
            }

        }
    }
}
