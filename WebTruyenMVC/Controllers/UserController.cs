using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebTruyenMVC.Models;
using WebTruyenMVC.Entity;

namespace WebTruyenMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<UserController> logger;
        private readonly IConfiguration configuration;

        public UserController(MongoContext mongoContext, ILogger<UserController> logger, IConfiguration configuration)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
            this.configuration = configuration;
        }

        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new UserModel(mongoContext, logger);
            var response = await csModel.GetAllUserAsync(request);

            return Ok(response);
        }

        // Hiển thị form cập nhật tài khoản
        [HttpGet("/User/Profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var csModel = new UserModel(mongoContext, logger);
            var response = await csModel.GetUserByIdAsync(userId);
            if (response.Code != 200 || response.Data == null)
                return NotFound();

            var user = response.Data as UserEntity;
            return View(user);
        }

        // Xử lý cập nhật thông tin tài khoản
        [HttpPost("/User/Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserEntity model)
        {
            var userId = HttpContext.Session.GetString("_id");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            model._id = userId;

            var updateRequest = new UpdateUserRequest
            {
                UserId = model._id,
                Email = model.Email,
                Avatar = model.Avatar,
                Password = string.IsNullOrWhiteSpace(model.Password) ? null : model.Password
            };

            var userModel = new UserModel(mongoContext, logger);
            var result = await userModel.UpdateUserAsync(updateRequest);

            if (result.Code == 200)
                ViewBag.Message = "Cập nhật thành công!";
            else
                ViewBag.Message = $"Cập nhật thất bại! {result.Message}";

            // Lấy lại thông tin user mới nhất để truyền vào View
            var response = await userModel.GetUserByIdAsync(userId);
            var user = response.Data as UserEntity;
            return View(user);
        }

        [HttpPut("Lock/{id}")]
        public async Task<IActionResult> Lock(string id)
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.LockUserAsync(id, currentUserRole);
            return Ok(response);
        }

        [HttpPut("Unlock/{id}")]
        public async Task<IActionResult> Unlock(string id)
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.UnlockUserAsync(id, currentUserRole);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.DeleteUserAsync(id, currentUserRole);
            return Ok(response);
        }

        [HttpPost("AddToShelf")]
        public async Task<IActionResult> AddToShelf([FromBody] AddToShelfRequest request)
        {
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.AddToShelfAsync(request);
            return Ok(response);
        }

        [HttpGet("Shelf/{userId}")]
        public async Task<IActionResult> GetShelf(string userId)
        {
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.GetShelfAsync(userId);
            return Ok(response);
        }
    }
}