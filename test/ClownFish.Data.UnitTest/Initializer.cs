using System;
using System.IO;
using ClownFish.Base.TypeExtend;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class Initializer
	{
		[AssemblyInitialize]
		public static void InitRuntime(TestContext context)
		{
			LoadProxyAssembly();


			string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Connections.config");

			// ########################################
			// ClownFish.Data 初始化 3 步骤：
			// ########################################

			ClownFish.Data.Initializer.Instance
							// 1、初始化连接字符串
							.InitConnection(configFile)

							// 2、加载 XmlCommand （如果不使用XmlCommand，可以忽略这个步骤）
							.LoadXmlCommandFromDirectory(/* 不指定参数，接受XmlCommand规范的默认目录  */)

							// 3、编译所有的实体代理类型
							.CompileAllEntityProxy();


			// 用于输出所有执行的SQL语句及命令参数（实现项目中不需要这个步骤）
			ExtenderManager.RegisterSubscriber(typeof(ClownFishDataEventSubscriber));
        }




		private static void LoadProxyAssembly()
		{
			// 在非ASP.NET程序中，没有引用到的程序集将不会加载，所以需要写点代码来加载它们。
			// 如果不强制加载它们，在ClownFish.Data中就不会使用它们。

			// 注意：在ASP.NET项目中，不需要这样写！

			string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.EntityProxy.dll");
			foreach( string f in files )
				System.Reflection.Assembly.LoadFrom(f);
		}
	}
}
