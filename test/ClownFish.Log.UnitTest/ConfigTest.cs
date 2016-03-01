using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Configuration;
using ClownFish.Log.Serializer;
using ClownFish.Base.Xml;
using ClownFish.Base;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class ConfigTest : TestBase
	{
		[TestMethod]
		public void Test1()
		{
			// 没什么具体意义，只是为了覆盖代码
			BaseWriterConfig config = new BaseWriterConfig();
			config.Valid();


			LogConfigException ex1 = new LogConfigException("aa");
			LogConfigException ex2 = new LogConfigException("bb", new Exception("xx"));
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test2()
		{
			// 没什么具体意义，只是为了覆盖代码
			FileWriterConfig config = new FileWriterConfig();
			config.Valid();
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test3()
		{
			// 没什么具体意义，只是为了覆盖代码
			MailWriterConfig config = new MailWriterConfig();
			config.Valid();
		}

#if _MongoDB_
		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test4()
		{
			// 没什么具体意义，只是为了覆盖代码
			MongDbWriterConfig config = new MongDbWriterConfig();
			config.Valid();
		}
#endif

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test5()
		{
			// 没什么具体意义，只是为了覆盖代码
			MsmqWriterConfig config = new MsmqWriterConfig();
			config.Valid();
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test6()
		{
			// 没什么具体意义，只是为了覆盖代码
			WinLogWriterConfig config = new WinLogWriterConfig();
			config.Valid();
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test7()
		{
			// 没什么具体意义，只是为了覆盖代码
			WinLogWriterConfig config = new WinLogWriterConfig();
			config.LogName = "1213";
			config.Valid();
		}



		private LogConfig GetCloneConfig()
		{
			string xml = XmlExtensions.ToXml(WriterFactory.Config);
			return XmlExtensions.FromXml<LogConfig>(xml);
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test8()
		{
			// 检验 Types 不配置的场景

			LogConfig config = GetCloneConfig();
			config.Types = null;		// 会引发异常

			WriterFactory.CheckConfig(config);
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test9()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new List<TypeItemConfig>();		// 会引发异常

			WriterFactory.CheckConfig(config);
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test10()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Writers = null;		// 会引发异常

			WriterFactory.CheckConfig(config);
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test11()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new List<TypeItemConfig>();

			TypeItemConfig tc = new TypeItemConfig();
			tc.DataType = null;			// 会引发异常
			config.Types.Add(tc);

			WriterFactory.CheckConfig(config);
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test12()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new List<TypeItemConfig>();

			TypeItemConfig tc = new TypeItemConfig();
			tc.DataType = "ClownFish.Log.Model.ExceptionInfo, ClownFish.Log";
			tc.Writers = null;			// 会引发异常
			config.Types.Add(tc);

			WriterFactory.CheckConfig(config);
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test13()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new List<TypeItemConfig>();

			TypeItemConfig tc = new TypeItemConfig();
			tc.DataType = "ClownFish.Log.Model.ExceptionInfo, ClownFish.Log";
			tc.Writers = ";";			// 会引发异常
			config.Types.Add(tc);

			WriterFactory.CheckConfig(config);
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test14()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new List<TypeItemConfig>();

			TypeItemConfig tc = new TypeItemConfig();
			tc.DataType = "ClownFish.Log.Model.ExceptionInfo, ClownFish.Log";
			tc.Writers = "abc";			// 会引发异常
			config.Types.Add(tc);

			WriterFactory.CheckConfig(config);
		}

	}
}
