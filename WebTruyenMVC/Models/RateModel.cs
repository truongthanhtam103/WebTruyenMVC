using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors.Infrastructure;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Service;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace WebTruyenMVC.Models
{
    public class RateModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "Ratings";

        public RateModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        public async Task<MessagesResponse> GetAllRateAsync(FilterEntity request)
        {
            var collection = mongoContext.GetCollection<RateEntity>(MongoCollection);
            var filter = FilterService.BuildFilter<RateEntity>(request.q, request.filter);

            // Đếm tổng số bản ghi
            var totalRecords = await collection.CountDocumentsAsync(filter);

            // Áp dụng sắp xếp
            var sortDefinition = SortService.SortBuilder.BuildSort<RateEntity>(request.OrderBy, request.OrderByDescending);

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

        public async Task<MessagesResponse> GetRateByIdAsync(string id)
        {
            var collection = mongoContext.GetCollection<RateEntity>(MongoCollection);
            var filter = Builders<RateEntity>.Filter.Eq(s => s.Id, id);
            var rate = await collection.Find(filter).FirstOrDefaultAsync();

            return new MessagesResponse
            {
                Code = rate != null ? 200 : 404,
                Message = rate != null ? "Record found" : "Not found",
                Data = rate
            };
        }

        public async Task<MessagesResponse> CreateRateAsync(RateEntity newStory)
        {
            var collection = mongoContext.GetCollection<RateEntity>(MongoCollection);
            await collection.InsertOneAsync(newStory);

            return new MessagesResponse
            {
                Code = 201,
                Message = "Created successfully",
                Data = newStory
            };
        }

        public async Task<MessagesResponse> UpdateRateAsync(RateEntity updateRate)
        {
            var collection = mongoContext.GetCollection<RateEntity>(MongoCollection);
            var filter = Builders<RateEntity>.Filter.Eq(s => s.Id, updateRate.Id);
            var updateResult = await collection.ReplaceOneAsync(filter, updateRate);

            return new MessagesResponse
            {
                Code = updateResult.ModifiedCount > 0 ? 200 : 404,
                Message = updateResult.ModifiedCount > 0 ? "Updated successfully" : "Not found",
                Data = updateRate
            };
        }

        public async Task<MessagesResponse> DeleteRateAsync(string id)
        {
            var collection = mongoContext.GetCollection<RateEntity>(MongoCollection);
            var filter = Builders<RateEntity>.Filter.Eq(s => s.Id, id);
            var deleteResult = await collection.DeleteOneAsync(filter);

            return new MessagesResponse
            {
                Code = deleteResult.DeletedCount > 0 ? 200 : 404,
                Message = deleteResult.DeletedCount > 0 ? "Deleted successfully" : "Not found"
            };
        }

    }
}
