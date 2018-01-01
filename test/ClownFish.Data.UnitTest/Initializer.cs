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
			// ClownFish.Data 初始化 3 步骤（都是可选步骤）：
			// ########################################

			ClownFish.Data.Initializer.Instance
							// 1、初始化连接字符串
							.InitConnection(configFile)

							// 2、加载 XmlCommand （如果不使用XmlCommand，可以忽略这个步骤）
							.LoadXmlCommandFromDirectory(/* 不指定参数，接受XmlCommand规范的默认目录  */)

							// 3、为数据实体生成代理类型，并加载已生成的实体代理程序集
							.CompileAllEntityProxy();


			// 关于实体代理类型的说明
			// 代理类的代码可参考文件：AutoCode1.cs
			// 主要用途有二块：1、记录属性的修改（xxxx_Proxy），2、快速加载实体（xxxx_Loader）

			// 有二种方法可以得到实体代理类型：
			// 1、像上面那样调用 CompileAllEntityProxy()，将在运行时生成临时程序集（占用运行时时间）
			// 2、先调用命令行工具 ClownFish.Data.ProxyGen 生成程序集文件，启动时再调用 CompileAllEntityProxy() 加载
			
			// 对于对性能没有苛求的项目，也可以不生成实体代理，那么ClownFish.Data会在运行时采用反射的方式来处理
			// 如果不生成代理类型，就不能以修改属性方式做CUD


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
