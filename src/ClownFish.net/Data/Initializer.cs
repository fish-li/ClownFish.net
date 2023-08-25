using ClownFish.Data.CodeDom;
using ClownFish.Data.MultiDB.DaMeng;
using ClownFish.Data.MultiDB.MsSQL;
using ClownFish.Data.MultiDB.MySQL;
using ClownFish.Data.MultiDB.PostgreSQL;
using ClownFish.Data.MultiDB.SQLite;
using ClownFish.Data.Xml;

namespace ClownFish.Data;

/// <summary>
/// 初始化接口封装类
/// </summary>
public sealed class Initializer
{
    private Initializer()
    {
    }

    /// <summary>
    /// Initializer的实例引用
    /// </summary>
    public static readonly Initializer Instance = new Initializer();


    /// <summary>
    /// 注册数据库客户端提供者实例
    /// </summary>
    /// <param name="providerName"></param>
    /// <param name="provider"></param>
    public Initializer RegisterClientProvider(string providerName, BaseClientProvider provider)
    {
        DbClientFactory.RegisterProvider(providerName, provider);

        return this;
    }

    /// <summary>
    /// 注册 SQLSERVER 客户端提供者
    /// </summary>
    /// <param name="flag">System.Data.SqlClient = 1 / Microsoft.Data.SqlClient = 2</param>
    /// <returns></returns>
    public Initializer RegisterSqlServerProvider(int flag = 1)
    {
        if( flag == 1 ) {
            // 在 .net framework 环境下，System.Data.SqlClient 会自动注册，所以不需要做什么

#if NETCOREAPP
            DbClientFactory.RegisterProvider(DatabaseClients.SqlClient, MsSqlClientProvider.Instance);
#endif
        }

        if( flag == 2 ) {
            // 这里注册2个名称，是为了【没办法的兼容性】
            // 有些场景下，直接使用了【默认值】，所以没办法做到根据这里的注册来切换            
            DbClientFactory.RegisterProvider(DatabaseClients.SqlClient, MsSqlClientProvider2.Instance);
            DbClientFactory.RegisterProvider(DatabaseClients.SqlClient2, MsSqlClientProvider2.Instance);
        }

        return this;
    }


    /// <summary>
    /// 注册 MySQL 客户端提供者
    /// </summary>
    /// <param name="flag">MySql.Data = 1 / MySqlConnector = 2 / both = 3 / auto = 0</param>
    /// <returns></returns>
    public Initializer RegisterMySqlProvider(int flag = 0)
    {
        MySqlUtils.RegisterProvider(flag);

        return this;
    }


    /// <summary>
    /// 注册 PostgreSQL 客户端提供者
    /// </summary>
    /// <returns></returns>
    public Initializer RegisterPostgreSqlProvider()
    {
        // Npgsql 6.0 对时间戳的映射方式进行了一些重要更改
        // https://www.npgsql.org/doc/types/datetime.html#timestamps-and-timezones
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        DbClientFactory.RegisterProvider(DatabaseClients.PostgreSQL, PostgreSqlClientProvider.Instance);

        return this;
    }


    /// <summary>
    /// 注册 SQLite 客户端提供者
    /// </summary>
    /// <returns></returns>
    public Initializer RegisterSQLiteProvider()
    {
        DbClientFactory.RegisterProvider(DatabaseClients.SQLite, SQLiteClientProvider.Instance);

        return this;
    }


    /// <summary>
    /// 注册 达梦 客户端提供者
    /// </summary>
    /// <returns></returns>
    public Initializer RegisterDamengProvider()
    {
        DbClientFactory.RegisterProvider(DatabaseClients.DaMeng, DaMengClientProvider.Instance);

        return this;
    }

    /// <summary>
    /// 默认的实例列表长度
    /// </summary>
    internal int ListInitLength { get; private set; } = 50;


    /// <summary>
    /// 设置默认的实例列表长度
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public Initializer SetListInitLength(int length)
    {
        if( length < 1 )
            throw new ArgumentOutOfRangeException("length");

        ListInitLength = length;
        return this;
    }

    /// <summary>
    /// 从配置文件中加载所有数据库连接配置
    /// </summary>
    /// <returns></returns>
    public Initializer InitConnection()
    {
        //ConnectionManager.Init(configFilePath);
        AppConfig.Init();  // 其实也可以不调用

        return this;
    }


    /// <summary>
    /// 从指定的目录中加载所有 XmlCommand 配置
    /// </summary>
    /// <param name="directoryPath">包含XmlCommand配置文件的目录，如果不指定就表示接受XmlCommand规范的默认目录</param>
    /// <returns></returns>
    public Initializer LoadXmlCommandFromDirectory(string directoryPath = null)
    {
        // 如果不指定目录，就采用XmlCommand规范的默认目录
        if( string.IsNullOrEmpty(directoryPath) )
            directoryPath = ConfigHelper.GetDirectoryAbsolutePath(@"App_Data/XmlCommand");

        if( Directory.Exists(directoryPath) )
            XmlCommandManager.Instance.LoadFromDirectory(directoryPath);

        // 如果目录不存在，就忽略，而不是抛异常
        // 因为这样便于写成【固定的启动代码】，在开发的早期阶段，这个目录也许就是没有，但是后来可以创建它

        return this;
    }

    /// <summary>
    /// 加载XML字符串中包含的 XmlCommand
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public Initializer LoadXmlCommandFromText(string xml)
    {
        XmlCommandManager.Instance.LoadFromText(xml);
        return this;
    }



    /// <summary>
    /// 自动搜索当前程序加载的所有实体类型，并为它们编译生成代理类型及注册。
    /// 如果已存在代理程序集，会直接加载，不会再次编译。
    /// </summary>
    /// <param name="dllOutPath"></param>
    /// <param name="useAttrFilter"></param>
    /// <returns></returns>
    public Initializer CompileAllEntityProxy(string dllOutPath, bool useAttrFilter = true)
    {
        ProxyBuilder.CompileAllEntityProxy(dllOutPath, useAttrFilter);
        return this;
    }


    /// <summary>
    /// 注册IDataFieldTypeHandler
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Initializer AddDataFieldTypeHandler(Type dataType, IDataFieldTypeHandler handler)
    {
        DataFieldTypeHandlerFactory.Add(dataType, handler);
        return this;
    }

}
