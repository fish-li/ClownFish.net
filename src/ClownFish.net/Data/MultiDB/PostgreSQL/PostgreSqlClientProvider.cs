namespace ClownFish.Data.MultiDB.PostgreSQL;

internal class PostgreSqlClientProvider : BaseClientProvider
{
    public static readonly BaseClientProvider Instance = new PostgreSqlClientProvider();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly IGetValue _getter;

#if NETCOREAPP
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2057: DynamicallyAccessedMemberTypes")]
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2080: DynamicallyAccessedMemberTypes")]
#endif
    internal PostgreSqlClientProvider()
    {
        Type factoryType = Type.GetType("Npgsql.NpgsqlFactory, Npgsql", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);


        _exceptionType = Type.GetType("Npgsql.PostgresException, Npgsql", true, false);
        PropertyInfo p = _exceptionType.GetProperty("SqlState");
        if( p == null )
            throw new RuntimeReflectionException("没有找到属性：Npgsql.PostgresException.SqlState");

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

    public override string GetConnectionString(IDbConfig dbConfig, bool includeDatabase)
    {
        return GetPostgreSQLConnectionString0(dbConfig, includeDatabase);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string GetPostgreSQLConnectionString0(IDbConfig dbConfig, bool includeDatabase)
    {
        // PostgreSQL 连接参数
        // https://www.npgsql.org/doc/connection-string-parameters.html

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append("Host=").Append(dbConfig.Server);

            if( dbConfig.Port.HasValue && dbConfig.Port.Value > 0 )
                sb.Append(";Port=").Append(dbConfig.Port.Value);

            if( includeDatabase && dbConfig.Database.HasValue() )
                sb.Append(";Database=").Append(dbConfig.Database);

            sb.Append(";Username=").Append(dbConfig.UserName)
                .Append(";Password=").Append(dbConfig.Password)
                .Append(";Application Name=").Append(EnvUtils.GetAppName())
                .Append(';').Append(dbConfig.Args);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }
}
