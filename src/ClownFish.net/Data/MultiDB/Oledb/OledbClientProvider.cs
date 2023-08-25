#if NETFRAMEWORK

namespace ClownFish.Data;

internal class OledbClientProvider : BaseClientProvider
{
    public static readonly BaseClientProvider Instance = new OledbClientProvider();

    public override DatabaseType DatabaseType => DatabaseType.Unknow;

    public override DbProviderFactory ProviderFactory => System.Data.OleDb.OleDbFactory.Instance;

    public override string GetParamterName(string name, DbContext dbContext)
    {
        return name;
    }

    public override string GetParamterPlaceholder(string name, DbContext dbContext)
    {
        return "?";
    }
}

internal class OdbcClientProvider : BaseClientProvider
{
    public static readonly BaseClientProvider Instance = new OdbcClientProvider();

    public override DatabaseType DatabaseType => DatabaseType.Unknow;

    public override DbProviderFactory ProviderFactory => System.Data.Odbc.OdbcFactory.Instance;

    public override string GetParamterName(string name, DbContext dbContext)
    {
        return name;
    }

    public override string GetParamterPlaceholder(string name, DbContext dbContext)
    {
        return "?";
    }
}

#endif
