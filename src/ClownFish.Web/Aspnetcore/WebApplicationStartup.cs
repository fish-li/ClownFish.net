using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace ClownFish.Web.Aspnetcore;


/// <summary>
/// Web应用程序的启动定制过程基类
/// </summary>
public class WebApplicationStartup
{
    /// <summary>
    /// 是否需要初始化 “数据访问层”
    /// </summary>
    public virtual bool AutoInitDAL => false;

    /// <summary>
    /// 是否需要初始化 “日志组件”
    /// </summary>
    public virtual bool AutoInitLog => false;

    /// <summary>
    /// 是否需要初始化 “身份认证模块”
    /// </summary>
    public virtual bool AutoInitAuth => false;

    /// <summary>
    /// 是否需要初始化 “链路日志”
    /// </summary>
    public virtual bool AutoInitTracing => false;

    /// <summary>
    /// 在 ClownFish 执行初始化之前的事件阶段。 默认行为：什么也不做。
    /// </summary>
    public virtual void BeforeClownFishInit()
    {
        // 什么也不做。
    }

    /// <summary>
    /// 在 ClownFish 执行初始化之后的事件阶段。 默认行为：什么也不做。
    /// </summary>
    public virtual void AfterClownFishInit()
    {
        // 什么也不做。
    }

    internal void ConfigDAL0()
    {
        if( this.AutoInitDAL )
            ClownFishInit.InitDAL();
        else
            this.ConfigDAL();
    }

    /// <summary>
    /// 设置 数据访问 组件，仅当 AutoInitDAL == false 时调用。 默认行为：什么也不做。
    /// </summary>
    public virtual void ConfigDAL()
    {
        // 什么也不做。
    }

    internal void ConfigLog0()
    {
        if( this.AutoInitLog )
            ClownFishInit.InitLogAsDefault();
        else
            this.ConfigLog();

        if( LogConfig.IsInited == false ) {
            Console2.Info("##### 注意 ClownFish.Log 组件没有初始化！");
        }
    }

    /// <summary>
    /// 设置 日志 组件，仅当 AutoInitLog == false 时调用。 默认行为：什么也不做。
    /// </summary>
    public virtual void ConfigLog()
    {
        // 什么也不做。
    }

    internal void ConfigTracing0()
    {
        if( this.AutoInitTracing )
            TracingUtils.Init();
        else
            this.ConfigTracing();
    }

    /// <summary>
    /// 设置 监控 组件，仅当 AutoInitTracing == false 时调用。 默认行为：什么也不做。
    /// </summary>
    public virtual void ConfigTracing()
    {
        // 什么也不做。
    }


    internal void ConfigAuth0()
    {
        ClownFishWebInit.InitOptions();

        if( this.AutoInitAuth )
            ClownFishWebInit.InitAuth();
        else
            this.ConfigAuth();
    }

    /// <summary>
    /// 设置 身份认证模块，仅当 AutoInitAuth == false 时调用。 默认行为：什么也不做。
    /// </summary>
    public virtual void ConfigAuth()
    {
        // 什么也不做。
    }


    public virtual WebApplicationBuilder CreateWebApplicationBuilder()
    {
        return WebApplication.CreateSlimBuilder();
    }


    /// <summary>
    /// 调用appBuilder.Build()之前触发。 默认行为：什么也不做。
    /// </summary>
    /// <param name="appBuilder"></param>
    public virtual void BeforeApplicationBuild(WebApplicationBuilder appBuilder)
    {
        // 什么也不做。
    }

    /// <summary>
    /// 调用appBuilder.Build()之后触发。 默认行为：什么也不做。
    /// </summary>
    /// <param name="app"></param>
    public virtual void AfterApplicationBuild(WebApplication app)
    {
        // 什么也不做。
    }

    /// <summary>
    /// 给 Ioc 容器注册组件。 默认行为：配置基本的KestrelServerOptions参数，UnicodeRanges.All
    /// </summary>
    /// <param name="services"></param>
    public virtual void ConfigureServices(IServiceCollection services)
    {
        // 注意：这里并没有调用 IMvcBuilder mvcBuilder = services.AddControllers(this.RegisterInnerMvcFilters);
        // 因此，不使用 asp.net core mvc/webapi 功能

        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options => {
            // https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-5.0
            options.AllowSynchronousIO = true;
            options.AddServerHeader = false;
            options.Limits.MaxRequestBodySize = ClownFishWebOptions.MaxRequestBodySize;
        });
        services.Configure<IISServerOptions>(options => {
            options.AllowSynchronousIO = true;
        });

        //解决UrlEncode中文被编码
        services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
    }


    /// <summary>
    /// 配置ASP.NET管道。 默认行为：什么也不做。
    /// </summary>
    /// <param name="app"></param>
    public virtual void ConfigureWeb(WebApplication app)
    {
        // 什么也不做。
    }

    /// <summary>
    /// 在 Aspnet 执行初始化之前的事件阶段。 默认行为：什么也不做。
    /// </summary>
    public virtual void BeforeAspnetInit()
    {
        // 什么也不做。
    }

    /// <summary>
    /// 在 Aspnet 执行初始化之后的事件阶段。 默认行为：什么也不做。
    /// </summary>
    public virtual void AfterAspnetInit()
    {
        // 什么也不做。
    }

    /// <summary>
    /// 在 ClownFish.Web 执行初始化之前的事件阶段。 默认行为：什么也不做。
    /// </summary>
    public virtual void BeforeClownFishWebInit()
    {
        // 什么也不做。
    }

    /// <summary>
    /// 在 ClownFish.Web 执行初始化之后的事件阶段。 默认行为：什么也不做。
    /// </summary>
    public virtual void AfterClownFishWebInit()
    {
        // 什么也不做。
    }


    /// <summary>
    /// 应用程序初始化逻辑写在这里。 默认行为：什么也不做。
    /// </summary>
    public virtual void AppInit()
    {
        // 什么也不做。
    }


    /// <summary>
    /// 注册 ClownFish.Web 内置的 MVC 过滤器
    /// </summary>
    /// <param name="x"></param>
    public void RegisterInnerMvcFilters(MvcOptions x)
    {
        int order = int.MinValue;
        x.Filters.Add(typeof(ClownFish.Web.Aspnetcore.Filters.FirstFilter), order++);
        x.Filters.Add(typeof(ClownFish.Web.AspnetCore.Filters.SimpleResultFilter), order++);
        x.Filters.Add(typeof(ClownFish.Web.AspnetCore.Filters.StatusCodeFilter), order++);
    }

    /* RegisterInnerMvcFilters 方法给派生类使用，例如下面的示例代码
    public override void ConfigureServices(IServiceCollection services)
    {
        IMvcBuilder mvcBuilder = services.AddControllers(this.RegisterInnerMvcFilters);
        mvcBuilder.AddNewtonsoftJson(SetMvcJsonOptions);

        base.ConfigureServices(services);
    }
    private static void SetMvcJsonOptions(MvcNewtonsoftJsonOptions x)
    {
        x.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        x.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    }
    */

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
