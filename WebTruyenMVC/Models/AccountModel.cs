using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WebTruyenMVC;
using WebTruyenMVC.Entity;

public class AccountModel
{
    private readonly MongoContext mongoContext;
    private readonly ILogger logger;
    private const string MongoCollection = "Users";

    public AccountModel(MongoContext mongoContext, ILogger logger)
    {
        this.mongoContext = mongoContext;
        this.logger = logger;
    }

    public async Task<UserEntity?> GetUserByIdAsync(string id)
    {
        var collection = mongoContext.GetCollection<UserEntity>(MongoCollection);
        var filter = Builders<UserEntity>.Filter.Eq(u => u._id, id);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateUserAsync(UserEntity user)
    {
        var collection = mongoContext.GetCollection<UserEntity>(MongoCollection);
        var filter = Builders<UserEntity>.Filter.Eq(u => u._id, user._id);

        var updateDef = new List<UpdateDefinition<UserEntity>>();
        if (!string.IsNullOrWhiteSpace(user.Email)) updateDef.Add(Builders<UserEntity>.Update.Set(u => u.Email, user.Email));
        if (!string.IsNullOrWhiteSpace(user.Avatar)) updateDef.Add(Builders<UserEntity>.Update.Set(u => u.Avatar, user.Avatar));
        if (!string.IsNullOrWhiteSpace(user.Password)) updateDef.Add(Builders<UserEntity>.Update.Set(u => u.Password, user.Password));
        if (!updateDef.Any()) return false;

        var update = Builders<UserEntity>.Update.Combine(updateDef);
        var result = await collection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }
}