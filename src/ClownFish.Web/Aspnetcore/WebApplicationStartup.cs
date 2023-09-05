#if NET6_0_OR_GREATER
using System.Text.Encodings.Web;
using System.Text.Unicode;
using ClownFish.Base.Xml;
using ClownFish.Log.Configuration;
using Microsoft.AspNetCore.StaticFiles;

namespace ClownFish.Web.Aspnetcore;


/// <summary>
/// Web应用程序的启动定制过程基类
/// </summary>
public class WebApplicationStartup
{
    public virtual bool EnableDAL => false;

    public virtual bool EnableLog => false;

    public virtual bool EnableAuth => false;

    /// <summary>
    /// 在执行框架初始化的一些自定义逻辑。 默认行为：什么也不做。
    /// </summary>
    public virtual void BeforeFrameworkInit()
    {

    }

    /// <summary>
    /// 给 Ioc 容器注册组件。 默认行为：配置基本的KestrelServerOptions参数
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
        
    }


    /// <summary>
    /// 应用程序初始化逻辑写在这里。 默认行为：什么也不做。
    /// </summary>
    public virtual void AppInit()
    {

    }


    /// <summary>
    /// 注册 HttpModules。 默认行为：什么也不做。
    /// </summary>
    public virtual void RegisterHttpModules()
    {

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


    public void UseStaticFiles(WebApplication app)
    {
        // https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1

        app.Environment.WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");

        StaticFileOptions options = new StaticFileOptions {
            OnPrepareResponse = OnPrepareResponseAction
        };

        app.UseStaticFiles(options);

        void OnPrepareResponseAction(StaticFileResponseContext ctx)
        {
            // 静态文件缓存 30 天
            ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=2592000");

            // Kestrel对于js文件，产生的响应头是：Content-Type: application/javascript
            // 这是一种过时的写法，可参考下面链接
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types#textjavascript
            if( ctx.File.IsDirectory == false && ctx.File.Name.EndsWithIgnoreCase(".js") ) {
                ctx.Context.Response.Headers.ContentType = "text/javascript";   // TODO: 以后.NET升级时要检查下Kestrel是否已改进，如果是则可删除这2行代码
            }
        }
    }
}
#endif
