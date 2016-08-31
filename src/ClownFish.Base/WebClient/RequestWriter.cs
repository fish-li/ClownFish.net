using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.Base.Xml;

namespace ClownFish.Base.WebClient
{
	internal class RequestWriter
	{
		private HttpWebRequest _request;

		private static readonly Encoding s_defaultEncoding = Encoding.UTF8;


		public RequestWriter(HttpWebRequest request)
		{
			if( request == null )
				throw new ArgumentNullException("request");

			_request = request;

			if( _request.Method.EqualsIgnoreCase("GET") )
				_request.Method = "POST";
		}

		public void Write(object data, SerializeFormat format)
		{
			using( Stream stream = _request.GetRequestStream() ) {
				Write(stream, data, format);
			}
		}

		public async Task WriteAsync(object data, SerializeFormat format)
		{
			using( Stream stream = await _request.GetRequestStreamAsync() ) {
				Write(stream, data, format);
			}
		}


		private void WriteText(Stream stream, string text)
		{
			if( text != null && text.Length > 0 ) {
				byte[] bb = s_defaultEncoding.GetBytes(text);

				if( bb != null && bb.Length > 0 ) {
					using( BinaryWriter bw = new BinaryWriter(stream, s_defaultEncoding, true) ) {
						bw.Write(bb);
					}
				}
			}
		}


		private void Write(Stream stream, object data, SerializeFormat format)
		{
			if( data.GetType() == typeof(string) ) {
				WriteText(stream, (string)data);
				return;
			}

			switch( format ) {
				case SerializeFormat.Text: {
						_request.ContentType = "text/plain";
						WriteText(stream, data.ToString());
						break;
					}

				case SerializeFormat.Json: {
						_request.ContentType = "application/json";
						string text = (data.GetType() == typeof(string))
										? (string)data
										: JsonExtensions.ToJson(data, false);
						WriteText(stream, text);
						break;
					}

				case SerializeFormat.Json2: {
						_request.ContentType = "application/json";
						string text = (data.GetType() == typeof(string))
										? (string)data
										: JsonExtensions.ToJson(data, true);
						WriteText(stream, text);
						break;
					}

				case SerializeFormat.Xml: {
						_request.ContentType = "application/xml";
						string text = (data.GetType() == typeof(string))
											? (string)data
											 : XmlHelper.XmlSerialize(data, Encoding.UTF8);
						WriteText(stream, text);
						break;
					}

				case SerializeFormat.Form: {
						FormDataCollection form = FormDataCollection.Create(data);

						if( form.HasFile )
							_request.ContentType = form.GetMultipartContentType();
						else
							_request.ContentType = "application/x-www-form-urlencoded";
						form.WriteToStream(stream, Encoding.UTF8);
						break;
					}

				default:
					throw new NotSupportedException();
			}
		}
	}
}
