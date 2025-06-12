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
    public class ReadingHistoryModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "ReadingHistory";

        public ReadingHistoryModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        public async Task<MessagesResponse> GetAllReadingHistoryAsync(FilterEntity request)
        {
            var collection = mongoContext.GetCollection<ReadingHistoryEntity>(MongoCollection);
            var filter = FilterService.BuildFilter<ReadingHistoryEntity>(request.q, request.filter);

            // Đếm tổng số bản ghi
            var totalRecords = await collection.CountDocumentsAsync(filter);

            // Áp dụng sắp xếp
            var sortDefinition = SortService.SortBuilder.BuildSort<ReadingHistoryEntity>(request.OrderBy, request.OrderByDescending);

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

        public async Task<MessagesResponse> GetReadingHistoryByIdAsync(string id)
        {
            var collection = mongoContext.GetCollection<ReadingHistoryEntity>(MongoCollection);
            var filter = Builders<ReadingHistoryEntity>.Filter.Eq(s => s.Id, id);
            var readingHistory = await collection.Find(filter).FirstOrDefaultAsync();

            return new MessagesResponse
            {
                Code = readingHistory != null ? 200 : 404,
                Message = readingHistory != null ? "Record found" : "Not found",
                Data = readingHistory
            };
        }

        public async Task<MessagesResponse> CreateReadingHistoryAsync(ReadingHistoryEntity newReadingHistory)
        {
            var collection = mongoContext.GetCollection<ReadingHistoryEntity>(MongoCollection);
            await collection.InsertOneAsync(newReadingHistory);

            return new MessagesResponse
            {
                Code = 201,
                Message = "Created successfully",
                Data = newReadingHistory
            };
        }

        public async Task<MessagesResponse> UpdateReadingHistoryAsync(ReadingHistoryEntity updateStory)
        {
            var collection = mongoContext.GetCollection<ReadingHistoryEntity>(MongoCollection);
            var filter = Builders<ReadingHistoryEntity>.Filter.Eq(s => s.Id, updateStory.Id);
            var updateResult = await collection.ReplaceOneAsync(filter, updateStory);

            return new MessagesResponse
            {
                Code = updateResult.ModifiedCount > 0 ? 200 : 404,
                Message = updateResult.ModifiedCount > 0 ? "Updated successfully" : "Not found",
                Data = updateStory
            };
        }

        public async Task<MessagesResponse> DeleteReadingHistoryAsync(string id)
        {
            var collection = mongoContext.GetCollection<ReadingHistoryEntity>(MongoCollection);
            var filter = Builders<ReadingHistoryEntity>.Filter.Eq(s => s.Id, id);
            var deleteResult = await collection.DeleteOneAsync(filter);

            return new MessagesResponse
            {
                Code = deleteResult.DeletedCount > 0 ? 200 : 404,
                Message = deleteResult.DeletedCount > 0 ? "Deleted successfully" : "Not found"
            };
        }

    }
}
