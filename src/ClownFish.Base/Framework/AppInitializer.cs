using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;

namespace ClownFish.Base.Framework
{
	/// <summary>
	/// 应用程序初始化工具类
	/// </summary>
	/// 
	public static class AppInitializer
	{
		/// <summary>
		/// 程序初始化 启动方法由<see cref="PreApplicationStartMethodAttribute"/>类配置并使用
		/// </summary>
		/// <exception cref="InvalidProgramException">反射调用配置的启动方法，方法调用失败，且该方法的内部异常为空，抛出的反射调用失败异常</exception>
		/// <exception cref="Exception">反射调用配置的启动方法，启动方法内部的异常</exception>
		public static void Start()
		{
			var assemblies = RunTimeEnvironment.GetLoadAssemblies();
			foreach( Assembly asm in assemblies ) {
				// 过滤以【System】开头的程序集，加快速度
				if( asm.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase) )
					continue;

				PreApplicationStartMethodAttribute[] attrs = asm.GetAttributes<PreApplicationStartMethodAttribute>();

				foreach( PreApplicationStartMethodAttribute attr in attrs ) {
					Invoke(attr);
				}
			}
		}

		internal static void Invoke(PreApplicationStartMethodAttribute applicationStartMethodAttribute)
		{
			MethodInfo method = applicationStartMethodAttribute.Type.GetMethod(applicationStartMethodAttribute.MethodName,
						BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
						null, Type.EmptyTypes, null);

			if( method == null ) {
				throw new InvalidProgramException(
					string.Format("PreApplicationStartMethodAttribute指定的设置无效，不能找到匹配的方法。Type: {0}, MethodName: {1}",
									applicationStartMethodAttribute.Type.FullName,
									applicationStartMethodAttribute.MethodName
					));
			}


			try {
				method.Invoke(null, null);
			}
			catch( TargetInvocationException ex ) {
				if( ex.InnerException != null )

					// 尽量将原始的错误信息暴露出来。
					throw ex.InnerException;
				throw;
			}
		}
	}
}
