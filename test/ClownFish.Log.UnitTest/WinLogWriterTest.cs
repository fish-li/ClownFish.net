using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class WinLogWriterTest: TestBase
	{

		[TestMethod]
		public void Test1()
		{
			// 正常情况测试

			Exception ex = new NotImplementedException(Guid.NewGuid().ToString());
			ExceptionInfo exInfo = ExceptionInfo.Create(ex);

			WinLogWriter writer = new WinLogWriter();
			writer.Write(exInfo);



			List<ExceptionInfo> list = new List<ExceptionInfo> { exInfo };
			writer.Write(list);

			// 不抛出异常就算是通过，所以没有断言
		}


		[TestMethod]
		public void Test2()
		{
			// 测试 s_initOK 的写入分支，检验方法就是查看代码覆盖率

			FieldInfo field	= typeof(WinLogWriter).GetField("s_initOK", BindingFlags.Static | BindingFlags.NonPublic);
			bool currentValue = (bool)field.GetValue(null);

			field.SetValue(null, false);

			try {
				Exception ex = new NotImplementedException("TestMessage: 顺序拟安排如下：集成专项、建模专项、越秀专项、售楼+公共专项 ");
				ExceptionInfo exInfo = ExceptionInfo.Create(ex);


				WinLogWriter writer = new WinLogWriter();
				writer.Write(exInfo);


				List<ExceptionInfo> list = new List<ExceptionInfo> { exInfo };
				writer.Write(list);

				// 不抛出异常就算是通过，所以没有断言

			}
			finally {
				field.SetValue(null, currentValue);
			}
		}

	}
}
