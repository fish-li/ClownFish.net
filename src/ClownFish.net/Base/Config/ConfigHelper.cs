namespace ClownFish.Base;

/// <summary>
/// 处理与配置相关操作的工具类
/// </summary>
public static class ConfigHelper
{
    private static readonly bool s_isSame = AppDomain.CurrentDomain.BaseDirectory.Is(Environment.CurrentDirectory);

    /// <summary>
    /// 根据指定的相对路径，尝试获取配置文件的绝对路径。 如果尝试失败，返回NULL
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetFileAbsolutePath(string relativePath)
    {
        if( string.IsNullOrEmpty(relativePath) )
            throw new ArgumentNullException(nameof(relativePath));

        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        if( File.Exists(path) )
            return path;

        if( s_isSame == false ) {
            path = Path.Combine(Environment.CurrentDirectory, relativePath);
            if( File.Exists(path) )
                return path;
        }


        // 没找到约定的路径，不管了~~
        return relativePath;
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

        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        if( Directory.Exists(path) )
            return path;

        if( s_isSame == false ) {
            path = Path.Combine(Environment.CurrentDirectory, relativePath);
            if( Directory.Exists(path) )
                return path;
        }

        // 这里不检查目录是否存，由调用方检查
        return relativePath;
    }
}
