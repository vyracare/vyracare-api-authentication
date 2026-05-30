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
/// Centraliza extens?es reutiliz?veis usadas pela aplica??o.
/// </summary>
public static class ServiceCollectionExtensions
{
/// <summary>
/// Registra os handlers, portas e servi?os centrais da aplica??o no container de depend?ncias.
/// </summary>
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
/// Registra os servi?os necess?rios para conectar a aplica??o ao MongoDB.
/// </summary>
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
