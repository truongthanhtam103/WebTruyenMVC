using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using WebTruyenMVC.Entity;

namespace WebTruyenMVC.Models
{
    public class UserModel
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger logger;
        private const string MongoCollection = "Users";
        private const string MongoCollectionStory = "Stories";

        public UserModel(MongoContext mongoContext, ILogger logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        public async Task<MessagesResponse> UpdateUserAsync(UpdateUserRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId) || request.UserId.Length != 24)
            {
                return new MessagesResponse
                {
                    Code = 400,
                    Message = "UserId không hợp lệ. UserId phải có đúng 24 ký tự."
                };
            }

            var users = mongoContext.GetCollection<UserEntity>(MongoCollection);
            var filter = Builders<UserEntity>.Filter.Eq(u => u._id, request.UserId);

            // Kiểm tra sự tồn tại của user trước khi update
            var existingUser = await users.Find(filter).FirstOrDefaultAsync();
            if (existingUser == null)
                return new MessagesResponse { Code = 404, Message = "Không tìm thấy tài khoản người dùng." };

            var updateDef = new List<UpdateDefinition<UserEntity>>();
            if (request.Email != null) updateDef.Add(Builders<UserEntity>.Update.Set(u => u.Email, request.Email));
            if (request.Password != null) updateDef.Add(Builders<UserEntity>.Update.Set(u => u.Password, request.Password));
            if (request.Avatar != null) updateDef.Add(Builders<UserEntity>.Update.Set(u => u.Avatar, request.Avatar));
            if (!updateDef.Any())
                return new MessagesResponse { Code = 400, Message = "No fields to update." };

            var update = Builders<UserEntity>.Update.Combine(updateDef);
            var result = await users.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
                return new MessagesResponse { Code = 400, Message = "User not found or no changes made." };

            return new MessagesResponse { Code = 200, Message = "User updated successfully." };
        }

        public async Task<MessagesResponse> LockUserAsync(string id)
        {
            var users = mongoContext.GetCollection<UserEntity>(MongoCollection);
            var user = await users.Find(u => u._id == id).FirstOrDefaultAsync();

            if (user == null)
                return new MessagesResponse { Code = 404, Message = "User not found." };

            if (user.Role != "admin")
                return new MessagesResponse { Code = 403, Message = "Only admin can lock accounts." };

            var update = Builders<UserEntity>.Update.Set(u => u.IsLocked, true);
            await users.UpdateOneAsync(u => u._id == id, update);

            return new MessagesResponse { Code = 200, Message = "User account locked successfully." };
        }

        public async Task<MessagesResponse> DeleteUserAsync(string id)
        {
            var users = mongoContext.GetCollection<UserEntity>(MongoCollection);
            var user = await users.Find(u => u._id == id).FirstOrDefaultAsync();

            if (user == null)
                return new MessagesResponse { Code = 404, Message = "User not found." };

            if (user.Role != "admin")
                return new MessagesResponse { Code = 403, Message = "Only admin can delete accounts." };

            await users.DeleteOneAsync(u => u._id == id);

            return new MessagesResponse { Code = 200, Message = "User account deleted successfully." };
        }

        public async Task<MessagesResponse> AddToShelfAsync(AddToShelfRequest request)
        {
            var users = mongoContext.GetCollection<UserEntity>(MongoCollection);
            var filter = Builders<UserEntity>.Filter.Eq(u => u._id, request.UserId);
            var update = Builders<UserEntity>.Update.AddToSet(u => u.Shelf, request.StoryId);

            var result = await users.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
                return new MessagesResponse { Code = 404, Message = "User not found or story already in favorites." };

            return new MessagesResponse { Code = 200, Message = "Story added to favorites." };
        }

        public async Task<MessagesResponse> GetShelfAsync(string userId)
        {
            var users = mongoContext.GetCollection<UserEntity>(MongoCollection);
            var user = await users.Find(u => u._id == userId).FirstOrDefaultAsync();

            if (user == null)
                return new MessagesResponse { Code = 404, Message = "User not found." };

            var favoriteStoryIds = user.Shelf ?? new List<string>();
            if (!favoriteStoryIds.Any())
                return new MessagesResponse { Code = 200, Message = "Success", Data = new List<object>() };

            var stories = mongoContext.GetCollection<StoryEntity>(MongoCollectionStory);
            var filter = Builders<StoryEntity>.Filter.In(s => s.Id, favoriteStoryIds);
            var storyList = await stories.Find(filter).ToListAsync();

            return new MessagesResponse
            {
                Code = 200,
                Message = "Success",
                Data = storyList
            };
        }
    }
}