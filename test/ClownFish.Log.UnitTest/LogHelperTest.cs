using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using ClownFish.AspnetMock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class LogHelperTest : TestBase
	{
		//[TestMethod]
		//public void DeleteEventLog()
		//{
		//	try {
		//		EventLog.DeleteEventSource("WinLogWriter日志消息");
		//	}
		//	catch { }

		//	try {
		//		EventLog.Delete("ClownFish-Log");
		//	}
		//	catch { }
		//}

		//[TestMethod]
		//public void DeleteEventLog()
		//{
		//	try {
		//		EventLog.DeleteEventSource("明源ERP错误消息");
		//	}
		//	catch { }

		//	try {
		//		EventLog.Delete("mysoft-erp6");
		//	}
		//	catch { }

		//	try {
		//		EventLog.Delete("Mysoft-ERP");
		//	}
		//	catch { }			
		//}


		//[TestMethod]
		//public void CreateEventSourece()
		//{
		//	string logName = "Mysoft-MetadataTfs";
		//	string sourceName = "Mysoft.MetadataTfs.WebApplication";

		//	try {
		//		EventLog.DeleteEventSource(sourceName);
		//	}
		//	catch { }
		//	try {
		//		EventLog.Delete(logName);
		//	}
		//	catch { }

		//	EventLog.CreateEventSource(sourceName, logName);
		//}

		internal static Exception CreateException()
		{
			try {
				int a = 2;
				int b = 0;
				int c = a / b;
				return null;
			}
			catch( Exception ex ) {
				return ex;
			}
		}

		internal static Exception CreateException(string message)
		{
			Exception ex = CreateException();
			return new InvalidOperationException(message, ex);
		}


		[TestMethod]
		public void Test_LogHelper_SyncWrite()
		{
			Exception ex = CreateException();
			ExceptionInfo exInfo = ExceptionInfo.Create(ex);
			LogHelper.SyncWrite(exInfo);

			AssertWriteOK(ex.Message);
		}


		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ExceptionInfo_Create_Argument_null()
		{
			ExceptionInfo info = ExceptionInfo.Create(null, null, null);
		}

		[TestMethod]
		public void Test_LogHelper_Write_DbCommand()
		{
			using( WebContext context = HttpInfoTest.CreateWebContext() ) {
				context.SetUserName("Fish Li");
				context.AddSession("session-1", "aaaaaaaaaaaa");
				context.AddSession("session-2", DateTime.Now);
				context.AddSession("session-3", null);
				context.Request.SetInputStream("a=1&b=2");

				DbCommand command = SqlInfoTest.CreateDbCommand();


				Exception ex = CreateException("Test: HttpInfo.Create");
				ExceptionInfo info = ExceptionInfo.Create(ex, context.HttpContext, command);
				info.Addition = Guid.NewGuid().ToString();

				LogHelper.SyncWrite(info);

				LogHelperTest.AssertWriteOK(info.Addition);

				AssertWriteOK(ex.Message);
			}			
		}


		[TestMethod]
		public void Test_LogHelper_AsyncWrite()
		{
			Exception ex = CreateException(Guid.NewGuid().ToString());
			ExceptionInfo exInfo = ExceptionInfo.Create(ex);
			LogHelper.Write(exInfo);

			// 等待定时器启动
			System.Threading.Thread.Sleep(1000);

			AssertWriteOK(ex.Message);
		}


		


		

		internal static void AssertWriteOK(string flagString)
		{
			FileWriter fileWriter = new FileWriter();
			string filePath = fileWriter.GetFilePath(typeof(ExceptionInfo));

			string logText = File.ReadAllText(filePath, Encoding.UTF8);

			Assert.IsTrue(logText.IndexOf(flagString) > 0);

		}


		[TestMethod]
		public void Test_LogHelper_RaiseErrorEvent()
		{
			string message = Guid.NewGuid().ToString();
			Exception ex1 = new Exception(message);

			LogHelper.RaiseErrorEvent(ex1);

			Assert.AreEqual(ex1.Message, _lastException.Message);
		}


		[TestMethod]
		public void Test_LogHelper_RetryLog()
		{
			List<Exception> list = new List<Exception>();
			list.Add(new NotImplementedException());

			CacheQueue<ExceptionInfo> queue = new CacheQueue<ExceptionInfo>();
			MethodInfo method = queue.GetType().GetMethod("ProcessFlushException", BindingFlags.Instance | BindingFlags.NonPublic);
			method.Invoke(queue, new object[] { list });
		}


		[TestMethod]
		public void Test_LogHelper_RetryLog2()
		{
			Hashtable hashTable = typeof(WriterFactory).InvokeMember("s_writerTable", 
									BindingFlags.GetField | BindingFlags.Static| BindingFlags.NonPublic, 
									null, null, null) as Hashtable;

			string key = typeof(WriterFactory).InvokeMember("ExceptionWriterKey",
									BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic,
									null, null, null) as string;

			object currentValue = hashTable[key];

			// 清除重试的Writer
			hashTable[key] = null;

			_lastException = null;

			List<Exception> list = new List<Exception>();
			list.Add(new NotImplementedException());

			CacheQueue<ExceptionInfo> queue = new CacheQueue<ExceptionInfo>();
			MethodInfo method = queue.GetType().GetMethod("ProcessFlushException", BindingFlags.Instance | BindingFlags.NonPublic);

			try {
				method.Invoke(queue, new object[] { list });
			}
			finally {
				// 还原重试的Writer
				hashTable[key] = currentValue;
			}

			Assert.IsNotNull(_lastException);
		}



		[TestMethod]
		public void Test_LogHelper_RetryLog3()
		{
			Hashtable hashTable = typeof(WriterFactory).InvokeMember("s_writerTable",
									BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic,
									null, null, null) as Hashtable;

			string key = typeof(WriterFactory).InvokeMember("ExceptionWriterKey",
									BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic,
									null, null, null) as string;

			object currentValue = hashTable[key];

			// 设置成一个新的Writer
			hashTable[key] = typeof(ErrorWriter);

			_lastException = null;

			List<Exception> list = new List<Exception>();
			list.Add(new NotImplementedException());

			CacheQueue<ExceptionInfo> queue = new CacheQueue<ExceptionInfo>();
			MethodInfo method = queue.GetType().GetMethod("ProcessFlushException", BindingFlags.Instance | BindingFlags.NonPublic);

			try {
				method.Invoke(queue, new object[] { list });
			}
			finally {
				// 还原重试的Writer
				hashTable[key] = currentValue;
			}

			Assert.IsNotNull(_lastException);
		}



		[ExpectedException(typeof(NotSupportedException))]
		[TestMethod]
		public void Test_LogHelper_NotSupportedException()
		{
			TestData d = new TestData();

			LogHelper.Write(d);
		}
		
	}


	public class TestData : BaseInfo { }
}
