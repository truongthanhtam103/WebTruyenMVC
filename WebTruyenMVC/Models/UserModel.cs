using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using WebTruyenMVC.Entity;
using Microsoft.AspNetCore.Identity.Data;
using DnsClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebTruyenMVC.Models
{
    public class UserModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "Users";

        public UserModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        public async Task<MessagesResponse> RegisterUserAsync(Register request)
        {
            var collection = mongoContext.GetCollection<UserEntity>(MongoCollection);

            // Kiểm tra xem email hoặc username đã tồn tại chưa
            var existingUser = await collection
                .Find(u => u.Email == request.Email || u.UserName == request.UserName)
                .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return new MessagesResponse
                {
                    Code = 400,
                    Message = "Người dùng đã tồn tại"
                };
            }

            var hashedPassword = request.Password; 

            var newUser = new UserEntity
            {
                Email = request.Email,
                Favorite_Stories = new List<string>(),
                Password = hashedPassword,
                Role = "user",
                UserName = request.UserName
            };

            await collection.InsertOneAsync(newUser);

            return new MessagesResponse
            {
                Code = 200,
                Message = "Đăng ký thành công",
                Data = new {newUser.UserName, newUser.Password, newUser.Email }
            };
        }

        public async Task<string> LoginAsync(Login request, IConfiguration configuration)
        {
            try
            {
                var usersCollection = mongoContext.GetCollection<UserEntity>(MongoCollection);
                var user = await usersCollection.Find(u => u.UserName == request.UserName && u.Password == request.Password).FirstOrDefaultAsync();

                if (user == null)
                {
                    return "Đăng nhập thất bại: Tên người dùng hoặc mật khẩu không chính xác.";
                }

                return "Đăng nhập thành công.";
            }
            catch (Exception ex)
            {
                logger.LogError($"Lỗi đăng nhập: {ex.Message}");
                return "Đã xảy ra lỗi trong quá trình đăng nhập.";
            }
        }

    }
}
