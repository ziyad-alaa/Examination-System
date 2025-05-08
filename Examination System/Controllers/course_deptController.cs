using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Examination_System.Models;
using System.Linq;
using System.Threading.Tasks;
using Examination_System.Models.View_Models.CourseDeptViewModel;

namespace Examination_System.Controllers
{
    public class CourseDeptController : Controller
    {
        private readonly Exam_sysContext _context;

        public CourseDeptController(Exam_sysContext context)
        {
            _context = context;
        }

        // GET: CourseDept/Index
        public async Task<IActionResult> Index(
            string searchTerm = "",
            string sortColumn = "Course",
            string sortDirection = "asc",
            int page = 1)
                {
                    var query = _context.course_depts
                        .Where(cd => cd.isActive)
                        .Include(cd => cd.crs)
                        .Include(cd => cd.dept)
                        .Include(cd => cd.branch)
                        .Include(cd => cd.ins).ThenInclude(i => i.ins)
                        .AsQueryable();

                    // Filter by search term
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query = query.Where(cd =>
                            cd.crs.crsname.Contains(searchTerm) ||
                            cd.dept.name.Contains(searchTerm) ||
                            cd.branch.name.Contains(searchTerm) ||
                            cd.ins.ins.name.Contains(searchTerm));
                    }

                    // Sorting logic
                    switch (sortColumn.ToLower())
                    {
                        case "course":
                            query = sortDirection == "asc"
                                ? query.OrderBy(cd => cd.crs.crsname)
                                : query.OrderByDescending(cd => cd.crs.crsname);
                            break;

                        case "department":
                            query = sortDirection == "asc"
                                ? query.OrderBy(cd => cd.dept.name)
                                : query.OrderByDescending(cd => cd.dept.name);
                            break;

                        case "branch":
                            query = sortDirection == "asc"
                                ? query.OrderBy(cd => cd.branch.name)
                                : query.OrderByDescending(cd => cd.branch.name);
                            break;

                        case "instructor":
                            query = sortDirection == "asc"
                                ? query.OrderBy(cd => cd.ins.ins.name)
                                : query.OrderByDescending(cd => cd.ins.ins.name);
                            break;

                        default:
                            query = query.OrderBy(cd => cd.crs.crsname);
                            break;
                    }

                    // Pagination settings
                    int pageSize = 10;
                    var totalItems = await query.CountAsync();
                    var assignments = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                    var viewModel = new CourseDeptIndexViewModel
                    {
                        Assignments = assignments,
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                        SearchTerm = searchTerm,
                        SortColumn = sortColumn,
                        SortDirection = sortDirection
                    };

                    return View(viewModel);
                }

    }
}
