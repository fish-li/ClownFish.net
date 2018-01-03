using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Data.UnitTest.Models;
using ClownFish.Web.Serializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ClownFish.Data.UnitTest
{
	public class MockModelBuilder : ModelBuilder
	{
		public static bool Called = false;

		public override void FillObjectFromHttp(HttpContext context, object model, ParameterInfo parameterInfo)
		{
			Called = true;
		}
	}

	public class TestController
	{
		/// <summary>
		/// EntityProxyAttribute 的用法演示
		/// </summary>
		/// <param name="p"></param>
		public void Action1([EntityProxy]Product p)
		{
			// 确实就可以这样调用！
			p.Insert();
		}
	}




	[TestClass]
	public class EntityHttpLoaderTest : BaseTest
	{
		[TestInitialize]
		public void Init()
		{
			// 先执行初始化
			typeof(EntityHttpLoader).InvokeMember("Init",
				BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic,
				null, null, null);

			bool isOK = (bool)typeof(EntityHttpLoader).InvokeMember("s_inited",
				BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic,
				null, null, null);

			// 确认初始化成功
			Assert.AreEqual(true, isOK);


			// 修改初始化的结果，替换成MOCK对象
			MethodInfo method = typeof(MockModelBuilder).GetMethod("FillObjectFromHttp");
			Assert.IsNotNull(method);


			typeof(EntityHttpLoader).InvokeMember("s_builderType",
				BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic,
				null, null, new object[] { typeof(MockModelBuilder) });

			typeof(EntityHttpLoader).InvokeMember("s_method",
				BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic,
				null, null, new object[] { method });
		}

		[TestCleanup]
		public void Cleanup()
		{
			// 恢复为未初始化状态
			typeof(EntityHttpLoader).InvokeMember("s_inited",
				BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic,
				null, null, new object[] { false });
		}


		[TestMethod]
		public void Test_EntityHttpLoader_LoadFromHttp()
		{
			// 准备调用参数
			HttpRequest request = new HttpRequest(@"c:\web\test\abc.aspx", "http://www.xx.com/abc.aspx", string.Empty);
			HttpResponse response = new HttpResponse(TextWriter.Null);
			HttpContext context = new HttpContext(request, response);


			MethodInfo method = typeof(TestController).GetMethod("Action1");
			Assert.IsNotNull(method);

			ParameterInfo p = method.GetParameters()[0];

			// 清除调用标记
			MockModelBuilder.Called = false;

			// 调用目标方法
			EntityProxyAttribute attr = new EntityProxyAttribute();
			object result = attr.GetHttpValue(context, p);

			// 确认已成功调用过
			Assert.AreEqual(true, MockModelBuilder.Called);

			// 确认与方法中申明的参数类型一致
			Assert.IsTrue(result is Product);  

			// 确认是一个实体代码，否则是不能执行Action1中的功能
			Assert.IsTrue(result is IEntityProxy);
		}
	}
}
