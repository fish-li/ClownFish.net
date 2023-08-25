namespace ClownFish.Data.MultiDB.MySQL;

internal sealed class MySqlDataClientProvider : BaseMySqlClientProvider
{
    public static readonly BaseClientProvider Instance = new MySqlDataClientProvider();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly IGetValue _getter;

    internal MySqlDataClientProvider()
    {
        Type factoryType = Type.GetType("MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);


        _exceptionType = Type.GetType("MySql.Data.MySqlClient.MySqlException, MySql.Data", true, false);
        PropertyInfo p = _exceptionType.GetProperty("Number");
        if( p == null )
            throw new InvalidOperationException("没有找到属性：MySql.Data.MySqlClient.MySqlException.Number");

        _getter = GetterSetterFactory.GetPropertyGetterWrapper(p);
    }

    public override DbProviderFactory ProviderFactory => _dbProviderFactory;

    public override bool IsDuplicateInsertException(Exception ex)
    {
        //if( ex is MySql.Data.MySqlClient.MySqlException mysqlEx1 ) {
        //    // eg.  "Duplicate entry '8a831fbb-44f5-4a01-9d29-014d210c97ea' for key 'IX_RowId'"
        //    return (mysqlEx1.Number == 1062);
        //}

        if( ex.GetType().IsCompatible(_exceptionType) ) {
            return (int)_getter.Get(ex) == 1062;
        }

        return false;
    }
}
