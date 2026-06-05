using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Vyracare.Auth.Infrastructure;

/// <summary>
/// Carrega parametros externos antes da aplicacao concluir o startup.
/// Essa etapa permite preencher configuracoes criticas, como connection string do Mongo e chave JWT,
/// sem mante-las versionadas no repositorio.
/// </summary>
public static class ParameterStoreBootstrapper
{
    /// <summary>
    /// Aplica na configuracao em memoria os valores resolvidos no AWS Systems Manager Parameter Store.
    /// O metodo tenta popular primeiro a connection string do Mongo e depois a chave JWT.
    /// </summary>
    /// <param name="configuration">Objeto de configuracao em construcao durante o startup.</param>
    public static async Task ApplyAsync(ConfigurationManager configuration)
    {
        var overrides = new Dictionary<string, string?>();

        await TryAddParameterValueAsync(
            configuration,
            overrides,
            parameterNameConfigKeys: ["Parameters:MongoParameterName", "Secrets:MongoSecretName"],
            parameterNameEnvironmentVariables: ["MONGO_PARAMETER_NAME", "MONGO_SECRET_NAME"],
            fallbackEnvironmentVariable: "MONGO_URI",
            targetConfigurationKey: "Mongo:ConnectionString",
            parameterPropertyName: "ConnectionString");

        await TryAddParameterValueAsync(
            configuration,
            overrides,
            parameterNameConfigKeys: ["Parameters:JwtParameterName", "Secrets:JwtSecretName"],
            parameterNameEnvironmentVariables: ["JWT_PARAMETER_NAME", "JWT_SECRET_NAME"],
            fallbackEnvironmentVariable: "JWT_KEY",
            targetConfigurationKey: "Jwt:Key",
            parameterPropertyName: "Key");

        if (overrides.Count > 0)
        {
            configuration.AddInMemoryCollection(overrides);
        }
    }

    /// <summary>
    /// Tenta resolver um parametro seguro e copia-lo para a chave de configuracao de destino.
    /// A ordem de prioridade respeita valor ja presente na configuracao, fallback por variavel
    /// de ambiente e, por ultimo, leitura do Parameter Store.
    /// </summary>
    private static async Task TryAddParameterValueAsync(
        IConfiguration configuration,
        IDictionary<string, string?> overrides,
        IReadOnlyList<string> parameterNameConfigKeys,
        IReadOnlyList<string> parameterNameEnvironmentVariables,
        string fallbackEnvironmentVariable,
        string targetConfigurationKey,
        string parameterPropertyName)
    {
        if (!string.IsNullOrWhiteSpace(configuration[targetConfigurationKey]) ||
            !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(fallbackEnvironmentVariable)))
        {
            return;
        }

        var parameterName = ResolveParameterName(configuration, parameterNameConfigKeys, parameterNameEnvironmentVariables);
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return;
        }

        try
        {
            using var client = new AmazonSimpleSystemsManagementClient();
            var response = await client.GetParameterAsync(new GetParameterRequest
            {
                Name = parameterName,
                WithDecryption = true
            });

            if (string.IsNullOrWhiteSpace(response.Parameter?.Value))
            {
                return;
            }

            overrides[targetConfigurationKey] = ExtractParameterValue(response.Parameter.Value, parameterPropertyName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Falha ao carregar o parametro '{parameterName}' para '{targetConfigurationKey}'.",
                ex);
        }
    }

    /// <summary>
    /// Resolve o nome do parametro considerando primeiro as variaveis de ambiente e depois a configuracao.
    /// Mantem compatibilidade temporaria com chaves antigas para evitar quebra durante a migracao.
    /// </summary>
    private static string? ResolveParameterName(
        IConfiguration configuration,
        IReadOnlyList<string> parameterNameConfigKeys,
        IReadOnlyList<string> parameterNameEnvironmentVariables)
    {
        foreach (var environmentVariable in parameterNameEnvironmentVariables)
        {
            var environmentValue = Environment.GetEnvironmentVariable(environmentVariable);
            if (!string.IsNullOrWhiteSpace(environmentValue))
            {
                return NormalizeParameterName(environmentValue);
            }
        }

        foreach (var configKey in parameterNameConfigKeys)
        {
            var configValue = configuration[configKey];
            if (!string.IsNullOrWhiteSpace(configValue))
            {
                return NormalizeParameterName(configValue);
            }
        }

        return null;
    }

    /// <summary>
    /// Garante compatibilidade com a convensao antiga da plataforma, que guardava o nome logico
    /// sem a barra inicial exigida pelo Parameter Store.
    /// </summary>
    private static string NormalizeParameterName(string parameterName)
    {
        var trimmedName = parameterName.Trim();
        if (trimmedName.Contains('/') && !trimmedName.StartsWith('/'))
        {
            return "/" + trimmedName;
        }

        return trimmedName;
    }

    /// <summary>
    /// Extrai do valor do parametro a propriedade esperada para a configuracao de destino.
    /// Quando o parametro nao for um JSON de objeto, o metodo devolve a string completa.
    /// </summary>
    private static string ExtractParameterValue(string parameterValue, string parameterPropertyName)
    {
        try
        {
            using var document = JsonDocument.Parse(parameterValue);
            if (document.RootElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in document.RootElement.EnumerateObject())
                {
                    if (string.Equals(property.Name, parameterPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return property.Value.GetString() ?? string.Empty;
                    }
                }
            }
        }
        catch (JsonException)
        {
        }

        return parameterValue;
    }
}
