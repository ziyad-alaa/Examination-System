using System.ComponentModel.DataAnnotations;
using Examination_System.DTOS;
using Examination_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "AdminOnly")]
    public class UserController : ControllerBase
    {
        private readonly Exam_sysContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(Exam_sysContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users
                .Where(u => u.isActive)
                .Include(u => u.Roles.Where(r => r.isActive))
                .Include(u => u.dept)
                .Include(u => u.branch)
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/User/inactive
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<User>>> GetInactiveUsers()
        {
            return await _context.Users
                .Where(u => !u.isActive)
                .Include(u => u.Roles)
                .Include(u => u.dept)
                .Include(u => u.branch)
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Roles.Where(r => r.isActive))
                .Include(u => u.dept)
                .Include(u => u.branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.email == userDto.Email))
            {
                return Conflict("Email already exists");
            }

            var departmentExists = await _context.Departments
                .AnyAsync(d => d.dept_id == userDto.DeptId && d.isActive);
            var branchExists = await _context.Branches
                .AnyAsync(b => b.branch_id == userDto.BranchId && b.isActive);

            if (!departmentExists || !branchExists)
            {
                return BadRequest("Invalid department or branch selection");
            }

            try
            {
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

                _logger.LogInformation("User {Email} created by {CurrentUser}",
                    user.email, User.Identity?.Name);

                return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "An error occurred while creating the user");
            }
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.name = userDto.Name ?? user.name;
                user.phone = userDto.Phone ?? user.phone;

                if (userDto.DeptId.HasValue)
                {
                    var departmentExists = await _context.Departments
                        .AnyAsync(d => d.dept_id == userDto.DeptId && d.isActive);
                    if (!departmentExists)
                    {
                        return BadRequest("Invalid department selection");
                    }
                    user.dept_id = userDto.DeptId.Value;
                }

                if (userDto.BranchId.HasValue)
                {
                    var branchExists = await _context.Branches
                        .AnyAsync(b => b.branch_id == userDto.BranchId && b.isActive);
                    if (!branchExists)
                    {
                        return BadRequest("Invalid branch selection");
                    }
                    user.branch_id = userDto.BranchId.Value;
                }

                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    user.SetPassword(userDto.Password);
                }

                if (userDto.isActive.HasValue)
                {
                    user.isActive = userDto.isActive.Value;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("User {Id} updated by {CurrentUser}",
                    id, User.Identity?.Name);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", id);
                return StatusCode(500, "An error occurred while updating the user");
            }
        }

        // PATCH: api/User/5/deactivate
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.isActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {Id} deactivated by {CurrentUser}",
                id, User.Identity?.Name);

            return NoContent();
        }

        // PATCH: api/User/5/reactivate
        [HttpPatch("{id}/reactivate")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.isActive = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {Id} reactivated by {CurrentUser}",
                id, User.Identity?.Name);

            return NoContent();
        }

        // POST: api/User/5/assign-role
        [HttpPost("{userId}/assign-role")]
        public async Task<IActionResult> AssignRole(int userId, [FromBody] int roleId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.id == userId);

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == roleId && r.isActive);

            if (user == null || role == null)
            {
                return NotFound();
            }

            if (user.Roles.Any(r => r.RoleId == roleId))
            {
                return Conflict("User already has this role");
            }

            user.Roles.Add(role);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Role {RoleId} assigned to user {UserId} by {CurrentUser}",
                roleId, userId, User.Identity?.Name);

            return Ok();
        }

        // DELETE: api/User/5/remove-role/2
        [HttpDelete("{userId}/remove-role/{roleId}")]
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

            _logger.LogInformation("Role {RoleId} removed from user {UserId} by {CurrentUser}",
                roleId, userId, User.Identity?.Name);

            return NoContent();
        }

        // PATCH: api/User/5/reset-password
        [HttpPatch("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.SetPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password reset for user {Id} by {CurrentUser}",
                id, User.Identity?.Name);

            return NoContent();
        }
    }

    public class ResetPasswordDto
    {
        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}