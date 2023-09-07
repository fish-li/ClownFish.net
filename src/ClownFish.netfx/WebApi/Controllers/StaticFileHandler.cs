using ClownFish.WebHost.Config;

namespace ClownFish.WebApi.Controllers;

/// <summary>
/// 一个简单的静态文件处理器，用于响应静态文件并在输出时设置缓存响应头
/// </summary>
internal sealed class StaticFileHandler : IHttpHandler
{
    // 每种扩展名对应诉Mime类型对照表
    private static readonly Hashtable s_mineTable = Hashtable.Synchronized(new Hashtable(10, StringComparer.OrdinalIgnoreCase));

    private StaticFileHandler() { }


    internal static void Init(ServerOption option)
    {
        if( option.Website.StaticFiles.IsNullOrEmpty() == false )
            foreach( var x in option.Website.StaticFiles )
                s_mineTable[x.Ext] = x;
    }

    internal static StaticFileHandler Create(string filePath)
    {
        return new StaticFileHandler {
            _fileInfo = new FileInfo(filePath)
        };
    }


    private NHttpContext _httpContext;
    private FileInfo _fileInfo;

    /// <summary>
    /// 处理请求，输出文件内容以及设置缓存响应头
    /// </summary>
    /// <param name="context"></param>
    public void ProcessRequest(NHttpContext context)
    {
        _httpContext = context;

        // 判断是不是可能用 304 来响应
        if( Can304Response() )
            return;

        // 读取文件内容
        byte[] filebody = File.ReadAllBytes(_fileInfo.FullName);

        // 设置响应头
        SetHeaders();

        // 输出文件内容
        //context.Response.EnableGzip();  // TODO:Gzip

        context.Response.WriteAll(filebody);
    }


    /// <summary>
    /// 是否以304做为响应并结束请求
    /// </summary>
    /// <returns></returns>
    private bool Can304Response()
    {
        NHttpContext context = this._httpContext;

        string etagHeader = context.Request.Header("If-None-Match");
        if( string.IsNullOrEmpty(etagHeader) == false ) {

            // 如果文件没有修改，就返回304响应
            if( _fileInfo.LastWriteTime.Ticks.ToString() == etagHeader ) {
                context.Response.StatusCode = 304;
                context.Response.End();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 设置响应头
    /// </summary>
    private void SetHeaders()
    {
        NHttpContext context = this._httpContext;

        StaticFileOption option = GetStaticFileOption(_fileInfo.Extension);

        if( option.Cache > 0 ) {
            // 设置缓存响应头
            context.Response.SetHeader("Cache-Control", "public, max-age=" + option.Cache);
            context.Response.SetHeader("X-StaticFileHandler", option.Cache.ToString());
            context.Response.SetHeader("ETag", _fileInfo.LastWriteTime.Ticks.ToString());
        }
        else {
            //参考链接：https://www.coderxing.com/http-cache-control.html
            context.Response.SetHeader("Cache-Control", "no-store, max-age=0");
        }

        // 设置响应内容标头
        context.Response.ContentType = option.Mine;
    }


    /// <summary>
    /// 计算响应头ContentType对应的内容
    /// </summary>
    /// <param name="extname"></param>
    /// <returns></returns>
    private StaticFileOption GetStaticFileOption(string extname)
    {
        StaticFileOption option = (StaticFileOption)s_mineTable[extname];
        if( option == null ) {
            option = new StaticFileOption();
            option.Cache = 3600 * 24 * 365;      // 默认缓存1年
            option.Mine = ResponseContentType.OctetStream;

            s_mineTable[extname] = option;
        }
        return option;
    }


    ///// <summary>
    ///// 根据扩展名获取对应的MimeType
    ///// </summary>
    ///// <param name="extname"></param>
    ///// <returns></returns>
    //internal static string GetMimeType(string extname)
    //      {
    //          string mimeType = "application/octet-stream";
    //          if( string.IsNullOrEmpty(extname) )
    //              return mimeType;

    //          using( RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(extname.ToLower()) ) {
    //              if( regKey != null ) {
    //                  object regValue = regKey.GetValue("Content Type");
    //                  if( regValue != null )
    //                      mimeType = regValue.ToString();
    //              }
    //          }
    //          return mimeType;
    //      }




}
