namespace ClownFish.Data.MultiDB.MySQL;

internal abstract class BaseMySqlClientProvider : BaseClientProvider
{
    public override DatabaseType DatabaseType => DatabaseType.MySQL;

    public override string GetObjectFullName(string symbol)
    {
        return "`" + symbol + "`";
    }

    public override CPQuery GetNewIdQuery(CPQuery query, object entity)
    {
        return query + "; SELECT LAST_INSERT_ID();";
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
        // 参考：https://www.connectionstrings.com/mysql/

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append("Server=").Append(dbConfig.Server);

            if( dbConfig.Port.HasValue && dbConfig.Port.Value > 0 )
                sb.Append(";Port=").Append(dbConfig.Port.Value);

            if( includeDatabase && dbConfig.Database.HasValue() )
                sb.Append(";Database=").Append(dbConfig.Database);

            sb.Append(";Uid=").Append(dbConfig.UserName)
                .Append(";Pwd=").Append(dbConfig.Password)
                .Append(';').Append(dbConfig.Args);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }
}
