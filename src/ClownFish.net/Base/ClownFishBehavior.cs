namespace ClownFish.Base;

/// <summary>
/// 定义框架的一些默认行为。应用程序可以重写这个类来实现定制行为。
/// 注意：继承类型必须保证所有方法的实现都是线程安全的。
/// </summary>
public class ClownFishBehavior
{
    /// <summary>
    /// 单例引用。
    /// 如果需要修改框架行为，可以继承此类型并为此属性重新指定新的实例。
    /// </summary>
    public static ClownFishBehavior Instance = new ClownFishBehavior();


    /// <summary>
    /// 获取当前应用程序的名称
    /// </summary>
    /// <returns></returns>
    public virtual string GetApplicationName()
    {
        return EnvUtils.ApplicationName;
    }

    /// <summary>
    /// 获取当前进程所在的机器名称
    /// </summary>
    /// <returns></returns>
    public virtual string GetHostName()
    {
        return EnvUtils.HostName;
    }
       

    /// <summary>
    /// 获取当前进程所在的部署环境名称
    /// </summary>
    /// <returns></returns>
    public virtual string GetEnvName()
    {
        return EnvUtils.EnvName;
    }

    /// <summary>
    /// 获取进程能使用的临时目录
    /// </summary>
    /// <returns></returns>
    public virtual string GetTempPath()
    {
        return EnvUtils.TempPath;
    }

}
