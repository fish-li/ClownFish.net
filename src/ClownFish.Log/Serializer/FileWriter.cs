using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Xml;
using ClownFish.Log.Configuration;


namespace ClownFish.Log.Serializer
{
	/// <summary>
	/// 将日志记录到文件的写入器
	/// </summary>
	public sealed class FileWriter : ILogWriter
	{
		private static string s_rootDirectory = null;

		private static readonly string s_separateLine = "<!--###############-f2781505-f286-4c9d-b73d-fa78eae22723-###############-->";


		#region ILogWriter 成员

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="config"></param>
		[MethodImpl( MethodImplOptions.Synchronized)]
		public void Init(WriterSection config)
		{
			string value = config.GetOptionValue("RootDirectory");
			if( string.IsNullOrEmpty(value) )
				throw new LogConfigException("日志配置文件中，没有为FileWriter指定RootDirectory属性。");


			if( s_rootDirectory != null )
				return;


			// 支持绝对路径，和相对路径
			string rootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);


			// 检查日志根目录是否存在
			if( Directory.Exists(rootDirectory) == false )
				Directory.CreateDirectory(rootDirectory);


			if( rootDirectory.EndsWith("\\") == false )
				rootDirectory = rootDirectory + "\\";
			

			// 检查需要记录的各个数据类型的子目录是否存在。
			foreach( var item in WriterFactory.Config.Types ) {
				string path = rootDirectory + item.Type.Name;
				if( Directory.Exists(path) == false )
					Directory.CreateDirectory(path);
			}

			s_rootDirectory = rootDirectory;
		}



		internal string GetFilePath(Type t)
		{
			return  string.Format(@"{0}{1}\{2}_{3}.log",
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


			string xml = XmlHelper.XmlSerialize(info, Encoding.UTF8);

			string contents = xml + "\r\n\r\n" + s_separateLine + "\r\n\r\n";
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
				string xml = XmlHelper.XmlSerialize(info, Encoding.UTF8);
				string contents = xml + "\r\n\r\n" + s_separateLine + "\r\n\r\n";
				sb.Append(contents);
			}


			if( sb.Length > 0)
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
