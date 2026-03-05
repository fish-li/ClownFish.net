namespace ClownFish.Base;

/// <summary>
/// 读取配置文件的接口
/// </summary>
public interface IConfigFile
{
    /// <summary>
    /// 读取一个配置文件的全部内容
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    string GetFile(string filename, bool checkExist);
}


/// <summary>
/// 与配置文件相关的工具类
/// </summary>
public static class ConfigFile
{
    /// <summary>
    /// 默认的 AppConfig 文件名
    /// </summary>
    public static string AppConfigFileName => AsmHelper.GetExeName() + ".config.ini";

    /// <summary>
    /// 默认的 LogConfig 文件名
    /// </summary>
    public static string LogConfigFileName => AsmHelper.GetExeName() + ".logconfig.ini";


    private static IConfigFile s_instance = DefaultConfigFileImpl.Instance;

    /// <summary>
    /// 设置实现方式
    /// </summary>
    /// <param name="instance"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetImpl(IConfigFile instance)
    {
        s_instance = instance ?? DefaultConfigFileImpl.Instance;
    }


    /// <summary>
    /// 从配置服务或者本地目录中获取指定的配置文件内容
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static string GetFile(string filename, bool checkExist = false)
    {
        return s_instance.GetFile(filename, checkExist);
    }

}


internal sealed class DefaultConfigFileImpl : IConfigFile
{
    public static readonly DefaultConfigFileImpl Instance = new DefaultConfigFileImpl();

    public string GetFile(string filename, bool checkExist)
    {
        if( string.IsNullOrEmpty(filename) )
            throw new ArgumentNullException(nameof(filename));

        // 先从内存中读取
        string fileBody = MemoryConfig.GetFile(filename);
        if( fileBody.HasValue() ) {
            return fileBody;
        }

        // 从配置服务中读取
        fileBody = ConfigClient.Instance.GetConfigFile(filename);
        if( fileBody.HasValue() ) {
            return fileBody;
        }

        // 再尝试从本地目录中读取配置文件
        fileBody = GetLocalFile(filename);
        if( fileBody.HasValue() ) {
            return fileBody;
        }

        if( checkExist ) {
            throw new FileNotFoundException($"没有找到配置文件: {filename}");
        }
        else {
            return null;
        }
    }


    internal static string GetLocalFile(string filename)
    {
        return GetLocalFile2(filename, out string _);
    }

    internal static string GetLocalFile2(string filename, out string readFilePath)
    {
        // 尝试用绝对路径
        string filePath = PathUtils.GetFileAbsolutePath(filename);
        if( File.Exists(filePath) ) {
            readFilePath = filePath;
            return RetryFile.ReadAllText(filePath, Encoding.UTF8);
        }


        // 第二次使用绝对路径，并且加上一个固定的目录
        string filePath2 = Path.Combine(AppContext.BaseDirectory, "_config", filename);
        if( File.Exists(filePath2) ) {
            readFilePath = filePath2;
            return RetryFile.ReadAllText(filePath2, Encoding.UTF8);
        }

        readFilePath = null;
        return null;
    }
}

