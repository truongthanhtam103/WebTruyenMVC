using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors.Infrastructure;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Service;
using System.Threading.Tasks;

namespace WebTruyenMVC.Models
{
    public class StoryModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "Stories";

        public StoryModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        public async Task<MessagesResponse> GetAllCasesAsync(FilterEntity request)
        {
            var collection = mongoContext.GetCollection<StoryEntity>(MongoCollection);

            var filter = FilterService.BuildFilter(request.q, request.filter);

            // Đếm tổng số bản ghi
            var totalRecords = await collection.CountDocumentsAsync(filter);

            // Áp dụng sắp xếp
            var sortDefinition = SortService.SortBuilder.BuildSort(request.OrderBy, request.OrderByDescending);

            // Lấy dữ liệu phân trang
            var data = await collection.Find(filter)
                                        .Sort(sortDefinition)
                                        .Skip((request.Page - 1) * request.PageSize)
                                        .Limit(request.PageSize)
                                        .ToListAsync();

            // Tạo đối tượng MessagesResponse
            var response = new MessagesResponse
            {
                Code = 200,
                Message = "Successful",
                Data = new
                {
                    TotalItemCounts = totalRecords,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    ListData = data
                }
            };

            return response;
        }
    }
}
