using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyApp.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class PropertiesEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = string.Empty;
    public int IdOwner { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public double Price { get; set; } = default!;
    public string Img { get; set; } = default!;
    public int IdProperty { get; set; } = default!;
    public string CodeInternal { get; set; } = string.Empty;
    public int Year { get; set; } = default!;
}