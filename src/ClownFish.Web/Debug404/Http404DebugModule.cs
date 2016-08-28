using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using ClownFish.Base.Reflection;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Reflection;

namespace ClownFish.Web.Debug404
{
	/// <summary>
	/// 用于诊断404错误的HTTP模块
	/// </summary>
	internal sealed class Http404DebugModule : IHttpModule
	{
		private static readonly string DiagnoseResultKey = "ClownFish.Web-DiagnoseResult-HttpContext-Item-Key";

		
		/// <summary>
		/// Init
		/// </summary>
		/// <param name="app"></param>
		public void Init(HttpApplication app)
		{
			// 仅仅在 Debug 模式有效
			if( WebRuntime.Instance.IsDebugMode ) {
				app.BeginRequest += new EventHandler(app_BeginRequest);
				app.Error += new EventHandler(app_Error);

			}
		}



		internal void app_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			app.Context.Items[DiagnoseResultKey] = new DiagnoseResult();
		}

		/// <summary>
		/// 尝试从HttpContext实例中获取关联的DiagnoseResult实例
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static DiagnoseResult TryGetDiagnoseResult(HttpContext context)
		{
			if( WebRuntime.Instance.IsDebugMode == false )
				return null;

			return context.Items[DiagnoseResultKey] as DiagnoseResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IHttpHandler TryGetHttp404PageHandler(HttpContext context)
		{
			// 当诊断模式启用时，优先选择直接返回Http404PageHandler的方式，避免进入错误解析模式。

			DiagnoseResult diagnoseResult = TryGetDiagnoseResult(context);
			if( diagnoseResult != null ) {
				if( diagnoseResult.AssemblyList == null ) {
					ControllerRecognizer recognizer = ObjectFactory.New<ControllerRecognizer>();
					List<Assembly> list = recognizer.GetControllerAssembly();
					diagnoseResult.AssemblyList = (from a in list  select a.FullName).ToList();
				}
				
				diagnoseResult.UrlActionInfo = context.Items[UrlActionInfo.HttpContextItemKey] as UrlActionInfo;

				//diagnoseResult.ErrorMessages.Add("不能根据当前URL创建请求处理器，当前URL：" + context.Request.RawUrl);
				
				return new Http404PageHandler(diagnoseResult);
			}
			else
				return null;
		}

		void app_Error(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;

			Exception ex = app.Server.GetLastError();

			HttpException httpException = ex as HttpException;
			if( httpException != null && httpException.GetHttpCode() == 404 ) {

				IHttpHandler handler = TryGetHttp404PageHandler(app.Context);
				if( handler == null )
					return;		// 应该不会执行到这里！

				app.Server.ClearError();
				app.Response.StatusCode = 404;
				app.Response.AddHeader("x-Http404DebugModule", "OK");

				(handler as Http404PageHandler).DiagnoseResult.ErrorMessages.Add(httpException.Message);

				this.ShowDebugInfo(app.Context, handler);
			}
		}

		private void ShowDebugInfo(HttpContext context, IHttpHandler handler)
		{
			if( HttpRuntime.UsingIntegratedPipeline == false ) {
				handler.ProcessRequest(context);
			}
			else {
				bool isAfterMapRequestHandler = false;
				try {
					context.RemapHandler(handler);
				}
				catch( InvalidOperationException ) {
					// 在IIS7的集成管线模式下，当前阶段 >= MapRequestHandler 时，会引发这个异常
					isAfterMapRequestHandler = true;
				}

				if( isAfterMapRequestHandler )
					handler.ProcessRequest(context);
			}
						

		}

		
		public void Dispose()
		{
		}
	}
}
