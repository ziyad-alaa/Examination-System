using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Examination_System.Models;
using System.Linq;
using System.Threading.Tasks;
using Examination_System.Models.View_Models.CourseViewModel;

namespace Examination_System.Controllers
{
    public class CourseController : Controller
    {
        private readonly Exam_sysContext _context;

        public CourseController(Exam_sysContext context)
        {
            _context = context;
        }


        //public async Task<IActionResult> Index()
        //{
        //    var courses = await _context.courses
        //        .Where(c => c.isActive)
        //        .Include(c => c.course_depts)
        //        .Include(c => c.Exams)
        //        .Include(c => c.Topics)
        //        .Include(c => c.Question_Banks)
        //        .Include(c => c.Student_courses)
        //        .ToListAsync();
        //    return View(courses);
        //}

        public async Task<IActionResult> Index(
            string searchTerm = "",
            string sortColumn = "Name",
            string sortDirection = "asc",
            int page = 1)
                {
                    var query = _context.courses
                        .Where(c => c.isActive)
                        .Include(c => c.course_depts)
                        .Include(c => c.Exams)
                        .Include(c => c.Topics)
                        .Include(c => c.Question_Banks)
                        .Include(c => c.Student_courses)
                        .AsQueryable();

                    // Filter by search term
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query = query.Where(c => c.crsname.Contains(searchTerm));
                    }

            // Sorting logic
            switch (sortColumn.ToLower())
            {
                case "id":
                    query = sortDirection == "asc"
                        ? query.OrderBy(c => c.crsid)
                        : query.OrderByDescending(c => c.crsid);
                    break;
                case "name":
                    query = sortDirection == "asc"
                        ? query.OrderBy(c => c.crsname)
                        : query.OrderByDescending(c => c.crsname);
                    break;
                case "hours":
                    query = sortDirection == "asc"
                        ? query.OrderBy(c => c.hours)
                        : query.OrderByDescending(c => c.hours);
                    break;
                default:
                    query = query.OrderBy(c => c.crsid); // Default
                    break;
            }

            // Pagination settings
            int pageSize = 5;
                    var totalItems = await query.CountAsync();
                    var courses = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                    var viewModel = new CourseIndexViewModel
                    {
                        Courses = courses,
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                        SearchTerm = searchTerm,
                        SortColumn = sortColumn,
                        SortDirection = sortDirection
                    };

                    return View(viewModel);
                }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.courses
                .Include(c => c.course_depts)
                .Include(c => c.Exams)
                .Include(c => c.Topics)
                .Include(c => c.Question_Banks)
                .Include(c => c.Student_courses)
                .FirstOrDefaultAsync(c => c.crsid == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(course newCourse)
        {
            if (_context.courses.Any(c => c.crsname == newCourse.crsname))
            {
                ModelState.AddModelError("crsname", "Course name already exists!");
                return View(newCourse);
            }

            _context.courses.Add(newCourse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.courses.FindAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, course updatedCourse)
        {
            if (id != updatedCourse.crsid)
                return BadRequest();

            _context.Entry(updatedCourse).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.courses.FindAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.courses.FindAsync(id);
            if (course == null)
                return NotFound();

            course.isActive = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }






        public IActionResult AssignInstructorToCourse(int? courseId)
        {
            var viewModel = new AssignInstructorToCourseViewModel
            {
                Courses = _context.courses
                            .Where(c => c.isActive)
                            .Select(c => new SelectListItem
                            {
                                Value = c.crsid.ToString(),
                                Text = c.crsname
                            }).ToList(),

                Departments = _context.Departments
                            .Where(d => d.isActive)
                            .Select(d => new SelectListItem
                            {
                                Value = d.dept_id.ToString(),
                                Text = d.name
                            }).ToList(),

                Branches = _context.Branches
                            .Where(b => b.isActive)
                            .Select(b => new SelectListItem
                            {
                                Value = b.branch_id.ToString(),
                                Text = b.name
                            }).ToList(),

                Instructors = _context.Instructors
                            .Where(i => i.isActive)
                            .Select(i => new SelectListItem
                            {
                                Value = i.insid.ToString(),
                                Text = i.ins.name
                            }).ToList()
            };

            if (courseId.HasValue)
            {
                viewModel.crsid = courseId.Value;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignInstructorToCourse(AssignInstructorToCourseViewModel model)
        {
            //if (!ModelState.IsValid)
            if (model.crsid == 0 || model.dept_id == 0 || model.branch_id == 0 || model.insid == 0)
            {
                // إعادة تعبئة القوائم المنسدلة في حالة وجود أخطاء
                model.Courses = _context.courses
                            .Where(c => c.isActive)
                            .Select(c => new SelectListItem
                            {
                                Value = c.crsid.ToString(),
                                Text = c.crsname
                            }).ToList();

                model.Departments = _context.Departments
                            .Where(d => d.isActive)
                            .Select(d => new SelectListItem
                            {
                                Value = d.dept_id.ToString(),
                                Text = d.name
                            }).ToList();

                model.Branches = _context.Branches
                            .Where(b => b.isActive)
                            .Select(b => new SelectListItem
                            {
                                Value = b.branch_id.ToString(),
                                Text = b.name
                            }).ToList();

                model.Instructors = _context.Instructors
                            .Where(i => i.isActive)
                            .Select(i => new SelectListItem
                            {
                                Value = i.insid.ToString(),
                                Text = i.ins.name
                            }).ToList();

                return View(model);
            }

            var existingAssignment = await _context.course_depts.FirstOrDefaultAsync(cd =>
                cd.crsid == model.crsid &&
                cd.dept_id == model.dept_id &&
                cd.insid == model.insid &&
                cd.branch_id == model.branch_id);

            if (existingAssignment != null)
            {
                if (!existingAssignment.isActive)
                {
                    existingAssignment.isActive = true;
                    _context.Update(existingAssignment);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Details", new { id = model.crsid });
            }
            else
            {
                var newAssignment = new course_dept
                {
                    crsid = model.crsid,
                    dept_id = model.dept_id,
                    insid = model.insid,
                    branch_id = model.branch_id,
                    isActive = true
                };

                _context.course_depts.Add(newAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = model.crsid });
            }
        }
    }

    public class AssignInstructorToCourseViewModel
    {
        public int crsid { get; set; }
        public int dept_id { get; set; }
        public int branch_id { get; set; }
        public int insid { get; set; }

        public IEnumerable<SelectListItem> Courses { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Branches { get; set; }
        public IEnumerable<SelectListItem> Instructors { get; set; }
    }
}
