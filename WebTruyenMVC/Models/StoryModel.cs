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
    public class StoryModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "Stories";
        private const string MongoCollectionRate = "Ratings";

        public StoryModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }


        public async Task<MessagesResponse> GetAllStoryAsync(FilterEntity request)
        {
            var collection = mongoContext.GetCollection<StoryEntity>(MongoCollection);
            var filter = FilterService.BuildFilter<StoryEntity>(request.q, request.filter);

            // Đếm tổng số bản ghi
            var totalRecords = await collection.CountDocumentsAsync(filter);

            // Áp dụng sắp xếp
            var sortDefinition = SortService.SortBuilder.BuildSort<StoryEntity>(request.OrderBy, request.OrderByDescending);

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

        public async Task<MessagesResponse> GetStoryByIdAsync(string id)
        {
            var collection = mongoContext.GetCollection<StoryEntity>(MongoCollection);
            var filter = Builders<StoryEntity>.Filter.Eq(s => s.Id, id);
            var story = await collection.Find(filter).FirstOrDefaultAsync();

            return new MessagesResponse
            {
                Code = story != null ? 200 : 404,
                Message = story != null ? "Record found" : "Not found",
                Data = story
            };
        }

        public async Task<MessagesResponse> CreateStoryAsync(StoryEntity newStory)
        {
            var collection = mongoContext.GetCollection<StoryEntity>(MongoCollection);
            await collection.InsertOneAsync(newStory);

            return new MessagesResponse
            {
                Code = 201,
                Message = "Created successfully",
                Data = newStory
            };
        }

        public async Task<MessagesResponse> UpdateStoryAsync(StoryEntity updateStory)
        {
            var collection = mongoContext.GetCollection<StoryEntity>(MongoCollection);
            var filter = Builders<StoryEntity>.Filter.Eq(s => s.Id, updateStory.Id);
            var updateResult = await collection.ReplaceOneAsync(filter, updateStory);

            return new MessagesResponse
            {
                Code = updateResult.ModifiedCount > 0 ? 200 : 404,
                Message = updateResult.ModifiedCount > 0 ? "Updated successfully" : "Not found",
                Data = updateStory
            };
        }

        public async Task<MessagesResponse> DeleteStoryAsync(string id)
        {
            var collection = mongoContext.GetCollection<StoryEntity>(MongoCollection);
            var filter = Builders<StoryEntity>.Filter.Eq(s => s.Id, id);
            var deleteResult = await collection.DeleteOneAsync(filter);

            return new MessagesResponse
            {
                Code = deleteResult.DeletedCount > 0 ? 200 : 404,
                Message = deleteResult.DeletedCount > 0 ? "Deleted successfully" : "Not found"
            };
        }

        public async Task<MessagesResponse> GetTopRatedStoriesAsync()
        {
            var rateCollection = mongoContext.GetCollection<RateEntity>(MongoCollectionRate);
            var storyCollection = mongoContext.GetCollection<StoryEntity>(MongoCollection);

            // Aggregation để tính trung bình rating theo StoryID
            var ratingAggregation = await rateCollection.Aggregate()
                .Group(r => r.StoryID, g => new
                {
                    StoryID = g.Key,
                    AverageRating = g.Average(r => r.Rating)
                })
                .SortByDescending(g => g.AverageRating)
                .Limit(20)
                .ToListAsync();

            // Lấy danh sách StoryID của top 20 truyện
            var topStoryIds = ratingAggregation.Select(r => r.StoryID).ToList();

            // Lọc danh sách truyện có ID trong topStoryIds
            var filter = Builders<StoryEntity>.Filter.In(s => s.Id, topStoryIds);
            var topStories = await storyCollection.Find(filter).ToListAsync();

            // Map kết quả trung bình Rating vào danh sách truyện
            var storyList = topStories.Select(story =>
            {
                var ratingInfo = ratingAggregation.FirstOrDefault(r => r.StoryID == story.Id);
                return new
                {
                    story.Id,
                    story.Title,
                    story.Description,
                    story.CoverImage,
                    story.Author,
                    story.Categories,
                    story.Status,
                    story.Views,
                    story.Created,
                    AverageRating = ratingInfo?.AverageRating ?? 0 // Nếu không có rating thì mặc định 0
                };
            }).OrderByDescending(s => s.AverageRating).ToList();

            // Trả về response
            var response = new MessagesResponse
            {
                Code = 200,
                Message = "Successful",
                Data = new
                {
                    TotalItemCounts = storyList.Count,
                    ListData = storyList
                }
            };

            return response;
        }

    }
}
