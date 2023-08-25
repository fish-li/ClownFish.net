namespace ClownFish.Data;

/// <summary>
/// 一些常用的数据访问客户端 ProviderName
/// </summary>
public static class DatabaseClients
{
    /// <summary>
    /// string "System.Data.SqlClient"
    /// </summary>
    public static readonly string SqlClient = "System.Data.SqlClient";

    /// <summary>
    /// string "Microsoft.Data.SqlClient"
    /// </summary>
    public static readonly string SqlClient2 = "Microsoft.Data.SqlClient";

    /// <summary>
    /// string "MySql.Data.MySqlClient"
    /// </summary>
    public static readonly string MySqlClient = "MySql.Data.MySqlClient";

    /// <summary>
    /// string "Npgsql"
    /// </summary>
    public static readonly string PostgreSQL = "Npgsql";

    /// <summary>
    /// string "System.Data.SQLite"
    /// </summary>
    public static readonly string SQLite = "System.Data.SQLite";

    /// <summary>
    /// string "Oracle.ManagedDataAccess.Client"
    /// </summary>
    public static readonly string Oracle = "Oracle.ManagedDataAccess.Client";

    /// <summary>
    /// string "Dm"
    /// </summary>
    public static readonly string DaMeng = "Dm";
}
