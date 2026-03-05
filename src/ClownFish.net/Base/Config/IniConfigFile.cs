namespace ClownFish.Base.Config;

// 为了保持依赖简单，这里没有引用任何第三方INI解析库，而是自己实现一个简单的INI文件读取功能。
// 目前只支持【基本的】读取功能，不支持写入。
// INI文件格式参考：https://en.wikipedia.org/wiki/INI_file

/// <summary>
/// 读取 INI 配置文件的工具类
/// </summary>
public static class IniConfigFile
{
    private static readonly CacheDictionary<IniConfigData> s_cache = new CacheDictionary<IniConfigData>(4);

    /// <summary>
    /// 读取一个INI配置文件
    /// </summary>
    /// <param name="iniFilePath"></param>
    /// <param name="cacheSeconds">结果缓存多长时间（单位：秒）</param>
    /// <returns></returns>
    public static IniConfigData LoadFile(string iniFilePath, int cacheSeconds = 0)
    {
        if( File.Exists(iniFilePath) == false )
            return null;

        IniConfigData result = s_cache.Get(iniFilePath);
        if( result != null )
            return result;


        string body = File.ReadAllText(iniFilePath, Encoding.UTF8);
        IniConfigData data = LoadText(body);

        if( cacheSeconds > 0 ) {
            s_cache.Set(iniFilePath, data, DateTime.Now.AddSeconds(cacheSeconds));
        }

        return data;
    }

    /// <summary>
    /// 读取一段INI格式的文本内容
    /// </summary>
    /// <param name="iniText"></param>
    /// <returns></returns>
    public static IniConfigData LoadText(string iniText)
    {
        if( string.IsNullOrEmpty(iniText) )
            return null;

        using StringReader reader = new StringReader(iniText);
        return Load0(reader);
    }


    internal static IniConfigData Load0(TextReader reader)
    {
        IniConfigData config = new IniConfigData();

        IniSection currentSection = null;

        string line = null;
        while( true ) {
            line = reader.ReadLine();
            if( line == null )
                break;

            line = line.TrimStart();

            if( line.IsNullOrEmpty() )
                continue;

            if( line[0] == '#' || line[0] == ';' )   // 注释行
                continue;

            if( line[0] == '[' && line[line.Length - 1] == ']' ) {   // 新的节开始
                if( line.Length < 3 )
                    continue;   // 无效的节名称

                string name = line.Substring(1, line.Length - 2).Trim();

                //IniSection section = config.GetSection(name);
                //if( section != null ) {
                //    currentSection = section;   // 已经存在该节，则继续使用它
                //    continue;
                //}

                currentSection = new IniSection { Name = name };
                config.AddSection(currentSection);
                continue;
            }

            // 如果某个KV没有包含在配置节中，则忽略它

            if( currentSection != null ) {

                // 解析键值对，这里不支持转义字符及其它复杂(不常用)特性
                // 如果是复杂类型，可用 json or base64 来表示
                NameValue nv = NameValue.Parse(line, '=');
                if( nv != null ) {
                    currentSection.AddItem(nv.Name, nv.Value);
                }
            }
        }

        return config;
    }
}



/// <summary>
/// 从一个INI文件中读取到的结果
/// </summary>
public sealed class IniConfigData
{
    private readonly Dictionary<string, IniSection> _sectionDict = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 所有配置数据
    /// </summary>
    public IReadOnlyDictionary<string, IniSection> Sections => _sectionDict;

    /// <summary>
    /// 获取所有配置节名称
    /// </summary>
    /// <returns></returns>
    public string[] GetSectionNames()
    {
        return (from s in _sectionDict select s.Key).ToArray();
    }

    /// <summary>
    /// 根据名称获取一个配置节
    /// </summary>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public IniSection GetSection(string sectionName)
    {
        return _sectionDict.TryGet(sectionName);
    }

    internal void AddSection(IniSection section)
    {
        _sectionDict[section.Name] = section;
    }
}

/// <summary>
/// 一个INI配置节
/// </summary>
public sealed class IniSection
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public string Name { get; internal set; }

    private readonly Dictionary<string, string> _itemDict = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 配置节中的所有键值对
    /// </summary>
    public IReadOnlyDictionary<string, string> Items => _itemDict;

    internal void AddItem(string name, string value)
    {
        _itemDict[name] = value;
    }

    /// <summary>
    /// 获取某个配置参数值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public string GetValue(string key, string defaultValue = null)
    {
        return _itemDict.TryGetValue(key, out var value) ? value : defaultValue;
    }


    /// <summary>
    /// 从配置节中获取一个对象，属性名称与配置项名称对应，属性值从配置项字符串值转换而来
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prefix"></param>
    /// <returns></returns>
    [UnconditionalSuppressMessage("Trimming", "IL2090: typeof(T).GetProperties")]
    public T GetObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string prefix) where T : new()
    {
        T obj = new T();

        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach( PropertyInfo p in properties ) {

            string key = prefix.HasValue() ? $"{prefix}_{p.Name}" : p.Name;
            string strVal = GetValue(key);
            if( strVal != null ) {
                try {
                    object pValue = StringConverter.ChangeType(strVal, p.PropertyType.GetRealType());
                    p.FastSetValue(obj, pValue);
                }
                catch( Exception ex ) {
                    throw new InvalidCastException($"类型转换失败，目标类型：{p.PropertyType.Name}，当前属性名： {p.Name}，当前字符串值：{strVal}", ex);
                }
            }
        }

        return obj;
    }

}

