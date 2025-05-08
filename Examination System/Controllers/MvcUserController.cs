using Examination_System.Models;
using Examination_System.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Examination_System.Controllers
{
    //[Authorize(Policy = "AdminOnly")]
    public class MvcUserController : Controller
    {
        private readonly Exam_sysContext _context;
        private readonly ILogger<MvcUserController> _logger;

        public MvcUserController(Exam_sysContext context, ILogger<MvcUserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Where(u => u.isActive)
                .Include(u => u.Roles.Where(r => r.isActive))
                .Include(u => u.dept)
                .Include(u => u.branch)
                .AsNoTracking()
                .ToListAsync();

            return View(users);
        }

        // GET: /User/Inactive
        public async Task<IActionResult> Inactive()
        {
            var users = await _context.Users
                .Where(u => !u.isActive)
                .Include(u => u.Roles.Where(r => r.isActive))
                .Include(u => u.dept)
                .Include(u => u.branch)
                .AsNoTracking()
                .ToListAsync();

            return View(users);
        }

        // GET: /User/Create
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments.Where(d => d.isActive).ToList();
            ViewBag.Branches = _context.Branches.Where(b => b.isActive).ToList();
            return View();
        }

        // POST: /User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDto userDto)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.email == userDto.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(userDto);
                }

                var user = new User
                {
                    name = userDto.Name,
                    email = userDto.Email,
                    phone = userDto.Phone,
                    dept_id = userDto.DeptId,
                    branch_id = userDto.BranchId,
                    isActive = true
                };

                user.SetPassword(userDto.Password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["ToastMessage"] = $"User {user.name} created successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments.Where(d => d.isActive).ToList();
            ViewBag.Branches = _context.Branches.Where(b => b.isActive).ToList();
            return View(userDto);
        }

        // GET: /User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _context.Departments.Where(d => d.isActive).ToList();
            ViewBag.Branches = _context.Branches.Where(b => b.isActive).ToList();
            return View(new UserUpdateDto
            {
                Name = user.name,
                Email = user.email,
                Phone = user.phone,
                DeptId = user.dept_id,
                BranchId = user.branch_id
            });
        }

        // POST: /User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserUpdateDto userDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.name = userDto.Name ?? user.name;
                user.phone = userDto.Phone ?? user.phone;

                if (userDto.DeptId.HasValue)
                {
                    user.dept_id = userDto.DeptId.Value;
                }

                if (userDto.BranchId.HasValue)
                {
                    user.branch_id = userDto.BranchId.Value;
                }

                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    user.SetPassword(userDto.Password);
                }

                await _context.SaveChangesAsync();
                TempData["ToastMessage"] = $"User {user.name} updated successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments.Where(d => d.isActive).ToList();
            ViewBag.Branches = _context.Branches.Where(b => b.isActive).ToList();
            return View(userDto);
        }

        // POST: /User/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.isActive = false;
            await _context.SaveChangesAsync();

            TempData["ToastMessage"] = $"User {user.name} deactivated successfully";
            return RedirectToAction(nameof(Index));
        }

        // POST: /User/Reactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.isActive = true;
            await _context.SaveChangesAsync();

            TempData["ToastMessage"] = $"User {user.name} reactivated successfully";
            return RedirectToAction(nameof(Inactive));
        }

        // GET: /User/ManageRoles/5
        public async Task<IActionResult> ManageRoles(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.dept)
                .Include(u => u.branch)
                .FirstOrDefaultAsync(u => u.id == userId);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.AllRoles = await _context.Roles
                .Where(r => r.isActive)
                .ToListAsync();

            return View(user);
        }

        // POST: /User/AssignRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.id == userId);

            var role = await _context.Roles.FindAsync(roleId);

            if (user == null || role == null)
            {
                return NotFound();
            }

            if (!user.Roles.Any(r => r.RoleId == roleId))
            {
                user.Roles.Add(role);
                await _context.SaveChangesAsync();
                TempData["ToastMessage"] = $"Role {role.RoleTitle} assigned successfully";
            }

            return RedirectToAction(nameof(ManageRoles), new { userId });
        }

        // POST: /User/RemoveRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(int userId, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.id == userId);

            var role = user?.Roles.FirstOrDefault(r => r.RoleId == roleId);

            if (user == null || role == null)
            {
                return NotFound();
            }

            user.Roles.Remove(role);
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = $"Role {role.RoleTitle} removed successfully";

            return RedirectToAction(nameof(ManageRoles), new { userId });
        }
    }
}