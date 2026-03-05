namespace ClownFish.Base;

/// <summary>
/// 内存中的配置参数。
/// </summary>
public static class MemoryConfig
{
    private static readonly TSafeDictionary<string, string> s_settings = new(256, StringComparer.OrdinalIgnoreCase);

    private static readonly TSafeDictionary<string, DbConfig> s_db = new(128, StringComparer.OrdinalIgnoreCase);

    private static readonly TSafeDictionary<string, string> s_files = new(32, StringComparer.OrdinalIgnoreCase);

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
    /// 删除指定名称的配置参数
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool RemoveSetting(string name)
    {
        return s_settings.TryRemove(name, out var _);
    }

    /// <summary>
    /// 获取指定名称的配置参数
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
    /// 删除指定名称的数据库连接配置
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool RemoveDbConfig(string name)
    {
        return s_db.TryRemove(name, out var _);
    }

    /// <summary>
    /// 获取指定名称的数据库连接配置
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
    /// 删除指定名称的配置文件
    /// </summary>
    /// <param name="filename"></param>
    public static void RemoveFile(string filename)
    {
        s_files.TryRemove(filename, out string _);
    }


    internal static DebugReportBlock GetDebugReportBlock()
    {
        DebugReportBlock block = new DebugReportBlock { Category = nameof(MemoryConfig), Order = 100 };

        block.AppendLine("---Setting---");
        foreach( var name in s_settings.GetKeys()) {
            block.AppendLine($"{name} = {s_settings[name]?.ToString2()}");
        }

        block.AppendLine("---DbConfig---");
        foreach( var name in s_db.GetKeys() ) {
            block.AppendLine($"{name} = {s_db[name]?.ToString()}");
        }

        block.AppendLine("---Files---");
        foreach( var name in s_files.GetKeys() ) {
            block.AppendLine(name);
        }

        return block;
    }
}
