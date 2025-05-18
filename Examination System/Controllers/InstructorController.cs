using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Examination_System.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Examination_System.Models.View_Models.InstructorViewModel;

namespace Examination_System.Controllers
{
    public class InstructorController : Controller
    {
        private readonly Exam_sysContext _context;

        public InstructorController(Exam_sysContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
    string searchTerm = "",
    string sortColumn = "Name",
    string sortDirection = "asc",
    int page = 1)
        {
            var query = from instr in _context.Instructors
                        join usr in _context.Users on instr.insid equals usr.id
                        join dept in _context.Departments on usr.dept_id equals dept.dept_id
                        join br in _context.Branches on usr.branch_id equals br.branch_id
                        where usr.isActive == true
                        select new InstructorIndexViewModel
                        {
                            InstructorId = instr.insid,
                            Name = usr.name,
                            StCity = usr.st_city,
                            DeptName = dept.name,
                            Email = usr.email,
                            Phone = usr.phone,
                            BranchName = br.name,
                            UserIsActive = usr.isActive,
                            JobTitle = instr.jobTitle,
                            Salary = instr.salary ?? 0,
                            InstructorIsActive = instr.isActive
                        };

            // Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(i =>
                    i.Name.Contains(searchTerm) ||
                    i.DeptName.Contains(searchTerm) ||
                    i.BranchName.Contains(searchTerm) ||
                    i.Email.Contains(searchTerm) ||
                    i.Phone.Contains(searchTerm));
            }

            // Sorting logic
            switch (sortColumn.ToLower())
            {
                case "name":
                    query = sortDirection == "asc"
                        ? query.OrderBy(i => i.Name)
                        : query.OrderByDescending(i => i.Name);
                    break;

                case "department":
                    query = sortDirection == "asc"
                        ? query.OrderBy(i => i.DeptName)
                        : query.OrderByDescending(i => i.DeptName);
                    break;

                case "branch":
                    query = sortDirection == "asc"
                        ? query.OrderBy(i => i.BranchName)
                        : query.OrderByDescending(i => i.BranchName);
                    break;

                case "email":
                    query = sortDirection == "asc"
                        ? query.OrderBy(i => i.Email)
                        : query.OrderByDescending(i => i.Email);
                    break;

                case "phone":
                    query = sortDirection == "asc"
                        ? query.OrderBy(i => i.Phone)
                        : query.OrderByDescending(i => i.Phone);
                    break;

                case "jobtitle":
                    query = sortDirection == "asc"
                        ? query.OrderBy(i => i.JobTitle)
                        : query.OrderByDescending(i => i.JobTitle);
                    break;

                case "salary":
                    query = sortDirection == "asc"
                        ? query.OrderBy(i => i.Salary)
                        : query.OrderByDescending(i => i.Salary);
                    break;

                default:
                    query = query.OrderBy(i => i.Name);
                    break;
            }

            // Pagination settings
            int pageSize = 5;
            var totalItems = await query.CountAsync();
            var instructors = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var viewModel = new InstructorListViewModel
            {
                Instructors = instructors,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                SearchTerm = searchTerm,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var instructor = await _context.Instructors
                .Include(i => i.ins)
                .FirstOrDefaultAsync(m => m.insid == id && m.ins.isActive == true);

            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments.Where(d => d.isActive), "dept_id", "name");
            ViewBag.Branches = new SelectList(_context.Branches.Where(b => b.isActive), "branch_id", "name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("name,st_city,dept_id,password,email,phone,branch_id,isActive")] User user,
            [Bind("jobTitle,salary,isActive")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                instructor.insid = user.id;
                instructor.ins = user;
                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = new SelectList(_context.Departments.Where(d => d.isActive), "dept_id", "name");
            ViewBag.Branches = new SelectList(_context.Branches.Where(b => b.isActive), "branch_id", "name");
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var instructor = await _context.Instructors
                .Include(i => i.ins)
                .FirstOrDefaultAsync(i => i.insid == id);
            if (instructor == null)
                return NotFound();

            ViewBag.Departments = new SelectList(_context.Departments.Where(d => d.isActive), "dept_id", "name", instructor.ins.dept_id);
            ViewBag.Branches = new SelectList(_context.Branches.Where(b => b.isActive), "branch_id", "name", instructor.ins.branch_id);
            return View(instructor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("jobTitle,salary,isActive")] Instructor instructor,
            [Bind("name,st_city,dept_id,password,email,phone,branch_id")] User user)
        {
            var existingInstructor = await _context.Instructors
                .Include(i => i.ins)
                .FirstOrDefaultAsync(i => i.insid == id);
            if (existingInstructor == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    existingInstructor.ins.name = user.name;
                    existingInstructor.ins.st_city = user.st_city;
                    existingInstructor.ins.dept_id = user.dept_id;
                    existingInstructor.ins.password = user.password;
                    existingInstructor.ins.email = user.email;
                    existingInstructor.ins.phone = user.phone;
                    existingInstructor.ins.branch_id = user.branch_id;

                    var userIsActiveStr = Request.Form["userIsActive"].ToString();
                    existingInstructor.ins.isActive = bool.Parse(userIsActiveStr);

                    existingInstructor.jobTitle = instructor.jobTitle;
                    existingInstructor.salary = instructor.salary;

                    var instrIsActiveStr = Request.Form["instrIsActive"].ToString();
                    existingInstructor.isActive = bool.Parse(instrIsActiveStr);

                    _context.Update(existingInstructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(existingInstructor.insid))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(existingInstructor);
        }

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var instructor = await _context.Instructors
                .Include(i => i.ins) // Include user data
                .FirstOrDefaultAsync(i => i.insid == id);

            if (instructor == null || !instructor.ins.isActive)
                return NotFound();

            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.ins)
                .FirstOrDefaultAsync(i => i.insid == id);
            if (instructor != null)
            {
                instructor.ins.isActive = false;
                instructor.isActive = false;
                _context.Update(instructor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        

        

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.insid == id);
        }
    }
   
}
