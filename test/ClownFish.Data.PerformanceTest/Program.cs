using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.PerformanceTest
{
	class Program
	{
		public static string ConnectionString = null;

		static void AppInit()
		{
			ClownFish.Data.Initializer.Instance
						.InitConnection("Connections.config")   // 初始化连接字符串
						.CompileAllEntityProxy();               // 编译所有的实体代理类型


			// 静态变量赋值
			ConnectionString = ConnectionManager.GetConnection("default").ConnectionString;
			
			// 第一次打开与SQLSERVER的连接
			DataTable table = TestHelper.GetOrderInfoTable();
		}
		

		static void Main(string[] args)
		{
			AppInit();


			Type[] testTypes = (from t in typeof(Program).Assembly.GetTypes()
								where t.GetCustomAttribute<TestMethodAttribute>() != null
								select t).ToArray();
			ShowMenu(testTypes);

			while( true ) {
				int testIndex = GetTestIndex();	
				if( testIndex < 0 )
					break;

				if( testIndex == 0 ) {
					ShowMenu(testTypes);
					continue;
				}

				if( testIndex == 5 ) {
					RunAllTest(testTypes);
					continue;
				}

				Type t = testTypes[testIndex - 1];
				RunTest(t, x=> Console.WriteLine(x.ToString()));
			}
		}


		private static void RunTest(Type t, Action<TimeSpan> action)
		{
			int pagesize = 50; 
			IPerformanceTest instance = Activator.CreateInstance(t, pagesize) as IPerformanceTest;

			long sumTicks = 0;
			int count1 = 10;		// 大循环次数
			int count2 = 3000;		// 每轮循环中调用方法次数

			for( int i = 0; i < count1; i++ ) {

				Stopwatch watch = Stopwatch.StartNew();

				for( int k = 0; k < count2; k++ ) {
					var result = instance.Run();
				}

				watch.Stop();

				sumTicks += watch.Elapsed.Ticks;
				action(watch.Elapsed);
			}

			TimeSpan ts = new TimeSpan(sumTicks / count1);
			action(ts);
		}

		private static void RunAllTest(Type[] testTypes) 
		{
			GroupData data1 = RunTest(testTypes[0]);
			GroupData data2 = RunTest(testTypes[1]);
			GroupData data3 = RunTest(testTypes[2]);
			GroupData data4 = RunTest(testTypes[3]);

			Console.WriteLine("{0,-22}{1,-22}{2,-22}{3,-22}", data1.Group, data2.Group, data3.Group, data4.Group);

			for (int i = 0; i < data1.List.Count; i++)
				Console.WriteLine("{0,-22}{1,-22}{2,-22}{3,-22}", data1.List[i], data2.List[i], data3.List[i], data4.List[i]);

		}

		private static GroupData RunTest(Type t) 
		{
			GroupData data = new GroupData();
			data.List = new List<TimeSpan>();

			TestMethodAttribute a = t.GetCustomAttribute<TestMethodAttribute>();
			data.Group = a.Description;

			RunTest(t, x => data.List.Add(x));

			return data;
		}

		private class GroupData 
		{
			public string Group { get; set; }
			public List<TimeSpan> List { get; set; }
		}


		private static void ShowMenu(Type[] testTypes)
		{
			Console.WriteLine("测试菜单:");
			Console.WriteLine("===================================");
			Console.WriteLine("x---结束测试");
			Console.WriteLine("0---重新显示菜单");

			int i = 1;
			foreach(Type t in testTypes ) {
				TestMethodAttribute a = t.GetCustomAttribute<TestMethodAttribute>();

				string text = string.Format("{0}---{1}", i++, a.Description);
				Console.WriteLine(text);				
			}
			Console.WriteLine("5---运行所有测试");
		}


		private static int GetTestIndex()
		{
			int number = 0;
			while( true ) {
				Console.Write("\r\n>");
				string input = Console.ReadLine();

				if( input.Equals("x", StringComparison.OrdinalIgnoreCase) )
					return -1;

				if( int.TryParse(input, out number) )
					return number;
			}

		}

	}
}
