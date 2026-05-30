using MongoDB.Driver;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;
using Vyracare.Auth.Infrastructure.Persistence.Documents;

namespace Vyracare.Auth.Infrastructure.Persistence;

public sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection;

    public MongoUserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserDocument>("users");
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var document = await _collection.Find(item => item.Email == email).FirstOrDefaultAsync();
        return document is null ? null : MapToDomain(document);
    }

    public async Task<User> AddAsync(User user)
    {
        var document = MapToDocument(user);
        await _collection.InsertOneAsync(document);
        user.Id = document.Id;
        return user;
    }

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

    public async Task<bool> UpdatePasswordAsync(string email, string passwordHash)
    {
        var filter = Builders<UserDocument>.Filter.Eq(item => item.Email, email);
        var update = Builders<UserDocument>.Update.Set(item => item.PasswordHash, passwordHash);
        var result = await _collection.UpdateOneAsync(filter, update);
        return result.MatchedCount > 0;
    }

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
        PasswordHash = user.PasswordHash,
        CreatedAt = user.CreatedAt
    };

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
