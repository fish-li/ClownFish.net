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
	public class WriterTest
	{
		[TestMethod]
		public void Test_NotImplementedException()
		{
			// 这个方法验证所有Writer中所有没有实现的方法

			int count = 0;

			FileWriter fileWriter = new FileWriter();
			count = Test_Get_GetList(fileWriter, count);	// + 2

			MailWriter mailWriter = new MailWriter();
			count = Test_Get_GetList(mailWriter, count);	// + 2

			MsmqWriter msmqWriter = new MsmqWriter();
			count = Test_Get_GetList(msmqWriter, count);	// + 2

			WinLogWriter winLogWriter = new WinLogWriter();
			count = Test_Get_GetList(winLogWriter, count);	// + 2

			NullWriter nullWriter = new NullWriter();
			nullWriter.Init(null);
			count = Test_Get_GetList(nullWriter, count);	// + 2

			try {
				// 这个是空调用，不应该有异常
				ExceptionInfo info = new ExceptionInfo();
				nullWriter.Write<ExceptionInfo>(info);
			}
			catch( NotImplementedException ) {
				count++;
			}

			try {
				// 这个是空调用，不应该有异常
				List<ExceptionInfo> list = new List<ExceptionInfo>();
				nullWriter.Write(list);
			}
			catch( NotImplementedException ) {
				count++;
			}

			Assert.AreEqual(10, count);
		}



		private int Test_Get_GetList(ILogWriter writer, int count)
		{
			Guid guid = Guid.NewGuid();
			DateTime t1 = DateTime.Now;
			DateTime t2 = DateTime.Now;

			try {
				writer.Get<ExceptionInfo>(guid);
			}
			catch( NotImplementedException ) {
				count++;
			}

			try {
				writer.GetList<ExceptionInfo>(t1, t2);
			}
			catch( NotImplementedException ) {
				count++;
			}

			return count;
		}



		[TestMethod]
		public void Test_4Writers_NullArgument()
		{
			// 测试参数值为空的场景，其实没什么具体意义，只是为了代码覆盖率

			FileWriter fileWriter = new FileWriter();
			Call_NullArgument(fileWriter);


			MailWriter mailWriter = new MailWriter();
			Call_NullArgument(mailWriter);


			MsmqWriter msmqWriter = new MsmqWriter();
			Call_NullArgument(msmqWriter);

			WinLogWriter winLogWriter = new WinLogWriter();
			Call_NullArgument(winLogWriter);
		}


		private void Call_NullArgument(ILogWriter writer)
		{
			// 测试参数值为空的场景，其实没什么具体意义，只是为了代码覆盖率

			writer.Write<ExceptionInfo>((ExceptionInfo)null);

			List<ExceptionInfo> list = new List<ExceptionInfo>();
			writer.Write(list);
			writer.Write<ExceptionInfo>((List<ExceptionInfo>)null);
		}


		[TestMethod]
		public void Test_MailWriter_SendEmail_NullArgument()
		{
			int count = 0;

			MethodInfo method = typeof(MailWriter).GetMethod("SendEmail", BindingFlags.Static | BindingFlags.NonPublic);

			try {
				string[] to = new string[0];
				method.Invoke(null, new object[] { to, "aaa", "bbb" });
			}
			catch( System.Reflection.TargetInvocationException ex ) {
				if( ex.InnerException.GetType() == typeof(ArgumentNullException) 
					&& ex.InnerException.Message.IndexOf("to") > 0 )
					count++;
			}


			try {
				method.Invoke(null, new object[] { null, "aaa", "bbb" });
			}
			catch( System.Reflection.TargetInvocationException ex ) {
				if( ex.InnerException.GetType() == typeof(ArgumentNullException)
					&& ex.InnerException.Message.IndexOf("to") > 0 )
					count++;
			}

			Assert.AreEqual(2, count);
		}
	}
}
