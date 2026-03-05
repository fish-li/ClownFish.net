namespace ClownFish.Base;

/// <summary>
/// 用于读取 KV 配置文件（env/properties） 的工具类
/// </summary>
public static class KvConfigFile
{
    /// <summary>
    /// 从一个配置文件中加载所有配置参数到字典集合
    /// </summary>
    /// <param name="configFilePath"></param>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static int LoadFile(string configFilePath, Dictionary<string, string> dict)
    {
        if( File.Exists(configFilePath) == false )
            return -1;

        if( dict == null )
            return -2;

        using FileStream fileStream = RetryFile.OpenRead(configFilePath);
        using StreamReader reader = new StreamReader(fileStream, Encoding.UTF8, true);

        return Load0(reader, dict);
    }


    /// <summary>
    /// 从一段配置文本中加载所有配置参数到字典集合
    /// </summary>
    /// <param name="text"></param>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static int LoadText(string text, Dictionary<string, string> dict)
    {
        if( text.IsNullOrEmpty() )
            return -1;

        if( dict == null )
            return -2;

        using StringReader reader = new StringReader(text);
        return Load0(reader, dict);
    }


    private static int Load0(TextReader reader, Dictionary<string, string> dict)
    {
        int count = 0;

        string line = null;
        while( true ) {
            line = reader.ReadLine();
            if( line == null )
                break;

            line = line.TrimStart();

            if( line.IsNullOrEmpty() )
                continue;

            if( line[0] == '#' || line.StartsWith0("//") )   // 注释行
                continue;

            NameValue nv = NameValue.Parse(line, '=');
            if( nv != null ) {
                dict[nv.Name] = nv.Value;    // 如果 KEY 重复，【后出现】的配置会 覆盖【前面出现】的配置
                count++;
            }
        }
        return count;
    }
}
