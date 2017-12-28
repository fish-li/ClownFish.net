using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Configuration;

namespace ClownFish.Log.Serializer
{
	/// <summary>
	/// 将日志记录到JSON文件的写入器
	/// </summary>
	public sealed class JsonWriter : ILogWriter
	{
		private static string s_rootDirectory = null;

		private static readonly string s_separateLine = "<!--###############-f2781505-f286-4c9d-b73d-fa78eae22723-###############-->";


		#region ILogWriter 成员

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="config"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Init(WriterSection config)
		{
			string value = config.GetOptionValue("RootDirectory");
			if( string.IsNullOrEmpty(value) )
				throw new LogConfigException("日志配置文件中，没有为JsonWriter指定RootDirectory属性。");


			if( s_rootDirectory != null )
				return;


            s_rootDirectory = DirectoryHelper.InitDirectory(value);
        }



		internal string GetFilePath(Type t)
		{
			return string.Format(@"{0}{1}\{2}_{3}.json.log",
								s_rootDirectory, t.Name, t.Name, DateTime.Now.ToString("yyyy-MM-dd"));
		}

		/// <summary>
		/// 写入单条日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="info"></param>
		public void Write<T>(T info) where T : Model.BaseInfo
		{
			if( info == null )
				return;

			// 注意：取类型名称时，不采用 info.GetType().Name ，因为可能有继承情况
			string filePath = GetFilePath(typeof(T));


			string json = ClownFish.Base.JsonExtensions.ToJson(info);

			string contents = json + "\r\n\r\n" + s_separateLine + "\r\n\r\n";
			File.AppendAllText(filePath, contents, Encoding.UTF8);
		}

		/// <summary>
		/// 批量写入日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public void Write<T>(List<T> list) where T : Model.BaseInfo
		{
			if( list == null || list.Count == 0 )
				return;

			// 注意：取类型名称时，不采用 info.GetType().Name ，因为可能有继承情况
			string filePath = GetFilePath(typeof(T));

			StringBuilder sb = new StringBuilder();

			foreach( T info in list ) {
				string json = ClownFish.Base.JsonExtensions.ToJson(info);
				string contents = json + "\r\n\r\n" + s_separateLine + "\r\n\r\n";
				sb.Append(contents);
			}


			if( sb.Length > 0 )
				File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// 根据日志ID获取单条日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="guid"></param>
		/// <returns></returns>
		public T Get<T>(Guid guid) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 根据指定的一段时间获取对应的日志记录
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public List<T> GetList<T>(DateTime t1, DateTime t2) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
