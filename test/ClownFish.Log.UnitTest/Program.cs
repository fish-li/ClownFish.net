using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Xml;
using ClownFish.Log.Configuration;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class Program
	{
		//[TestMethod]
		public void GenerateConnectionString()
		{
			MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder();
			mongoUrlBuilder.DatabaseName = "Test";
			mongoUrlBuilder.Server = new MongoServerAddress("10.5.106.100");
			mongoUrlBuilder.ConnectTimeout = TimeSpan.FromSeconds(5d);
			mongoUrlBuilder.SocketTimeout = TimeSpan.FromSeconds(5d);

			string connectionString = mongoUrlBuilder.ToMongoUrl().ToString();
			Console.WriteLine(connectionString);
		}


		//[TestMethod]
		public void Main()
		{
			LogHelper.OnError += LogHelper_OnError;

			if( Init() == false )
				return;

			Test(1, -5);

			Test(1, 0);
		}

		void LogHelper_OnError(object sender, LogExceptionEventArgs e)
		{
			Console.WriteLine(e.Exception.ToString());
		}

		private void Test(int a, int b)
		{
			try {
				if( b < 0 )
					throw new NotImplementedException();

				int c = a / b;
			}
			catch( Exception ex ) {
				ExceptionInfo exInfo = ExceptionInfo.Create(ex);
				LogHelper.Write(exInfo);
			}

			System.Threading.Thread.Sleep(1000);
		}

		private bool Init()
		{
			CreaetFile();

			try {
				WriterFactory.Init();

				Console.WriteLine("Init OK.");

				return true;
			}
			catch( Exception ex ) {
				Console.WriteLine(ex.ToString());
				return false;
			}
		}


		//[TestMethod]
		public void CreaetFile()
		{
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClownFish.Log.config");

			
			LogConfig config = new LogConfig();
			config.Enable = true;
			config.Writers = new WriterSection[5];
			config.TimerPeriod = 100;

			WriterSection file = new WriterSection();
			file.Name = "File";
			file.Type = "ClownFish.Log.Serializer.FileWriter, ClownFish.Log";
			file.Options = new WriterOption[1];
			file.Options[0] = new WriterOption { Key = "RootDirectory", Value = "Log" };
			config.Writers[0] = file;

			WriterSection mail = new WriterSection();
			mail.Name = "Mail";
			mail.Type = "ClownFish.Log.Serializer.MailWriter, ClownFish.Log";
			mail.Options = new WriterOption[1];
			mail.Options[0] = new WriterOption { Key = "Receivers", Value = "aaaaaa@163.com" };
			config.Writers[1] = mail;
			
#if _MongoDB_
			WriterSection mongodb = new WriterSection();
			mongodb.Name = "MongoDb";
			mongodb.Type = "ClownFish.Log.Serializer.MongoDbWriter, ClownFish.Log";
			mongodb.Options = new WriterOption[1];
			mongodb.Options[0] = new WriterOption { Key = "ConnectionString", Value = "mongodb://10.5.106.100/Test?connectTimeout=5s;socketTimeout=5s" };
			config.Writers[2] = mongodb;
#endif

			WriterSection msmq = new WriterSection();
			msmq.Name = "Msmq";
			msmq.Type = "ClownFish.Log.Serializer.MsmqWriter, ClownFish.Log";
			msmq.Options = new WriterOption[1];
			msmq.Options[0] = new WriterOption { Key = "RootPath", Value = @".\private$\ClownFish-Log-test" };
			config.Writers[3] = msmq;


			WriterSection winlog = new WriterSection();
			winlog.Name = "WinLog";
			winlog.Type = "ClownFish.Log.Serializer.WinLogWriter, ClownFish.Log";
			winlog.Options = new WriterOption[2];
			winlog.Options[0] = new WriterOption { Key = "LogName", Value = "ClownFish-Log" };
			winlog.Options[1] = new WriterOption { Key = "SourceName", Value = "ClownFish-Log-Message" };
			config.Writers[4] = winlog;


			config.Types = new TypeItemConfig[2];
			TypeItemConfig t1 = new TypeItemConfig();
			t1.DataType = "ClownFish.Log.Model.ExceptionInfo, ClownFish.Log";
			t1.Writers = "MongoDb,File";
			config.Types[0] = t1;

			TypeItemConfig t2 = new TypeItemConfig();
			t2.DataType = "ClownFish.Log.Model.PerformanceInfo, ClownFish.Log";
			t2.Writers = "MongoDb";
			config.Types[1] = t2;

			config.Performance = new PerformanceConfig();
			config.Performance.DbExecuteTimeout = 3;
			config.Performance.HttpExecuteTimeout = 3;

			config.ExceptionWriter = "WinLog";

			XmlHelper.XmlSerializeToFile(config, filePath, Encoding.UTF8);

			Console.WriteLine("Create Config file OK.");
		}
	}
}
