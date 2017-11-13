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

        private void Write(Stream stream, object data, SerializeFormat format)
        {
            switch( format ) {
                case SerializeFormat.Text:
                    WriteAsTextFormat(stream, data);
                    break;

                case SerializeFormat.Json:
                    WriteAsJsonFormat(stream, data);
                    break;

                case SerializeFormat.Json2:
                    WriteAsJson2Format(stream, data);
                    break;

                case SerializeFormat.Xml:
                    WriteAsXmlFormat(stream, data);
                    break;

                case SerializeFormat.Form:
                    WriteAsFormFormat(stream, data);
                    break;

                case SerializeFormat.Auto:
                case SerializeFormat.None:
                    WriteAsAutoFormat(stream, data);
                    break;

                default:
                    throw new NotSupportedException();
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

        private void WriteBinary(Stream stream, byte[] bb)
        {
            if( bb != null && bb.Length > 0 ) {
                using( BinaryWriter bw = new BinaryWriter(stream, s_defaultEncoding, true) ) {
                    bw.Write(bb);
                }
            }
        }

        private void WriteAsTextFormat(Stream stream, object data)
        {
            _request.ContentType = "text/plain";
            WriteText(stream, data.ToString());
        }

        private void WriteAsJsonFormat(Stream stream, object data)
        {
            _request.ContentType = "application/json";
            string text = (data.GetType() == typeof(string))
                            ? (string)data
                            : JsonExtensions.ToJson(data, false);
            WriteText(stream, text);
        }

        private void WriteAsJson2Format(Stream stream, object data)
        {
            _request.ContentType = "application/json";
            string text = (data.GetType() == typeof(string))
                            ? (string)data
                            : JsonExtensions.ToJson(data, true);    // 序列化时保留类型信息
            WriteText(stream, text);
        }

        private void WriteAsXmlFormat(Stream stream, object data)
        {
            _request.ContentType = "application/xml";
            string text = (data.GetType() == typeof(string))
                                ? (string)data
                                 : XmlHelper.XmlSerialize(data, Encoding.UTF8);
            WriteText(stream, text);
        }

        private void WriteAsFormFormat(Stream stream, object data)
        {
            if( data.GetType() == typeof(string) ) {
                _request.ContentType = "application/x-www-form-urlencoded";
                WriteText(stream, (string)data);
            }
            else {
                FormDataCollection form = FormDataCollection.Create(data);

                if( form.HasFile )
                    _request.ContentType = form.GetMultipartContentType();
                else
                    _request.ContentType = "application/x-www-form-urlencoded";
                form.WriteToStream(stream, Encoding.UTF8);
            }
        }

        private void WriteAsAutoFormat(Stream stream, object data)
        {
            // 这二类场景就不指定内容头了
            if( data.GetType() == typeof(string) ) {
                WriteText(stream, (string)data);
            }
            else if( data.GetType() == typeof(byte[]) ) {
                WriteBinary(stream, (byte[])data);
            }
            else {
                throw new NotSupportedException();
            }
        }

        



	}
}
