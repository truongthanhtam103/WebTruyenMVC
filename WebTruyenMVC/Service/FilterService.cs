using MongoDB.Driver;
using WebTruyenMVC.Entity;

namespace WebTruyenMVC.Service
{
    public class FilterService
    {
        public static FilterDefinition<StoryEntity> BuildFilter(string q, Filter filter)
        {
            var builder = Builders<StoryEntity>.Filter;
            var filterDefinition = builder.Empty;

            // Lọc theo Title nếu q có giá trị
            if (!string.IsNullOrEmpty(q))
            {
                filterDefinition &= builder.Regex("Title", new MongoDB.Bson.BsonRegularExpression(q, "i"));
            }

            return filterDefinition;
        }
    }
}
