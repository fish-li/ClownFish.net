using System.Runtime;

namespace ClownFish.Base;   // 很多项目会引用这个命名空间，所以用它会省事

/// <summary>
/// ClownFish初始化辅助工具类
/// </summary>
public static class ClownFishInit
{
    private static readonly CancellationTokenSource s_exitTokenSource = new CancellationTokenSource();
    /// <summary>
    /// 应用程序结束时通知对象
    /// </summary>
    public static CancellationToken AppExitToken => s_exitTokenSource.Token;

    /// <summary>
    /// 【此方法仅供框架内部使用】通知后台线程执行退出操作。
    /// </summary>
    public static void ApplicationEnd()
    {
        if( s_exitTokenSource.IsCancellationRequested )
            return;

        Console2.WriteSeparatedLine();

        // 通知所有后台线程，应用程序即将退出
        s_exitTokenSource.Cancel();

        Console2.WriteLine("Application End!");
    }

    /// <summary>
    /// 执行一些最基础的初始化，不包含 Data/Log 部分
    /// </summary>
    public static void InitBase()
    {
        BaseInitUtils.InitBase();
    }


    /// <summary>
    /// 初始化 ClownFish.Data
    /// </summary>
    public static void InitDAL()
    {
        DataInitUtils.InitDAL();
    }

    /// <summary>
    /// 按照默认方式初始化日志组件
    /// </summary>
    /// <param name="baseConfig">一介默认的配置，在合并时，它做为基础来源</param>
    /// <param name="addConfig">新增配置，合并时，它的参数将会覆盖baseConfig</param>
    public static void InitLogAsDefault(LogConfiguration baseConfig = null, LogConfiguration addConfig = null)
    {
        LogInitUtils.InitLogAsDefault(baseConfig, addConfig);
    }

    /// <summary>
    /// 初始化 ClownFish.Log
    /// </summary>
    /// <param name="config"></param>
    public static void InitLog(LogConfiguration config)
    {
        LogInitUtils.InitLog(config);
    }


    /// <summary>
    /// 初始化 ClownFish.Log
    /// </summary>
    /// <param name="filePath">ClownFish.Log.config的完整路径</param>
    public static void InitLog(string filePath)
    {
        LogInitUtils.InitLog(filePath);
    }



    /// <summary>
    /// 设置存储当前应用程序配置参数的Windows注册表路径，如果不指定将使用默认值：HKEY_CURRENT_USER\SOFTWARE\ClownFish_LocalSettings\appname
    /// </summary>
    /// <param name="regPath"></param>
    public static void SetRegPath(string regPath)
    {
        WinRegSetting.SetRegPath(regPath);
    }
}
