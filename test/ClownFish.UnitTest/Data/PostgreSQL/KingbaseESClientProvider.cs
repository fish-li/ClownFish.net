#if NET8_0_OR_GREATER

namespace ClownFish.UnitTest.Data.PostgreSQL;

using ClownFish.Data.MultiDB.PostgreSQL;
using Kdbndp;


/// <summary>
/// 用  人大金仓-KingbaseES 数据库来展示如何实现 BaseClientProvider
/// </summary>
internal class KingbaseESClientProvider : BaseClientProvider
{
    public static readonly BaseClientProvider Instance = new KingbaseESClientProvider();

    #region 定义2个"常量"，可以避免在其它代码中出现“硬编码”。
    public static readonly string ProviderName = "Kdbndp";

    // 由于 DatabaseType 是枚举，无法扩展，所以只能使用“强转”方式
    public static readonly DatabaseType DatabaseTypeKingbaseES = (DatabaseType)7777;
    #endregion


    public static void RegisterProvider()
    {
        // Npgsql 6.0 对时间戳的映射方式进行了一些重要更改
        // https://www.npgsql.org/doc/types/datetime.html#timestamps-and-timezones
        AppContext.SetSwitch("Kdbndp.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Kdbndp.DisableDateTimeInfinityConversions", true);

        DbClientFactory.RegisterProvider(ProviderName, Instance);
    }

    public override DatabaseType DatabaseType => DatabaseTypeKingbaseES;

    public override DbProviderFactory ProviderFactory => Kdbndp.KdbndpFactory.Instance;

    public override string GetObjectFullName(string symbol)
    {
        // https://help.kingbase.com.cn/v8/faq/faq-new/sql.html#id12
        return "\"" + symbol + "\"";
    }


    public override CPQuery GetNewIdQuery(CPQuery query, object entity)
    {
        return query + "; SELECT lastval();";  // 参考 PostgreSqlClientProvider
    }


    public override bool IsDuplicateInsertException(Exception ex)
    {
        if( ex is Kdbndp.KingbaseException ex2 ) {
            return ex2.SqlState == "23505";  // 参考 PostgreSqlClientProvider
        }

        return false;
    }


    public override CPQuery SetPagedQuery(CPQuery query, int skip, int take)
    {
        return StdClientProvider.SetPagedQuery(query, skip, take);
    }

    public override Page2Query GetPagedCommand(BaseCommand query, PagingInfo pagingInfo)
    {
        return StdClientProvider.GetPagedCommand(query, pagingInfo);
    }

    public override string GetConnectionString(IDbConfig dbConfig, bool includeDatabase)
    {
        DbConnectionStringBuilder sb = new Npgsql.NpgsqlConnectionStringBuilder();
        return PostgreSqlClientProvider.BuildConnectionString(sb, dbConfig, includeDatabase);
    }
}

#endif
