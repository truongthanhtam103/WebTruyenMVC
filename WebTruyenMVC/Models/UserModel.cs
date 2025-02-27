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

            // Mã hóa mật khẩu (nên dùng thư viện BCrypt, ví dụ: BCrypt.Net.BCrypt.HashPassword)
            // Ở đây dùng trực tiếp mật khẩu gốc cho ví dụ, tuy nhiên bạn nên thay bằng mật khẩu đã được mã hóa.
            var hashedPassword = request.Password; // Thay thế bằng hàm mã hóa thực tế

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
                    return null; // Đăng nhập thất bại
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.Role), // Gán role vào token
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = configuration["Jwt:Issuer"],
                    Audience = configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError($"Login error: {ex.Message}");
                return null;
            }
        }
    }
}
