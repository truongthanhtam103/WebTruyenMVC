using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebTruyenMVC.Models;
using WebTruyenMVC.Entity;
using DnsClient;
using MongoDB.Driver;

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
            var users = mongoContext.GetCollection<UserEntity>("Users");
            var filter = Builders<UserEntity>.Filter.Eq(u => u._id, request.UserId);
            var update = Builders<UserEntity>.Update.Combine(
                request.Email != null ? Builders<UserEntity>.Update.Set(u => u.Email, request.Email) : null,
                request.Password != null ? Builders<UserEntity>.Update.Set(u => u.Password, request.Password) : null,
                request.Avatar != null ? Builders<UserEntity>.Update.Set(u => u.Avatar, request.Avatar) : null
            );

            if (update == null)
                return BadRequest("No fields to update.");

            var result = await users.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
                return NotFound("User not found or no changes made.");

            return Ok("User updated successfully.");
        }

        [HttpPut("Lock/{id}")]
        public async Task<IActionResult> LockUser(string id)
        {
            var users = mongoContext.GetCollection<UserEntity>("Users");
            var user = await users.Find(u => u._id == id).FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found.");

            if (user.Role != "admin")
                return Forbid("Only admin can lock accounts.");

            var update = Builders<UserEntity>.Update.Set(u => u.IsLocked, true);
            await users.UpdateOneAsync(u => u._id == id, update);

            return Ok("User account locked successfully.");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var users = mongoContext.GetCollection<UserEntity>("Users");
            var user = await users.Find(u => u._id == id).FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found.");

            if (user.Role != "admin")
                return Forbid("Only admin can delete accounts.");

            await users.DeleteOneAsync(u => u._id == id);

            return Ok("User account deleted successfully.");
        }

        [HttpPost("AddToShelf")]
        public async Task<IActionResult> AddToShelf([FromBody] AddToShelfRequest request)
        {
            var users = mongoContext.GetCollection<UserEntity>("Users");
            var filter = Builders<UserEntity>.Filter.Eq(u => u._id, request.UserId);
            var update = Builders<UserEntity>.Update.AddToSet(u => u.Shelf, request.StoryId);

            var result = await users.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
                return NotFound("User not found or story already in shelf.");

            return Ok("Story added to shelf.");
        }

        [HttpGet("Shelf/{userId}")]
        public async Task<IActionResult> GetShelf(string userId)
        {
            var users = mongoContext.GetCollection<UserEntity>("Users");
            var user = await users.Find(u => u._id == userId).FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found.");

            // Nếu muốn trả về chi tiết truyện, cần truy vấn thêm từ collection truyện
            return Ok(user.Shelf); // Trả về danh sách Id truyện
        }
    }
}
