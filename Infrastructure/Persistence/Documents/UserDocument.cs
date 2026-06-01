using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vyracare.Auth.Infrastructure.Persistence.Documents;

/// <summary>
/// Representa o formato persistido do usuário dentro da collection <c>users</c> no MongoDB.
/// Esta classe existe para isolar detalhes de serialização e mapeamento do banco do restante da aplicação.
/// </summary>
public sealed class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    /// <summary>
    /// Obtém ou define o identificador do documento no MongoDB.
    /// </summary>
    public string? Id { get; set; }

    [BsonElement("email")]
    /// <summary>
    /// Obtém ou define o e-mail persistido para o usuário.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    [BsonElement("fullName")]
    /// <summary>
    /// Obtém ou define o nome completo salvo no documento.
    /// </summary>
    public string? FullName { get; set; }

    [BsonElement("role")]
    /// <summary>
    /// Obtém ou define o papel ou cargo salvo para o usuário.
    /// </summary>
    public string? Role { get; set; }

    [BsonElement("department")]
    /// <summary>
    /// Obtém ou define o departamento persistido.
    /// </summary>
    public string? Department { get; set; }

    [BsonElement("phone")]
    /// <summary>
    /// Obtém ou define o telefone salvo para contato.
    /// </summary>
    public string? Phone { get; set; }

    [BsonElement("accessLevel")]
    /// <summary>
    /// Obtém ou define o nível de acesso salvo no documento.
    /// </summary>
    public string? AccessLevel { get; set; }

    [BsonElement("active")]
    /// <summary>
    /// Obtém ou define se o usuário está ativo na plataforma.
    /// </summary>
    public bool Active { get; set; }

    [BsonElement("passwordHash")]
    /// <summary>
    /// Obtém ou define o hash da senha armazenado para o usuário.
    /// Quando vazio, a aplicação entende que o primeiro acesso ainda não foi concluído.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    /// <summary>
    /// Obtém ou define a data de criação do documento em UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
