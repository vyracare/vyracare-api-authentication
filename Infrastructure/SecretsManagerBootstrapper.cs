using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Vyracare.Auth.Infrastructure;

/// <summary>
/// Carrega segredos externos antes da aplicação concluir o startup.
/// Essa etapa permite preencher configurações críticas, como connection string do Mongo e chave JWT,
/// sem mantê-las versionadas no repositório.
/// </summary>
public static class SecretsManagerBootstrapper
{
    /// <summary>
    /// Aplica na configuração em memória os valores resolvidos no AWS Secrets Manager.
    /// O método tenta popular primeiro a connection string do Mongo e depois a chave JWT.
    /// </summary>
    /// <param name="configuration">Objeto de configuração em construção durante o startup.</param>
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

    /// <summary>
    /// Tenta resolver um secret e copiá-lo para a chave de configuração de destino.
    /// O método respeita a ordem de prioridade: valor já presente na configuração, fallback por variável
    /// de ambiente e, por último, consulta ao Secrets Manager.
    /// </summary>
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

        var secretName = Environment.GetEnvironmentVariable(secretNameEnvironmentVariable) ??
            configuration[secretNameConfigKey];

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

    /// <summary>
    /// Extrai do JSON do secret a propriedade esperada para a configuração de destino.
    /// Quando o secret não for um JSON de objeto, o método devolve a string completa.
    /// </summary>
    /// <param name="secretString">Conteúdo bruto retornado pelo AWS Secrets Manager.</param>
    /// <param name="secretPropertyName">Nome da propriedade esperada dentro do JSON.</param>
    /// <returns>Valor da propriedade encontrada ou o texto bruto do secret.</returns>
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
