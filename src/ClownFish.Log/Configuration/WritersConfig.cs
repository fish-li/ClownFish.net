using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// 所有Writer的配置集合
	/// </summary>
	public sealed class WritersConfig
	{

		// 在这里直接定义5个Writer，会影响扩展性（应该定义成集合形式）
		// 但是，这样做法会生成较好的配置性

//#if _MongoDB_
		/// <summary>
		/// MongDbWriterConfig
		/// </summary>
		public MongDbWriterConfig MongDb { get; set; }
//#endif 

		/// <summary>
		/// FileWriterConfig
		/// </summary>
		public FileWriterConfig File { get; set; }

		/// <summary>
		/// MailWriterConfig
		/// </summary>
		public MailWriterConfig Mail { get; set; }

		/// <summary>
		/// MsmqWriterConfig
		/// </summary>
		public MsmqWriterConfig Msmq { get; set; }

		/// <summary>
		/// WinLogWriterConfig
		/// </summary>
		public WinLogWriterConfig WinLog { get; set; }

		/// <summary>
		/// 获取集合的迭代器
		/// </summary>
		/// <returns></returns>
		public IEnumerable<BaseWriterConfig> GetWriters()
		{
//#if _MongoDB_
			if( MongDb != null )
				yield return MongDb;
//#endif
			if( File != null )
				yield return File;

			if( Mail != null )
				yield return Mail;

			if( Msmq != null )
				yield return Msmq;

			if( WinLog != null )
				yield return WinLog;
		}
	}
}
