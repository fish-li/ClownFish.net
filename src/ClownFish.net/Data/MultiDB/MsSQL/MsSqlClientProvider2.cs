namespace ClownFish.Data.MultiDB.MsSQL;

// 这个实现是为了支持 Microsoft.Data.SqlClient
// 根据微软文档， Microsoft.Data.SqlClient 是 System.Data.SqlClient 的可替代版本，它会包含更多的新特性，
// https://learn.microsoft.com/zh-cn/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace?view=sql-server-ver16

// 然而它的新版本的兼容性并不好！ 尤其是 4.0 之后，原本正常运行的程序就不能运行了，会出现以下异常：
// Microsoft.Data.SqlClient.SqlException:
//   A connection was successfully established with the server, but then an error occurred during the login process.
//   (provider: SSL Provider, error: 0 - 证书链是由不受信任的颁发机构颁发的。)
//   ---> System.ComponentModel.Win32Exception: 证书链是由不受信任的颁发机构颁发的。

// 参考 https://learn.microsoft.com/zh-cn/troubleshoot/sql/database-engine/connect/certificate-chain-not-trusted?tabs=ole-db-driver-19
// 增加参数：TrustServerCertificate=true 并没有作用，设置 Encrypt=False 也不起作用，
// 将版本下降到  3.1.3 即可解决！

// 既然是这样，那就没有必要内建支持 Microsoft.Data.SqlClient 了，因为这货又有一堆依赖，
// 所以，ClownFish.net 没有添加对 Microsoft.Data.SqlClient 的依赖引用，
// 为了一些老项目的兼容，这里采用反射方式提供支持


internal sealed class MsSqlClientProvider2 : BaseMsSqlClientProvider
{
    public static readonly BaseClientProvider Instance = new MsSqlClientProvider2();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly IGetValue _getter;

    internal MsSqlClientProvider2()
    {
        Type factoryType = Type.GetType("Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);

        _exceptionType = Type.GetType("Microsoft.Data.SqlClient.SqlException, Microsoft.Data.SqlClient", true, false);
        PropertyInfo p = _exceptionType.GetProperty("Number");
        if( p == null )
            throw new InvalidOperationException("没有找到属性：Microsoft.Data.SqlClient.SqlException.Number");

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
