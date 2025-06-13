using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MongoDB.Driver;
using System.Diagnostics;
using System.Net.Mail;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

namespace WebTruyenMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMongoCollection<UserEntity> _users;

        public AuthController(ILogger<AuthController> logger, MongoContext context)
        {
            _logger = logger;
            _users = context.GetCollection<UserEntity>("Users");
        }

        public IActionResult Login() => View();
        public IActionResult Register() => View();

        /// <summary>
        /// Đăng ký tài khoản mới với xác thực email.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register(UserEntity model)
        {
            // Kiểm tra UserName đã tồn tại chưa
            var userNameExists = await _users.Find(u => u.UserName == model.UserName).AnyAsync();
            if (userNameExists)
            {
                ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
                return View(model);
            }

            // Đếm số tài khoản đã đăng ký với email này
            var emailCount = await _users.CountDocumentsAsync(u => u.Email == model.Email);
            if (emailCount >= 3)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng tối đa 3 lần để đăng ký.");
                return View(model);
            }

            // Sinh mã xác thực và lưu vào session
            var verificationCode = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("RegisterUserName", model.UserName);
            HttpContext.Session.SetString("RegisterEmail", model.Email);
            HttpContext.Session.SetString("RegisterPassword", model.Password);
            HttpContext.Session.SetString("RegisterVerificationCode", verificationCode);
            HttpContext.Session.SetString("RegisterVerificationExpiry", DateTime.UtcNow.AddMinutes(10).ToString());

            // Gửi email xác thực
            SendVerificationEmail(model.Email, verificationCode);

            TempData["Success"] = "Vui lòng kiểm tra email để lấy mã xác nhận.";
            return RedirectToAction("VerifyEmailRegister");
        }

        public IActionResult VerifyEmailRegister()
        {
            return View();
        }

        /// <summary>
        /// Xác thực mã đăng ký tài khoản.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> VerifyEmailRegister(string code)
        {
            var sessionCode = HttpContext.Session.GetString("RegisterVerificationCode");
            var expiryStr = HttpContext.Session.GetString("RegisterVerificationExpiry");
            if (sessionCode == null || expiryStr == null)
            {
                ModelState.AddModelError("", "Phiên đăng ký đã hết hạn. Vui lòng đăng ký lại.");
                return View();
            }

            if (sessionCode != code)
            {
                ModelState.AddModelError("", "Mã xác nhận không đúng.");
                return View();
            }

            if (DateTime.UtcNow > DateTime.Parse(expiryStr))
            {
                ModelState.AddModelError("", "Mã xác nhận đã hết hạn.");
                return View();
            }

            // Lấy thông tin từ session và tạo tài khoản
            var user = new UserEntity
            {
                UserName = HttpContext.Session.GetString("RegisterUserName"),
                Email = HttpContext.Session.GetString("RegisterEmail"),
                Password = HttpContext.Session.GetString("RegisterPassword"),
                //IsVerified = true,
                Role = "User",
                Avatar = "default.png"
            };

            await _users.InsertOneAsync(user);

            // Xóa session
            HttpContext.Session.Remove("RegisterUserName");
            HttpContext.Session.Remove("RegisterEmail");
            HttpContext.Session.Remove("RegisterPassword");
            HttpContext.Session.Remove("RegisterVerificationCode");
            HttpContext.Session.Remove("RegisterVerificationExpiry");

            TempData["Success"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Đăng nhập tài khoản.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var user = await _users.Find(u => u.UserName == userName).FirstOrDefaultAsync();
            if (user == null || !password.Equals(user.Password))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View();
            }

            // Kiểm tra tài khoản bị khóa
            if (user.IsLocked)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên để biết thêm chi tiết.");
                return View();
            }

            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserId", user._id);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Gửi email xác thực đăng ký tài khoản.
        /// </summary>
        private void SendVerificationEmail(string toEmail, string code)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("WebTruyenMVC", "truongthanhtam8a@gmail.com"));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "Xác nhận đăng ký tài khoản";
                message.Body = new TextPart("plain")
                {
                    Text = $"Mã xác nhận của bạn là: {code}"
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate("truongthanhtam8a@gmail.com", "zitp gtao isxe pztp"); // Mật khẩu ứng dụng
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("mail_error.txt", ex.ToString());
                throw new Exception("Không thể gửi email xác thực. Vui lòng kiểm tra lại cấu hình email.", ex);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
