using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vyracare.Auth.Models;

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("fullName")]
    public string? FullName { get; set; }

    [BsonElement("role")]
    public string? Role { get; set; }

    [BsonElement("department")]
    public string? Department { get; set; }

    [BsonElement("phone")]
    public string? Phone { get; set; }

    [BsonElement("accessLevel")]
    public string? AccessLevel { get; set; }

    [BsonElement("active")]
    public bool Active { get; set; } = true;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
