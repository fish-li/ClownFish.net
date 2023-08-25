namespace ClownFish.Web.Aspnetcore;


/// <summary>
/// Web应用程序的启动定制过程基类
/// </summary>
public class WebApplicationStartup
{
    /// <summary>
    /// 给 Ioc 容器注册组件
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
    }


    /// <summary>
    /// 配置ASP.NET管道
    /// </summary>
    /// <param name="app"></param>
    public virtual void ConfigureWeb(WebApplication app)
    {
        
    }


    /// <summary>
    /// 注册 HttpModules
    /// </summary>
    public virtual void RegisterHttpModules()
    {

    }

}
