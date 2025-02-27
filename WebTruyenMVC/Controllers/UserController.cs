using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebTruyenMVC.Models;
using WebTruyenMVC.Entity;
using DnsClient;

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

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Register request)
        {
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.RegisterUserAsync(request);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var userModel = new UserModel(mongoContext, logger);
            var token = await userModel.LoginAsync(request, configuration);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new { token });
        }
    }
}
