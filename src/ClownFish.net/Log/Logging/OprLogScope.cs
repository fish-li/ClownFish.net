namespace ClownFish.Log.Logging;

/// <summary>
/// Operating Logging scope
/// </summary>
public sealed class OprLogScope : IDisposable
{
    private static readonly AsyncLocal<OprLogScope> s_local = new AsyncLocal<OprLogScope>();

    private readonly object _lockObject = new object();

    private Exception _exObject;

    private bool _isEnd = false;

    private List<StepItem> _steps;

    internal List<StepItem> GetStepItems() => _steps;

    private List<NameTime> _logs;

    internal List<NameTime> GetLogs() => _logs;

    /// <summary>
    /// 日志对象引用
    /// </summary>
    public OprLog OprLog { get; private set; }


    /// <summary>
    /// 指示当前实例是否为“空实例”
    /// </summary>
    public bool IsNull { get; private set; }


    /// <summary>
    /// 一个不与任何线程关联的 “空对象”。
    /// 空对象不会做日志持久化。
    /// </summary>
    public static readonly OprLogScope NullObject = new OprLogScope {
        _isEnd = true,
        IsNull = true,
        OprLog = new OprLog()
    };


    private OprLogScope()
    {
        // 这个类型必须配合 AsyncLocal 一起使用，因为对于性能监控这类操作，不可能指望用传递 OprLog 的方式来实现，
        // 所以这个类不允许在外部实例化，只允许用 Start 方法来创建。
    }

    /// <summary>
    /// 开启监控
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    internal static OprLogScope Start(BasePipelineContext context = null)
    {
        if( s_local.Value != null )
            throw new InvalidOperationException("OprLogScope不允许嵌套使用！");  // 只允许在【顶层】使用

        var scope = new OprLogScope();
        scope.OprLog = OprLog.CreateNew(context);

        s_local.Value = scope;
        return scope;
    }


    /// <summary>
    /// 获取当前线程关联的ActionLogScope实例
    /// </summary>
    /// <returns></returns>
    public static OprLogScope Get()
    {
        OprLogScope scope = s_local.Value;

        // HttpPipelineContext实例可能【逃逸】了，原因是 new Thread/Task.Run 之类的写法导致
        // 所以这里还要检查它的有效性
        if( scope != null && scope._isEnd ) {
            s_local.Value = null;

            return NullObject;
        }

        // 说明：返回 NullObject 可以避免 NullReferenceException 的可能性，代码写起来也更容易。
        return scope ?? NullObject;
    }


    /// <summary>
    /// 释放与当前线程相关的引用
    /// </summary>
    private void Release()
    {
        s_local.Value = null;
        _isEnd = true;
    }

    void IDisposable.Dispose()
    {
        this.Release();
    }


    /// <summary>
    /// 记录请求过程中的一些耗时步骤，例如：SQL，HTTP外发调用等等。
    /// </summary>
    /// <param name="step"></param>
    public int AddStep(StepItem step)
    {
        if( step == null )
            return -1;

        if( _isEnd )  // 防止OprLogScope实例逃逸（被其它线程捕获）
            return -2;

        lock( _lockObject ) {
            if( _steps == null )
                _steps = new List<StepItem>(32);

            if( _steps.Count < LoggingLimit.OprLog.StepsMaxCount ) {
                _steps.Add(step);
                return 1;
            }
            else {
                return 2;
            }
        }
    }


    /// <summary>
    /// 记录请求过程中的一些耗时步骤
    /// </summary>
    /// <param name="start">开始时间</param>    
    /// <param name="name">操作名称</param>
    /// <param name="detail">细节描述</param>
    /// <param name="end">结束时间</param>
    /// <param name="ex">异常对象</param>
    /// <returns></returns>
    public int AddStep(DateTime start, string name, string detail = null, DateTime? end = null, Exception ex = null)
    {
        if( name.IsNullOrEmpty() )
            return 0;

        if( _isEnd )  // 防止OprLogScope实例逃逸（被其它线程捕获）
            return -2;

        StepItem step = StepItem.CreateNew(start);
        step.StepKind = "ext";
        step.StepName = name;
        step.Detail = detail.IfEmpty("NULL");
        step.SetException(ex);
        step.End(end);

        return this.AddStep(step);
    }

    /// <summary>
    /// 在OprLog.Logs中添加一条时间消息
    /// </summary>
    /// <param name="message"></param>
    public int Log(string message)
    {
        if( message.IsNullOrEmpty() )
            return 0;

        if( _isEnd )  // 防止OprLogScope实例逃逸（被其它线程捕获）
            return -2;

        lock( _lockObject ) {
            if( _logs == null )
                _logs = new List<NameTime>(32);

            if( _logs.Count < LoggingLimit.OprLog.LogsMaxCount ) {
                _logs.Add(new NameTime(message.SubstringN(LoggingLimit.OprLog.LogsTextMaxLen), DateTime.Now));
                return 1;
            }
            else {
                return 2;
            }
        }
    }

    /// <summary>
    /// 填充异常信息
    /// </summary>
    /// <param name="ex">异常对象</param>
    public int SetException(Exception ex)
    {
        if( ex == null )
            return -1;

        if( _isEnd )
            return -2;

        _exObject = ex;

        return this.OprLog.SetException(ex);
    }


    /// <summary>
    /// 结束监控，填充一些数据成员，并记录日志
    /// </summary>
    /// <param name="context"></param>
    internal int SaveOprLog(BasePipelineContext context)
    {
        if( context == null )
            throw new ArgumentNullException(nameof(context));

        if( _isEnd )
            return 0;

        this.EndSet0(context);

        // 持久化操作日志
        LogHelper.Write(this.OprLog);

        if( LoggingOptions.InvokeLogEnable ) {
            InvokeLog log2 = this.OprLog.ToInvokeLog();
            LogHelper.Write(log2);
        }

        return 1;
    }


    /// <summary>
    /// 结束监控，填充一些数据成员
    /// </summary>
    internal void EndSet0(BasePipelineContext context)
    {
        this.Release();

        this.OprLog.IsLongTask = context.IsLongTask ? 1 : 0;
        this.OprLog.CalcTime(context.PerformanceThresholdMs, context.EndTime);

        // 如果执行时间超过性能阀值
        if( this.OprLog.IsSlow == 1 ) {

            // 填充执行过程中的所有步骤描述
            this.OprLog.StepCount = _steps == null ? 0 : _steps.Count;
            this.OprLog.OprDetails = GetOprDetails();
        }


        // 如果过程中出现异常，或者执行时间超过性能阀值
        // 填充当前操作的详细描述，例如：httpRequest
        if( _exObject != null || this.OprLog.IsSlow == 1 || LoggingOptions.Http.MustLogRequest ) {
            try {
                this.OprLog.Request = context.GetRequest()?.GetLogText();
            }
            catch(Exception ex) {
                this.OprLog.Request = "### 不能读取 Request，原因：" + ex.ToString();
                // 忽略读取错误
            }
        }

        this.OprLog.Logs = GetLogsText();

        // 检查并截断一些较长的文本字段
        this.OprLog.TruncateTextnField();
    }


    internal string GetOprDetails()
    {
        if( _steps == null || _steps.Count == 0 )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( var x in _steps ) {

                if( sb.Length > 0 ) {
                    sb.AppendLineRN().AppendLineRN(TextUtils.StepSeparatedLine);  // 2个步骤之间的分隔行
                }

                // 如果内容过大，就立即停止拼接
                if( sb.Length > LoggingLimit.OprLog.DetailsMaxLen ) {
                    StepItem step = CreateCutoffStep(x);
                    string text = step.GetLogText1();
                    sb.AppendLineRN(text);

                    // 跳出循环，结束拼接
                    break;
                }
                else {
                    string text = x.GetLogText1();
                    sb.AppendLineRN(text);
                }
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    private string GetLogsText()
    {
        if( _logs == null ) {
            return null;
        }
        else {
            StringBuilder sb = StringBuilderPool.Get();
            try {
                foreach( var x in _logs )
                    sb.AppendLineRN(x.Time.ToTime23String() + ": " + x.Name);

                return sb.ToString();
            }
            finally {
                StringBuilderPool.Return(sb);
            }
        }
    }


    private StepItem CreateCutoffStep(StepItem x)
    {
        StepItem step = StepItem.CreateNew(x.StartTime);
        step.StepKind = "ext";
        step.StepName = "已截断";
        step.Detail = "执行步骤过多，剩余部分已丢弃！";
        step.End(x.StartTime);

        return step;
    }



    internal void CheckError500()
    {
        OprLog log = this.OprLog;

        if( log.Status == 500 && log.HasError == 0 ) {
            Exception ex = this.GetStepItems()?.FirstOrDefault(x => x.ExceptionObject != null)?.ExceptionObject;
            if( ex != null ) {
                log.SetException(ex);

                if( log.Status != 500 )   // 避免 ex.GetErrorCode() ！= 500，给人造成困扰
                    log.Status = 500;
            }
            else {
                log.HasError = 1;   // 强制修改
            }
        }
    }
}
