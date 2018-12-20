using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Base.WebClient
{
	/// <summary>
	/// 一个用于发送HTTP请求的客户端
	/// </summary>
	public sealed class HttpClient : BaseEventObject
	{
		#region 处理HTTPS错误

		static HttpClient()
		{
            SysNetInitializer.Init();
        }

		#endregion

		/// <summary>
		/// HttpWebRequest实例引用
		/// </summary>
		public HttpWebRequest Request { get; private set; }

		private BeforeSendRequestEventArgs _beforeArgs = new BeforeSendRequestEventArgs();



		#region 事件定义及处理

		/// <summary>
		/// 创建HttpWebRequest前将会引发此事件，提供最后一个修改请求参数的机会。
		/// 例如：可以添加一些全局的请求头。
		/// （扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<BeforeCreateRequestEventArgs> OnBeforeCreateRequest;

		/// <summary>
		/// 创建请求前的事件参数类型
		/// </summary>
		public sealed class BeforeCreateRequestEventArgs : System.EventArgs
		{
			/// <summary>
			/// 将要用来创建请求的URL地址
			/// </summary>
			public string Url { get; internal set; }

			/// <summary>
			/// 实际要请求的URL，允许在事件中修改，最终以修改后的结果发送请求。
			/// </summary>
			public string RequestUrl { get; set; }
		}


		private string ExecuteBeforeCreateRequestEvent(string url)
		{
			// 创建HttpWebRequest前事件
			EventHandler<BeforeCreateRequestEventArgs> handler = this.OnBeforeCreateRequest;
			if( handler != null ) {
				var e = new BeforeCreateRequestEventArgs { Url = url };
				handler(this, e);

				// 如果在事件订阅中修改了RequestUrl，就以事件中修改的结果为准。
				if( string.IsNullOrEmpty(e.RequestUrl) == false )
					return e.RequestUrl;
			}

			return url;
		}



		/// <summary>
		/// 创建HttpWebRequest之后将会引发此事件，提供最后一个修改请求参数的机会。
		/// 例如：可以添加一些全局的请求头。
		/// （扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<BeforeSendRequestEventArgs> OnBeforeSendRequest;

		/// <summary>
		/// 创建请求之后的事件参数类型
		/// </summary>
		public sealed class BeforeSendRequestEventArgs : System.EventArgs
		{
			/// <summary>
			/// HttpWebRequest实例
			/// </summary>
			public HttpWebRequest Request { get; internal set; }

			/// <summary>
			/// 当前请求提交的数据，可能为 null
			/// </summary>
			public object Data { get; internal set; }

			/// <summary>
			/// 提交数据的序列化格式
			/// </summary>
			public SerializeFormat Format { get; internal set; }
		}


		internal static readonly string DllVersion
			= System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(HttpClient).Assembly.Location).FileVersion;

		internal void ExecuteBeforeSendRequestEvent()
		{
            // 有些2B程序不规范，用了自己特定定的数据格式，不允许指定"Content-Type"，所以这里不能自动增加这个请求头
            // 例如： jira，它就不允许在GET时指定"Content-Type"，
            //        即使是上传文件，虽然是POST，但是它没有使用标准的multipart/form-data，也不能指定"Content-Type"
            // 所以，为了与这些2B程序能正常交互，只能注释下面二行代码。

            //if( string.IsNullOrEmpty(Request.ContentType) && Request.Headers["Content-Type"] == null )
            //    Request.ContentType = "application/x-www-form-urlencoded";

            if( string.IsNullOrEmpty(Request.UserAgent) && Request.Headers["User-Agent"] == null )
				Request.UserAgent = "ClownFish.net.HttpClient/"	 + DllVersion;


			// 发送请求前事件
			EventHandler<BeforeSendRequestEventArgs> handler = this.OnBeforeSendRequest;
			if( handler != null ) {
				_beforeArgs.Request = this.Request;
				handler(this, _beforeArgs);
			}
		}


		#endregion

		

		/// <summary>
		/// 创建HttpWebRequest
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public HttpWebRequest CreateWebRequest(string url)
		{
			if( string.IsNullOrEmpty(url) )
				throw new ArgumentNullException("url");

			// 触发事件
			string requestUrl = ExecuteBeforeCreateRequestEvent(url);
			

			Request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
			Request.Method = "GET";
			Request.ServicePoint.Expect100Continue = false;

             return Request;
		}



		/// <summary>
		/// 设置要提交的数据
		/// </summary>
		/// <param name="data"></param>
		/// <param name="format"></param>
		public void SetRequestData(object data, SerializeFormat format)
		{
			if( data == null )
				return;

			_beforeArgs.Data = data;
			_beforeArgs.Format = format;

			RequestWriter writer = new RequestWriter(Request);
			writer.Write(data, format);
		}

		/// <summary>
		/// 设置要提交的数据（以异步方式）
		/// </summary>
		/// <param name="data"></param>
		/// <param name="format"></param>
		public async Task SetRequestDataAsync(object data, SerializeFormat format)
		{
			if( data == null )
				return;

			_beforeArgs.Data = data;
			_beforeArgs.Format = format;

			RequestWriter writer = new RequestWriter(Request);
			await writer.WriteAsync(data, format);
		}

		/// <summary>
		/// 提交请求，并获取服务端响应结果
		/// </summary>
		/// <returns></returns>
		public HttpWebResponse GetResponse()
		{
            // 2018-10-19 注释这里，因为对于POST请求，执行到这里已经向服务端发起请求了
            //// 触发事件
            //ExecuteBeforeSendRequestEvent();

            // 获取服务端响应
            return (HttpWebResponse)this.Request.GetResponse();
		}

		/// <summary>
		/// 提交请求，并获取服务端响应结果（以异步方式）
		/// </summary>
		/// <returns></returns>
		public async Task<HttpWebResponse> GetResponseAsync()
		{
			//// 触发事件
			//ExecuteBeforeSendRequestEvent();

			// 获取服务端响应
			return (HttpWebResponse)await this.Request.GetResponseAsync();
		}

		/// <summary>
		/// 从HttpWebResponse读取结果
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="response"></param>
		/// <returns></returns>
		public T GetResult<T>(HttpWebResponse response)
		{
			if( response == null )
				throw new ArgumentNullException("response");


			using( ResponseReader reader = new ResponseReader(response) ) {
				return reader.Read<T>();
			}
		}
		


		/// <summary>
		/// 根据指定的URL以及提交数据，用【同步】方式发起一次HTTP请求
		/// </summary>
		/// <typeparam name="T">返回值的类型参数</typeparam>
		/// <param name="url">要访问的URL地址</param>
		/// <param name="data">要提交的数据对象</param>
		/// <param name="format">数据对象在传输过程中采用的序列化方式</param>
		/// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
		public static T Send<T>(string url, 
								object data = null, 
								SerializeFormat format = SerializeFormat.Form)
		{
            return new HttpOption {
                Url = url,
                Data = data,
                Format = format
            }.Send<T>();
		}


		/// <summary>
		/// 根据指定的URL以及提交数据，用【同步】方式发起一次HTTP请求
		/// </summary>
		/// <typeparam name="T">返回值的类型参数</typeparam>
		/// <param name="url">要访问的URL地址</param>
		/// <param name="data">要提交的数据对象</param>
		/// <param name="format">数据对象在传输过程中采用的序列化方式</param>
		/// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
		public async static Task<T> SendAsync<T>(string url, 
												object data = null, 
												SerializeFormat format = SerializeFormat.Form)
		{
            return await new HttpOption {
                Url = url,
                Data = data,
                Format = format
            }.SendAsync<T>();
        }
	}





}
