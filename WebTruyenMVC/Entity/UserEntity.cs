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
        public bool IsLocked { get; set; }
        public List<string> Shelf { get; set; } = new List<string>();

    }

    public class UpdateUserRequest
    {
        public string UserId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
    }

    public class AddToShelfRequest
    {
        public string UserId { get; set; }
        public string StoryId { get; set; }
    }
}
