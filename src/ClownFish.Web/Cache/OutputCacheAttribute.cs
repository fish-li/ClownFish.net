using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace ClownFish.Web
{
	/// <summary>
	/// 用于给Action返回结果指定输出缓存的修饰属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class OutputCacheAttribute : Attribute
	{
		private OutputCacheParameters _cacheSettings 
				= new OutputCacheParameters { Duration = 600, VaryByParam = "none" };

		internal OutputCacheParameters CacheSettings
		{
			get { return _cacheSettings; }
		}

		/// <summary>
		/// 获取或设置 OutputCacheProfile 名称，该名称与输出缓存项的设置关联。
		/// </summary>
		[XmlAttribute]
		public string CacheProfile
		{
			get { return _cacheSettings.CacheProfile ?? String.Empty; }
			set { _cacheSettings.CacheProfile = value; }
		}

		
		/// <summary>
		/// 取或设置缓存项要保留在输出缓存中的时间。
		/// </summary>
		[XmlAttribute]
		public int Duration
		{
			get { return _cacheSettings.Duration; }
			set { _cacheSettings.Duration = value; }
		}

		/// <summary>
		/// 获取或设置一个值，该值确定缓存项的位置。
		/// </summary>
		[XmlAttribute]
		public OutputCacheLocation Location
		{
			get { return _cacheSettings.Location; }
			set { _cacheSettings.Location = value; }
		}

		/// <summary>
		/// 获取或设置一个值，该值确定是否设置了 HTTP Cache-Control: no-store 指令。
		/// </summary>
		[XmlAttribute]
		public bool NoStore
		{
			get { return _cacheSettings.NoStore; }
			set { _cacheSettings.NoStore = value; }
		}
		/// <summary>
		/// 获取或设置缓存项依赖的一组数据库和表名称对。
		/// </summary>
		[XmlAttribute]
		public string SqlDependency
		{
			get { return _cacheSettings.SqlDependency ?? String.Empty; }
			set { _cacheSettings.SqlDependency = value; }
		}
		/// <summary>
		/// 获取或设置用于改变缓存项的一组逗号分隔的字符集（内容编码）。
		/// </summary>
		[XmlAttribute]
		public string VaryByContentEncoding
		{
			get { return _cacheSettings.VaryByContentEncoding ?? String.Empty; }
			set { _cacheSettings.VaryByContentEncoding = value; }
		}
		/// <summary>
		/// 获取输出缓存用来改变缓存项的自定义字符串列表。
		/// </summary>
		[XmlAttribute]
		public string VaryByCustom
		{
			get { return _cacheSettings.VaryByCustom ?? String.Empty; }
			set { _cacheSettings.VaryByCustom = value; }
		}
		/// <summary>
		/// 获取或设置用于改变缓存项的一组逗号分隔的标头名称。标头名称标识与请求关联的 HTTP 标头。
		/// </summary>
		[XmlAttribute]
		public string VaryByHeader
		{
			get { return _cacheSettings.VaryByHeader ?? String.Empty; }
			set { _cacheSettings.VaryByHeader = value; }
		}
		/// <summary>
		/// 获取查询字符串或窗体 POST 参数的逗号分隔列表，该列表由输出缓存用来改变缓存项。
		/// </summary>
		[XmlAttribute]
		public string VaryByParam
		{
			get { return _cacheSettings.VaryByParam ?? String.Empty; }
			set { _cacheSettings.VaryByParam = value; }
		}

		internal void SetResponseCache(HttpContext context)
		{
			if( context == null )
				throw new ArgumentNullException("context");

			OutputCachedPage page = new OutputCachedPage(_cacheSettings);
			page.ProcessRequest(context);
		}

		private sealed class OutputCachedPage : Page
		{
			private OutputCacheParameters _cacheSettings;

			public OutputCachedPage(OutputCacheParameters cacheSettings)
			{
				this.ID = Guid.NewGuid().ToString();
				_cacheSettings = cacheSettings;
			}

			protected override void FrameworkInitialize()
			{
				base.FrameworkInitialize();
				InitOutputCache(_cacheSettings);
			}
		}
	}
}
