using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Web.Client
{
	/// <summary>
	/// 定义HttpClient的扩展方法的工具类
	/// </summary>
	public static class ClientExtensions
	{
		/// <summary>
		/// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求
		/// </summary>
		/// <param name="option">HttpOption的实例，用于描述请求参数</param>
		/// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
		public static string Send(this HttpOption option)
		{
			return Send<string>(option);
		}

		/// <summary>
		/// 根据指定的HttpOption参数，用【异步】方式发起一次HTTP请求
		/// </summary>
		/// <param name="option">HttpOption的实例，用于描述请求参数</param>
		/// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
		public async static Task<string> SendAsync(this HttpOption option)
		{
			return await SendAsync<string>(option);
		}


		/// <summary>
		/// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求
		/// </summary>
		/// <typeparam name="T">返回值的类型参数</typeparam>
		/// <param name="option">HttpOption的实例，用于描述请求参数</param>
		/// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
		public static T Send<T>(this HttpOption option)
		{
			if( option == null )
				throw new ArgumentNullException("option");

			option.CheckInput();

			HttpClient client = ObjectFactory.New<HttpClient>();
			bool urlQuery = option.IsMustQueryString();

			string requestUrl = option.Url;
			if( option.Data != null && urlQuery ) {	//GET 请求，需要将参数合并到URL，形成查询字符串参数
				if( option.Url.IndexOf('?') < 0 )
					requestUrl = option.Url + "?" + HttpClient.GetQueryString(option.Data);
				else
					requestUrl = option.Url + "&" + HttpClient.GetQueryString(option.Data);
			}
				
			HttpWebRequest request = client.CreateWebRequest(requestUrl);

			SetWebRequest(request, option);

			if( option.SetRequestAction != null )
				option.SetRequestAction(request);


			if( option.Data != null && urlQuery == false )	// POST提交数据
				client.SetRequestData(option.Data, option.Format);

			using( HttpWebResponse response = client.GetResponse() ) {

				if( option.ReadResponseAction != null )
					option.ReadResponseAction(response);

				return client.GetResult<T>(response);
			}
		}


		/// <summary>
		/// 根据指定的HttpOption参数，用【异步】方式发起一次HTTP请求
		/// </summary>
		/// <typeparam name="T">返回值的类型参数</typeparam>
		/// <param name="option">HttpOption的实例，用于描述请求参数</param>
		/// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
		public async static Task<T> SendAsync<T>(this HttpOption option)
		{
			if( option == null )
				throw new ArgumentNullException("option");

			option.CheckInput();



			HttpClient client = ObjectFactory.New<HttpClient>();
			bool urlQuery = option.IsMustQueryString();

			string requestUrl = option.Url;
			if( option.Data != null && urlQuery ) {	//GET 请求，需要将参数合并到URL，形成查询字符串参数
				if( option.Url.IndexOf('?') < 0 )
					requestUrl = option.Url + "?" + HttpClient.GetQueryString(option.Data);
				else
					requestUrl = option.Url + "&" + HttpClient.GetQueryString(option.Data);
			}

			HttpWebRequest request = client.CreateWebRequest(requestUrl);

			SetWebRequest(request, option);

			if( option.SetRequestAction != null )
				option.SetRequestAction(request);


			if( option.Data != null && urlQuery == false )	// POST提交数据
				await client.SetRequestDataAsync(option.Data, option.Format);

			using( HttpWebResponse response = await client.GetResponseAsync() ) {

				if( option.ReadResponseAction != null )
					option.ReadResponseAction(response);

				return client.GetResult<T>(response);
			}
		}


		private static void SetWebRequest(HttpWebRequest request, HttpOption option)
		{
			request.Method = option.Method;

			foreach( NameValue item in option.Headers )
				request.Headers.InternalAdd(item.Name, item.Value);

			if( option.Cookie != null )
				request.CookieContainer = option.Cookie;

			if( option.Credentials != null )
				request.Credentials = option.Credentials;		// CredentialCache.DefaultCredentials;

			if( option.Timeout.HasValue )
				request.Timeout = option.Timeout.Value;

			if( string.IsNullOrEmpty(option.ContentType) == false)
				request.ContentType = option.ContentType;

			if( string.IsNullOrEmpty(option.UserAgent) == false )
				request.UserAgent = option.UserAgent;

			if( option.DisableAutoRedirect )
				request.AllowAutoRedirect = false;

		}

	}
}
