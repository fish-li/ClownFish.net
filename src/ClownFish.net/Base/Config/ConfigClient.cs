namespace ClownFish.Base;

/// <summary>
/// ConfigService（配置服务）的客户端工具类
/// </summary>
public sealed class ConfigClient
{
    /// <summary>
    /// 默认单例引用，用于实现静态调用。
    /// </summary>
    public static readonly ConfigClient Instance = new ConfigClient();

    private IConfigClient _client = NullValueClient.Instance;

    private IConfigClient GetClient() => _client;


    /// <summary>
    /// 当 EnableConfigService=false 时，指定一个替代的客户端实现
    /// </summary>
    /// <param name="client"></param>
    public void SetClient(IConfigClient client)
    {
        if( client == null )
            throw new ArgumentNullException("client");

        _client = client;
    }


    internal void ResetNull()
    {
        _client = NullValueClient.Instance;
    }


    // ############################################################################
    // 由于客户端内部有缓存，因此异步实现的意义并不大，反而会造成大量重复代码，
    // 所以这里的方法全部采用同步方式实现
    // ############################################################################



    #region Settings Methods



    /// <summary>
    /// 获取一个配置参数的值
    /// </summary>
    /// <param name="name"></param>
    /// <param name="checkExist">如果没有指定参数项，或者参数项目不存在，就需要抛出异常</param>
    /// <returns></returns>
    public string GetSetting(string name, bool checkExist = false)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        string value = GetClient().GetSetting(name);

        if( checkExist && value.IsNullOrEmpty() )
            throw new ConfigurationErrorsException("没有找到指定的配置参数，name：" + name);

        return value;
    }



    /// <summary>
    /// 从配置服务获取一个参数值，并将 key=value;key=value 形式的字符串转换成指定的类型
    /// </summary>
    /// <typeparam name="T">返回值的类型参数</typeparam>
    /// <param name="name">参数名称</param>
    /// <param name="checkExist">如果没有指定参数项，或者参数项目不存在，就需要抛出异常</param>
    /// <returns></returns>
    public T GetSetting<T>(string name, bool checkExist = false) where T : class, new()
    {
        string value = GetSetting(name, checkExist);
        T result = value.ToObject<T>();

        return result;
    }

    #endregion



    #region database connection methods



    /// <summary>
    /// 根据 连接名称 创建对应的数据库连接
    /// </summary>
    /// <param name="connName">数据库连接名称</param>
    /// <param name="checkExist">如果设置为true，当没有找到匹配的数据库连接配置时将抛出异常</param>
    /// <returns></returns>
    public DbConfig GetAppDbConfig(string connName, bool checkExist = true)
    {
        if( connName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(connName));

        DbConfig value = GetClient().GetAppDbConfig(connName);

        if( value == null && checkExist )
            throw new ConfigurationErrorsException("没有找到指定的数据库连接参数，connName：" + connName);

        return value;
    }



    /// <summary>
    /// 根据 连接类别和租户ID 创建对应的数据库连接
    /// </summary>
    /// <param name="connType">数据库连接类别，例如：xsql, influx</param>
    /// <param name="tenantId">租户ID</param>
    /// <param name="flag">额外参数，如果连接只读库，则指定 "_readonly"</param>
    /// <param name="checkExist">如果设置为true，当没有找到匹配的数据库连接配置时将抛出异常</param>
    /// <returns></returns>
    public DbConfig GetTntDbConfig(string connType, string tenantId, string flag = null, bool checkExist = true)
    {
        if( tenantId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(tenantId));

        if( connType.IsNullOrEmpty() )
            connType = "xsql";

        DbConfig tenant = GetClient().GetTntDbConfig(connType, tenantId, flag);

        if( tenant == null && checkExist ) 
             throw new ConfigurationErrorsException($"没有找到指定的数据库连接参数，connType='{connType}', tenantId='{tenantId}', flag='{flag}'");

        return tenant;
    }



    #endregion


    /// <summary>
    /// 获取某个配置文件的内容，用于替代应用程序目录下的配置文件。
    /// 一般而言，配置文件会在应用程序启动时加载，直到下次启动才会再次读取，所以此方法不做缓存设计。
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="checkExist">如果没有指定参数项，或者参数项目不存在，就需要抛出异常</param>
    /// <returns></returns>
    public string GetConfigFile(string filename, bool checkExist = false)
    {
        if( filename.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filename));

        string text = GetClient().GetConfigFile(filename);

        if( checkExist && text.IsNullOrEmpty() )
            throw new ConfigurationErrorsException("没有找到指定的配置文件或者文件内容为空，filename：" + filename);

        return text;
    }

}
