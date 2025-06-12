using WebTruyenMVC.Entity;
using MongoDB.Driver;

namespace WebTruyenMVC.Seeder
{
    public class MongoSeeder
    {
        public static void Seed(MongoContext context)
        {
            var users = context.GetCollection<UserEntity>("Users");

            if (users.CountDocuments(Builders<UserEntity>.Filter.Empty) == 0)
            {
                var defaultUser = new UserEntity
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    Password = "123456", 
                    Role = "Admin",
                    Avatar = "default.png"
                };

                users.InsertOne(defaultUser);
                Console.WriteLine("✅ Đã tạo user mặc định.");
            }
        }
    }
}
