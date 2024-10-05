using MongoDB.Driver;
using Post.API.Models;

namespace Post.API.Data
{
public class MongoDBContext
{
    private readonly IMongoDatabase _database;

    public MongoDBContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Image> Images => _database.GetCollection<Image>("Images");
}
}
