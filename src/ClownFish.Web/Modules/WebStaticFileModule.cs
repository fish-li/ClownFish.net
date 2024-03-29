﻿using ClownFish.Web.Handlers;

namespace ClownFish.Web.Modules;
public sealed class WebStaticFileModule : NHttpModule
{
    // 这里只挑选了一些常见的静态文件类型，
    // 来源于：https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types

    internal static readonly Dictionary<string, string> MimeDict = new Dictionary<string, string>(128, StringComparer.OrdinalIgnoreCase) {
        { ".htm",  "text/html" },
        { ".html", "text/html" },        
        { ".js",   "text/javascript" },   //{ ".js", "application/javascript" }, 这个是过时写法！
        { ".css",  "text/css" },
        { ".jpg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".ico", "image/x-icon" },
        { ".svg", "image/svg+xml" },
        { ".eot", "application/vnd.ms-fontobject" },
        { ".ttf", "font/ttf" },
        { ".woff", "font/woff" },
        { ".woff2", "font/woff2" },
    };

    //public override void PostAuthenticateRequest(NHttpContext httpContext)
    //{
    //    // 如果需要对静态文件做授权检查，可以自行开发一个 NHttpModule，然后重写 PostAuthenticateRequest 方法
    //    // 在那个方法中，检查 用户是否可以访问某个 URL 
    //}

    public override void ResolveRequestCache(NHttpContext httpContext)
    {
        if( httpContext.PipelineContext.Action != null )
            return;

        string path = httpContext.Request.Path;
        string ext = Path.GetExtension(path);

        if( MimeDict.TryGetValue(ext, out var contentType) ) {
            string filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", path.TrimStart('/'));
            if( File.Exists(filePath) ) {
                httpContext.PipelineContext.SetHttpHandler(new StaticFileHandler(filePath, contentType));
                return;
            }
        }
    }


}
