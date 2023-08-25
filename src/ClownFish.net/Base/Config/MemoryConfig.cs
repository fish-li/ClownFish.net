namespace ClownFish.Base;

/// <summary>
/// 内存中的配置参数，用于配置服务不启用的场景。
/// </summary>
public static class MemoryConfig
{
    private static readonly TSafeDictionary<string, string> s_settings = new(256);

    private static readonly TSafeDictionary<string, DbConfig> s_db = new(128);

    private static readonly TSafeDictionary<string, string> s_files = new(32);

    /// <summary>
    /// 添加一个 key=value 的配置参数，如果指定的name存在则覆盖。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public static void AddSetting(string name, string value)
    {
        if( name.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(name));
        if( value == null )
            throw new ArgumentNullException(nameof(value));

        s_settings.Set(name, value);
    }

    /// <summary>
    /// 获取某个配置参数
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetSetting(string name)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        return s_settings.TryGet(name);
    }


    /// <summary>
    /// 添加一个数据库连接配置，如果指定的name存在则覆盖。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="config"></param>
    public static void AddDbConfig(string name, DbConfig config)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));
        if( config == null )
            throw new ArgumentNullException(nameof(config));

        s_db.Set(name, config);
    }


    /// <summary>
    /// 获取一个数据库连接配置
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static DbConfig GetDbConfig(string name)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        return s_db.TryGet(name)?.Clone();
    }

    /// <summary>
    /// 添加一个配置文件，如果指定的name存在则覆盖。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fileText"></param>
    public static void AddFile(string name, string fileText)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));
        if( fileText == null )
            throw new ArgumentNullException(nameof(fileText));

        s_files.Set(name, fileText);
    }

    /// <summary>
    /// 获取一个配置文件的内容
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetFile(string name)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        return s_files.TryGet(name);
    }


    /// <summary>
    /// 设置 AppConfig 的文件内容
    /// </summary>
    /// <param name="xml"></param>
    public static void SetAppConfig(string xml)
    {
        if( xml == null )
            throw new ArgumentNullException(nameof(xml));

        string filename = ConfigFile.AppConfigFileName;
        AddFile(filename, xml);
    }


    /// <summary>
    /// 设置 LogConfig 的文件内容
    /// </summary>
    /// <param name="xml"></param>
    public static void SetLogConfig(string xml)
    {
        if( xml == null )
            throw new ArgumentNullException(nameof(xml));

        string filename = ConfigFile.LogConfigFileName;
        AddFile(filename, xml);
    }

}
