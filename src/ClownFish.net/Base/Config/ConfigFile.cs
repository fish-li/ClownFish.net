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


internal sealed class DefaultConfigFileImpl : IConfigFile
{
    public static readonly DefaultConfigFileImpl Instance = new DefaultConfigFileImpl();

    public string GetFile(string filename, bool checkExist)
    {
        if( string.IsNullOrEmpty(filename) )
            throw new ArgumentNullException(nameof(filename));

        // 先从内存中读取
        string fileBody = MemoryConfig.GetFile(filename);
        if( fileBody.HasValue() )
            return fileBody;

        // 从配置服务中读取
        fileBody = ConfigClient.Instance.GetConfigFile(filename);
        if( fileBody.HasValue() )
            return fileBody;

        // 再尝试从本地目录中读取配置文件
        fileBody = GetLocalFile(filename);
        if( fileBody.HasValue() )
            return fileBody;

        // 按简化的文件名再查找一次
        // 注意：由于ApplicationName允许用环境变量重新指定，那么在开发服务时，不可能指定一个完整的文件名，此时的查找会非常有用
        string prefix = EnvUtils.GetAppName() + ".";
        if( filename.StartsWithIgnoreCase(prefix) ) {
            string filename2 = filename.Substring(prefix.Length);

            fileBody = GetLocalFile(filename2);
            if( fileBody.HasValue() )
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
        string fileBody = null;

        // 尝试用绝对路径
        string filePath = ConfigHelper.GetFileAbsolutePath(filename);

        if( File.Exists(filePath) ) {
            fileBody = RetryFile.ReadAllText(filePath, Encoding.UTF8);
        }

        if( fileBody.IsNullOrEmpty() ) {

            // 第二次使用绝对路径，并且加上一个固定的目录
            filePath = Path.Combine(AppContext.BaseDirectory, "_config", filename);

            if( File.Exists(filePath) )
                fileBody = RetryFile.ReadAllText(filePath, Encoding.UTF8);
        }

        return fileBody;
    }
}

/// <summary>
/// 与配置文件相关的工具类
/// </summary>
public static class ConfigFile
{
    /// <summary>
    /// 默认的 App.Config 文件名
    /// </summary>
    public static string AppConfigFileName => EnvUtils.GetAppName() + ".App.Config";

    /// <summary>
    /// 默认的 Log.Config 文件名
    /// </summary>
    public static string LogConfigFileName => EnvUtils.GetAppName() + ".Log.Config";


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
