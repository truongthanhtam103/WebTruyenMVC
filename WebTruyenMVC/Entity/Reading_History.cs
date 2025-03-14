using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebTruyenMVC.Entity
{
    public class Reading_History
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string StoryID { get; set; } = string.Empty;
        public int LastReadChapter { get; set; } = 0;
        public DateTime LastReadAt { get; set; }
    }
}
