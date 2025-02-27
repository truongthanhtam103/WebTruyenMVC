using MongoDB.Driver;

namespace WebTruyenMVC
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
