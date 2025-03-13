using MongoDB.Driver;
using WebTruyenMVC.Entity;

namespace WebTruyenMVC.Service
{
    public class FilterService
    {
        public static FilterDefinition<T> BuildFilter<T>(string q, Filter filter) where T : class
        {
            var builder = Builders<T>.Filter;
            var filterDefinition = builder.Empty;

            // Lọc theo trường Title nếu q có giá trị (áp dụng với mọi entity có thuộc tính Title)
            if (!string.IsNullOrEmpty(q))
            {
                filterDefinition &= builder.Regex("Title", new MongoDB.Bson.BsonRegularExpression(q, "i"));
            }

            return filterDefinition;
        }

        public static FilterDefinition<AuthorEntity> BuildAuthorFilter(string q, Filter filter)
        {
            var builder = Builders<AuthorEntity>.Filter;
            var filterDefinition = builder.Empty;

            // Lọc theo Title nếu q có giá trị
            if (!string.IsNullOrEmpty(q))
            {
                filterDefinition &= builder.Regex("Title", new MongoDB.Bson.BsonRegularExpression(q, "i"));
            }

            return filterDefinition;
        }

        public static FilterDefinition<CategoryEntity> BuildCategoryFilter(string q, Filter filter)
        {
            var builder = Builders<CategoryEntity>.Filter;
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
