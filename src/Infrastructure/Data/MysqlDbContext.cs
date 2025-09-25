using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MyApp.Infrastructure.Entities;

namespace MyApp.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        // Configurar convenciones de serialización
        var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("camelCase", conventionPack, type => true);
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<PropertiesEntity> Properties => _database.GetCollection<PropertiesEntity>("properties");
}