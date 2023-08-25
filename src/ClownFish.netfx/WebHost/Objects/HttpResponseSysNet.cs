using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebHost.Objects
{
    internal class HttpResponseSysNet : NHttpResponse
    {
		private bool _bodyIsSend = false;
		private readonly System.Net.HttpListenerResponse _response;

		public override object OriginalHttpResponse => _response;

		public HttpResponseSysNet(System.Net.HttpListenerResponse response, NHttpContext httpContext)
			: base(httpContext)
		{
			_response = response;
		}

		public override int StatusCode {
			get { return _response.StatusCode; }
			set { _response.StatusCode = value; }
		}

		public override string ContentType {
			get { return _response.ContentType; }
			set { _response.ContentType = value; }
		}

		public override Stream OutputStream => _response.OutputStream;

		public override bool HasStarted => _bodyIsSend;

        public override long ContentLength {
            get => _response.ContentLength64;
            set => _response.ContentLength64 = value;
        }

        public override Encoding ContentEncoding {
			get => _response.ContentEncoding;
			set => _response.ContentEncoding = value;
		}

		public override void SetCookie2(string name, string value, DateTime? expires = null)
		{
			if( name.IsNullOrEmpty() )
				throw new ArgumentNullException(nameof(name));

			System.Net.Cookie cookie = new System.Net.Cookie(name, value);
			cookie.HttpOnly = true;
			cookie.Path = "/";

			if( expires != null && expires.HasValue )
				cookie.Expires = expires.Value;

			_response.Cookies.Add(cookie);
		}

		public override bool SetHeader(string name, string value, bool ignoreExist)
		{
            value = value ?? string.Empty;

            if( (ignoreExist == false) || (_response.Headers.AllKeys.Contains(name) == false) ) {
                _response.Headers.Set(name, value);
                return true;
            }
            else {
                return false;
            }
		}

        public override bool RemoveHeader(string name)
        {
            _response.Headers.Remove(name);
            return true;
        }

        public override bool SetHeaders(string name, string[] values, bool ignoreExist)
        {
            if( values.IsNullOrEmpty() )
                return false;

            if( (ignoreExist == false) || (_response.Headers.AllKeys.Contains(name) == false) ) {
                foreach( var value in values )
                    _response.Headers.InternalAdd(name, value);
                return true;
            }
            else {
                return false;
            }
        }

        public override void ClearHeaders()
        {
			_response.Headers.Clear();
		}


		public override void Write(byte[] buffer)
		{
			if( buffer != null && buffer.Length > 0 ) {

				_bodyIsSend = true;
				_response.OutputStream.Write(buffer, 0, buffer.Length);
			}
		}

        public override void WriteAll(byte[] buffer)
        {
            if( buffer != null && buffer.Length > 0 ) {

                _bodyIsSend = true;
                _response.ContentLength64 = buffer.Length;
                _response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }

        public override async Task WriteAsync(byte[] buffer)
		{
			if( buffer != null && buffer.Length > 0 ) {

				_bodyIsSend = true;
				await _response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
			}
		}

        public override async Task WriteAllAsync(byte[] buffer)
        {
            if( buffer != null && buffer.Length > 0 ) {

                _bodyIsSend = true;
                _response.ContentLength64 = buffer.Length;
                await _response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        public override void Close()
		{
			_response.Close();
		}

        public override IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetAllHeaders()
        {
            foreach(string name in _response.Headers.AllKeys ) {
                string[] values = _response.Headers.GetValues(name);
                yield return new KeyValuePair<string, IEnumerable<string>>(name, values);
            }
        }
    }
}
