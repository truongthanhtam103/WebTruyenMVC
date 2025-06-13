using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebTruyenMVC;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

public class AccountController : Controller
{
    private readonly MongoContext mongoContext;
    private readonly ILogger<AccountController> logger;

    public AccountController(MongoContext mongoContext, ILogger<AccountController> logger)
    {
        this.mongoContext = mongoContext;
        this.logger = logger;
    }

    // Hiển thị form cập nhật tài khoản
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        var accountModel = new AccountModel(mongoContext, logger);
        var user = await accountModel.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        return View(user);
    }

    // Xử lý cập nhật thông tin tài khoản
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(UserEntity model)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        model._id = userId;

        var accountModel = new AccountModel(mongoContext, logger);
        var result = await accountModel.UpdateUserAsync(model);

        if (result)
            ViewBag.Message = "Cập nhật thành công!";
        else
            ViewBag.Message = "Cập nhật thất bại!";

        // Lấy lại thông tin user mới nhất để truyền vào View
        var user = await accountModel.GetUserByIdAsync(userId);
        return View(user);
    }
}