using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.Http
{
	/// <summary>
	/// HTTP头的存储集合
	/// </summary>
	public sealed class HttpHeaderCollection : List<NameValue>
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public HttpHeaderCollection() : base(8) { }

		/// <summary>
		/// 索引器，根据名称访问集合
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string this[string name]
		{
			get
			{
				// 如果KEY重复，这里只返回第一个匹配的结果
				// KEY重复的场景需要提供GetValues方法，暂且先不实现
				NameValue item = this.FirstOrDefault(
					x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

				if( item != null )
					return item.Value;
				else
					return null;
			}
		}


		/// <summary>
		/// 增加一个键值对
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void Add(string name, string value)
		{
			if( string.IsNullOrEmpty(name) )
				throw new ArgumentNullException("name");

			//if( string.IsNullOrEmpty(value) )
			if( value == null )
				throw new ArgumentNullException("value");

			NameValue nv = new NameValue { Name = name, Value = value };
			this.Add(nv);
		}

		/// <summary>
		/// 根据指定的名称删除键值列表元素
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			if( string.IsNullOrEmpty(name) )
				throw new ArgumentNullException("name");


			int index = this.FindIndex(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			if( index >= 0 )
				this.RemoveAt(index);
		}
	}
}
