using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// 用于邮件发送的Writer
	/// </summary>
	public sealed class MailWriterConfig : BaseWriterConfig
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public MailWriterConfig()
		{
			this.Name = "Mail";
		}

		/// <summary>
		/// 邮件接收人地址
		/// </summary>
		[XmlAttribute]
		public string Receivers { get; set; }


		/// <summary>
		/// 验证属性是否配置正确
		/// </summary>
		public override void Valid()
		{
			if( string.IsNullOrEmpty(Receivers) )
				throw new LogConfigException("日志配置文件中，没有为MailWriter指定Receivers属性。");

		}


	}


}
