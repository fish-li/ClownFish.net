namespace ClownFish.Base;

/// <summary>
/// 处理与配置相关操作的工具类
/// </summary>
public static class ConfigHelper
{
    // 判断2个目录是否相同，
    // 一般情况下，假设 AppContext.BaseDirectory        :  /app/
    //            那么 Environment.CurrentDirectory    :  /app
    // 所以在比较时，要去掉结尾字符
    private static readonly bool s_isSame = AppContext.BaseDirectory.TrimEnd('/').TrimEnd('\\').Is(Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\'));

    // 补充说明：在 .netcore 及以后版本中  AppDomain.CurrentDomain.BaseDirectory => AppContext.BaseDirectory
    //          在 .netframework 中      AppContext.BaseDirectory =>  AppDomain.CurrentDomain.BaseDirectory

    /// <summary>
    /// 根据指定的相对路径，尝试获取配置文件的绝对路径。 如果尝试失败，返回NULL
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetFileAbsolutePath(string relativePath)
    {
        if( string.IsNullOrEmpty(relativePath) )
            throw new ArgumentNullException(nameof(relativePath));

        string path = Path.Combine(AppContext.BaseDirectory, relativePath);
        if( File.Exists(path) )
            return path;

        if( s_isSame == false ) {
            path = Path.Combine(Environment.CurrentDirectory, relativePath);
            if( File.Exists(path) )
                return path;
        }


        // 没找到约定的路径，不管了~~
        return path;
    }


    /// <summary>
    /// 根据指定的相对路径，获取配置目录的绝对路径。 如果尝试失败，返回NULL
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetDirectoryAbsolutePath(string relativePath)
    {
        if( string.IsNullOrEmpty(relativePath) )
            throw new ArgumentNullException(nameof(relativePath));

        string path = Path.Combine(AppContext.BaseDirectory, relativePath);
        if( Directory.Exists(path) )
            return path;

        if( s_isSame == false ) {
            path = Path.Combine(Environment.CurrentDirectory, relativePath);
            if( Directory.Exists(path) )
                return path;
        }

        // 这里不检查目录是否存，由调用方检查
        return path;
    }
}
