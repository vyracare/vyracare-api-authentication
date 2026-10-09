using Microsoft.Extensions.Configuration;
using Vyracare.Auth.Infrastructure;

namespace Vyracare.Auth.Tests.Infrastructure;

public sealed class ParameterStoreBootstrapperTests
{
    [Fact]
    public async Task ApplyAsync_MapsFallbackEnvironmentVariableToTargetConfiguration()
    {
        const string variableName = "TENANCY_INTERNAL_API_KEY";
        const string expected = "local-internal-key";
        var previous = Environment.GetEnvironmentVariable(variableName);
        try
        {
            Environment.SetEnvironmentVariable(variableName, expected);
            var configuration = new ConfigurationManager
            {
                ["Mongo:ConnectionString"] = "mongodb://configured",
                ["Jwt:Key"] = "configured-jwt-key"
            };

            await ParameterStoreBootstrapper.ApplyAsync(configuration);

            Assert.Equal(expected, configuration["TenancyApi:InternalApiKey"]);
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, previous);
        }
    }
}
