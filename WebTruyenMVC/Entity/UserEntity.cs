using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebTruyenMVC.Entity
{
    public class UserEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Favorite_Stories { get; set; } = new List<string>();
    }
}
