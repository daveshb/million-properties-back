using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyApp.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class OwnerEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = string.Empty;
    
    public int IdOwner { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
}
