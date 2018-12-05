using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;
using ClownFish.Base.Framework;
using System.Reflection;

namespace ClownFish.Data
{
	/// <summary>
	/// DbProviderFactory的辅助工具类
	/// </summary>
	internal static class ProviderFactoryHelper
	{
		/// <summary>
		/// 根据指定的数据提供者名称创建对应的DbProviderFactory实例，
		/// 如果不能创建指定的提供者，将会抛出异常
		/// </summary>
		/// <param name="providerName">数据提供者名称</param>
		/// <returns>与数据提供者名称对应的DbProviderFactory实例</returns>
		public static DbProviderFactory GetDbProviderFactory(string providerName)
		{
			if (string.IsNullOrEmpty(providerName))
				//throw new ArgumentNullException("providerName");
				// 默认就是使用SQLSERVER
				return System.Data.SqlClient.SqlClientFactory.Instance;


			// 常用类型就直接返回固定结果，优化性能
			if (providerName == "System.Data.SqlClient")
				return System.Data.SqlClient.SqlClientFactory.Instance;

			try {
				return DbProviderFactories.GetFactory(providerName);
			}
			catch {
				// 有时为了方便使用，只需要将dll拷到bin目录下就可以运行了，不需要在web.config中注册

				DbProviderFactory factory = GetDbProviderFactoryViaReflection(providerName);
				if (factory == null)
					throw;
				else
					return factory;
			}
		}


		/// <summary>
		/// 当DbProviderFactories.GetFactory的注册机制无效时，再尝试使用反射方式查找DbProviderFactory
		/// </summary>
		/// <param name="providerName">数据提供者名称</param>
		/// <returns>与数据提供者名称对应的DbProviderFactory实例</returns>
		private static DbProviderFactory GetDbProviderFactoryViaReflection(string providerName)
		{
			Type factoryType = (from asm in RunTimeEnvironment.GetLoadAssemblies(true)
								from t in asm.GetPublicTypes()
								where t.Namespace == providerName && typeof(DbProviderFactory).IsAssignableFrom(t)
								select t).FirstOrDefault();

			if (factoryType == null)
				return null;

			return (DbProviderFactory)factoryType.InvokeMember("Instance",
									BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);
		}
	}
}
