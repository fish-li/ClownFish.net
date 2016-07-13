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
using System.Collections;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class ConfigTest : TestBase
	{
		[TestMethod]
		public void Test1()
		{
			// 没什么具体意义，只是为了覆盖代码
			LogConfigException ex1 = new LogConfigException("aa");
			LogConfigException ex2 = new LogConfigException("bb", new Exception("xx"));
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test2()
		{
			WriterSection file = new WriterSection();
			file.Name = "File";
			file.Type = "ClownFish.Log.Serializer.FileWriter, ClownFish.Log";
			
			FileWriter writer  = new FileWriter();
			writer.Init(file);
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test3()
		{
			WriterSection mail = new WriterSection();
			mail.Name = "Mail";
			mail.Type = "ClownFish.Log.Serializer.MailWriter, ClownFish.Log";

			MailWriter writer = new MailWriter();
			writer.Init(mail);
		}

#if _MongoDB_
		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test4()
		{
			WriterSection mongodb = new WriterSection();
			mongodb.Name = "MongoDb";
			mongodb.Type = "ClownFish.Log.Serializer.MongoDbWriter, ClownFish.Log";

			MongoDbWriter writer = new MongoDbWriter();
			writer.Init(mongodb);
		}
#endif

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test5()
		{
			WriterSection msmq = new WriterSection();
			msmq.Name = "Msmq";
			msmq.Type = "ClownFish.Log.Serializer.MsmqWriter, ClownFish.Log";

			MsmqWriter writer = new MsmqWriter();
			writer.Init(msmq);
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test6()
		{
			WriterSection winlog = new WriterSection();
			winlog.Name = "WinLog";
			winlog.Type = "ClownFish.Log.Serializer.WinLogWriter, ClownFish.Log";

			WinLogWriter writer = new WinLogWriter();
			writer.Init(winlog);
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test7()
		{
			WriterSection winlog = new WriterSection();
			winlog.Name = "WinLog";
			winlog.Type = "ClownFish.Log.Serializer.WinLogWriter, ClownFish.Log";

			winlog.Options = new WriterOption[1];
			winlog.Options[0] = new WriterOption { Key = "LogName", Value = "ClownFish-Log" };

			WinLogWriter writer = new WinLogWriter();
			writer.Init(winlog);
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
			config.Types = null;        // 会引发异常

			ConfigLoader loader = new ConfigLoader();
			loader.Load(config, new Hashtable());
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test9()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new TypeItemConfig[0];       // 会引发异常

			ConfigLoader loader = new ConfigLoader();
			loader.Load(config, new Hashtable());
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test10()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Writers = null;      // 会引发异常

			ConfigLoader loader = new ConfigLoader();
			loader.Load(config, new Hashtable());
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test11()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new TypeItemConfig[1];

			TypeItemConfig tc = new TypeItemConfig();
			tc.DataType = null;         // 会引发异常
			config.Types[0] = tc;

			ConfigLoader loader = new ConfigLoader();
			loader.Load(config, new Hashtable());
		}


		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test12()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new TypeItemConfig[1];

			TypeItemConfig tc = new TypeItemConfig();
			tc.DataType = "ClownFish.Log.Model.ExceptionInfo, ClownFish.Log";
			tc.Writers = null;          // 会引发异常
			config.Types[0] = tc;

			ConfigLoader loader = new ConfigLoader();
			loader.Load(config, new Hashtable());
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test13()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new TypeItemConfig[1];

			TypeItemConfig tc = new TypeItemConfig();
			tc.DataType = "ClownFish.Log.Model.ExceptionInfo, ClownFish.Log";
			tc.Writers = ";";           // 会引发异常
			config.Types[0] = tc;

			ConfigLoader loader = new ConfigLoader();
			loader.Load(config, new Hashtable());
		}

		[ExpectedException(typeof(LogConfigException))]
		[TestMethod]
		public void Test14()
		{
			// 没什么具体意义，只是为了覆盖代码

			LogConfig config = GetCloneConfig();
			config.Types = new TypeItemConfig[1];

			TypeItemConfig tc = new TypeItemConfig();
			tc.DataType = "ClownFish.Log.Model.ExceptionInfo, ClownFish.Log";
			tc.Writers = "abc";         // 会引发异常
			config.Types[0] = tc;

			ConfigLoader loader = new ConfigLoader();
			loader.Load(config, new Hashtable());
		}

	}
}
