using System.Security.Claims;
using Examination_System.DTOS;
using Examination_System.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly Exam_sysContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            Exam_sysContext context,
            ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Views
        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            PopulateDropdowns();
            return View();
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private void PopulateDropdowns()
        {
            ViewBag.Departments = _context.Departments
                .Where(d => d.isActive)
                .Select(d => new SelectListItem
                {
                    Value = d.dept_id.ToString(),
                    Text = d.name
                }).ToList();

            ViewBag.Branches = _context.Branches
                .Where(b => b.isActive)
                .Select(b => new SelectListItem
                {
                    Value = b.branch_id.ToString(),
                    Text = b.name
                }).ToList();
        }
        #endregion

        #region API Endpoints
        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.email == loginDto.Email && u.isActive);

            if (user == null || !user.VerifyPassword(loginDto.Password))
            {
                _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
                ModelState.AddModelError("", "Invalid credentials");
                return View(loginDto);
            }

            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            //    new Claim(ClaimTypes.Email, user.email),
            //    new Claim(ClaimTypes.Name, user.name),
            //    new Claim("DeptId", user.dept_id.ToString()),
            //    new Claim("BranchId", user.branch_id.ToString())
            //};

            //foreach (var role in user.Roles.Where(r => r.isActive))
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role.RoleTitle));
            //}
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
    new Claim(ClaimTypes.Email, user.email),
    new Claim(ClaimTypes.Name, user.name),
    new Claim("DeptId", user.dept_id.ToString()),
    new Claim("BranchId", user.branch_id.ToString())
};

            if (user.Roles.Any(r => r.RoleTitle == "Student" && r.isActive))
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.stdid == user.id);
                if (student != null)
                {
                    claims.Add(new Claim("StudentId", student.stdid.ToString()));
                }
            }

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = loginDto.RememberMe,
                ExpiresUtc = loginDto.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2),
                AllowRefresh = true
            };
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                authProperties);

            _logger.LogInformation("User {Email} logged in", user.email);

            //return LocalRedirect(returnUrl ?? Url.Content("~/"));
            return RedirectToAction("Profile", "Base");
        }

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            PopulateDropdowns();

            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            if (registerDto.Password != registerDto.CPassword)
            {
                ModelState.AddModelError("CPassword", "Passwords do not match");
                return View(registerDto);
            }

            try
            {
                if (await _context.Users.AnyAsync(u => u.email == registerDto.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(registerDto);
                }

                var departmentExists = await _context.Departments
                    .AnyAsync(d => d.dept_id == registerDto.DeptId && d.isActive);
                var branchExists = await _context.Branches
                    .AnyAsync(b => b.branch_id == registerDto.BranchId && b.isActive);

                if (!departmentExists || !branchExists)
                {
                    ModelState.AddModelError("", "Invalid department or branch selection");
                    return View(registerDto);
                }

                var newUser = new User
                {
                    name = registerDto.Name,
                    email = registerDto.Email,
                    phone = registerDto.Phone,
                    st_city = registerDto.StCity, 
                    dept_id = registerDto.DeptId,
                    branch_id = registerDto.BranchId,
                    isActive = false 
                };

                newUser.SetPassword(registerDto.Password);
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var studentRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleTitle == "Student" && r.isActive)
                    ?? throw new Exception("Student role not found or inactive");

                newUser.Roles.Add(studentRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New user registered: {Email}", newUser.email);

                // Auto-login after registration
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, newUser.id.ToString()),
                        new Claim(ClaimTypes.Email, newUser.email),
                        new Claim(ClaimTypes.Name, newUser.name),
                        new Claim(ClaimTypes.Role, "Student"),
                        new Claim("DeptId", newUser.dept_id.ToString()),
                        new Claim("BranchId", newUser.branch_id.ToString()),
                        //new Claim("StudentId",newUser.Student.stdid.ToString()),
                    }, CookieAuthenticationDefaults.AuthenticationScheme)));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", registerDto.Email);
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View(registerDto);
            }
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");
            return RedirectToAction("Login", "Auth");
        }
        #endregion

        #region Helpers
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.email == email);
            return Json(!emailExists);
        }
        #endregion
    }
}