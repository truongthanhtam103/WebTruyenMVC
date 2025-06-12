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
    public class ChapterModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "Chapters";
        private const string MongoCollectionStory = "Stories";
        private const string MongoCollectionReadingHistory = "ReadingHistory";

        public ChapterModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        public async Task<MessagesResponse> GetAllChapterAsync(FilterEntity request)
        {
            var collection = mongoContext.GetCollection<ChapterEntity>(MongoCollection);
            var filter = FilterService.BuildFilter<ChapterEntity>(request.q, request.filter);

            // Đếm tổng số bản ghi
            var totalRecords = await collection.CountDocumentsAsync(filter);

            // Áp dụng sắp xếp
            var sortDefinition = SortService.SortBuilder.BuildSort<ChapterEntity>(request.OrderBy, request.OrderByDescending);

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

        public async Task<MessagesResponse> GetChapterByIdAsync(string id)
        {
            var collection = mongoContext.GetCollection<ChapterEntity>(MongoCollection);
            var filter = Builders<ChapterEntity>.Filter.Eq(s => s.Id, id);
            var chapter = await collection.Find(filter).FirstOrDefaultAsync();

            return new MessagesResponse
            {
                Code = chapter != null ? 200 : 404,
                Message = chapter != null ? "Record found" : "Not found",
                Data = chapter
            };
        }

        public async Task<MessagesResponse> CreateChapterAsync(ChapterEntity newChapter)
        {
            var collection = mongoContext.GetCollection<ChapterEntity>(MongoCollection);
            await collection.InsertOneAsync(newChapter);

            return new MessagesResponse
            {
                Code = 201,
                Message = "Created successfully",
                Data = newChapter
            };
        }

        public async Task<MessagesResponse> UpdateChapterAsync(ChapterEntity updateChapter)
        {
            var collection = mongoContext.GetCollection<ChapterEntity>(MongoCollection);
            var filter = Builders<ChapterEntity>.Filter.Eq(s => s.Id, updateChapter.Id);
            var updateResult = await collection.ReplaceOneAsync(filter, updateChapter);

            return new MessagesResponse
            {
                Code = updateResult.ModifiedCount > 0 ? 200 : 404,
                Message = updateResult.ModifiedCount > 0 ? "Updated successfully" : "Not found",
                Data = updateChapter
            };
        }

        public async Task<MessagesResponse> DeleteChapterAsync(string id)
        {
            var collection = mongoContext.GetCollection<ChapterEntity>(MongoCollection);
            var filter = Builders<ChapterEntity>.Filter.Eq(s => s.Id, id);
            var deleteResult = await collection.DeleteOneAsync(filter);

            return new MessagesResponse
            {
                Code = deleteResult.DeletedCount > 0 ? 200 : 404,
                Message = deleteResult.DeletedCount > 0 ? "Deleted successfully" : "Not found"
            };
        }

        public async Task<MessagesResponse> IncreaseStoryView(string storyId)
        {
            var collection = mongoContext.GetCollection<StoryEntity>(MongoCollectionStory);
            var filter = Builders<StoryEntity>.Filter.Eq(s => s.Id, storyId);
            var update = Builders<StoryEntity>.Update.Inc(s => s.Views, 1);
            var updateResult = await collection.UpdateOneAsync(filter, update);

            return new MessagesResponse
            {
                Code = updateResult.ModifiedCount > 0 ? 200 : 404,
                Message = updateResult.ModifiedCount > 0 ? "View count updated successfully" : "Story not found"
            };
        }

        public async Task<MessagesResponse> UpdateReadingHistory(string userId, string storyId, int chapterNumber)
        {
            var collection = mongoContext.GetCollection<ReadingHistoryEntity>(MongoCollectionReadingHistory);
            var filter = Builders<ReadingHistoryEntity>.Filter.Where(h => h.UserID == userId && h.StoryID == storyId);
            var update = Builders<ReadingHistoryEntity>.Update
                .Set(h => h.LastReadChapter, chapterNumber)
                .Set(h => h.LastReadAt, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<ReadingHistoryEntity>
            {
                IsUpsert = true // Nếu không tìm thấy sẽ tạo mới
            };

            var updateResult = await collection.FindOneAndUpdateAsync(filter, update, options);

            return new MessagesResponse
            {
                Code = updateResult != null ? 200 : 500,
                Message = updateResult != null ? "Reading history updated successfully" : "Failed to update reading history"
            };
        }

        public async Task ReadChapter(string userId, string storyId, int chapterNumber)
        {
            await IncreaseStoryView(storyId);
            await UpdateReadingHistory(userId, storyId, chapterNumber);
        }
    }
}
