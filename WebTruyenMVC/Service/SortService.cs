using MongoDB.Driver;
using WebTruyenMVC.Entity;

namespace WebTruyenMVC.Service
{
    public class SortService
    {
        public static class SortBuilder
        {
            public static SortDefinition<T> BuildSort<T>(string orderBy, bool orderByDescending) where T : class
            {
                var builder = Builders<T>.Sort;

                if (string.IsNullOrEmpty(orderBy))
                {
                    return builder.Ascending("_id"); // Sắp xếp mặc định theo ID
                }

                return orderByDescending ? builder.Descending(orderBy) : builder.Ascending(orderBy);
            }
        }
    }
}
