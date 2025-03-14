using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebTruyenMVC.Entity
{
    public class ChapterEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string StoryID { get; set; } = string.Empty;
        public int ChapterNumber { get; set; } = 0;
        public DateTime Created { get; set; }
    }
}
