using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Base.Framework
{
	/// <summary>
	/// 当前应用程序的运行时环境
	/// </summary>
	public static class RunTimeEnvironment
	{
		/// <summary>
		/// 当前运行的程序是不是ASP.NET程序
		/// </summary>
		public static readonly bool IsAspnetApp = (HttpRuntime.AppDomainAppId != null);


		/// <summary>
		/// 获取当前程序加载的所有程序集
		/// </summary>
		/// <returns></returns>
		public static Assembly[] GetLoadAssemblies()
		{
			if( IsAspnetApp ) {
				System.Collections.ICollection assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies();
				return (from a in assemblies.Cast<Assembly>() select a).ToArray();
			}
			else
				return System.AppDomain.CurrentDomain.GetAssemblies();
		}
	}
}
