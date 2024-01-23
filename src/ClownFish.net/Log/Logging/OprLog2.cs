namespace ClownFish.Log.Logging;
public partial class OprLog
{
    /// <summary>
    /// 创建一个新的日志ID
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetNewId()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 创建一个新的日志ID
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetNewId(DateTime time)
    {
        return time.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N");
    }

    internal static OprLog CreateNew(BasePipelineContext context)
    {
        OprLog log = new OprLog();

        log.SetBaseInfo(context);

        return log;
    }

    /// <summary>
    /// 填充一些基本信息
    /// </summary>
    /// <param name="context"></param>
    public void SetBaseInfo(BasePipelineContext context = null)
    {
        if( context == null ) {
            this.StartTime = DateTime.Now;
            this.OprId = GetNewId(this.StartTime);
        }
        else {
            this.StartTime = context.StartTime;
            this.OprId = context.ProcessId;
        }

        this.Status = 200;
        this.AppName = EnvUtils.GetAppName();
        this.HostName = EnvUtils.GetHostName();
        this.EnvName = EnvUtils.GetClusterName();
        this.AppKind = LoggingOptions.AppKindDefaultValue;
    }

    internal void CalcTime(long performanceThresholdMs, DateTime endTime)
    {
        TimeSpan time = endTime - this.StartTime;
        this.Duration = (long)time.TotalMilliseconds;

        // 判断是否为慢请求
        bool isslow = performanceThresholdMs > 0
                                ? this.Duration >= performanceThresholdMs
                                : false;
        this.IsSlow = isslow ? 1 : 0;
    }

    /// <summary>
    /// 绑定 HttpRequest 信息
    /// </summary>
    /// <param name="httpContext"></param>
    public void SetHttpRequest(NHttpContext httpContext)
    {
        if( this.Url != null )  // 避免多次调用
            return;

        try {
            if( httpContext == null
                || httpContext.Request == null  // ASP.NET程序在初始化时，访问Request属性可能会出现异常
                )
                return;
        }
        catch {
            return;
        }

        NHttpRequest request = httpContext.Request;

        this.HttpMethod = request.HttpMethod;
        this.Url = request.FullUrl;        
        this.HttpRef = request.Header(HttpHeaders.Request.Referer);
        this.InSize = request.ContentLength;

        if( LoggingOptions.LogUserAgent ) {
            // UserAgent 这个东西在绝大部分情况下根本没有作用，并且这个东西通常很巨大，导致日志会占用大量存储空间
            this.UserAgent = request.Header(HttpHeaders.Request.UserAgent);
        }
    }


    internal void SetHttpData(NHttpContext httpContext)
    {
        this.OutSize = httpContext.Response.ContentLength;
        this.Status = httpContext.Response.StatusCode;

        if( this.OprKind.IsNullOrEmpty() ) {
            this.OprKind = OprKinds.HttpIn;
        }


        if( httpContext.IsTransfer == false && this.Module == null && this.Controller == null && this.Action == null && httpContext.PipelineContext.Action != null ) {
            ActionDescription action = httpContext.PipelineContext.Action;

            MethodBase actionMethod = action.MethodInfo;
            Type controllerType = action.ControllerType;

            ControllerTitleAttribute a1 = controllerType.GetMyAttribute<ControllerTitleAttribute>();
            ActionTitleAttribute a2 = actionMethod.GetMyAttribute<ActionTitleAttribute>();

            this.Module = a1?.Module ?? controllerType.Namespace;
            this.Controller = a1?.Name ?? controllerType.Name;
            this.Action = a2?.Name ?? actionMethod.Name;

            if( this.OprName.IsNullOrEmpty() ) {
                this.OprName = controllerType.Name + "/" + actionMethod.Name;
            }
        }

        if( this.OprName.IsNullOrEmpty() ) {
            this.OprName = "HttpRequest";
        }
    }


    /// <summary>
    /// 用于无埋点日志收集一些业务信息
    /// </summary>
    /// <param name="httpContext"></param>
    internal void TryGetBizInfo(NHttpContext httpContext)
    {
        if( this.TenantId == null ) {
            this.TenantId = httpContext.Items.TryGet("TenantId") as string;
        }
        if( this.UserId == null ) {
            this.UserId = httpContext.Items.TryGet("UserId") as string;
        }
        if( this.UserCode == null ) {
            this.UserCode = httpContext.Items.TryGet("UserCode") as string;
        }
        if( this.UserName == null ) {
            this.UserName = httpContext.Items.TryGet("UserName") as string;
        }
        if( this.UserRole == null ) {
            this.UserRole = httpContext.Items.TryGet("UserRole") as string;
        }

        if( this.BizId == null ) {
            this.BizId = httpContext.Items.TryGet("Biz-Id") as string;
        }
        if( this.BizName == null ) {
            this.BizName = httpContext.Items.TryGet("Biz-Name") as string;
        }

        if( this.Module == null ) {
            this.Module = httpContext.Items.TryGet("Biz-Module") as string;
        }
        if( this.Controller == null ) {
            this.Controller = httpContext.Items.TryGet("Biz-Controller") as string;
        }
        if( this.Action == null ) {
            this.Action = httpContext.Items.TryGet("Biz-Action") as string;
        }
    }

    internal void SetResponseData(NHttpContext httpContext)
    {
        if( this.Response.HasValue() )
            return;

        if( LoggingOptions.Http.MustLogResponse == false )
            return;

        if( httpContext.PipelineContext.ActionResult == null )
            return;

        string contentType = httpContext.Response.ContentType;
        bool bodyIsText = HttpUtils.ResponseBodyIsText(contentType);
        if( bodyIsText == false )
            return;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            string statusMessage = HttpUtils.GetStatusReasonPhrase(httpContext.Response.StatusCode);
            sb.Append("HTTP/1.1 ").Append(httpContext.Response.StatusCode).Append(' ').Append(statusMessage).AppendLineRN();

            httpContext.Response.AccessHeaders((k, v) => sb.Append(k).Append(": ").AppendLineRN(v));

            if( LoggingOptions.Http.LogResponseBody ) {
                sb.AppendLineRN();

                if( contentType.Contains("json") ) {
                    sb.Append(httpContext.PipelineContext.ActionResult.ToJson());
                }
                else if( contentType.Contains("xml") ) {
                    sb.Append(httpContext.PipelineContext.ActionResult.ToXml2());
                }
                else {
                    sb.Append(httpContext.PipelineContext.ActionResult.ToString());
                }
            }
        }
        catch( Exception ex ) {
            sb.Append("###ClownFish.net在记录Response时出现异常：").Append(ex.ToString());
        }
        finally {
            this.Response = sb.ToString();
            StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// 保存异常数据到日志数据中
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public int SetException(Exception ex)
    {
        if( ex != null ) {
            this.Status = ex.GetErrorCode();
            this.HasError = StatusCodeUtils.IsServerError(this.Status) ? 1 : 0;

            this.ExType = ex.GetType().FullName;
            this.ExMessage = ex.Message;
            this.ExAll = ex.GetErrorLogText();
            return 1;
        }
        return 0;
    }

    /// <summary>
    /// 根据【当前调用所在的方法】填充 Module,Controller,Action
    /// </summary>
    public int SetMCA(int skipFrames = 1)
    {
        try {
            StackFrame stack = new StackFrame(skipFrames, false);
            var method = stack.GetMethod();

            return SetMCA(method, null);
        }
        catch {
            // 这里吃掉异常
            return 0;
        }
    }

    /// <summary>
    /// 填充 Module,Controller,Action
    /// </summary>
    /// <param name="actionMethod"></param>
    /// <param name="controllerType"></param>
    internal int SetMCA(MethodBase actionMethod, Type controllerType = null)
    {
        if( this.Controller != null )  // 避免多次调用
            return -1;

        if( actionMethod == null )
            return -2;

        if( controllerType == null )
            controllerType = actionMethod.DeclaringType;

        this.Module = controllerType.Namespace;
        this.Controller = controllerType.Name;
        this.Action = actionMethod.Name;
        return 1;
    }


    /// <summary>
    /// 检查并截断一些较长的文本字段
    /// </summary>
    public void TruncateTextFields()
    {
        if( this.Url != null && this.Url.Length > 1024 )
            this.Url = this.Url.Substring(0, 1024);

        if( this.HttpRef != null && this.HttpRef.Length > 1024 )
            this.HttpRef = this.HttpRef.Substring(0, 1024);

        if( this.Request != null && this.Request.Length > LoggingLimit.HttpBodyMaxLen )
            this.Request = this.Request.SubstringN(LoggingLimit.HttpBodyMaxLen);

        if( this.Response != null && this.Response.Length > LoggingLimit.HttpBodyMaxLen )
            this.Response = this.Response.SubstringN(LoggingLimit.HttpBodyMaxLen);

        if( this.CtxData != null && this.CtxData.Length > LoggingLimit.OprLog.TextnMaxLen )
            this.CtxData = this.CtxData.SubstringN(LoggingLimit.OprLog.TextnMaxLen);

        if( this.Addition != null && this.Addition.Length > LoggingLimit.OprLog.TextnMaxLen )
            this.Addition = this.Addition.SubstringN(LoggingLimit.OprLog.TextnMaxLen);



        if( this.Text1 != null && this.Text1.Length > LoggingLimit.OprLog.TextnMaxLen )
            this.Text1 = this.Text1.SubstringN(LoggingLimit.OprLog.TextnMaxLen);

        if( this.Text2 != null && this.Text2.Length > LoggingLimit.OprLog.TextnMaxLen )
            this.Text2 = this.Text2.SubstringN(LoggingLimit.OprLog.TextnMaxLen);

        if( this.Text3 != null && this.Text3.Length > LoggingLimit.OprLog.TextnMaxLen )
            this.Text3 = this.Text3.SubstringN(LoggingLimit.OprLog.TextnMaxLen);

        if( this.Text4 != null && this.Text4.Length > LoggingLimit.OprLog.TextnMaxLen )
            this.Text4 = this.Text4.SubstringN(LoggingLimit.OprLog.TextnMaxLen);

        if( this.Text5 != null && this.Text5.Length > LoggingLimit.OprLog.TextnMaxLen )
            this.Text5 = this.Text5.SubstringN(LoggingLimit.OprLog.TextnMaxLen);


        // 注释下面二段是因为：这样粗鲁的做法可能会破坏“内容格式”，造成后续解析出错
        //int logsLen = LoggingLimit.OprLog.LogsTextMaxLen * LoggingLimit.OprLog.LogsMaxCount;
        //if( this.Logs != null && this.Logs.Length > logsLen )
        //    this.Logs = this.Logs.Substring(0, logsLen);

        //int detailLen = LoggingLimit.OprLog.DetailsMaxLen + LoggingLimit.OprLog.StepDetailMaxLen * 2;
        //if( this.OprDetails != null && this.OprDetails.Length > detailLen )
        //    this.OprDetails = this.OprDetails.Substring(0, detailLen);
    }

    private void CompressOprDetails()
    {
        if( this.OprDetails.IsNullOrEmpty() )
            return;

        // 此字段已做过压缩，表示对于当前日志对象，已执行过BeforeWrite
        if( this.OprDetails.StartsWith0("[StepId]") == false )
            return;


        // .NET48的标准库中没有支持 Brotli 算法，
        // ClownFish又不想为了日志而引入第三方的包，所以对于 netfx 项目，就使用 Gzip 压缩
        // 目前发现一个规律，Gzip 压缩的结果，总是以 H4sIAAAAAAA 开头，Venus将使用这个特殊标记来识别是使用的哪种压缩算法，
        // 当然了，最稳妥的做法是增加一个前缀，例如：gzip:xxxxxxxx or br:xxxxxxxxxxx，但是这种方式比较浪费性能和内存，
        // 考虑到日志基本上是在微服务项目中使用，所以没有必要浪费这些性能，因此这里采用简化的做法！

#if NETCOREAPP
        this.OprDetails = BrotliHelper.Compress(this.OprDetails);
#else
        this.OprDetails = GzipHelper.Compress(this.OprDetails);
#endif
    }

    /// <summary>
    /// 根据异常对象构造一错误日志
    /// </summary>
    /// <param name="ex">Exception实例（必选）</param>
    /// <param name="httpContext">HttpContext实例（可选）</param>
    /// <returns></returns>
    public static OprLog CreateErrLog(Exception ex, NHttpContext httpContext = null)
    {
        if( ex == null )
            throw new ArgumentNullException(nameof(ex));

        OprLog log = new OprLog();
        log.SetBaseInfo();

        log.OprKind = OprKinds.Error;
        log.OprName = "NULL";

        if( httpContext != null ) {
            log.SetHttpRequest(httpContext);
            log.Request = httpContext.Request.ToLoggingText().SubstringN(LoggingLimit.HttpBodyMaxLen);
        }
        else {
            log.Url = "error://" + ex.GetType().FullName;
        }

        log.SetException(ex);

        return log;
    }


    /// <summary>
    /// 从当前OprLog对象生成一个InvokeLog对象
    /// </summary>
    /// <returns></returns>
    public InvokeLog ToInvokeLog()
    {
        InvokeLog log = new InvokeLog();
        log.AppName = this.AppName;
        log.ProcessId = this.OprId;
        log.IsLongTask = this.IsLongTask;
        log.StartTime = this.StartTime;
        log.ExecuteTime = TimeSpan.FromMilliseconds(this.Duration);
        log.IsSlow = this.IsSlow;
        log.HasError = this.HasError;
        log.Status = this.Status;
        log.ActionType = GetActionType(this.OprKind);

        if( EnvUtils.IsProdEnv == false )
            log.Title = this.OprName;

        return log;
    }


    internal static int GetActionType(string oprKind)
    {
        if( oprKind == OprKinds.HttpIn || oprKind == OprKinds.HttpProxy )
            return 100;

        if( oprKind == OprKinds.Msg )
            return 200;

        if( oprKind == OprKinds.BTask )
            return 300;


        return 400;
    }

}
