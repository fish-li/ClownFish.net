namespace ClownFish.Base;

/// <summary>
/// 与配置文件相关的工具类
/// </summary>
public static class ConfigFile
{
    internal static readonly string AppConfigFileName = ClownFishBehavior.Instance.GetApplicationName() + ".App.Config";

    internal static readonly string LogConfigFileName = ClownFishBehavior.Instance.GetApplicationName() + ".Log.Config";

    /// <summary>
    /// 从配置服务或者本地目录中获取指定的配置文件内容
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static string GetFile(string filename, bool checkExist = false)
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
        string prefix = ClownFishBehavior.Instance.GetApplicationName() + ".";
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
