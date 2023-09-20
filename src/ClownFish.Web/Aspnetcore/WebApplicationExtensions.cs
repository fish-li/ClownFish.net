using Microsoft.AspNetCore.StaticFiles;

namespace ClownFish.Web.Aspnetcore;

public static class WebApplicationExtensions
{
    /// <summary>
    /// UseStaticFiles方法的增强版本。
    /// 在响应这些静态文件时会设置缓存响应头，让这些静态文件能缓存30天
    /// </summary>
    /// <param name="app"></param>
    public static void UseStaticFilesCache30Days(this WebApplication app)
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
