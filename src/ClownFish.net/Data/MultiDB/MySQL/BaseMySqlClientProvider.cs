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
}
