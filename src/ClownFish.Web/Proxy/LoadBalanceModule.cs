//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;

//namespace ClownFish.Web.Proxy
//{
//	/// <summary>
//	/// 用于实现简单的软负载均衡
//	/// </summary>
//	public class LoadBalanceModule : IHttpModule
//	{
//		/// <summary>
//		/// 实现 IHttpModule 接口的 Init方法
//		/// </summary>
//		/// <param name="app"></param>
//		public void Init(HttpApplication app)
//		{
//			app.BeginRequest += app_BeginRequest;
//		}

//		void app_BeginRequest(object sender, EventArgs e)
//		{
//			throw new NotImplementedException();
//		}

//		/// <summary>
//		/// 实现 IHttpModule 接口的 Dispose方法
//		/// </summary>
//		public void Dispose()
//		{
//		}
//	}
//}
