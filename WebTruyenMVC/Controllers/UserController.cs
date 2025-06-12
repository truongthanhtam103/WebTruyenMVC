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

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.UpdateUserAsync(request);
            return Ok(response);
        }

        [HttpPut("Lock/{id}")]
        public async Task<IActionResult> LockUser(string id)
        {
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.LockUserAsync(id);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userModel = new UserModel(mongoContext, logger);
            var response = await userModel.DeleteUserAsync(id);
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