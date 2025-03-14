using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebTruyenMVC.Entity
{
    public class StoryEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new List<string>();
        public string Status { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Views { get; set; } = 0;
        public DateTime Created { get; set; }
    }
}
