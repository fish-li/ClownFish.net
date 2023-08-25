#if NETCOREAPP
namespace ClownFish.Data.MultiDB.MsSQL;

internal sealed class MsSqlClientProvider : BaseMsSqlClientProvider
{
    public static readonly BaseClientProvider Instance = new MsSqlClientProvider();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly IGetValue _getter;

    internal MsSqlClientProvider()
    {
        Type factoryType = Type.GetType("System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);

        _exceptionType = Type.GetType("System.Data.SqlClient.SqlException, System.Data.SqlClient", true, false);
        PropertyInfo p = _exceptionType.GetProperty("Number");
        if( p == null )
            throw new InvalidOperationException("没有找到属性：System.Data.SqlClient.SqlException.Number");

        _getter = GetterSetterFactory.GetPropertyGetterWrapper(p);
    }

    public override DbProviderFactory ProviderFactory => _dbProviderFactory;

    public override bool IsDuplicateInsertException(Exception ex)
    {
        if( ex.GetType().IsCompatible(_exceptionType) ) {
            int number = (int)_getter.Get(ex);

            return number == 2601 || number == 2627;
        }

        return false;
    }
}

#endif

