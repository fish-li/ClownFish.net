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
    /// 是否需要初始化 “数据访问层”
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
    /// 在执行框架初始化的一些自定义逻辑。 默认行为：什么也不做。
    /// </summary>
    public virtual void BeforeFrameworkInit()
    {
        // 什么也不做。
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
        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options => {
            // https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-5.0
            options.AllowSynchronousIO = true;
            options.AddServerHeader = false;
            options.Limits.MaxRequestBodySize = LocalSettings.GetUInt("AspNetCore_Kestrel_MaxRequestBodySize", 1080 * 1024);
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
        x.Filters.Add(typeof(ClownFish.Web.Aspnetcore.Filters.MvcLogFilter), order++);
        x.Filters.Add(typeof(ClownFish.Web.AspnetCore.Filters.SimpleResultFilter), order++);
        x.Filters.Add(typeof(ClownFish.Web.AspnetCore.Filters.StatusCodeFilter), order++);
    }


    
}
