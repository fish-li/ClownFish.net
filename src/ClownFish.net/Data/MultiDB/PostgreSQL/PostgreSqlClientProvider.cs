namespace ClownFish.Data.MultiDB.PostgreSQL;

internal class PostgreSqlClientProvider : BaseClientProvider
{
    public static readonly BaseClientProvider Instance = new PostgreSqlClientProvider();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly IGetValue _getter;

    internal PostgreSqlClientProvider()
    {
        Type factoryType = Type.GetType("Npgsql.NpgsqlFactory, Npgsql", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);


        _exceptionType = Type.GetType("Npgsql.PostgresException, Npgsql", true, false);
        PropertyInfo p = _exceptionType.GetProperty("SqlState");
        if( p == null )
            throw new InvalidOperationException("没有找到属性：Npgsql.PostgresException.SqlState");

        _getter = GetterSetterFactory.GetPropertyGetterWrapper(p);
    }

    public override DatabaseType DatabaseType => DatabaseType.PostgreSQL;

    public override DbProviderFactory ProviderFactory => _dbProviderFactory;


    public override string GetObjectFullName(string symbol)
    {
        return "\"" + symbol + "\"";
    }

    public override CPQuery GetNewIdQuery(CPQuery query, object entity)
    {
        return query + "; SELECT lastval();";
    }

    public override bool IsDuplicateInsertException(Exception ex)
    {
        //if( ex is PostgresException npgEx ) {
        //    // eg. "23505: duplicate key value violates unique constraint "test_insert_pkey"
        //    return (npgEx.SqlState == "23505");
        //}

        if( ex.GetType().IsCompatible(_exceptionType) ) {
            return (string)_getter.Get(ex) == "23505";
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
