using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors.Infrastructure;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Service;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebTruyenMVC.Models
{
    public class AuthorModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "Authors";

        public AuthorModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        public async Task<MessagesResponse> GetAllAuthorAsync(FilterEntity request)
        {
            var collection = mongoContext.GetCollection<AuthorEntity>(MongoCollection);
            var filter = FilterService.BuildFilter<AuthorEntity>(request.q, request.filter);

            // Đếm tổng số bản ghi
            var totalRecords = await collection.CountDocumentsAsync(filter);

            // Áp dụng sắp xếp
            var sortDefinition = SortService.SortBuilder.BuildSort<AuthorEntity>(request.OrderBy, request.OrderByDescending);

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

        public async Task<MessagesResponse> GetAuthorByIdAsync(string id)
        {
            var collection = mongoContext.GetCollection<AuthorEntity>(MongoCollection);
            var filter = Builders<AuthorEntity>.Filter.Eq(s => s.Id, id);
            var story = await collection.Find(filter).FirstOrDefaultAsync();

            return new MessagesResponse
            {
                Code = story != null ? 200 : 404,
                Message = story != null ? "Record found" : "Not found",
                Data = story
            };
        }

        public async Task<MessagesResponse> CreateAuthorAsync(AuthorEntity newStory)
        {
            var collection = mongoContext.GetCollection<AuthorEntity>(MongoCollection);
            await collection.InsertOneAsync(newStory);

            return new MessagesResponse
            {
                Code = 201,
                Message = "Created successfully",
                Data = newStory
            };
        }

        public async Task<MessagesResponse> UpdateAuthorAsync(AuthorEntity updateStory)
        {
            var collection = mongoContext.GetCollection<AuthorEntity>(MongoCollection);
            var filter = Builders<AuthorEntity>.Filter.Eq(s => s.Id, updateStory.Id);
            var updateResult = await collection.ReplaceOneAsync(filter, updateStory);

            return new MessagesResponse
            {
                Code = updateResult.ModifiedCount > 0 ? 200 : 404,
                Message = updateResult.ModifiedCount > 0 ? "Updated successfully" : "Not found",
                Data = updateStory
            };
        }

        public async Task<MessagesResponse> DeleteAuthorAsync(string id)
        {
            var collection = mongoContext.GetCollection<AuthorEntity>(MongoCollection);
            var filter = Builders<AuthorEntity>.Filter.Eq(s => s.Id, id);
            var deleteResult = await collection.DeleteOneAsync(filter);

            return new MessagesResponse
            {
                Code = deleteResult.DeletedCount > 0 ? 200 : 404,
                Message = deleteResult.DeletedCount > 0 ? "Deleted successfully" : "Not found"
            };
        }

    }
}
