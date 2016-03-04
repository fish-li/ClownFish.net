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

		private static void Test(int a, int b)
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

			System.Threading.Thread.Sleep(8000);
		}

		private static bool Init()
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

		private static void CreaetFile()
		{
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClownFish.Log.config");

			if( File.Exists(filePath) )
				return;

			LogConfig config = new LogConfig();
			config.Enable = true;
			config.Writers = new WritersConfig();
			config.TimerPeriod = 3000;

			config.Writers.File = new FileWriterConfig();
			config.Writers.File.RootDirectory = "Log";
			config.Writers.File.WriteType = "ClownFish.Log.Serializer.FileWriter, ClownFish.Log";

			config.Writers.Mail = new MailWriterConfig();
			config.Writers.Mail.Receivers = "liqf01@mysoft.com.cn";
			config.Writers.Mail.WriteType = "ClownFish.Log.Serializer.MailWriter, ClownFish.Log";

#if _MongoDB_
			config.Writers.MongDb = new MongDbWriterConfig();
			//config.Writers.MongDb.ConnectionString = "server=10.5.106.100;database=Test;connectTimeout=5s;socketTimeout=5s";
			config.Writers.MongDb.ConnectionString = "mongodb://10.5.106.100/Test?connectTimeout=5s;socketTimeout=5s";
			config.Writers.MongDb.WriteType = "ClownFish.Log.Serializer.MongDbWriter, ClownFish.Log";
#endif

			config.Writers.Msmq = new MsmqWriterConfig();
			config.Writers.Msmq.RootPath = @".\private$\ClownFish-Log-test";
			config.Writers.Msmq.WriteType = "ClownFish.Log.Serializer.MsmqWriter, ClownFish.Log";

			config.Writers.WinLog = new WinLogWriterConfig();
			config.Writers.WinLog.LogName = "ClownFish-Log";
			config.Writers.WinLog.SourceName = "ClownFish-Log-Message";
			config.Writers.WinLog.WriteType = "ClownFish.Log.Serializer.WinLogWriter, ClownFish.Log";

			config.Types = new List<TypeItemConfig>();
			TypeItemConfig t1 = new TypeItemConfig();
			t1.DataType = "ClownFish.Log.Model.ExceptionInfo, ClownFish.Log";
			t1.Writers = "MongDb,File";
			config.Types.Add(t1);

			TypeItemConfig t2 = new TypeItemConfig();
			t2.DataType = "ClownFish.Log.Model.PerformanceInfo, ClownFish.Log";
			t2.Writers = "MongDb";
			config.Types.Add(t2);

			config.Performance = new PerformanceConfig();
			config.Performance.DbExecuteTimeout = 3;
			config.Performance.HttpExecuteTimeout = 3;

			config.ExceptionWriter = "WinLog";

			XmlHelper.XmlSerializeToFile(config, filePath, Encoding.UTF8);

			Console.WriteLine("Create Config file OK.");
		}
	}
}
