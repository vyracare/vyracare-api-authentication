using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Vyracare.Auth.Infrastructure;

/// <summary>
/// Aplica os valores externos e segredos necessários durante a inicialização da aplicação.
/// </summary>
public static class SecretsManagerBootstrapper
{
/// <summary>
/// Aplica os valores externos necessários antes da inicialização completa da aplicação.
/// </summary>
    public static async Task ApplyAsync(ConfigurationManager configuration)
    {
        var overrides = new Dictionary<string, string>();

        await TryAddSecretValueAsync(
            configuration,
            overrides,
            secretNameConfigKey: "Secrets:MongoSecretName",
            secretNameEnvironmentVariable: "MONGO_SECRET_NAME",
            fallbackEnvironmentVariable: "MONGO_URI",
            targetConfigurationKey: "Mongo:ConnectionString",
            secretPropertyName: "ConnectionString");

        await TryAddSecretValueAsync(
            configuration,
            overrides,
            secretNameConfigKey: "Secrets:JwtSecretName",
            secretNameEnvironmentVariable: "JWT_SECRET_NAME",
            fallbackEnvironmentVariable: "JWT_KEY",
            targetConfigurationKey: "Jwt:Key",
            secretPropertyName: "Key");

        if (overrides.Count > 0)
        {
            configuration.AddInMemoryCollection(overrides);
        }
    }

    private static async Task TryAddSecretValueAsync(
        IConfiguration configuration,
        IDictionary<string, string> overrides,
        string secretNameConfigKey,
        string secretNameEnvironmentVariable,
        string fallbackEnvironmentVariable,
        string targetConfigurationKey,
        string secretPropertyName)
    {
        if (!string.IsNullOrWhiteSpace(configuration[targetConfigurationKey]) ||
            !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(fallbackEnvironmentVariable)))
        {
            return;
        }

        var secretName = configuration[secretNameConfigKey] ??
            Environment.GetEnvironmentVariable(secretNameEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(secretName))
        {
            return;
        }

        try
        {
            using var client = new AmazonSecretsManagerClient();
            var response = await client.GetSecretValueAsync(new GetSecretValueRequest
            {
                SecretId = secretName
            });

            if (string.IsNullOrWhiteSpace(response.SecretString))
            {
                return;
            }

            overrides[targetConfigurationKey] = ExtractSecretValue(response.SecretString, secretPropertyName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Falha ao carregar o secret '{secretName}' para '{targetConfigurationKey}'.",
                ex);
        }
    }

    private static string ExtractSecretValue(string secretString, string secretPropertyName)
    {
        try
        {
            using var document = JsonDocument.Parse(secretString);
            if (document.RootElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in document.RootElement.EnumerateObject())
                {
                    if (string.Equals(property.Name, secretPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return property.Value.GetString() ?? string.Empty;
                    }
                }
            }
        }
        catch (JsonException)
        {
        }

        return secretString;
    }
}
