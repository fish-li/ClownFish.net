using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

// 此处代码来源于博客【在.net中读写config文件的各种方法】的示例代码
// http://www.cnblogs.com/fish-li/archive/2011/12/18/2292037.html


namespace ClownFish.Base.Xml
{
	/// <summary>
	/// 支持CDATA序列化的包装类
	/// </summary>
	public sealed class XmlCdata : IXmlSerializable
	{
		private string _value;

		/// <summary>
		/// 构造函数
		/// </summary>
		public XmlCdata() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="value">初始值</param>
		public XmlCdata(string value)
		{
			this._value = value;
		}

		/// <summary>
		/// 原始值。
		/// </summary>
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			this._value = reader.ReadElementContentAsString();
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteCData(this._value);
		}

		/// <summary>
		/// ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this._value;
		}

		/// <summary>
		/// 重载操作符，支持隐式类型转换。
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static implicit operator XmlCdata(string text)
		{
			return new XmlCdata(text);
		}

		/// <summary>
		/// 重载操作符，支持隐式类型转换。
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static implicit operator string(XmlCdata text)
		{
			return text.ToString();
		}
	}
}
