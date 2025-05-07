using System.ComponentModel.DataAnnotations;
using Examination_System.DTOS;
using Examination_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Controllers
{
    [Route("[controller]")]
    //[Authorize(Policy = "AdminOnly")]
    public class RoleController : Controller
    {
        private readonly Exam_sysContext _context;
        private readonly ILogger<RoleController> _logger;

        public RoleController(Exam_sysContext context, ILogger<RoleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Views
        [HttpGet]
        public async Task<IActionResult> RoleIndex()
        {
            var roles = await _context.Roles
                .Where(r => r.isActive)
                .Include(r => r.Pers.Where(p => p.isActive))
                .Include(r => r.Users.Where(u => u.isActive))
                .ToListAsync();

            // Get user counts through the Users navigation property
            ViewBag.RoleUserCounts = roles.ToDictionary(
                r => r.RoleId,
                r => r.Users.Count
            );

            return View(roles);
        }

        [HttpGet("Inactive")]
        public async Task<IActionResult> Inactive()
        {
            var roles = await _context.Roles
                .Where(r => !r.isActive)
                .Include(r => r.Pers)
                .ToListAsync();

            return View(roles);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Pers.Where(p => p.isActive))
                .Include(r => r.Users.Where(u => u.isActive))
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpGet("ManagePermissions/{id}")]
        public async Task<IActionResult> ManagePermissions(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Pers.Where(p => p.isActive))
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            var assignedPermissionIds = role.Pers.Select(p => p.PeriD).ToList();
            var availablePermissions = await _context.Permissions
                .Where(p => !assignedPermissionIds.Contains(p.PeriD) && p.isActive)
                .ToListAsync();

            ViewBag.AvailablePermissions = availablePermissions;
            return View(role);
        }

        [HttpGet("RoleUsers/{id}")]
        public async Task<IActionResult> RoleUsers(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Users) // Load all users for the role
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            // Pass the role ID or title to the view for display
            ViewData["RoleId"] = role.RoleId;
            ViewData["RoleTitle"] = role.RoleTitle;

            // Pass the list of users to the view
            return View(role.Users);
        }
        #endregion

        #region API Endpoints
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return View(roleDto);
            }

            if (await _context.Roles.AnyAsync(r => r.RoleTitle == roleDto.RoleTitle))
            {
                ModelState.AddModelError("RoleTitle", "Role with this title already exists");
                return View(roleDto);
            }

            try
            {
                var role = new Role
                {
                    RoleTitle = roleDto.RoleTitle,
                    isActive = true
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role {RoleTitle} created by {CurrentUser}",
                    role.RoleTitle, User.Identity?.Name);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                ModelState.AddModelError("", "An error occurred while creating the role.");
                return View(roleDto);
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return View(roleDto);
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            if (await _context.Roles.AnyAsync(r => r.RoleId != id && r.RoleTitle == roleDto.RoleTitle))
            {
                ModelState.AddModelError("RoleTitle", "Another role with this title already exists");
                return View(roleDto);
            }

            try
            {
                role.RoleTitle = roleDto.RoleTitle;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role {Id} updated by {CurrentUser}",
                    id, User.Identity?.Name);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the role.");
                return View(roleDto);
            }
        }

        [HttpPost("Deactivate/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.Users.Any())
            {
                TempData["ErrorMessage"] = "Cannot deactivate role that has assigned users";
                return RedirectToAction(nameof(Index));
            }

            role.isActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Role {Id} deactivated by {CurrentUser}",
                id, User.Identity?.Name);

            TempData["SuccessMessage"] = "Role deactivated successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Reactivate/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            role.isActive = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Role {Id} reactivated by {CurrentUser}",
                id, User.Identity?.Name);

            TempData["SuccessMessage"] = "Role reactivated successfully";
            return RedirectToAction(nameof(Inactive));
        }

        [HttpPost("AssignPermission/{roleId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPermission(int roleId, [FromForm] int permissionId)
        {
            var role = await _context.Roles
                .Include(r => r.Pers)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.PeriD == permissionId && p.isActive);

            if (role == null || permission == null)
            {
                return NotFound();
            }

            if (role.Pers.Any(p => p.PeriD == permissionId))
            {
                TempData["ErrorMessage"] = "Role already has this permission";
                return RedirectToAction(nameof(ManagePermissions), new { id = roleId });
            }

            role.Pers.Add(permission);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Permission {PermissionId} assigned to role {RoleId} by {CurrentUser}",
                permissionId, roleId, User.Identity?.Name);

            TempData["SuccessMessage"] = "Permission assigned successfully";
            return RedirectToAction(nameof(ManagePermissions), new { id = roleId });
        }

        [HttpPost("RemovePermission/{roleId}/{permissionId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePermission(int roleId, int permissionId)
        {
            var role = await _context.Roles
                .Include(r => r.Pers)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            var permission = role?.Pers.FirstOrDefault(p => p.PeriD == permissionId);

            if (role == null || permission == null)
            {
                return NotFound();
            }

            role.Pers.Remove(permission);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Permission {PermissionId} removed from role {RoleId} by {CurrentUser}",
                permissionId, roleId, User.Identity?.Name);

            TempData["SuccessMessage"] = "Permission removed successfully";
            return RedirectToAction(nameof(ManagePermissions), new { id = roleId });
        }
        #endregion

        #region Helpers
        //[AcceptVerbs("GET", "POST")]
        //public async Task<IActionResult> VerifyRoleTitle(string roleTitle, int roleId = 0)
        //{
        //    var exists = await _context.Roles.AnyAsync(r =>
        //        r.RoleTitle == roleTitle &&
        //        r.RoleId != roleId);

        //    return Json(!exists);
        //}
        #endregion
    }

    public class RoleDto
    {
        [Required]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Role title must be between 3 and 255 characters")]
        [Remote(action: "VerifyRoleTitle", controller: "Role", AdditionalFields = "roleId",
            ErrorMessage = "Role title already exists")]
        public string RoleTitle { get; set; }

        public int roleId { get; set; }
    }
}