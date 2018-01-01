using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public abstract class BaseTest
	{
		[TestInitialize]
		public void RestCPQueryParamIndex()
		{
			// 将CPQuery的内部参数序号计数器重置为零，便于做SQL语句的断言
			typeof(CPQuery).InvokeMember("s_index",
								BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
								null, null, new object[] { 0 });
		}



		public void AssertLastExecuteSQL(string text)
		{
			Assert.AreEqual(text, ClownFishDataEventSubscriber.LastExecuteSQL);
		}


		public string GetSql(string xmlcommandName)
		{
			// 这个测试类为了简单，就直接借用XmlCommand中定义的SQL语句

			XmlCommandItem x1 = XmlCommandManager.GetCommand(xmlcommandName);
			return x1.CommandText;
		}

		public void ShowCurrentThread()
		{
			System.Console.WriteLine("ThreadId: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
		}


		public bool ObjectIsEquals(object a, object b)
		{
			if( a == null && b == null )
				return true;

			if( a == null || b == null )
				return false;


			// 采用 JSON序列化 的方式比较二个对象的属性
			string json1 = a.ToJson();
			string json2 = b.ToJson();

			return json1 == json2;				
		}
	}
}
