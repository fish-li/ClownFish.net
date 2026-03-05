#if NETCOREAPP
namespace ClownFish.Data.MultiDB.MsSQL;

internal sealed class MsSqlClientProvider : BaseMsSqlClientProvider
{
    public static readonly BaseClientProvider Instance = new MsSqlClientProvider();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly PropertyInfo _exNumber;
    private readonly Type _connStringBuilderType;

    [UnconditionalSuppressMessage("Trimming", "IL2080: exceptionType.GetProperty")]
    internal MsSqlClientProvider()
    {
        Type factoryType = Type.GetType("System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);

        _connStringBuilderType = Type.GetType("System.Data.SqlClient.SqlConnectionStringBuilder, System.Data.SqlClient", true, false);

        _exceptionType = Type.GetType("System.Data.SqlClient.SqlException, System.Data.SqlClient", true, false);
        PropertyInfo p = _exceptionType.GetProperty("Number");
        if( p == null )
            throw new RuntimeReflectionException("没有找到属性：System.Data.SqlClient.SqlException.Number");

        _exNumber = p;
    }

    public override DbProviderFactory ProviderFactory => _dbProviderFactory;

    public override bool IsDuplicateInsertException(Exception ex)
    {
        if( ex.GetType().IsCompatible(_exceptionType) ) {
            int number = (int)_exNumber.FastGetValue(ex);

            return number == 2601 || number == 2627;
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

#endif

