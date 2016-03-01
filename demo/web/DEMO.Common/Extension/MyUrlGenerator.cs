using System;
using System.Collections.Specialized;
using System.Web;

namespace DEMO.Common
{
	/// <summary>
	/// 用于生成URL的工具类
	/// </summary>
	public class MyUrlGenerator : NameValueCollection
	{
		/// <summary>
		/// 一个完整的URL，比如："/dir/aaa.aspx?id=2&xx=3"
		/// </summary>
		/// <param name="requestRawUrl"></param>
		public MyUrlGenerator(string requestRawUrl)
		{
			if( string.IsNullOrEmpty(requestRawUrl) )
				throw new ArgumentNullException("requestRawUrl");

			int p = requestRawUrl.IndexOf('?');
			bool includeQueryString = (p > 1 || p == requestRawUrl.Length);
			string filepath = (includeQueryString ? requestRawUrl.Substring(0, p) : requestRawUrl);
			string querystring = (includeQueryString ? requestRawUrl.Substring(p+1) : string.Empty);

			this.m_requestPath = filepath;
			if( querystring.Length > 0 )
				this.Add(HttpUtility.ParseQueryString(querystring));
		}


		//public MyUrlGenerator(string path, NameValueCollection queryString)
		//{
		//    if( string.IsNullOrEmpty(path) )
		//        throw new ArgumentNullException("path");
		//    if( queryString == null )
		//        throw new ArgumentNullException("queryString");

		//    this.m_requestPath = path;
		//    this.Add(queryString);
		//}


		private string m_requestPath = null;


		/// <summary>
		/// <para>生成一个新的URL，将会增加或修改参数，还有可能会忽略一些值。</para>
		/// <para>说明：这个方法不会修改现有的查询字符串参数。</para>
		/// </summary>
		/// <param name="name">参数名称</param>
		/// <param name="newValue">新的参数值</param>
		/// <param name="skipKeys">要忽略的Key名称</param>
		/// <returns>新的URL字符串</returns>
		public string GetNewUrl(string name, string newValue, params string[] skipKeys)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			int n = this.Count;
			bool isExist = false;

			for( int i = 0; i < n; i++ ) {
				string key = this.GetKey(i);

				if( skipKeys != null && skipKeys.Length > 0 ) {
					if( Array.IndexOf<string>(skipKeys, key) >= 0 )
						continue;
				}

				if( string.Compare(key, name, true) == 0 ) {
					isExist = true;
					if( !string.IsNullOrEmpty(newValue) )
						sb.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(newValue));
				}
				else {
					sb.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(this[i]));
				}
			}

			if( (!isExist) && (!string.IsNullOrEmpty(name)) && (!string.IsNullOrEmpty(newValue)) )
				sb.AppendFormat("&{0}={1}", name, newValue);

			return string.Concat(this.m_requestPath,
				(sb.Length > 0 ? string.Concat("?", sb.ToString().Substring(1)) : string.Empty));
		}

		/// <summary>
		/// 往集合中添加一个参数值。
		/// </summary>
		/// <param name="name">name</param>
		/// <param name="value">value</param>
		/// <returns>当前MyUrlGenerator对象</returns>
		new public MyUrlGenerator Add(string name, string value)
		{
			if( string.IsNullOrEmpty(name) )
				return this;

			if( string.IsNullOrEmpty(value) == false )
				base.Add(name, value);

			return this;	// 返回新对象，便于写成连缀的语句。
		}


		/// <summary>
		/// 从集合中删除一批参数值。
		/// </summary>
		/// <param name="names">names</param>
		/// <returns>当前MyUrlGenerator对象</returns>
		public MyUrlGenerator Remove(params string[] names)
		{
			if( names == null || names.Length == 0 )
				return this;

			foreach( string name in names )
				base.Remove(name);

			return this;
		}

		/// <summary>
		/// <para>修改集合中某个参数值。</para>
		/// <para>如果集合中已经存在指定的键，此方法将用指定值改写现有值列表。若要向现有值列表添加新值，请使用 Add 方法。</para>
		/// <para>如果集合中没有指定的键，此方法将使用指定键和指定值创建一个新项。</para>
		/// </summary>
		/// <param name="name">name</param>
		/// <param name="value">value</param>
		/// <returns>当前MyUrlGenerator对象</returns>
		new public MyUrlGenerator Set(string name, string value)
		{
			if( string.IsNullOrEmpty(name) )
				return this;

			if( string.IsNullOrEmpty(value) )
				base.Remove(name);
			else
				base.Set(name, value);

			return this;	// 返回新对象，便于写成连缀的语句。
		}

		/// <summary>
		/// 将集合中所有的参数以一个URL格式返回。返回值会包含原始的请求文件路径。
		/// </summary>
		/// <returns>将集合中所有的参数以一个URL格式返回</returns>
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach( string key in this.AllKeys ) {
				string value = this[key];
				if( (string.IsNullOrEmpty(key) == false) && (string.IsNullOrEmpty(value) == false) )
					sb.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(value));

			}
			return string.Concat(this.m_requestPath,
				(sb.Length > 0 ? string.Concat("?", sb.ToString().Substring(1)) : string.Empty));
		}

	}
}