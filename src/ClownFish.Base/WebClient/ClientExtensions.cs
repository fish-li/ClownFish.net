using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Base.WebClient
{
	/// <summary>
	/// 定义HttpClient的扩展方法的工具类
	/// </summary>
	public static class ClientExtensions
	{
        #region Send 方法遇到 WebException 异常封装，支持失败重试


        /// <summary>
        /// 封装HttpOption的Send扩展方法发送HTTP请求，并提供失败重试功能。
        /// 如果遇到WebException异常，就转换成RemoteWebException异常
        /// </summary>
        /// <param name="option">HttpOption实例，用于包含HTTP请求的发送参数</param>
        /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
        /// <returns></returns>
        public static string GetResult(this HttpOption option, Retry retry = null)
        {
            try {
                if( retry == null)
                    return option.Send<string>();
                else
                    return retry.Run(option.Send);
            }
            catch( WebException ex ) {
                // 返回一个容易获取异常消息的异常类型
                throw new RemoteWebException(ex, option.Url);
            }
        }

        /// <summary>
        /// 封装HttpOption的Send扩展方法发送HTTP请求，并提供失败重试功能。
        /// 如果遇到WebException异常，就转换成RemoteWebException异常
        /// </summary>
        /// <param name="option">HttpOption实例，用于包含HTTP请求的发送参数</param>
        /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
        /// <returns></returns>
        public async static Task<string> GetResultAsync(this HttpOption option, Retry retry = null)
        {
            try {
                if( retry == null)
                    return await option.SendAsync();
                else
                    return await retry.RunAsync(option.SendAsync);
            }
            catch( WebException ex ) {
                // 返回一个容易获取异常消息的异常类型
                throw new RemoteWebException(ex, option.Url);
            }
        }

        /// <summary>
        /// 封装HttpOption的Send扩展方法发送HTTP请求，并提供失败重试功能。
        /// 如果遇到WebException异常，就转换成RemoteWebException异常
        /// </summary>
        /// <param name="option">HttpOption实例，用于包含HTTP请求的发送参数</param>
        /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
        /// <returns></returns>
        public static T GetResult<T>(this HttpOption option, Retry retry = null)
        {
            try {
                if( retry ==  null)
                    return option.Send<T>();
                else
                    return retry.Run(option.Send<T>);
            }
            catch( WebException ex ) {
                // 返回一个容易获取异常消息的异常类型
                throw new RemoteWebException(ex, option.Url);
            }
        }


        /// <summary>
        /// 封装HttpOption的Send扩展方法发送HTTP请求，并提供失败重试功能。
        /// 如果遇到WebException异常，就转换成RemoteWebException异常
        /// </summary>
        /// <param name="option">HttpOption实例，用于包含HTTP请求的发送参数</param>
        /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试。</param>
        /// <returns></returns>
        public async static Task<T> GetResultAsync<T>(this HttpOption option, Retry retry = null)
        {

            try {
                if( retry == null)
                    return await option.SendAsync<T>();
                else
                    return await retry.RunAsync(option.SendAsync<T>);
            }
            catch( WebException ex ) {
                // 返回一个容易获取异常消息的异常类型
                throw new RemoteWebException(ex, option.Url);
            }
        }

        #endregion



		#region 发送请求

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

            // 获取实际请求址，并创建 HttpWebRequest 实例
            string requestUrl = option.GetRequestUrl();
			HttpWebRequest request = client.CreateWebRequest(requestUrl);

            // 设置HttpWebRequest请求头之类的属性
            SetWebRequest(request, option);

			option.SetRequestAction?.Invoke(request);		// 调用委托

            // 设置提交数据
			client.SetRequestData(option.GetPostData(), option.Format);

			using( HttpWebResponse response = client.GetResponse() ) {

				option.ReadResponseAction?.Invoke(response);	// 调用委托

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

            // 获取实际请求址，并创建 HttpWebRequest 实例
            string requestUrl = option.GetRequestUrl();
            HttpWebRequest request = client.CreateWebRequest(requestUrl);

            SetWebRequest(request, option);

			option.SetRequestAction?.Invoke(request);       // 调用委托

            // 设置提交数据
            await client.SetRequestDataAsync(option.GetPostData(), option.Format);

			using( HttpWebResponse response = await client.GetResponseAsync() ) {

				option.ReadResponseAction?.Invoke(response);	// 调用委托

				return client.GetResult<T>(response);
			}
		}

		#endregion

		#region 内部方法

		

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

            if( HttpOption.RequestHasBody(option.Method) ) {
                if( string.IsNullOrEmpty(option.ContentType) == false )
                    request.ContentType = option.ContentType;
            }

			if( string.IsNullOrEmpty(option.UserAgent) == false )
				request.UserAgent = option.UserAgent;

			if( option.DisableAutoRedirect )
				request.AllowAutoRedirect = false;

		}

		#endregion

	}
}
