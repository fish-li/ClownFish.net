namespace ClownFish.Log;

/// <summary>
/// 文件操作工具类
/// </summary>
internal static class FileUtils
{
    private static bool s_inited = false;

    /// <summary>
    /// 日志文件的根目录，包含反斜杠。
    /// </summary>
    internal static string RootPath { get; private set; }

    /// <summary>
    /// 单个文件的最大长度（单位：byte）
    /// </summary>
    internal static long MaxLength { get; private set; }


    internal static int MaxCount { get; private set; }


    /// <summary>
    /// 初始化文件的保存目录
    /// </summary>
    public static void InitDirectory(LogConfiguration config)
    {
        // 这个方法被 FileWriter 调用，但 FileWriter 是个基类，所以可能会被多次调用。
        if( s_inited )
            return;

        FileUtils.MaxLength = config.File.MaxLength.ParseLength();
        FileUtils.MaxCount = config.File.MaxCount;

        // 支持绝对路径，和相对路径
        string rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.File.RootPath);


        // 检查日志根目录是否存在
        if( Directory.Exists(rootPath) == false )
            Directory.CreateDirectory(rootPath);


        rootPath = rootPath.TrimEnd('\\', '/') + "/";


        // 检查需要记录的各个数据类型的子目录是否存在。
        foreach( var item in config.Types ) {
            string path = rootPath + item.TypeObject.Name;
            if( Directory.Exists(path) == false )
                Directory.CreateDirectory(path);
        }

        // 完整的日志根目录
        FileUtils.RootPath = rootPath;

        // 标记初始化已完成
        s_inited = true;
    }


    

}
