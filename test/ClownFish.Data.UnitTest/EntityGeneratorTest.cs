using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ClownFish.Data.CodeDom;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class EntityGeneratorTest
	{
		[TestMethod]
		public void Test_生成实体代理类型代码()
		{
			EntityGenerator g = new EntityGenerator();
			string code = g.GetCode<Product>();

			code = EntityGenerator.UsingCodeBlock + code;

			//File.WriteAllText("..\\AutoCode1.cs", code, Encoding.UTF8);
			File.WriteAllText("EntityGeneratorTest_code.cs", code, Encoding.UTF8);
		}


		[TestMethod]
		public void Test_生成实体代理类程序集()
		{
			Type[] entityTypes = new Type[] { typeof(Product), typeof(Customer) };
			string dllFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test.EntityProxy.dll");

			if( File.Exists(dllFilePath) )
				File.Delete(dllFilePath);

			var result = ProxyBuilder.Compile(entityTypes, dllFilePath);

			Assert.IsTrue(File.Exists(dllFilePath));

			// 加载程序集并确认结果
			Assembly asm = Assembly.LoadFrom(dllFilePath);

			Type[] types = asm.GetExportedTypes();

			var t1 = (from x in types
						 where x.Name.StartsWith("Customer_") && x.Name.EndsWith("_Loader")
						 select x).First();

			var t2 = (from x in types
					  where x.Name.StartsWith("Customer_") && x.Name.EndsWith("_Proxy")
					  select x).First();

			var t3 = (from x in types
					  where x.Name.StartsWith("Product_") && x.Name.EndsWith("_Loader")
					  select x).First();

			var t4 = (from x in types
					  where x.Name.StartsWith("Product_") && x.Name.EndsWith("_Proxy")
					  select x).First();



		}
	}
}
