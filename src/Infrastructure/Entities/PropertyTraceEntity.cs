using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyApp.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class PropertyTraceEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = string.Empty;
    
    public int IdProperty { get; set; } = default!;
    public DateTime DateSale { get; set; } = default!;
    public string Name { get; set; } = default!;
    public double Value { get; set; } = default!;
    public double Tax { get; set; } = default!;
}
