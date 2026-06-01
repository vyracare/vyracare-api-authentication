namespace Vyracare.Auth.Common.Configuration;

/// <summary>
/// Representa as opções de configuração carregadas da aplicação.
/// </summary>
public sealed class MongoOptions
{
    public const string SectionName = "Mongo";

/// <summary>
/// Obtém ou define a string de conexão usada para acessar o banco de dados.
/// </summary>
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
/// <summary>
/// Obtém ou define o nome do banco de dados utilizado pela aplicação.
/// </summary>
    public string Database { get; set; } = "vyracare_db";
}
