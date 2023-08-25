namespace ClownFish.Data.MultiDB.MySQL;

internal sealed class MySqlConnectorClientProvider : BaseMySqlClientProvider
{
    public static readonly BaseClientProvider Instance = new MySqlConnectorClientProvider();
    
    private readonly DbProviderFactory _dbProviderFactory;
    private readonly string _namespace;
    private readonly Type _exceptionType;
    private readonly IGetValue _getter;

    internal MySqlConnectorClientProvider()
    {
        Type factoryType = Type.GetType("MySqlConnector.MySqlConnectorFactory, MySqlConnector", false, false);

        if( factoryType != null ) {
            _namespace = "MySqlConnector";
        }
        else {
            // 再按老版本的方式尝试查找
            // 在 0.x 版本中，命名空间是 MySql.Data.MySqlClient
            factoryType = Type.GetType("MySql.Data.MySqlClient.MySqlClientFactory, MySqlConnector", false, false);

            if( factoryType != null ) {
                _namespace = "MySql.Data.MySqlClient";
            }
            else {
                // 抛出异常，注意第2个参数
                _ = Type.GetType("MySqlConnector.MySqlConnectorFactory, MySqlConnector", true, false);
            }
        }


        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);


        _exceptionType = Type.GetType(_namespace + ".MySqlException, MySqlConnector", true, false);
        PropertyInfo p = _exceptionType.GetProperty("Number", BindingFlags.Instance | BindingFlags.Public);
        if( p == null )
            throw new InvalidOperationException($"没有找到属性：{_namespace}.MySqlException.Number");

        _getter = GetterSetterFactory.GetPropertyGetterWrapper(p);
    }

    public override DbProviderFactory ProviderFactory => _dbProviderFactory;

    public override bool IsDuplicateInsertException(Exception ex)
    {
        //if( ex is MySqlConnector.MySqlException mysqlEx2 ) {
        //    return (mysqlEx2.Number == 1062);
        //}

        if( ex.GetType().IsCompatible(_exceptionType) ) {
            return (int)_getter.Get(ex) == 1062;
        }

        return false;
    }
}
