using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MockAspnetRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class PerformanceMoudleTest : TestBase
	{
		private void SetAsyncWriteEnabled(bool enabled)
		{
			FieldInfo field = typeof(LogHelper).GetField("s_enableAsyncWrite", BindingFlags.Static | BindingFlags.NonPublic);
			field.SetValue(null, enabled);
		}

		[TestMethod]
		public void Test()
		{
			// 测试 HTTP 请求执行超时场景
			using( WebContext context = HttpInfoTest.CreateWebContext() ) {

				PerformanceModule module = new PerformanceModule();


				System.Threading.Thread.Sleep(WriterFactory.Config.TimerPeriod + 1000);
				// 暂停异常写入
				SetAsyncWriteEnabled(false);

				// 这个调用仅仅为了覆盖代码，没什么具体意义
				module.Init(context.Application.Instance);

				// 代替 HttpApplicaton 触发 PreRequestHandlerExecute事件
				MethodInfo method = module.GetType().GetMethod("app_PreRequestHandlerExecute", BindingFlags.Instance | BindingFlags.NonPublic);
				method.Invoke(module, new object[] { context.Application.Instance, null });

				// 获取结果，检查上面的调用是否成功
				object value = context.HttpContext.Items["289e8920-b291-4167-80b0-793cd46cad22"];
				Assert.IsTrue(value.GetType() == typeof(DateTime));


				// 模拟超时
				System.Threading.Thread.Sleep(WriterFactory.Config.Performance.HttpExecuteTimeout + 1000);


				// 代替 HttpApplicaton 触发 PostRequestHandlerExecute事件
				method = module.GetType().GetMethod("app_PostRequestHandlerExecute", BindingFlags.Instance | BindingFlags.NonPublic);
				method.Invoke(module, new object[] { context.Application.Instance, null });

				// 获取调用 LogHelper.Write(info); 的数据

				List<PerformanceInfo> list = GetQueueData();

				Assert.AreEqual(1, list.Count);

				PerformanceInfo performanceInfo = list[0];
				Assert.AreEqual("http://www.bing.com/sfdjosfdj/slfjsfj/sdjfosf.aspx", performanceInfo.HttpInfo.Url);

				// 启用异常写入
				SetAsyncWriteEnabled(true);

				module.Dispose();
			}
		}


		private List<PerformanceInfo> GetQueueData()
		{
			MethodInfo getCacheQueueMethod = typeof(LogHelper).GetMethod("GetCacheQueue", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo getCacheQueueMethod2 = getCacheQueueMethod.MakeGenericMethod(typeof(PerformanceInfo));

			CacheQueue<PerformanceInfo> queue = getCacheQueueMethod2.Invoke(null, null) as CacheQueue<PerformanceInfo>;

			FieldInfo field = queue.GetType().GetField("_list", BindingFlags.Instance | BindingFlags.NonPublic);
			return field.GetValue(queue) as List<PerformanceInfo>;
		}


		[TestMethod]
		public void Test2()
		{
			// 测试 SQL 执行超时场景

			PerformanceModule module = new PerformanceModule();

			System.Threading.Thread.Sleep(WriterFactory.Config.TimerPeriod + 1000);
			// 暂停异常写入
			SetAsyncWriteEnabled(false);


			TimeSpan timeSpan = TimeSpan.FromSeconds(2d);
			DbCommand command = SqlInfoTest.CreateDbCommand();

			MethodInfo method = module.GetType().GetMethod("CheckExecuteTime", BindingFlags.Static | BindingFlags.NonPublic);
			
			// 第一次调用，第一个参数command = null
			method.Invoke(null, new object[] { null, timeSpan });

			// 第二次调用
			method.Invoke(null, new object[] { command, timeSpan });



			// 获取调用 LogHelper.Write(info); 的数据
			List<PerformanceInfo> list = GetQueueData();

			Assert.AreEqual(1, list.Count);

			PerformanceInfo performanceInfo = list[0];
			Assert.AreEqual(command.CommandText, performanceInfo.SqlInfo.SqlText.ToString());

			// 启用异常写入
			SetAsyncWriteEnabled(true);
		}
	}
}
