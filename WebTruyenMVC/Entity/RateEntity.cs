using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebTruyenMVC.Entity
{
    public class RateEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string StoryID { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public DateTime Created { get; set; }
    }
}
