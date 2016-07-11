using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Web.Config;
using Microsoft.Win32;

namespace ClownFish.Web
{
	/// <summary>
	/// 静态文件处理器，用于响应静态文件并在输出时设置缓存响应头
	/// </summary>
	public sealed class StaticFileHandler : IHttpHandler
	{
		private static readonly object s_lock = new object();
		private static bool s_inited = false;

		// 默认缓存时间：10分钟
		private static readonly int s_DefaultDuration = 600;

		// 统一的缓存时间，以及没有指定扩展名的缓存时间
		private static int s_CacheDuration;
		// 针对指定扩展名的过期时间
		private static Hashtable s_durationTable;
		// 每种扩展名对应诉Mime类型对照表
		private static readonly Hashtable s_mineTable = Hashtable.Synchronized(new Hashtable(10, StringComparer.OrdinalIgnoreCase));
		/// <summary>
		/// 用于提取CSS中的引用图片的正则表达式
		/// </summary>
		private static readonly Regex s_CssBackgroundImageRegex = new Regex(@"\burl\((?<file>\/[^\)]+)\)", RegexOptions.Compiled);

		private class LazyFileInfo
		{
			private string _filePath;
			private DateTime? _lastWriteTime;
			private string _extension;

			public LazyFileInfo(string filePath)
			{
				_filePath = filePath;
			}

			public DateTime LastWriteTime
			{
				get
				{
					if( _lastWriteTime.HasValue == false )
						_lastWriteTime = File.GetLastWriteTime(_filePath);
					return _lastWriteTime.Value;
				}
			}

			public string Extension
			{
				get
				{
					if( _extension == null )
						_extension = Path.GetExtension(_filePath);
					return _extension;
				}
			}
		}

		private static void Init()
		{
			if( s_inited == false ) {
				lock( s_lock ) {
					if( s_inited == false ) {
						LoadConfig();
					}
				}
			}
		}

		private static void LoadConfig()
		{
			string configValue = ConfigurationManager.AppSettings["ClownFish.Web:StaticFileHandler-CacheDuration"]
								 ?? FrameworkConfig.Instance.StaticFileHandler.CacheDuration;

			if( string.IsNullOrEmpty(configValue) == false ) {

				int duration = s_DefaultDuration;
				if( int.TryParse(configValue, out duration) )
					s_CacheDuration = duration;
				else {
					// 此时的格式应该是：js:100;css:100;png:10000;jpg:10000;*:200
					Hashtable table = ParseConfig(configValue, out duration);

					if( table != null && table.Count > 0 )
						s_durationTable = Hashtable.Synchronized(table);

					if( duration > 0 )
						s_CacheDuration = duration;
				}
			}

			// 确保有一个有效的缓存时间
			if( s_CacheDuration <= 0 )
				s_CacheDuration = s_DefaultDuration;
		}


		/// <summary>
		/// 处理请求，输出文件内容以及设置缓存响应头
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			Init();

			string filePath = context.Request.PhysicalPath;
			bool isCssFile = filePath.EndsWith(".css", StringComparison.OrdinalIgnoreCase);

			if( File.Exists(filePath) == false ) {
				new Http404Result().Ouput(context);
				return;
			}

			LazyFileInfo fileinfo = new LazyFileInfo(filePath);

			//string etagHeader = context.Request.Headers["If-None-Match"];
			//if( string.IsNullOrEmpty(etagHeader) == false ) {
			//        // 如果文件没有修改，就返回304响应
			//    if( fileinfo.LastWriteTime.Ticks.ToString() == etagHeader ) {
			//        context.Response.StatusCode = 304;
			//        context.Response.End();
			//        return;
			//    }
			//}

			// 说明：Etag响应头在有些场景下会不起使用，例如VS自带的WEB服务器，或者Windows身份认证中。
			//        因此现在采用SetLastModified + "If-Modified-Since" 的方式来识别304请求。

			// 如果加了版本号，就表示需要长期缓存的文件，此时当文件没有修改时，用304来结束请求。
			if( isCssFile == false // CSS文件不能用304来响应，否则图片永远不能刷新
				&& context.Request.QueryString.Count > 0 ) {

				string modifiedSince = context.Request.Headers["If-Modified-Since"];
				if( string.IsNullOrEmpty(modifiedSince) == false ) {
					DateTime dt;
					if( DateTime.TryParse(modifiedSince, out dt) ) {

						// 因为要排除毫秒，所以判断是否小于1秒
						if( (fileinfo.LastWriteTime - dt).TotalSeconds < 1.0 ) {
							context.Response.StatusCode = 304;
							context.Response.End();
							return;
						}
					}
				}
			}


			// 设置输出缓存头
			context.Response.Cache.SetCacheability(HttpCacheability.Public);

			// 如果请求的URL包含查询字符串，就认为是包含了版本参数，此时设置缓存时间为【一年】
			if( isCssFile == false		// 注意：这里仍然要排除CSS文件
				&& context.Request.QueryString.Count > 0 ) {
				context.Response.AppendHeader("X-StaticFileHandler", "1year");
				context.Response.Cache.SetMaxAge(TimeSpan.FromSeconds(24 * 60 * 60 * 365));
				context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
			}
			else {
				int duration = GetDuration(fileinfo);
				context.Response.AppendHeader("X-StaticFileHandler", duration.ToString());
				context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(duration));
				//context.Response.Cache.SetMaxAge(TimeSpan.FromSeconds(duration));
				// 上面的代码不起作用，只能用下面的方法来处理了。
				context.Response.Cache.AppendCacheExtension("max-age=" + duration.ToString());
			}


			// 设置HTTP缓存响应头
			//context.Response.Cache.SetETag(fileinfo.LastWriteTime.Ticks.ToString());
			context.Response.Cache.SetLastModified(fileinfo.LastWriteTime);

			// 设置响应内容标头
			string contentType = (string)s_mineTable[fileinfo.Extension];
			if( contentType == null ) {
				contentType = GetMimeType(fileinfo);
				s_mineTable[fileinfo.Extension] = contentType;
			}
			context.Response.ContentType = contentType;


			// 输出文件内容
			if( isCssFile )
				OutputCssFile(context);

			else
				context.Response.TransmitFile(filePath);
		}

		private string GetMimeType(LazyFileInfo file)
		{
			string mimeType = "application/octet-stream";
			if( string.IsNullOrEmpty(file.Extension) )
				return mimeType;

			using( RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(file.Extension.ToLower()) ) {
				if( regKey != null ) {
					object regValue = regKey.GetValue("Content Type");
					if( regValue != null )
						mimeType = regValue.ToString();
				}
			}
			return mimeType;
		}

		private static Hashtable ParseConfig(string text, out int defaultDuration)
		{
			// 此时的格式应该是：js:100;css:100;png:10000;jpg:10000;*:200

			defaultDuration = s_DefaultDuration;
			List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();

			string[] pairs = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach( string pair in pairs ) {
				string pp = pair.Trim();

				string[] kv = pp.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
				if( kv.Length != 2 )
					throw new ConfigurationErrorsException("无效的配置：StaticFileHandler-CacheDuration");

				int duration = 0;
				int.TryParse(kv[1], out duration);
				if( duration <= 0 )
					throw new ConfigurationErrorsException("无效的配置：StaticFileHandler-CacheDuration");

				if( string.IsNullOrEmpty(kv[0]) || kv[0] == "." )
					throw new ConfigurationErrorsException("无效的配置：StaticFileHandler-CacheDuration");

				if( kv[0] == "*" ) {
					defaultDuration = duration;
				}
				else {
					string key = kv[0].StartsWith(".") ? kv[0] : "." + kv[0];

					if( list.FindIndex(x => string.Compare(key, x.Key, StringComparison.OrdinalIgnoreCase) == 0) >= 0 )
						throw new ConfigurationErrorsException("无效的配置：StaticFileHandler-CacheDuration");

					list.Add(new KeyValuePair<string, int>(key, duration));
				}
			}

			if( list.Count == 0 )
				return null;


			Hashtable table = new Hashtable(10, StringComparer.OrdinalIgnoreCase);

			foreach( KeyValuePair<string, int> kv in list )
				table[kv.Key] = kv.Value;

			return table;
		}

		private int GetDuration(LazyFileInfo file)
		{
			if( s_durationTable == null )
				return s_CacheDuration;

			object val = s_durationTable[file.Extension];
			if( val == null )
				return s_CacheDuration;

			return (int)val;
		}


		private void OutputCssFile(HttpContext context)
		{
			// 1. 先读出文件内容。注意这里使用UTF-8编码
			// 2. 用正则表达式搜索所有的引用文件
			// 3. 循环匹配结果，
			// 4. 对于匹配之外的内容，直接写入StringBuilder实例，
			// 5. 如果是文件，则计算版本号，再一起写入到StringBuilder实例
			// 6. 最后，StringBuilder实例包含的内容就是处理后的结果。

			string text = File.ReadAllText(context.Request.PhysicalPath, Encoding.UTF8);

			MatchCollection matches = s_CssBackgroundImageRegex.Matches(text);
			if( matches != null && matches.Count > 0 ) {
				int lastIndex = 0;
				StringBuilder sb = new StringBuilder(text.Length * 2);

				foreach( Match m in matches ) {
					Group g = m.Groups["file"];
					if( g.Success ) {
						sb.Append(text.Substring(lastIndex, g.Index - lastIndex + g.Length));
						lastIndex = g.Index + g.Length;

						//string fileFullPath = HttpRuntime.AppDomainAppPath.TrimEnd('\\') + g.Value.Replace("/", "\\");
						string fileFullPath = WebRuntime.Instance.GetPhysicalPath(g.Value.Replace("/", "\\"));
						if( File.Exists(fileFullPath) ) {
							string version = File.GetLastWriteTimeUtc(fileFullPath).Ticks.ToString();
							sb.Append("?_v=").Append(version);
						}
					}
				}

				if( lastIndex > 0 && lastIndex < text.Length )
					sb.Append(text.Substring(lastIndex));

				context.Response.Write(sb.ToString());
			}
			else {
				context.Response.Write(text);
			}
		}

		/// <summary>
		/// 实现 IHttpHandler.IsReusable 属性
		/// </summary>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}
