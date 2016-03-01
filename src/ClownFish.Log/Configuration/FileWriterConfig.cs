using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// 执行文件写入的Writer
	/// </summary>
	public sealed class FileWriterConfig : BaseWriterConfig
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public FileWriterConfig()
		{
			this.Name = "File";
		}

		/// <summary>
		/// 文件的写入根路径（日志组件会根据数据类型再创建子目录）
		/// </summary>
		[XmlAttribute]
		public string RootDirectory { get; set; }


		/// <summary>
		/// 验证属性是否配置正确
		/// </summary>
		public override void Valid()
		{
			if( string.IsNullOrEmpty(RootDirectory) )
				throw new LogConfigException("日志配置文件中，没有为FileWriter指定RootDirectory属性。");


			//if( System.IO.Directory.Exists(RootDirectory) == false )
			//	System.IO.Directory.CreateDirectory(RootDirectory);
		}
	}



}
