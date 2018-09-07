using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Web.Config;
using ClownFish.Web.Serializer;
using ClownFish.Base;
using ClownFish.Base.Http;

namespace ClownFish.Web
{

	/// <summary>
	/// 表示Action的执行结果为JSON
	/// </summary>
	public sealed class JsonResult : IActionResult
	{
		/// <summary>
		/// 需要以JSON形式输出的数据对象
		/// </summary>
		public object Model { get; private set; }
		/// <summary>
		/// 是否在JSON序列化时保留类型信息
		/// </summary>
		public bool KeepTypeInfo { get; private set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="model">将要序列化的对象</param>
		public JsonResult(object model) : this(model, false)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="model">将要序列化的对象</param>
		/// <param name="keepTypeInfo">是否在序列化时保留类型信息，可用于服务端之间的反序列化。</param>
		public JsonResult(object model, bool keepTypeInfo)
		{
			if( model == null )
				throw new ArgumentNullException("model");

			this.Model = model;
			this.KeepTypeInfo = keepTypeInfo;
		}


		void IActionResult.Ouput(HttpContext context)
		{
			string jsonpCallbackParameterName = ConfigurationManager.AppSettings["ClownFish.Web:JsonpCallback-ParameterName"]
												?? FrameworkConfig.Instance.Action.JsonpCallback;

			string jsonpCallback = context.TryGetJsonpCallback(jsonpCallbackParameterName);
			if( jsonpCallback != null ) {
				// 按JSONP方式响应
				context.Response.ContentType = "text/javascript";
				string json = jsonpCallback + "(" + this.Model.ToJson() + ");";
				context.Response.Write(json);
			}
			else {
				context.Response.ContentType = ResponseContentType.Json;
				string json = this.Model.ToJson(this.KeepTypeInfo);
				context.Response.Write(json);
			}
		}

	}


}
