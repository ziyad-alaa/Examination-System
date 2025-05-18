using Examination_System.DTOS;
using Examination_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Examination_System.Controllers
{
    //[Authorize(Roles = "Admin,BranchManager")]
    public class PermissionController : Controller
    {
        private readonly Exam_sysContext _context;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(
            Exam_sysContext context,
            ILogger<PermissionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int? GetCurrentUserBranchId()
        {
            var branchIdClaim = User.FindFirst("BranchId")?.Value;
            return branchIdClaim != null ? int.Parse(branchIdClaim) : null;
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageBranchManagers()
        {
            var model = await _context.Branches
                .Include(b => b.Manager)
                .ThenInclude(m => m.ins)
                .Where(b => b.isActive)
                .ToListAsync();

            ViewBag.Instructors = await _context.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive)
                .ToListAsync();

            return View(model);
        }

        //[Authorize(Roles = "Admin,BranchManager")]
        public async Task<IActionResult> ManageDepartmentManagers()
        {
            var currentBranchId = GetCurrentUserBranchId();

            var query = _context.branch_depts
                .Include(bd => bd.Department)
                .Include(bd => bd.Branch)
                .Include(bd => bd.Manager)
                .ThenInclude(m => m.ins)
                .Where(bd => bd.isActive);

            if (currentBranchId.HasValue)
            {
                query = query.Where(bd => bd.branch_id == currentBranchId.Value);
            }

            var model = await query.ToListAsync();

            ViewBag.Branches = await GetAvailableBranches();
            ViewBag.Departments = await GetAvailableDepartments();
            ViewBag.Instructors = await GetAvailableInstructors(currentBranchId);

            return View(model);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignBranchManager(PermissionAssignmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.branch_id == dto.BranchId && b.isActive);

            var instructor = await _context.Instructors
                .Include(i => i.ins)
                .FirstOrDefaultAsync(i => i.insid == dto.InstructorId && i.isActive);

            if (branch == null || instructor == null)
                return NotFound("Branch or Instructor not found");

            branch.ManagerId = dto.InstructorId;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Branch {branch.name} manager set to {instructor.ins.name}");
                return RedirectToAction(nameof(ManageBranchManagers));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error assigning branch manager");
                return StatusCode(500, "Error saving changes");
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,BranchManager")]
        public async Task<IActionResult> AssignDepartmentManager(PermissionAssignmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            var currentBranchId = GetCurrentUserBranchId();
            if (currentBranchId.HasValue && dto.BranchId != currentBranchId.Value)
                return Forbid();

            var branchDept = await _context.branch_depts
                .FirstOrDefaultAsync(bd =>
                    bd.branch_id == dto.BranchId &&
                    bd.dept_id == dto.DepartmentId &&
                    bd.isActive);

            var instructor = await _context.Instructors
                .Include(i => i.ins)
                .FirstOrDefaultAsync(i => i.insid == dto.InstructorId && i.isActive);

            if (branchDept == null || instructor == null)
                return NotFound("Branch/Department association or Instructor not found");

            branchDept.ManagerId = dto.InstructorId;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Department {branchDept.Department.name} manager set to {instructor.ins.name}");
                return RedirectToAction(nameof(ManageDepartmentManagers));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error assigning department manager");
                return StatusCode(500, "Error saving changes");
            }
        }

        private async Task<List<Branch>> GetAvailableBranches()
        {
            var currentBranchId = GetCurrentUserBranchId();

            var query = _context.Branches
                .Where(b => b.isActive);

            if (currentBranchId.HasValue)
            {
                query = query.Where(b => b.branch_id == currentBranchId.Value);
            }

            return await query.ToListAsync();
        }

        private async Task<List<Department>> GetAvailableDepartments()
        {
            var currentBranchId = GetCurrentUserBranchId();

            if (currentBranchId.HasValue)
            {
                return await _context.branch_depts
                    .Where(bd => bd.branch_id == currentBranchId.Value && bd.isActive)
                    .Select(bd => bd.Department)
                    .Distinct()
                    .ToListAsync();
            }

            return await _context.Departments
                .Where(d => d.isActive)
                .ToListAsync();
        }

        private async Task<List<Instructor>> GetAvailableInstructors(int? branchId)
        {
            var query = _context.Instructors
                .Include(i => i.ins)
                .Where(i => i.isActive);

            if (branchId.HasValue)
            {
                query = query.Where(i => i.ins.branch_id == branchId.Value);
            }

            return await query.ToListAsync();
        }
    }
}