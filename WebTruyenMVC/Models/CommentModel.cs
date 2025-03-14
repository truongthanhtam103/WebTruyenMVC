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
    public class CommentModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "Comments";

        public CommentModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        public async Task<MessagesResponse> GetAllCommentAsync(FilterEntity request)
        {
            var collection = mongoContext.GetCollection<CommentEntity>(MongoCollection);
            var filter = FilterService.BuildFilter<CommentEntity>(request.q, request.filter);

            // Đếm tổng số bản ghi
            var totalRecords = await collection.CountDocumentsAsync(filter);

            // Áp dụng sắp xếp
            var sortDefinition = SortService.SortBuilder.BuildSort<CommentEntity>(request.OrderBy, request.OrderByDescending);

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

        public async Task<MessagesResponse> GetCommentByIdAsync(string id)
        {
            var collection = mongoContext.GetCollection<CommentEntity>(MongoCollection);
            var filter = Builders<CommentEntity>.Filter.Eq(s => s.Id, id);
            var comment = await collection.Find(filter).FirstOrDefaultAsync();

            return new MessagesResponse
            {
                Code = comment != null ? 200 : 404,
                Message = comment != null ? "Record found" : "Not found",
                Data = comment
            };
        }

        public async Task<MessagesResponse> CreateCommentAsync(CommentEntity newComment)
        {
            var collection = mongoContext.GetCollection<CommentEntity>(MongoCollection);
            await collection.InsertOneAsync(newComment);

            return new MessagesResponse
            {
                Code = 201,
                Message = "Created successfully",
                Data = newComment
            };
        }

        public async Task<MessagesResponse> UpdateCommentAsync(CommentEntity updateComment)
        {
            var collection = mongoContext.GetCollection<CommentEntity>(MongoCollection);
            var filter = Builders<CommentEntity>.Filter.Eq(s => s.Id, updateComment.Id);
            var updateResult = await collection.ReplaceOneAsync(filter, updateComment);

            return new MessagesResponse
            {
                Code = updateResult.ModifiedCount > 0 ? 200 : 404,
                Message = updateResult.ModifiedCount > 0 ? "Updated successfully" : "Not found",
                Data = updateComment
            };
        }

        public async Task<MessagesResponse> DeleteCommentAsync(string id)
        {
            var collection = mongoContext.GetCollection<CommentEntity>(MongoCollection);
            var filter = Builders<CommentEntity>.Filter.Eq(s => s.Id, id);
            var deleteResult = await collection.DeleteOneAsync(filter);

            return new MessagesResponse
            {
                Code = deleteResult.DeletedCount > 0 ? 200 : 404,
                Message = deleteResult.DeletedCount > 0 ? "Deleted successfully" : "Not found"
            };
        }

    }
}
