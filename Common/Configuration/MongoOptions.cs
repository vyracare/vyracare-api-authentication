namespace Vyracare.Auth.Common.Configuration;

/// <summary>
/// Representa as configurações usadas para abrir a conexão da API com o MongoDB.
/// A connection string normalmente é injetada por secret, enquanto o nome do banco varia por ambiente.
/// </summary>
public sealed class MongoOptions
{
    /// <summary>
    /// Nome da seção de configuração usada para popular esta classe.
    /// </summary>
    public const string SectionName = "Mongo";

    /// <summary>
    /// Obtém ou define a string de conexão usada para criar o cliente do MongoDB.
    /// Em produção, esse valor deve ser obtido do Secrets Manager ou de variáveis de ambiente.
    /// </summary>
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";

    /// <summary>
    /// Obtém ou define o nome do banco de dados usado pela aplicação.
    /// A esteira publica <c>vyracare_db</c> em produção e <c>vyracare_db_dev</c> em desenvolvimento.
    /// </summary>
    public string Database { get; set; } = "vyracare_db";
}
