using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;
using Vyracare.Auth.Infrastructure.Persistence.Documents;

namespace Vyracare.Auth.Infrastructure.Persistence;

/// <summary>
/// Implementa a porta <see cref="IUserRepository"/> usando MongoDB.
/// Esta classe é o adapter responsável por transformar entidade de domínio em documento e vice-versa.
/// </summary>
public sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection;

    /// <summary>
    /// Inicializa o repositório apontando para a collection <c>users</c> do banco configurado.
    /// </summary>
    /// <param name="database">Banco Mongo resolvido para o ambiente atual.</param>
    public MongoUserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserDocument>("users");
    }

    /// <summary>
    /// Localiza um usuário pelo e-mail e converte o documento encontrado para a entidade de domínio.
    /// </summary>
    /// <param name="email">E-mail usado como filtro da busca.</param>
    /// <returns>Entidade de domínio correspondente ou <see langword="null"/> quando não houver registro.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        var document = await _collection.Find(item => item.Email == email).FirstOrDefaultAsync();
        return document is null ? null : MapToDomain(document);
    }

    /// <summary>
    /// Pesquisa usuarios ativos por nome, e-mail ou telefone sem expor credenciais.
    /// </summary>
    public async Task<IReadOnlyCollection<User>> SearchActiveAsync(string? search, int limit)
    {
        var filter = Builders<UserDocument>.Filter.Eq(item => item.Active, true);
        var normalizedSearch = search?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            var expression = new MongoDB.Bson.BsonRegularExpression(Regex.Escape(normalizedSearch), "i");
            var digits = Regex.Replace(normalizedSearch, @"\D", string.Empty);
            var phonePattern = digits.Length > 0
                ? string.Join(@"\D*", digits.Select(character => Regex.Escape(character.ToString())))
                : Regex.Escape(normalizedSearch);
            var phoneExpression = new MongoDB.Bson.BsonRegularExpression(phonePattern, "i");
            filter &= Builders<UserDocument>.Filter.Or(
                Builders<UserDocument>.Filter.Regex(item => item.FullName, expression),
                Builders<UserDocument>.Filter.Regex(item => item.Email, expression),
                Builders<UserDocument>.Filter.Regex(item => item.Phone, phoneExpression));
        }

        var documents = await _collection.Find(filter)
            .SortBy(item => item.FullName)
            .Limit(Math.Clamp(limit, 1, 20))
            .ToListAsync();

        return documents.Select(MapToDomain).ToArray();
    }

    /// <summary>
    /// Pesquisa todos os usuarios para a tela administrativa, preservando o filtro textual seguro.
    /// </summary>
    public async Task<IReadOnlyCollection<User>> SearchAllAsync(string? search, int limit)
    {
        var filter = BuildSearchFilter(search);
        var documents = await _collection.Find(filter)
            .SortBy(item => item.FullName)
            .Limit(Math.Clamp(limit, 1, 100))
            .ToListAsync();

        return documents.Select(MapToDomain).ToArray();
    }

    /// <summary>
    /// Localiza um usuario pelo ObjectId sem propagar erro para identificadores malformados.
    /// </summary>
    public async Task<User?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return null;
        var document = await _collection.Find(item => item.Id == id).FirstOrDefaultAsync();
        return document is null ? null : MapToDomain(document);
    }

    /// <summary>
    /// Atualiza somente os campos administrativos do usuario e preserva senha e data de criacao.
    /// </summary>
    public async Task<bool> UpdateAsync(User user)
    {
        if (!ObjectId.TryParse(user.Id, out _)) return false;
        var update = Builders<UserDocument>.Update
            .Set(item => item.FullName, user.FullName)
            .Set(item => item.Email, user.Email)
            .Set(item => item.Role, user.Role)
            .Set(item => item.Department, user.Department)
            .Set(item => item.Phone, user.Phone)
            .Set(item => item.AccessLevel, user.AccessLevel)
            .Set(item => item.Active, user.Active);
        var result = await _collection.UpdateOneAsync(item => item.Id == user.Id, update);
        return result.MatchedCount > 0;
    }

    /// <summary>
    /// Altera isoladamente o status ativo usado pelo login e pelos seletores operacionais.
    /// </summary>
    public async Task<bool> SetActiveAsync(string id, bool active)
    {
        if (!ObjectId.TryParse(id, out _)) return false;
        var result = await _collection.UpdateOneAsync(
            item => item.Id == id,
            Builders<UserDocument>.Update.Set(item => item.Active, active));
        return result.MatchedCount > 0;
    }

    /// <summary>
    /// Remove definitivamente um usuário a partir de um ObjectId válido.
    /// </summary>
    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return false;
        var result = await _collection.DeleteOneAsync(item => item.Id == id);
        return result.DeletedCount > 0;
    }

    /// <summary>
    /// Insere um novo usuário na collection e devolve a entidade com o identificador persistido.
    /// </summary>
    /// <param name="user">Entidade de domínio pronta para gravação.</param>
    /// <returns>Entidade persistida com o identificador retornado pelo MongoDB.</returns>
    public async Task<User> AddAsync(User user)
    {
        var document = MapToDocument(user);
        await _collection.InsertOneAsync(document);
        user.Id = document.Id;
        return user;
    }

    /// <summary>
    /// Monta o filtro textual compartilhado pela consulta administrativa.
    /// </summary>
    private static FilterDefinition<UserDocument> BuildSearchFilter(string? search)
    {
        var normalizedSearch = search?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedSearch))
        {
            return Builders<UserDocument>.Filter.Empty;
        }

        var expression = new BsonRegularExpression(Regex.Escape(normalizedSearch), "i");
        var digits = Regex.Replace(normalizedSearch, @"\D", string.Empty);
        var phonePattern = digits.Length > 0
            ? string.Join(@"\D*", digits.Select(character => Regex.Escape(character.ToString())))
            : Regex.Escape(normalizedSearch);
        var phoneExpression = new BsonRegularExpression(phonePattern, "i");
        return Builders<UserDocument>.Filter.Or(
            Builders<UserDocument>.Filter.Regex(item => item.FullName, expression),
            Builders<UserDocument>.Filter.Regex(item => item.Email, expression),
            Builders<UserDocument>.Filter.Regex(item => item.Phone, phoneExpression));
    }

    /// <summary>
    /// Define a senha do usuário somente se o hash atual ainda estiver vazio ou nulo.
    /// Esse comportamento protege o fluxo de primeiro acesso contra sobrescrita de senha já existente.
    /// </summary>
    /// <param name="email">E-mail do usuário que receberá a senha inicial.</param>
    /// <param name="passwordHash">Hash calculado da senha informada.</param>
    /// <returns><see langword="true"/> quando a senha foi gravada; caso contrário, <see langword="false"/>.</returns>
    public async Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash)
    {
        var filter = Builders<UserDocument>.Filter.Eq(item => item.Email, email)
                     & Builders<UserDocument>.Filter.Or(
                         Builders<UserDocument>.Filter.Eq(item => item.PasswordHash, string.Empty),
                         Builders<UserDocument>.Filter.Eq(item => item.PasswordHash, null)
                     );

        var update = Builders<UserDocument>.Update.Set(item => item.PasswordHash, passwordHash);
        var result = await _collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Atualiza a senha de um usuário existente sem exigir que o hash anterior esteja vazio.
    /// Esse método é usado no fluxo de recuperação de senha.
    /// </summary>
    /// <param name="email">E-mail do usuário que terá a senha atualizada.</param>
    /// <param name="passwordHash">Hash da nova senha.</param>
    /// <returns><see langword="true"/> quando o usuário foi encontrado; caso contrário, <see langword="false"/>.</returns>
    public async Task<bool> UpdatePasswordAsync(string email, string passwordHash)
    {
        var filter = Builders<UserDocument>.Filter.Eq(item => item.Email, email);
        var update = Builders<UserDocument>.Update.Set(item => item.PasswordHash, passwordHash);
        var result = await _collection.UpdateOneAsync(filter, update);
        return result.MatchedCount > 0;
    }

    /// <summary>
    /// Converte a entidade de domínio em documento de persistência.
    /// </summary>
    private static UserDocument MapToDocument(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role,
        Department = user.Department,
        Phone = user.Phone,
        AccessLevel = user.AccessLevel,
        Active = user.Active,
        PasswordHash = user.PasswordHash ?? string.Empty,
        CreatedAt = user.CreatedAt
    };

    /// <summary>
    /// Converte o documento retornado pelo MongoDB em entidade de domínio.
    /// </summary>
    private static User MapToDomain(UserDocument document) => new()
    {
        Id = document.Id,
        Email = document.Email,
        FullName = document.FullName,
        Role = document.Role,
        Department = document.Department,
        Phone = document.Phone,
        AccessLevel = document.AccessLevel,
        Active = document.Active,
        PasswordHash = document.PasswordHash,
        CreatedAt = document.CreatedAt
    };
}
