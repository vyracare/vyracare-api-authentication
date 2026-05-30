using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vyracare.Auth.Infrastructure.Persistence.Documents;

/// <summary>
/// Representa o formato persistido no MongoDB para esta entidade.
/// </summary>
public sealed class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
/// <summary>
/// Identificador do registro ou do recurso processado.
/// </summary>
    public string? Id { get; set; }

    [BsonElement("email")]
/// <summary>
/// Obt?m ou define e ma il.
/// </summary>
    public string Email { get; set; } = string.Empty;

    [BsonElement("fullName")]
/// <summary>
/// Obt?m ou define f ul ln am e.
/// </summary>
    public string? FullName { get; set; }

    [BsonElement("role")]
/// <summary>
/// Obt?m ou define r ol e.
/// </summary>
    public string? Role { get; set; }

    [BsonElement("department")]
/// <summary>
/// Obt?m ou define d ep ar tm en t.
/// </summary>
    public string? Department { get; set; }

    [BsonElement("phone")]
/// <summary>
/// Obt?m ou define p ho ne.
/// </summary>
    public string? Phone { get; set; }

    [BsonElement("accessLevel")]
/// <summary>
/// Obt?m ou define a cc es sl ev el.
/// </summary>
    public string? AccessLevel { get; set; }

    [BsonElement("active")]
/// <summary>
/// Obt?m ou define a ct iv e.
/// </summary>
    public bool Active { get; set; }

    [BsonElement("passwordHash")]
/// <summary>
/// Obt?m ou define p as sw or dh as h.
/// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("createdAt")]
/// <summary>
/// Data de cria??o do registro.
/// </summary>
    public DateTime CreatedAt { get; set; }
}
