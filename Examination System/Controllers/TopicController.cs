using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Examination_System.Models;
using System.Linq;
using System.Threading.Tasks;
using Examination_System.Models.View_Models.TopicViewModel;

namespace Examination_System.Controllers
{
    public class TopicController : Controller
    {
        private readonly Exam_sysContext _context;

        public TopicController(Exam_sysContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
    string searchTerm = "",
    string sortColumn = "Title",
    string sortDirection = "asc",
    int page = 1)
        {
            var query = _context.Topics
                .Where(t => t.isActive && t.crs != null)
                .Include(t => t.crs)
                .AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t =>
                    t.title.Contains(searchTerm) ||
                    t.crs.crsname.Contains(searchTerm));
            }

            // Sorting logic
            switch (sortColumn.ToLower())
            {
                case "title":
                    query = sortDirection == "asc"
                        ? query.OrderBy(t => t.title)
                        : query.OrderByDescending(t => t.title);
                    break;

                case "course":
                    query = sortDirection == "asc"
                        ? query.OrderBy(t => t.crs.crsname)
                        : query.OrderByDescending(t => t.crs.crsname);
                    break;

                case "status":
                    query = sortDirection == "asc"
                        ? query.OrderBy(t => t.isActive)
                        : query.OrderByDescending(t => t.isActive);
                    break;

                default:
                    query = query.OrderBy(t => t.title);
                    break;
            }

            // Pagination settings
            int pageSize = 10;
            var totalItems = await query.CountAsync();
            var topics = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var viewModel = new TopicIndexViewModel
            {
                Topics = topics,
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

            var topic = await _context.Topics
                .Include(t => t.crs)
                .FirstOrDefaultAsync(t => t.topicid == id);
            if (topic == null)
                return NotFound();

            return View(topic);
        }

        public IActionResult Create()
        {
            ViewBag.CourseList = _context.courses
                .Where(c => c.isActive)
                .AsEnumerable()
                .Select(c => new SelectListItem
                {
                    Value = c.crsid.ToString(),
                    Text = c.crsname
                })
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Topic topic)
        {
            if (ModelState.IsValid)
            {
                topic.isActive = true;
                _context.Add(topic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CourseList = _context.courses
                .Where(c => c.isActive)
                .AsEnumerable()
                .Select(c => new SelectListItem
                {
                    Value = c.crsid.ToString(),
                    Text = c.crsname
                })
                .ToList();
            return View(topic);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
                return NotFound();

            ViewBag.CourseList = _context.courses
                .Where(c => c.isActive)
                .AsEnumerable() 
                .Select(c => new SelectListItem
                {
                    Value = c.crsid.ToString(),
                    Text = c.crsname
                })
                .ToList();

            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Topic topic)
        {
            if (id != topic.topicid)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    topic.isActive = true;
                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicExists(topic.topicid))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CourseList = _context.courses
                .Where(c => c.isActive)
                .AsEnumerable()
                .Select(c => new SelectListItem
                {
                    Value = c.crsid.ToString(),
                    Text = c.crsname
                })
                .ToList();
            return View(topic);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var topic = await _context.Topics
                .FirstOrDefaultAsync(t => t.topicid == id);
            if (topic == null)
                return NotFound();

            return View(topic);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                topic.isActive = false;
                _context.Topics.Update(topic);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(t => t.topicid == id);
        }
    }
}
