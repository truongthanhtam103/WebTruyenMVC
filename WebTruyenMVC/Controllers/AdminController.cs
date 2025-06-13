using Microsoft.AspNetCore.Mvc;

namespace WebTruyenMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Account()
        {
            return View();
        }
    }
}