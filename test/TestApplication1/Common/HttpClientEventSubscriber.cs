using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;
using ClownFish.Base.WebClient;
using ClownFish.Web;



namespace ClownFish.TestApplication1.Common
{
	public class HttpClientEventSubscriber : EventSubscriber<HttpClient>
	{
		internal static CookieContainer ShareCookie = null;

		internal static bool EnableCorsTest = true;

		private string _currentUrl = null;


		public override void SubscribeEvent(HttpClient instance)
		{
			if( EnableCorsTest )
				instance.OnBeforeCreateRequest += instance_OnBeforeCreateRequest;

			instance.OnBeforeSendRequest += instance_OnBeforeSendRequest;
		}

		
		void instance_OnBeforeCreateRequest(object sender, HttpClient.BeforeCreateRequestEventArgs e)
		{
			_currentUrl = e.Url;

			// 将请求址切换到反向代理的URL
			e.RequestUrl = "http://www.fish-ajax-cors.com/cors-transfer/test.aspx";
		}

		void instance_OnBeforeSendRequest(object sender, HttpClient.BeforeSendRequestEventArgs e)
		{
			e.Request.Timeout = 5000;
			e.Request.AllowAutoRedirect = false;

			if( e.Request.CookieContainer == null )
				e.Request.CookieContainer = ShareCookie;

			if( _currentUrl != null ) {
				// 将实际请求地址写入请求头，供反向代理使用。
				e.Request.Headers.Add("x-target-url", _currentUrl);
			}

			//e.Request.Proxy = new WebProxy("127.0.0.1", 8888);
		}


		
	}
}
