using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using ClownFish.Base;
using ClownFish.Base.Common;
using ClownFish.Base.Framework;
using ClownFish.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// ClownFish.Web在运行时与底层交互的类型封装，重写这些方法可支持友好的单元测试
	/// </summary>
	public class WebRuntime
	{
		private static readonly LazyObject<WebRuntime> s_instance = new LazyObject<WebRuntime>(true);

		/// <summary>
		/// MvcRuntime的实例
		/// </summary>
		public static WebRuntime Instance
		{
			get { return s_instance.Instance; }
		}

		/// <summary>
		/// 获取网站部署目录
		/// 等效于：HttpRuntime.AppDomainAppPath;
		/// </summary>
		/// <returns></returns>
		public virtual string GetWebSitePath()
		{
			// 扩展点：允许测试时，指定不同的目录

			return RunTimeEnvironment.IsAspnetApp
						? System.Web.HttpRuntime.AppDomainAppPath
						: AppDomain.CurrentDomain.BaseDirectory;
		}


		/// <summary>
		/// 根据指定的站内相对路径，计算文件在磁盘中的物理存放路径（可用于替代 Server.MapPath）
		/// 等效于：Path.Combine(HttpRuntime.AppDomainAppPath, filePath);
		/// </summary>
		/// <param name="filePath">相对网站根目录的文件名，不能以 / 开头</param>
		/// <returns></returns>
		public virtual string GetPhysicalPath(string filePath)
		{
			// 扩展点：允许测试时，指定不同的目录

			return Path.Combine(GetWebSitePath(), filePath);
		}

		/// <summary>
		/// 写响应头
		/// </summary>
		/// <param name="response">HttpResponse实例</param>
		/// <param name="headerName">响应头的名字</param>
		/// <param name="headerValue">响应头的内容</param>
		public virtual void WriteResponseHeader(HttpResponse response, string headerName, string headerValue)
		{
			response.Headers.Add(headerName, headerValue);
		}


		/// <summary>
		/// 获取当前站点是不是以 DEBUG 方式运行
		/// </summary>
		public virtual bool IsDebugMode
		{
			get { return WebConfig.IsDebugMode; }
		}

		/// <summary>
		/// 根据指定的页面虚拟路径返回对应的HttpHandler的实例
		/// </summary>
		/// <param name="pageVirtualPath"></param>
		/// <returns></returns>
		public virtual Page CreateInstanceFromVirtualPath(string pageVirtualPath)
		{
			return BuildManager.CreateInstanceFromVirtualPath(
											pageVirtualPath, typeof(object)) as Page;
		}
		
	}
}
