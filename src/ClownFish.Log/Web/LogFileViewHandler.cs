using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Log.Configuration;
using ClownFish.Log.Serializer;
using ClownFish.Base;

namespace ClownFish.Log.Web
{
    /// <summary>
    /// 查看由FileWriter记录的日志文件的HttpHandler
    /// </summary>
    public class LogFileViewHandler : IHttpHandler
    {
        /// <summary>
        /// 获取一个值，该值指示其他请求是否可以使用 System.Web.IHttpHandler 实例。
        /// </summary>
        public bool IsReusable => false;

        /// <summary>
        /// 日志文件保存的根目录
        /// </summary>
        private string _logPath;

        private HttpContext _context;

        /// <summary>
        /// 通过实现 System.Web.IHttpHandler 接口的自定义 HttpHandler 启用 HTTP Web 请求的处理。
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            _context = context;

            // 计算日志文件保存的根目录，用于校验查看范围（安全检查）
            _logPath = GetLogRootDirectory();
            if( _logPath == null )
                return;


            // 从URL中获取要查看的【文件路径】
            string filePath = context.Request.QueryString["file"];
            if( string.IsNullOrEmpty(filePath) == false ) {
                ShowFile(filePath);
                return;
            }

            // 从URL中获取要查看的【目录路径】
            string directory = context.Request.QueryString["dir"];
            if( string.IsNullOrEmpty(directory) == false ) {
                ShowDirectory(directory);
                return;
            }


            // 如果URL中没有指定文件和目录，就从ClownFish.Log.config自动读取日志的根目录
            ShowDirectory(null);
        }


        private void ShowFile(string filenameBase64)
        {
            // 计算文件名的相对路径，从BASE64字符串中还原，具体可参考 GetUrl 方法
            string filePath = filenameBase64.FromBase64();
            filePath = Path.Combine(_logPath, filePath);

            if( RetryFile.Exists(filePath) == false ) {
                WriteMessage("要查看的日志文件不存在：" + filePath);
                return;
            }

            // 安全检查，只允许查看日志下的文件
            if( filePath.StartsWith(_logPath, StringComparison.OrdinalIgnoreCase) == false ) {
                WriteMessage("无效的文件路径，已超出日志文件的根目录。");
                return;
            }

            // 读文件，写响应流
            string text = RetryFile.ReadAllText(filePath, Encoding.UTF8);
            // 由于是文本文件，所以就直接字符串输出
            WriteMessage(text);
        }

        private void ShowDirectory(string pathBase64)
        {
            string subPath = pathBase64.FromBase64();
            string path = string.IsNullOrEmpty(subPath)
                        ? _logPath
                        : Path.Combine(_logPath, subPath);

            if( Directory.Exists(path) == false ) {
                WriteMessage("要查看的日志目录不存在：" + path);
                return;
            }

            // 安全检查，只允许查看日志下的文件
            if( path.StartsWith(_logPath, StringComparison.OrdinalIgnoreCase) == false ) {
                WriteMessage("无效的目录路径，已超出日志文件的根目录。");
                return;
            }

            int index = 1;
            string rowFormat = "<tr><td>{0}</td><td><a href=\"{1}\" target=\"_blank\">{2}</a></td><td>{3}</td><td class=\"filesize\">{4}</td></tr>";
            StringBuilder html = new StringBuilder();
            DirectoryInfo dir = new DirectoryInfo(path);

            // 遍历目录下的子目录
            foreach(var d in dir.GetDirectories() ) {
                string relativePath = subPath + "\\" + d.Name;
                string link = GetUrl(relativePath, "dir");
				string time = string.Empty;
				html.AppendFormat(rowFormat, index++, link, d.Name, time, "[文件夹]");
            }

            // 遍历目录下的文件
            foreach(var f in dir.GetFiles() ) {
                string relativePath = subPath + "\\" + f.Name;
                string link = GetUrl(relativePath, "file");
				string time = f.LastWriteTime.ToTimeString();
				html.AppendFormat(rowFormat, index++, link, f.Name, time, f.Length.ToString());
            }

            string template = null;
            Stream stream  = this.GetType().Assembly.GetManifestResourceStream("ClownFish.Log.Web.FileListTemplate.html");
            using( StreamReader reader = new StreamReader(stream, Encoding.UTF8) ) {
                template = reader.ReadToEnd();
            }

            template = template
                        .Replace("<!--{current-path}-->", subPath)
                        .Replace("<!--{data-row}-->", html.ToString());

            _context.Response.ContentType = "text/html";
            _context.Response.Write(template);
        }

		/// <summary>
		/// 获取目录下的文件清单，
		/// 重写这个方法可以实现文件过滤效果
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public virtual IEnumerable<FileInfo> GetFiles(DirectoryInfo dir)
		{
			return dir.GetFiles();
		}

		/// <summary>
		/// 获取目录下的子目录清单，
		/// 重写这个方法可以实现文件过滤效果
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public virtual IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo dir)
		{
			return dir.GetDirectories();
		}


		private string GetUrl(string relativePath, string paramName)
        {
            string text = relativePath.Trim('\\').ToBase64();
            return _context.Request.FilePath + $"?{paramName}=" + HttpUtility.UrlEncode(text);
        }

		/// <summary>
		/// 获取要显示的日志根目录，
		/// 重写这个方法可修改默认的根目录（FileWriter指定的目录）。
		/// </summary>
		/// <returns></returns>
		public virtual string GetLogRootDirectory()
        {
            WriterFactory.Init();

            WriterSection config = (from x in WriterFactory.Config.Writers
                                    where x.Type.StartsWith(typeof(FileWriter).FullName + ",")
                                    select x).FirstOrDefault();

            if( config == null ) {
                WriteMessage("ClownFish.Log.config中没有配置FileWriter");
                return null;
            }

            string path = config.GetOptionValue("RootDirectory");
            if( string.IsNullOrEmpty(path) ) {
                WriteMessage("ClownFish.Log.config中，没有为FileWriter指定RootDirectory属性。");
                return null;
            }

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if( Directory.Exists(path) == false ) {
                WriteMessage("ClownFish.Log.config中，为FileWriter指定RootDirectory目录不存在。");
                return null;
            }

            return path;
        }

        private void WriteMessage(string message)
        {
            _context.Response.ContentType = "text/plain";
            _context.Response.Write(message);
        }
    }
}
