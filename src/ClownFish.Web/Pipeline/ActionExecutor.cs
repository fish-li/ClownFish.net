using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.Http;
using ClownFish.Base.Reflection;
using ClownFish.Base.TypeExtend;
using ClownFish.Base.WebClient;
using ClownFish.Web.Reflection;
using ClownFish.Web.Serializer;

namespace ClownFish.Web
{

	/// <summary>
	/// 执行Action的处理器
	/// </summary>
	public sealed class ActionExecutor : BaseEventObject
	{
		/// <summary>
		/// HttpContext实例引用
		/// </summary>
		public HttpContext HttpContext { get; private set; }

		private IHttpHandler Handler;

		private InvokeInfo InvokeInfo;

		/// <summary>
		/// 保存用户额外的数据，如果需要使用，请自行赋值。
		/// </summary>
		public System.Collections.Hashtable UserData { get; set; }

		internal static readonly string DllVersion
			= System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(ActionExecutor).Assembly.Location).FileVersion;

		#region 事件定义

		/// <summary>
		/// 开始进入执行阶段的事件（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<ActionEventArgs> BeginRequest;

		/// <summary>
		/// 执行跨域检查阶段的事件（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<CorsCheckEventArgs> CorsCheck;

		/// <summary>
		/// 授权检查阶段的事件，此事件发生在AuthorizeAttribute检查之后。（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<AuthorizeCheckEventArgs> AuthorizeRequest;

		/// <summary>
		/// 获取到Action参数后的事件（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<EndObtainParametersEventArgs> EndObtainParameters;

		/// <summary>
		/// 执行Action【前】的事件（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<BeforeExecuteActionEventArgs> BeforeExecuteAction;

		/// <summary>
		/// 执行Action【后】的事件（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<AfterExecuteActionEventArgs> AfterExecuteAction;

		/// <summary>
		/// 设置输出阶段的事件（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<ProcessResultEventArgs> ProcessResult;

		/// <summary>
		/// 结束Action执行的事件，即使发生异常，这个事件也会触发（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<ActionEventArgs> EndRequest;

		/// <summary>
		/// 执行过程中发生异常的事件（扩展点：允许EventSubscriber的继承类来订阅）
		/// </summary>
		public event EventHandler<ActionExceptionEventArgs> OnError;

		#endregion


		#region 核心执行逻辑


		private void SetController()
		{
			BaseController controller = this.InvokeInfo.Instance as BaseController;

			if( controller != null ) 
				controller.HttpContext = this.HttpContext;			
			
		}

		internal void ProcessRequest(HttpContext context, ActionHandler handler)
		{
			if( context == null )
				throw new ArgumentNullException("context");
			if( handler == null )
				throw new ArgumentNullException("handler");

			this.HttpContext = context;
			this.Handler = handler;
			this.InvokeInfo = handler.InvokeInfo;

			// 设置 BaseController 的相关属性
			SetController();
			
			// 进入请求处理阶段
			ExecuteBeginRequest();

			// 安全检查
			ExecuteSecurityCheck();

			// 授权检查
			ExecuteAuthorizeRequest();
			
			// 执行 Action
			object actionResult = ExecuteAction();

			// 设置输出缓存
			SetOutputCache();

			// 处理方法的返回结果
			ExecuteProcessResult(actionResult);
		}


		private object ExecuteAction()
		{
			// 准备要传给调用方法的参数
			object[] parameters = ExecuteObtainParameters();
						
			// 引发执行前事件
			TriggerBeforeExecuteAction(parameters);

			// 开始执行 Action 逻辑
			object actionResult = null;
			Exception lastException = null;
			try {
				if( this.InvokeInfo.Action.HasReturn )
					actionResult = this.InvokeInfo.Action.MethodInfo.FastInvoke(this.InvokeInfo.Instance, parameters);

				else
					this.InvokeInfo.Action.MethodInfo.FastInvoke(this.InvokeInfo.Instance, parameters);
			}
			catch( Exception ex ) {
				lastException = ex;
				throw;
			}
			finally {
				TriggerAfterExecuteAction(parameters, actionResult, lastException);
			}

			return actionResult;
		}


		
		internal async Task ProcessRequestAsync(HttpContext context, TaskAsyncActionHandler handler)
		{
			if( context == null )
				throw new ArgumentNullException("context");
			if( handler == null )
				throw new ArgumentNullException("handler");

			this.HttpContext = context;
			this.Handler = handler;
			this.InvokeInfo = handler.InvokeInfo;

			// 设置 BaseController 的相关属性
			SetController();

			// 在异步执行前，先保存当前同步上下文的实例，供异步完成后执行切换调用。
			SynchronizationContext syncContxt = SynchronizationContext.Current;
			if( syncContxt == null )
				throw new InvalidProgramException();


			// 进入请求处理阶段
			ExecuteBeginRequest();

			// 安全检查
			ExecuteSecurityCheck();

			// 授权检查
			ExecuteAuthorizeRequest();
			
			// 执行 Action
			object actionResult = await ExecuteActionAsync();
			
			// 切换到原先的上下文环境，执行余下操作
			syncContxt.Send(x => {
				// 设置输出缓存
				SetOutputCache();				

				// 处理方法的返回结果
				ExecuteProcessResult(actionResult);
			}, this.HttpContext);
		}
				
		
		
		private async Task<object> ExecuteActionAsync()
		{
			// 准备要传给调用方法的参数
			object[] parameters = ExecuteObtainParameters();
						
			// 引发执行前事件
			TriggerBeforeExecuteAction(parameters);


			// 开始执行 Action 逻辑
			object actionResult = null;
			Exception lastException = null;

			try {
				// 说明：能进入这里的，只能二类返回类型： Task, Task<T>，因此如果Action有返回值，只能是Task<T>类型

				if( this.InvokeInfo.Action.HasReturn ) {
					Task task = (Task)this.InvokeInfo.Action.MethodInfo.FastInvoke(this.InvokeInfo.Instance, parameters);
					await task;

					// 从 Task<T> 中获取返回值
					PropertyInfo property = task.GetType().GetProperty("Result", BindingFlags.Instance | BindingFlags.Public);
					actionResult = property.FastGetValue(task);
				}
				else {
					await (Task)this.InvokeInfo.Action.MethodInfo.FastInvoke(this.InvokeInfo.Instance, parameters);
				}
			}
			catch( Exception ex ) {
				lastException = ex;
				throw;
			}
			finally {
				TriggerAfterExecuteAction(parameters, actionResult, lastException);
			}
			return actionResult;
		}

		#endregion


		#region 阶段性执行过程（非关键性过程）


		private T CreateEventArgs<T>() where T : ActionEventArgs, new()
		{
			T e = new T();
			e.HttpContext = this.HttpContext;
			e.ControllerInstance = this.InvokeInfo.Instance;
			e.ActionMethod = this.InvokeInfo.Action.MethodInfo;
			return e;
		}

		private void ExecuteBeginRequest()
		{
			this.HttpContext.Response.AppendHeader("X-ClownFish.Web", DllVersion);

			EventHandler<ActionEventArgs> eventHandler = this.BeginRequest;
			if( eventHandler != null )
				eventHandler(this, CreateEventArgs<ActionEventArgs>());
		}

		internal void ExecuteEndRequest()
		{
			EventHandler<ActionEventArgs> eventHandler = this.EndRequest;
			if( eventHandler != null )
				eventHandler(this, CreateEventArgs<ActionEventArgs>());
		}

		internal bool ProcessException(Exception ex)
		{
			EventHandler<ActionExceptionEventArgs> eventHandler = this.OnError;
			if( eventHandler != null ) {
				ActionExceptionEventArgs e = CreateEventArgs<ActionExceptionEventArgs>();
				e.Exception = ex;

				// 调用外部的事件处理器
				eventHandler(this, e);

				return e.ExceptionHandled;
			}

			return false;
		}

		private void ExecuteAuthorizeRequest()
		{
			// 验证请求是否允许访问（身份验证）
			AuthorizeAttribute authorize = this.InvokeInfo.GetAuthorize();
			if( authorize != null ) {
				if( authorize.AuthenticateRequest(this.HttpContext) == false )
					ExceptionHelper.Throw403Exception(this.HttpContext);
			}


			EventHandler<AuthorizeCheckEventArgs> eventHandler = this.AuthorizeRequest;
			if( eventHandler != null ) {
				AuthorizeCheckEventArgs e = CreateEventArgs<AuthorizeCheckEventArgs>();
				e.Attribute = authorize;

				// 调用外部的事件处理器
				eventHandler(this, e);
			}
		}

		private void ExecuteSecurityCheck()
		{
			// ASP.NET的安全检查
			if( this.InvokeInfo.Action.Attr.NeedValidateRequest() )
				this.HttpContext.Request.ValidateInput();


			// AJAX跨域许可检查
			ExecuteCorsCheck();
		}

		private void ExecuteCorsCheck()
		{
			// 通常情况下，没有必要限制跨域，
			// 如果是希望确保安全调用，可以增加参数检查，单纯检查请求头可能没有意义
			// 所以，在ClownFish.Web中，默认是允许跨域，会自动应答。

			string origin = this.HttpContext.Request.Headers["Origin"];

			if( string.IsNullOrEmpty(origin) )
				return;

			EventHandler<CorsCheckEventArgs> eventHandler = this.CorsCheck;
			if( eventHandler != null ) {
				CorsCheckEventArgs e = CreateEventArgs<CorsCheckEventArgs>();
				e.Origin = origin;

				// 调用外部的事件处理器
				// 如果希望限制跨域来源，可以在事件中检查，并将 e.IsForbidden 设置为 true
				eventHandler(this, e);

				// 如果禁止跨域调用，将不再继续执行（忽略下面的输出响应头）
				if( e.IsForbidden )
					return;
			}


			// 为了支持单元测试，所以没有调用：this.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin);
			WebRuntime.Instance.WriteResponseHeader(this.HttpContext.Response, "Access-Control-Allow-Origin", origin);
			WebRuntime.Instance.WriteResponseHeader(this.HttpContext.Response, "Access-Control-Allow-Headers", "*");
			WebRuntime.Instance.WriteResponseHeader(this.HttpContext.Response, "Access-Control-Allow-Credentials", "true");
			WebRuntime.Instance.WriteResponseHeader(this.HttpContext.Response, "Access-Control-Allow-Methods", "*");
		}

		private object[] ExecuteObtainParameters()
		{
			ActionDescription action = this.InvokeInfo.Action;

			if( action.Parameters == null || action.Parameters.Length == 0 )
				return null;


			// 从工厂中获取一个能够构造Action参数的提供者实例。
			IActionParametersProvider provider 
						= ActionParametersProviderFactory.Instance.CreateProvider(this.HttpContext);

			object[] parameters = null;

			// 优先使用内部接口版本
			IActionParametersProvider2 p2 = provider as IActionParametersProvider2;
			if( p2 != null )
				parameters = p2.GetParameters(this.HttpContext, action);
			else
				parameters = provider.GetParameters(this.HttpContext, action.MethodInfo);


			// 引发事件，允许在外部修改获取到的事件参数，可修改数组中元素，但数组本身不允许重新指定。
			EventHandler<EndObtainParametersEventArgs> eventHandler = this.EndObtainParameters;
			if( eventHandler != null ) {
				EndObtainParametersEventArgs e = CreateEventArgs<EndObtainParametersEventArgs>();
				e.Parameters = parameters;

				// 调用外部的事件处理器
				eventHandler(this, e);
			}

			return parameters;
		}

		private void TriggerBeforeExecuteAction(object[] parameters)
		{
			EventHandler<BeforeExecuteActionEventArgs> beforeEvent = this.BeforeExecuteAction;
			if( beforeEvent != null ) {
				BeforeExecuteActionEventArgs e = CreateEventArgs<BeforeExecuteActionEventArgs>();
				e.Parameters = parameters;

				beforeEvent(this, e);
			}
		}

		private void TriggerAfterExecuteAction(object[] parameters, object actionResult, Exception ex)
		{
			EventHandler<AfterExecuteActionEventArgs> afterEvent = this.AfterExecuteAction;
			if( afterEvent != null ) {
				AfterExecuteActionEventArgs e = CreateEventArgs<AfterExecuteActionEventArgs>();
				e.Parameters = parameters;
				e.ExecuteResult = actionResult;
				e.Exception = ex;

				afterEvent(this, e);
			}
		}
		

		private void SetOutputCache()
		{
			// 设置OutputCache
			OutputCacheAttribute outputCache = this.InvokeInfo.GetOutputCacheSetting();
			if( outputCache != null )
				outputCache.SetResponseCache(this.HttpContext);
		}

		internal void ExecuteProcessResult(object result)
		{
			EventHandler<ProcessResultEventArgs> eventHandler = this.ProcessResult;
			if( eventHandler != null ) {
				ProcessResultEventArgs e = CreateEventArgs<ProcessResultEventArgs>();
				e.ExecuteResult = result;

				// 调用外部的事件处理器
				eventHandler(this, e);

				// 执行结果已被外部处理，不需要再继续执行。
				if( e.ResultHandled )
					return;
			}

			if( result == null )
				return;


			IActionResult actionResult = result as IActionResult;
			if( actionResult == null ) 
				actionResult = ObjectToResult(result, this.InvokeInfo.Action.Attr);
			

			actionResult.Ouput(this.HttpContext);
		}




		/// <summary>
		/// 尝试根据方法的修饰属性来构造IActionResult实例
		/// </summary>
		/// <param name="result">Action的执行结果</param>
		/// <param name="actionAttr">Action方法上的ActionAttribute实例</param>
		/// <returns></returns>
		private IActionResult ObjectToResult(object result, ActionAttribute actionAttr)
		{
            // 如果返回值是byte[]就直接按BinaryResult方式封装结果
            //if( result != null && result.GetType() == typeof(byte[]) )
            //    return new ClownFish.Web.Action.BinaryResult((byte[])result);

            IActionResult actionResult = null;
			SerializeFormat format = actionAttr.OutFormat;

			// 先判断是不是由客户端指定的自动模式，如果是就解析客户端需要的序列化格式
			if( format == SerializeFormat.Auto ) {
				// 如果是自动响应，那么就根据请求头的指定的方式来决定
				string expectFormat = this.HttpContext.Request.Headers["X-Result-Format"];

				if( string.IsNullOrEmpty(expectFormat) == false ) {
					SerializeFormat outFormat;
					if( Enum.TryParse<SerializeFormat>(expectFormat, true, out outFormat) )
						format = outFormat;
				}
			}


			// 根据已指定的序列化格式包装具体的IActionResult实例

			if( format == SerializeFormat.Json )
				actionResult = new JsonResult(result);

			else if( format == SerializeFormat.Json2 )
				actionResult = new JsonResult(result, true);

			else if( format == SerializeFormat.Xml )
				actionResult = new XmlResult(result);

			else if( format == SerializeFormat.Text )
				actionResult = new TextResult(result);

			else if( format == SerializeFormat.Form ) {
				string text = FormDataCollection.Create(result).ToString();
				actionResult = new TextResult(text);
			}


			// 无法构造出IActionResult实例，就交给ActionProcessor来处理
			if( actionResult == null ) {
				ActionHelper actionProcessor = ObjectFactory.New<ActionHelper>();
				actionResult = actionProcessor.ObjectToResult(result);
			}

			return actionResult;
		}

		#endregion


	}
}
