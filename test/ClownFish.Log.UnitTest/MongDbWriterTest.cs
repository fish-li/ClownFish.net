using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.UnitTest
{
#if _MongoDB_
	[TestClass]
	public class MongDbWriterTest :  TestBase
	{

		[TestMethod]
		public void Test_Write()
		{
			Exception ex = LogHelperTest.CreateException("M-" + Guid.NewGuid().ToString());
			ExceptionInfo exceptionInfo1 = ExceptionInfo.Create(ex);
			exceptionInfo1.Addition = "If you liked Fiddler, we also recommend";

			MongDbWriter writer = new MongDbWriter();
			writer.Write(exceptionInfo1);


			ExceptionInfo exceptionInfo2 = writer.Get<ExceptionInfo>(exceptionInfo1.InfoGuid);

			Assert.AreEqual(exceptionInfo1.Message.ToString(), exceptionInfo2.Message.ToString());
			Assert.AreEqual(exceptionInfo1.Addition.ToString(), exceptionInfo2.Addition.ToString());

			writer.Delete<ExceptionInfo>(exceptionInfo1.InfoGuid);
		}


		private List<ExceptionInfo> WriteList()
		{
			Exception ex1 = LogHelperTest.CreateException("M1-" + Guid.NewGuid().ToString());
			ExceptionInfo exceptionInfo1 = ExceptionInfo.Create(ex1);
			exceptionInfo1.Addition = "If you liked Fiddler, we also recommend";
			//exceptionInfo1.A1 = "aaaaaaaaaaaa";


			System.Threading.Thread.Sleep(1000);


			Exception ex2 = LogHelperTest.CreateException("M2-" + Guid.NewGuid().ToString());
			ExceptionInfo exceptionInfo2 = ExceptionInfo.Create(ex2);
			exceptionInfo2.Addition = "Test Studio - Create Automated Tests Quickly";
			//exceptionInfo2.A1 = "aaaaaaaaaaaa";


			MongDbWriter writer = new MongDbWriter();
			List<ExceptionInfo> list1 = new List<ExceptionInfo> { exceptionInfo1, exceptionInfo2 };
			writer.Write(list1);

			System.Threading.Thread.Sleep(1000);

			return list1;
		}


		[TestMethod]
		public void Test_WriteList()
		{
			// 写入2条数据
			List<ExceptionInfo> list = WriteList();

			MongDbWriter writer = new MongDbWriter();
			List<ExceptionInfo> list2 = writer.GetList<ExceptionInfo>(
					x => x.InfoGuid == list[0].InfoGuid || x.InfoGuid == list[1].InfoGuid);


			Assert.AreEqual(2, list2.Count);

			Assert.AreEqual(list[0].Message.ToString(), list2[1].Message.ToString());
			Assert.AreEqual(list[0].Addition.ToString(), list2[1].Addition.ToString());

			Assert.AreEqual(list[1].Message.ToString(), list2[0].Message.ToString());
			Assert.AreEqual(list[1].Addition.ToString(), list2[0].Addition.ToString());

			//writer.Delete<ExceptionInfo>(exceptionInfo1.InfoGuid, exceptionInfo2.InfoGuid);
		}


		[TestMethod]
		public void Test_GetList()
		{
			// 写入2条数据
			List<ExceptionInfo> list = WriteList();


			// 主要测试这个方法
			DateTime t1 = DateTime.Now.AddYears(-10);
			DateTime t2 = DateTime.Now;

			MongDbWriter writer = new MongDbWriter();
			List<ExceptionInfo> list2 = writer.GetList<ExceptionInfo>(t1, t2);

			// 确认数据是不是成功读取到
			ExceptionInfo info1 = list2.Find(x => x.InfoGuid == list[0].InfoGuid);
			Assert.IsNotNull(info1);

			ExceptionInfo info2 = list2.Find(x => x.InfoGuid == list[1].InfoGuid);
			Assert.IsNotNull(info2);
		}


		[TestMethod]
		public void Test_GetPageList()
		{
			// 写入2条数据
			List<ExceptionInfo> list = WriteList();


			// 主要测试这个方法
			int totalCount = 0;
			DateTime t1 = DateTime.Now.AddYears(-10);
			DateTime t2 = DateTime.Now;

			Expression<Func<ExceptionInfo, bool>> func = x => x.Time >= t1 && x.Time < t2;

			MongDbWriter writer = new MongDbWriter();
			List<ExceptionInfo> list2 = writer.GetPageList<ExceptionInfo>(0, 2, func, out totalCount);


			// 确认数据是不是成功读取到
			Assert.AreEqual(2, list2.Count);
		}


	}

#endif

}
