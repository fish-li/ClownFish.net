namespace ClownFish.Base;

internal static class PathUtils
{
    // 补充说明：在 .netcore 及以后版本中  AppDomain.CurrentDomain.BaseDirectory => AppContext.BaseDirectory
    //          在 .netframework 中      AppContext.BaseDirectory =>  AppDomain.CurrentDomain.BaseDirectory

    /// <summary>
    /// 根据指定的相对路径，尝试获取配置文件的绝对路径。
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetFileAbsolutePath(string relativePath)
    {
        if( string.IsNullOrEmpty(relativePath) )
            throw new ArgumentNullException(nameof(relativePath));

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }


    /// <summary>
    /// 根据指定的相对路径，获取配置目录的绝对路径。
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetDirectoryAbsolutePath(string relativePath)
    {
        if( string.IsNullOrEmpty(relativePath) )
            throw new ArgumentNullException(nameof(relativePath));

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
