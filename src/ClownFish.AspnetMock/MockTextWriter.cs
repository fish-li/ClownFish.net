using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ClownFish.AspnetMock
{
	/// <summary>
	/// 模拟输出流的TextWriter
	/// </summary>
	public sealed class MockTextWriter : System.IO.TextWriter
	{
		//internal MockTextWriter() { }

		private MemoryStream _stream = new MemoryStream();

		internal Encoding StreamEncoding = Encoding.UTF8;

		/// <summary>
		/// 获取输出流的编码
		/// </summary>
		public override Encoding Encoding
		{
			get { return this.StreamEncoding; }
		}


		/// <summary>
		/// 写输出流
		/// </summary>
		/// <param name="value"></param>
		public override void Write(string value)
		{
			Byte[] buffer = this.StreamEncoding.GetBytes(value);
			_stream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// 获取输出流的引用
		/// </summary>
		public Stream Stream
		{
			get { return _stream; }
		}

		/// <summary>
		/// 获取输出流中的文本
		/// </summary>
		/// <returns></returns>
		public string GetText()
		{
			Stream.Position = 0;

			byte[] buffer = new byte[Stream.Length];
			Stream.Read(buffer, 0, buffer.Length);

			return StreamEncoding.GetString(buffer);
		}


		protected override void Dispose(bool disposing)
		{
			_stream.Dispose();

			base.Dispose(disposing);
		}
		

	}
}
