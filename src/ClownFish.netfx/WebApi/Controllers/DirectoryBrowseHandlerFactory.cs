using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;
using ClownFish.WebHost.Config;

namespace ClownFish.WebApi.Controllers
{
    /// <summary>
    /// 实现目录浏览功能的HandlerFactory
    /// </summary>
    internal sealed class DirectoryBrowseHandlerFactory
    {
        public static readonly DirectoryBrowseHandlerFactory Instance = new DirectoryBrowseHandlerFactory();

        private string _websitePath;
        private string _template;
        private string[] _defaultFiles;

        internal void Init()
        {
            ServerOption option = ServerOption.Get();

            if( option.Website == null 
                || option.Website.DirectoryBrowse == null 
                || option.Website.DirectoryBrowse.Enabled != 1 )
                return;

            _websitePath = option.Website.LocalPath; // ?? AppDomain.CurrentDomain.BaseDirectory;

            if( option.Website.DirectoryBrowse.DefaultFile.IsNullOrEmpty() == false ) {
                _defaultFiles = option.Website.DirectoryBrowse.DefaultFile.ToArray2();
            }

            Stream stream = typeof(DirectoryBrowseHandlerFactory).Assembly.GetManifestResourceStream("ClownFish.WebApi.Controllers.FileListTemplate.html");
            using( StreamReader reader = new StreamReader(stream, Encoding.UTF8) ) {
                _template = reader.ReadToEnd();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IHttpHandler GetHandler(NHttpContext context)
        {
            if( _websitePath == null )
                return null;


            string physicalPath = Path.GetFullPath(Path.Combine(_websitePath, context.Request.Path.TrimStart('/')));


            // 安全检查，只允许查看指定站点下的文件
            if( physicalPath.StartsWith(_websitePath, StringComparison.OrdinalIgnoreCase) == false ) {
                return new Http404Handler();
            }

            if( File.Exists(physicalPath) ) {
                return StaticFileHandler.Create(physicalPath);
            }
            else if( Directory.Exists(physicalPath) ) {
                return GetDirectoryHandler(context, physicalPath);
            }
            else {
                return null;
            }

        }

        private string UrlEncode(string text)
        {
            if( string.IsNullOrEmpty(text) )
                return text;

            return System.Web.HttpUtility.UrlEncode(text);
        }


        private IHttpHandler GetDirectoryHandler(NHttpContext context, string physicalPath)
        {
            string curPath = context.Request.Path.TrimEnd('/');     // 这个路径如果包含特殊字符，得到的结果已被 UrlDecode
            string rawPath = context.Request.RawUrl.TrimEnd('/');   // 这个路径如果包含特殊字符，不会做 UrlDecode

            // 检查目录下是否存在默认文档
            if( _defaultFiles != null ) {
                foreach(string file in _defaultFiles ) {
                    string testFile = Path.Combine(physicalPath, file);
                    if( File.Exists(testFile) ) {
                        //string redireUrl = rawPath + "/" + UrlEncode(file);
                        //return new RedirectHttpHandler(redireUrl);

                        return StaticFileHandler.Create(testFile);
                    }
                }
            }


            int index = 1;
            
            string rowFormat = "<tr><td>{0}</td><td><a href=\"{1}\" >{2}</a></td><td>{3}</td><td class=\"filesize\">{4}</td></tr>\r\n";
            string rowFormat2 = "<tr><td>{0}</td><td><a href=\"{1}\" target=\"_blank\">{2}</a></td><td>{3}</td><td class=\"filesize\">{4}</td></tr>\r\n";

            StringBuilder html = new StringBuilder();
            DirectoryInfo dir = new DirectoryInfo(physicalPath);

            // 遍历目录下的子目录
            foreach( var d in dir.GetDirectories() ) {
                if( d.Attributes.HasFlag(FileAttributes.Hidden) )
                    continue;

                string link = rawPath + "/" + UrlEncode(d.Name) + "/";
                string time = d.LastWriteTime.ToTimeString();
                html.AppendFormat(rowFormat, index++, link, d.Name, time, "[文件夹]");
            }

            // 遍历目录下的文件
            foreach( var f in dir.GetFiles() ) {
                if( f.Attributes.HasFlag(FileAttributes.Hidden) )
                    continue;

                string link = rawPath + "/" + UrlEncode(f.Name);
                string time = f.LastWriteTime.ToTimeString();
                html.AppendFormat(rowFormat2, index++, link, f.Name, time, f.Length.ToString("N0"));
            }

            //string template = File.ReadAllText(@"D:\my-github\ClownFish.HttpServer\src\ClownFish.HttpServer\Handlers\FileListTemplate.html");
            string navigationLink = GetNavigationLink(curPath);
            string result = _template
                        .Replace("<!--{current-path}-->", navigationLink)
                        .Replace("<!--{data-row}-->", html.ToString());


            return new HtmlTextOutHandler(result);
        }


        private string GetNavigationLink(string curPath)
        {
            string rootHtml = "<a href=\"/\">/</a>";

            if( string.IsNullOrEmpty(curPath) )
                return rootHtml;


            string[] names = curPath.Trim('/').Split('/');
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(rootHtml);

            string path = string.Empty;

            for( int i = 0; i < names.Length; i++ ) {

                if( i > 0 )
                    sb.AppendLine("<span>/</span>");

                string x = names[i];
                path = path + "/" + UrlEncode(x);
                sb.AppendLine($"<a href=\"{path}\">{x}</a>");
            }

            return sb.ToString();
        }

        internal class HtmlTextOutHandler : IHttpHandler
        {
            private string _html;

            public HtmlTextOutHandler(string html)
            {
                _html = html;
            }

            public void ProcessRequest(NHttpContext context)
            {
                context.Response.ContentType = "text/html";
                context.Response.WriteAll(_html.GetBytes());
            }
        }


        //internal class RedirectHttpHandler : IHttpHandler
        //{
        //    private string _url;

        //    public RedirectHttpHandler(string url)
        //    {
        //        _url = url;
        //    }

        //    public void ProcessRequest(HttpContext context)
        //    {
        //        IActionResult result = new RedirectResult(_url);
        //        result.Ouput(context);
        //    }
        //}
    }
}
