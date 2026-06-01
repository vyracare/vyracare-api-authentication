using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Vyracare.Auth.Common.Configuration;
using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth.FirstAccessCheck;
using Vyracare.Auth.Features.Auth.FirstAccessSetPassword;
using Vyracare.Auth.Features.Auth.ForgotPassword;
using Vyracare.Auth.Features.Auth.Login;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Ports;
using Vyracare.Auth.Infrastructure.Persistence;
using Vyracare.Auth.Infrastructure.Security;
using Vyracare.Auth.Infrastructure.Time;

namespace Vyracare.Auth.Infrastructure.DependencyInjection;

/// <summary>
/// Centraliza os métodos de extensão responsáveis por montar o container de dependências da API.
/// Aqui fica o ponto de composição entre domínio, casos de uso e infraestrutura.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra os serviços centrais da feature de autenticação.
    /// Isso inclui abstrações de tempo, segurança, persistência e os handlers de cada caso de uso.
    /// </summary>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <returns>A própria coleção, para permitir encadeamento fluente.</returns>
    public static IServiceCollection AddAuthCore(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, Sha256PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IUserRepository, MongoUserRepository>();

        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<FirstAccessCheckHandler>();
        services.AddScoped<FirstAccessSetPasswordHandler>();
        services.AddScoped<ForgotPasswordHandler>();

        return services;
    }

    /// <summary>
    /// Registra o cliente MongoDB e o banco configurado para o ambiente corrente.
    /// O nome do banco e a connection string já devem estar resolvidos no momento em que este método é chamado.
    /// </summary>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <returns>A própria coleção, para permitir encadeamento fluente.</returns>
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddScoped(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            return sp.GetRequiredService<IMongoClient>().GetDatabase(options.Database);
        });

        return services;
    }
}
