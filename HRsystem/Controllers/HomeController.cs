using System.Diagnostics;
using System.Security.Claims;
using HRsystem.Data;
using HRsystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRsystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Home/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

            if (user == null || !PasswordHasher.VerifyPassword(model.Password, user.Password))
            {
                ModelState.AddModelError("", "اسم المستخدم أو كلمة المرور غير صحيحة");
                return View("Index", model);
            }

            // Validate selected role matches the user's actual role
            if (!string.Equals(user.Role, model.SelectedRole, StringComparison.OrdinalIgnoreCase))
            {
                var roleDisplay = model.SelectedRole switch
                {
                    "Admin" => "المشرف",
                    "HR" => "الموارد البشرية",
                    "Employee" => "الموظف",
                    _ => model.SelectedRole
                };
                ModelState.AddModelError("", $"هذا الحساب ليس حساب {roleDisplay}. يرجى اختيار الدور الصحيح");
                return View("Index", model);
            }

            if (user.Role == "Employee")
            {
                var hrEmployee = _context.HREmployees.FirstOrDefault(e => e.NationalId == user.Username);
                if(hrEmployee == null)
                {
                    ModelState.AddModelError("", "الموظف غير موجود في قاعدة البيانات");
                    return View("Index", model);
                }
                var claimsE = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, hrEmployee.Name),
                    new(ClaimTypes.Role, user.Role),
                    new("NationalId", user.Username)
                };
                var identityE = new ClaimsIdentity(
                    claimsE, CookieAuthenticationDefaults.AuthenticationScheme);

                var principalE = new ClaimsPrincipal(identityE);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principalE);
                if (user.IsActive == false)
                {
                    return RedirectToAction("ChangePassword", "EmployeeDashboard");
                }
                return RedirectToAction("Dashboard", "EmployeeDashboard");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
            return RedirectToAction("ListEmployees", "Employee");
        }

        [HttpGet]
        [Route("/access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }


    }
}
