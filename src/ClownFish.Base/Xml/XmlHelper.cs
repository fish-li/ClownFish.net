using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

// 此处代码来源于博客【在.net中读写config文件的各种方法】的示例代码
// http://www.cnblogs.com/fish-li/archive/2011/12/18/2292037.html

namespace ClownFish.Base.Xml
{
	/// <summary>
	/// 实现XML序列化与反序列化的包装工具类
	/// </summary>
	public static class XmlHelper
	{

		private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
		{
			if( o == null )
				throw new ArgumentNullException("o");
			if( encoding == null )
				throw new ArgumentNullException("encoding");

			XmlSerializer serializer = new XmlSerializer(o.GetType());

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineChars = "\r\n";
			settings.Encoding = encoding;
			settings.IndentChars = "    ";

			using( XmlWriter writer = XmlWriter.Create(stream, settings) ) {
				serializer.Serialize(writer, o);
			}
		}

		
		/// <summary>
		/// 将一个对象序列化为XML字符串
		/// </summary>
		/// <param name="o">要序列化的对象</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>序列化产生的XML字符串</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202")]
		public static string XmlSerialize(object o, Encoding encoding)
		{
			using( MemoryStream stream = new MemoryStream() ) {
				XmlSerializeInternal(stream, o, encoding);

				stream.Position = 0;
				using( StreamReader reader = new StreamReader(stream, encoding) ) {
					return reader.ReadToEnd();
				}
			}
		}


		/// <summary>
		/// 将一个对象按XML序列化的方式写入到一个文件
		/// </summary>
		/// <param name="o">要序列化的对象</param>
		/// <param name="path">保存文件路径</param>
		/// <param name="encoding">编码方式</param>
		public static void XmlSerializeToFile(object o, string path, Encoding encoding)
		{
			if( string.IsNullOrEmpty(path) )
				throw new ArgumentNullException("path");

			using( FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write) ) {
				XmlSerializeInternal(file, o, encoding);
			}
		}



		/// <summary>
		/// 从XML字符串流中反序列化对象
		/// </summary>
		/// <param name="stream">包含对象的XML字符串流</param>
		/// <param name="destType">要序列化的目标类型</param>
		/// <returns>反序列化得到的对象</returns>
		public static object XmlDeserialize(Stream stream, Type destType)
		{
			if( stream == null )
				throw new ArgumentNullException("stream");
			if( destType == null )
				throw new ArgumentNullException("destType");

			XmlSerializer mySerializer = new XmlSerializer(destType);

			using( StreamReader sr = new StreamReader(stream) ) {
				return mySerializer.Deserialize(sr);
			}
		}

		/// <summary>
		/// 从XML字符串中反序列化对象
		/// </summary>
		/// <param name="s">包含对象的XML字符串</param>
		/// <param name="destType">要序列化的目标类型</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>反序列化得到的对象</returns>
		public static object XmlDeserialize(string s, Type destType, Encoding encoding)
		{
			if( string.IsNullOrEmpty(s) )
				throw new ArgumentNullException("s");
			if( destType == null )
				throw new ArgumentNullException("destType");
			if( encoding == null )
				throw new ArgumentNullException("encoding");

			using( MemoryStream ms = new MemoryStream(encoding.GetBytes(s)) ) {
				return XmlDeserialize(ms, destType);
			}
		}


		/// <summary>
		/// 从XML字符串中反序列化对象
		/// </summary>
		/// <typeparam name="T">结果对象类型</typeparam>
		/// <param name="s">包含对象的XML字符串</param>
		/// <returns>反序列化得到的对象</returns>
		public static T XmlDeserialize<T>(string s)
		{
			return (T)XmlDeserialize(s, typeof(T), Encoding.UTF8);
		}




		/// <summary>
		/// 读入一个文件，并按XML的方式反序列化对象。
		/// </summary>
		/// <typeparam name="T">结果对象类型</typeparam>
		/// <param name="path">文件路径</param>
		/// <returns>反序列化得到的对象</returns>
		public static T XmlDeserializeFromFile<T>(string path)
		{
			if( string.IsNullOrEmpty(path) )
				throw new ArgumentNullException("path");

			try {
				using( FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read) ) {
					return (T)XmlDeserialize(fs, typeof(T));
				}
			}
			catch( Exception ex ) {
				throw new InvalidDataException("XML反序列失败，当前文件：" + path, ex);
			}
		}
	}
}
