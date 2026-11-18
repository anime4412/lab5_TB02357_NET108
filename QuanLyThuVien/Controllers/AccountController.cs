using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien.DAL;
using QuanLyThuVien.Models;
using QuanLyThuVien.Utils;

namespace QuanLyThuVien.Controllers
{
    /// <summary>
    /// Controller Đăng nhập, Đăng xuất và BÀI 2: Cấp lại mật khẩu ngẫu nhiên qua Email
    /// </summary>
    public class AccountController : Controller
    {
        private readonly EmployeeDAL _employeeDal;
        private readonly EmailService _emailService;

        public AccountController(EmployeeDAL employeeDal, EmailService emailService)
        {
            _employeeDal = employeeDal;
            _emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            model.ReturnUrl = returnUrl ?? model.ReturnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _employeeDal.Authenticate(model.Email, model.Password);

            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("EmployeeCode", user.EmployeeCode ?? string.Empty)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddHours(24) : null
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return LocalRedirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email hoặc Mật khẩu không chính xác hoặc tài khoản đã bị khóa.");
            return View(model);
        }

        // ==============================================================================
        // BÀI 2 (2 ĐIỂM): CHỨC NĂNG CẤP LẠI MẬT KHẨU NGẪU NHIÊN CHO NHÂN VIÊN
        // ==============================================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Đặt lại mật khẩu ngẫu nhiên trong CSDL SQL Server
            var (employee, newPassword) = _employeeDal.ResetRandomPassword(model.Email);

            if (employee == null || string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("", "Không tìm thấy tài khoản nhân viên đang hoạt động với Email này.");
                return View(model);
            }

            // 2. Gửi mật khẩu vừa tạo tới Email nhân viên
            var (sentViaSmtp, statusMessage) = await _emailService.SendResetPasswordEmailAsync(
                employee.Email,
                employee.FullName,
                newPassword
            );

            ViewBag.Success = true;
            ViewBag.StatusMessage = statusMessage;
            ViewBag.ResetEmail = employee.Email;
            // Hiển thị mật khẩu demo khi chạy kiểm thử offline không có SMTP server
            if (!sentViaSmtp)
            {
                ViewBag.DemoNewPassword = newPassword;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
