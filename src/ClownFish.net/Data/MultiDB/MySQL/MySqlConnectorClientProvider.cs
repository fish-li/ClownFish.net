namespace ClownFish.Data.MultiDB.MySQL;

internal sealed class MySqlConnectorClientProvider : BaseMySqlClientProvider
{
    public static readonly BaseClientProvider Instance = new MySqlConnectorClientProvider();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly PropertyInfo _exNumber;
    private readonly Type _connStringBuilderType;

    [UnconditionalSuppressMessage("Trimming", "IL2080: exceptionType.GetProperty")]
    internal MySqlConnectorClientProvider()
    {
        // 在 0.x 版本中，命名空间是 MySql.Data.MySqlClient， ClownFish 从 10.26.xx 版本开始不再支持老版本

        Type factoryType = Type.GetType("MySqlConnector.MySqlConnectorFactory, MySqlConnector", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);

        _connStringBuilderType = Type.GetType("MySqlConnector.MySqlConnectionStringBuilder, MySqlConnector", true, false);

        _exceptionType = Type.GetType("MySqlConnector.MySqlException, MySqlConnector", true, false);
        PropertyInfo p = _exceptionType.GetProperty("Number", BindingFlags.Instance | BindingFlags.Public);
        if( p == null )
            throw new RuntimeReflectionException($"没有找到属性：MySqlConnector.MySqlException.Number");

        _exNumber = p;
    }

    public override DbProviderFactory ProviderFactory => _dbProviderFactory;

    public override bool IsDuplicateInsertException(Exception ex)
    {
        //if( ex is MySqlConnector.MySqlException mysqlEx2 ) {
        //    return (mysqlEx2.Number == 1062);
        //}

        if( ex.GetType().IsCompatible(_exceptionType) ) {
            return (int)_exNumber.FastGetValue(ex) == 1062;
        }

        return false;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2077: Activator.CreateInstance")]
    public override string GetConnectionString(IDbConfig dbConfig, bool includeDatabase)
    {
        DbConnectionStringBuilder sb = (DbConnectionStringBuilder)Activator.CreateInstance(_connStringBuilderType);
        return BuildConnectionString(sb, dbConfig, includeDatabase);
    }
}
