namespace ClownFish.Data.MultiDB.SQLite;

internal class SQLiteClientProvider : BaseClientProvider
{
    public static readonly BaseClientProvider Instance = new SQLiteClientProvider();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly IGetValue _getter;

    // 说明：SQLite有2个驱动库：System.Data.SQLite,  Microsoft.Data.Sqlite
    // 这们不是一个东西！ 分别由不同的团队在维护。
    // 它们的由来可参考：https://docs.microsoft.com/zh-cn/dotnet/standard/data/sqlite/compare
    // ClownFish只支持 System.Data.SQLite

    internal SQLiteClientProvider()
    {
        Type factoryType = Type.GetType("System.Data.SQLite.SQLiteFactory, System.Data.SQLite", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);


        _exceptionType = Type.GetType("System.Data.SQLite.SQLiteException, System.Data.SQLite", true, false);
        PropertyInfo p = _exceptionType.GetProperty("ErrorCode");
        if( p == null )
            throw new InvalidOperationException("没有找到属性：System.Data.SQLite.SQLiteException.ErrorCode");

        _getter = GetterSetterFactory.GetPropertyGetterWrapper(p);
    }


    public override DatabaseType DatabaseType => DatabaseType.SQLite;

    public override DbProviderFactory ProviderFactory => _dbProviderFactory;

    public override string GetObjectFullName(string symbol)
    {
        return "[" + symbol + "]";
    }

    public override CPQuery GetNewIdQuery(CPQuery query, object entity)
    {
        return query + "; SELECT last_insert_rowid();";
    }

    public override bool IsDuplicateInsertException(Exception ex)
    {
        if( ex.GetType().IsCompatible(_exceptionType) ) {
            return (int)_getter.Get(ex) == 2067;
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

}
