namespace Vyracare.Auth.Common.Configuration;

/// <summary>
/// Representa uma configura??o tipada lida do appsettings ou das vari?veis de ambiente.
/// </summary>
public sealed class MongoOptions
{
    public const string SectionName = "Mongo";

/// <summary>
/// Obt?m ou define c on ne ct io ns tr in g.
/// </summary>
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
/// <summary>
/// Obt?m ou define d at ab as e.
/// </summary>
    public string Database { get; set; } = "vyracare";
}
