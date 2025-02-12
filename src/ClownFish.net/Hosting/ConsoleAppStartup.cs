namespace ClownFish.Hosting;

/// <summary>
/// 
/// </summary>
public class ConsoleAppStartup
{
    /// <summary>
    /// 是否需要初始化 “数据访问层”
    /// </summary>
    public virtual bool AutoInitDAL => false;

    /// <summary>
    /// 是否需要初始化 “日志组件”
    /// </summary>
    public virtual bool AutoInitLog => false;

    ///// <summary>
    ///// 是否需要初始化 “身份认证模块”
    ///// </summary>
    //public virtual bool AutoInitAuth => false;

    /// <summary>
    /// 是否需要初始化 “链路日志”
    /// </summary>
    public virtual bool AutoInitTracing => false;

    /// <summary>
    /// 程序启动后，阻塞主线程，直到收到进程结束的信号。
    /// </summary>
    internal virtual bool WaitToEnd => true;

    /// <summary>
    /// 在 ClownFish 执行初始化之前的事件阶段。 默认行为：什么也不做。
    /// </summary>
    public virtual void BeforeFrameworkInit()
    {
        // 什么也不做。
    }

    /// <summary>
    /// 在 ClownFish 执行初始化之后的事件阶段。 默认行为：什么也不做。
    /// </summary>
    public virtual void AfterFrameworkInit()
    {
        // 什么也不做。
    }


    ///// <summary>
    ///// 调用appBuilder.Build()之前触发。 默认行为：什么也不做。
    ///// </summary>
    ///// <param name="appBuilder"></param>
    //public virtual void BeforeApplicationBuild(HostApplicationBuilder appBuilder)
    //{
    //    // 什么也不做。
    //}

    ///// <summary>
    ///// 调用appBuilder.Build()之后触发。 默认行为：什么也不做。
    ///// </summary>
    ///// <param name="host"></param>
    //public virtual void AfterApplicationBuild(IHost host)
    //{
    //    // 什么也不做。
    //}

    ///// <summary>
    ///// 给 Ioc 容器注册组件。 
    ///// </summary>
    ///// <param name="services"></param>
    //public virtual void ConfigureServices(IServiceCollection services)
    //{

    //    //解决UrlEncode中文被编码
    //    services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
    //}


    /// <summary>
    /// 应用程序初始化逻辑写在这里。 默认行为：什么也不做。
    /// </summary>
    public virtual void AppInit()
    {
        // 什么也不做。
    }

    /// <summary>
    /// 在启动HOST之前的最后动作。
    /// 注意：启动HOST时主线程将被阻塞直到程序退出。
    /// 默认行为：什么也不做。
    /// </summary>
    public virtual void BeforeRun()
    {
        // 什么也不做。
    }

    /// <summary>
    /// 程序退出时的最后动作。默认行为：什么也不做。
    /// </summary>
    public virtual void AppEnd()
    {
        // 什么也不做。
    }
}

