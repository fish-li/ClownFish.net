using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using ClownFish.Base.Http;
using ClownFish.Base.Reflection;



namespace ClownFish.Base.WebClient
{
	/// <summary>
	/// 表示HTTP表单的数据集合（key=value ）
	/// </summary>
	public sealed class FormDataCollection
	{
		private static readonly string s_boundary = "###c9193a17c220432da54fe66073506e9c";

		private List<KeyValuePair<string, object>> _list = new List<KeyValuePair<string, object>>(32);

		/// <summary>
		/// 是否包含上传文件
		/// </summary>
		public bool HasFile { get; private set; }

		/// <summary>
		/// 获取上传文件的请求头
		/// </summary>
		/// <returns></returns>
		public string GetMultipartContentType()
		{
			if( HasFile == false )
				throw new InvalidOperationException();

			return "multipart/form-data; boundary=" + s_boundary;
		}



		/// <summary>
		/// 往集合中添加一个键值对（允许key重复）
		/// </summary>
		/// <param name="key">数据项的名称</param>
		/// <param name="value">数据值</param>
		public FormDataCollection AddString(string key, string value)
		{
			if( string.IsNullOrEmpty(key) )
				throw new ArgumentNullException("key");

			_list.Add(new KeyValuePair<string, object>(key, value ?? string.Empty));
			return this;
		}


		/// <summary>
		/// 往集合中添加一个键值对（允许key重复）
		/// </summary>
		/// <param name="key">数据项的名称</param>
		/// <param name="value">数据值</param>
		public FormDataCollection AddObject(string key, object value)
		{
			if( string.IsNullOrEmpty(key) )
				throw new ArgumentNullException("key");

			// 除了上传文件之外，其它数据都转换成字符串。

			if( value == null )
				return AddString(key, string.Empty);

			Type valueType = value.GetType();

			if( valueType == typeof(string) )
				return AddString(key, (string)value);


			if( valueType == typeof(FileInfo) ) {
				// -----------------------------------------------
				HasFile = true;			// 标记包含上传文件
				// -----------------------------------------------
				HttpFile httFile = HttpFile.CreateFromFileInfo((FileInfo)value);
				_list.Add(new KeyValuePair<string, object>(key, httFile));
				return this;
			}

			if( valueType == typeof(HttpFile) ) {
				// -----------------------------------------------
				HasFile = true;			// 标记包含上传文件
				// -----------------------------------------------
				_list.Add(new KeyValuePair<string, object>(key, value));
				return this;
			}

			if( valueType == typeof(byte[]) ) {
				string text = Convert.ToBase64String((byte[])value);
				return AddString(key, text);
			}

			// string[] ，不处理，可以通过给 Data 设置来解决（用一个KEY多次指定值）。

			_list.Add(new KeyValuePair<string, object>(key, value.ToString()));
			return this;
		}

		/// <summary>
		/// 输出集合数据为 "application/x-www-form-urlencoded" 格式。
		/// 注意：1、忽略上传文件
		///      2、每次调用都会重新计算（因此尽量避免重复调用）
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach( KeyValuePair<string, object> kvp in _list ) {
				if( kvp.Value.GetType() == typeof(string) ) {
					if( sb.Length > 0 )
						sb.Append("&");

					sb.Append(System.Web.HttpUtility.UrlEncode(kvp.Key))
						.Append("=")
						.Append(System.Web.HttpUtility.UrlEncode((string)kvp.Value ?? string.Empty));
				}
			}
			return sb.ToString();
		}


		

		/// <summary>
		/// 将收集的表单数据写入流
		/// </summary>
		/// <param name="stream">Stream实例，用于写入</param>
		/// <param name="encoding">字符编码方式</param>
		public void WriteToStream(Stream stream, Encoding encoding)
		{
			if( stream == null )
				throw new ArgumentNullException("stream");
			if( encoding == null )
				throw new ArgumentNullException("encoding");

			if( HasFile == false ) 
				WriteSimpleTextToStream(stream, encoding);
			
			else 
				WriteMultiFormToStream(stream, encoding);
		}

		private void WriteSimpleTextToStream(Stream stream, Encoding encoding)
		{
			// 获取编码后的字符串
			string text = this.ToString();

			if( string.IsNullOrEmpty(text) == false ) {
				byte[] postData = encoding.GetBytes(text);

				// 写输出流
				using( BinaryWriter bw = new BinaryWriter(stream, 
					encoding /* 指定的编码其实不起作用！ .net API 设计不合理！ */,
					true /* 保持流打开状态，由方法外面关闭 */ ) ) {

					bw.Write(postData);
				}				
			}
		}


		private void WriteMultiFormToStream(Stream stream, Encoding encoding)
		{
			// copy from: http://www.cnblogs.com/fish-li/archive/2011/07/17/2108884.html


			// 数据块的分隔标记，用于设置请求头，注意：这个地方最好不要使用汉字。
			// string boundary = "---------------------------" + Guid.NewGuid().ToString("N");
			// 数据块的分隔标记，用于写入请求体。
			//   注意：前面多了一段： "--" ，而且它们将独占一行。
			byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + s_boundary + "\r\n");

			// 设置请求头。指示是一个上传表单，以及各数据块的分隔标记。
			//request.ContentType = "multipart/form-data; boundary=" + boundary;


			// 写入非文件的key/value部分
			foreach( KeyValuePair<string, object> kvp in _list ) {
				if( kvp.Value.GetType() == typeof(string) ) {

					// 写入数据块的分隔标记
					stream.Write(boundaryBytes, 0, boundaryBytes.Length);

					// 写入数据项描述，这里的Value部分可以不用URL编码
					string str = string.Format(
							"Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}",
							kvp.Key, kvp.Value);

					byte[] data = encoding.GetBytes(str);
					stream.Write(data, 0, data.Length);
				}
			}


			// 写入要上传的文件
			foreach( KeyValuePair<string, object> kvp in _list ) {
				if( kvp.Value.GetType() == typeof(HttpFile) ) {
					HttpFile file = (HttpFile)kvp.Value;
					if( file.FileBody == null )
						throw new InvalidProgramException("没有为上传文件指定文件内容。");

					// 写入数据块的分隔标记
					stream.Write(boundaryBytes, 0, boundaryBytes.Length);

					// 写入文件描述，这里设置一个通用的类型描述：application/octet-stream，具体的描述在注册表里有。
					string description = string.Format(
							"Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
							"Content-Type: application/octet-stream\r\n\r\n",
							kvp.Key, Path.GetFileName(file.FileName));

					// 注意：这里如果不使用UTF-8，对于汉字会有乱码。
					byte[] header = Encoding.UTF8.GetBytes(description);
					stream.Write(header, 0, header.Length);

					// 写入文件内容
					stream.Write(file.FileBody, 0, file.FileBody.Length);
				}				
			}

			// 写入结束标记
			boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + s_boundary + "--\r\n");
			stream.Write(boundaryBytes, 0, boundaryBytes.Length);
		}






		/// <summary>
		/// 将一个对象按"application/x-www-form-urlencoded" 方式序列化
		/// 说明：这个实现与浏览器的实现是有差别的，它不支持数组，也不支持上传文件
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		internal static FormDataCollection Create(object obj)
		{
			if( obj == null )
				throw new ArgumentNullException("obj");


			FormDataCollection collection = obj as FormDataCollection;
			if( collection != null )
				return collection;


			IDictionary dict = obj as IDictionary;
			if( dict != null )
				return CreateFromDictionary(dict);


			NameValueCollection nvCollection = obj as NameValueCollection;
			if( nvCollection != null )
				return CreateFromCollection(nvCollection);


			// 按自定义类型来处理
			return CreateFromObject(obj);
		}


		private static FormDataCollection CreateFromCollection(NameValueCollection nvCollection)
		{
			FormDataCollection collection = new FormDataCollection();

			foreach( string key in nvCollection.AllKeys ) {
				string[] values = nvCollection.GetValues(key);

				foreach( string value in values ) 
					collection.AddString(key, value);
			}

			return collection;
		}

		private static FormDataCollection CreateFromDictionary(IDictionary dict)
		{
			FormDataCollection collection = new FormDataCollection();

			foreach( DictionaryEntry de in dict )
				collection.AddObject(de.Key.ToString(), de.Value);

			return collection;
		}


		private static FormDataCollection CreateFromObject(object obj)
		{
			FormDataCollection collection = new FormDataCollection();
			PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach( PropertyInfo p in properties ) {
				object value = p.FastGetValue(obj);
				if( value == null ) {
					collection.AddString(p.Name, string.Empty);
					continue;
				}

				if( value.GetType() == typeof(string[]) ) {
					foreach( string s in (string[])value )
						collection.AddString(p.Name, (s ?? string.Empty));

				}
				else {
					collection.AddObject(p.Name, value);
				}
			}

			return collection;
		}


	}
}
