namespace ClownFish.Log.Logging;

/// <summary>
/// 表示一个操作步骤
/// </summary>
public sealed class StepItem
{
    /// <summary>
    /// 步骤ID
    /// </summary>
    public string StepId { get; set; }
    /// <summary>
    /// 步骤类别，可使用StepKinds已预定义的名称。
    /// </summary>
    public string StepKind { get; set; }

    /// <summary>
    /// 步骤名称
    /// </summary>
    public string StepName { get; set; }


    /// <summary>
    /// 步骤的开始执行时间
    /// </summary>
    public DateTime StartTime { get; set; }


    /// <summary>
    /// 步骤的结束执行时间。
    /// 说明：这个属性一般情况下可以不指定，除非Duration的值不方便计算，例如在PHP中就不方便处理。
    /// </summary>
    public DateTime EndTime { get; set; }

    

    /// <summary>
    /// 步骤的执行时间，单位：毫秒
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 步骤的执行状态。 200成功，500失败
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 指示当前步骤是否出现异常，属于冗余字段，方便查询。
    /// </summary>
    public int HasError { get; set; }

    /// <summary>
    /// 是否为异步调用
    /// </summary>
    public int IsAsync { get; set; }


    /// <summary>
    /// 异常类型
    /// </summary>
    public string ExType { get; set; }

    /// <summary>
    /// 异常消息
    /// </summary>
    public string ExMessage { get; set; }

    /// <summary>
    /// 异常实例
    /// </summary>
    internal Exception ExceptionObject { get; set; }

    /// <summary>
    /// 用于描述当前步骤或者补充信息
    /// </summary>
    public string Detail { get; set; }


    /// <summary>
    /// 一个执行步骤，例如：SqlCommand, HttpOption 之类的对象。
    /// 它将在整个操作结束时被序列化到 Text 属性上。
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    public object Cmdx { get; set; }

    /// <summary>
    /// SQL事务隔离级别
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }


    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.StepName;
    }


    /// <summary>
    /// 开始一个子任务日志块
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="stepId"></param>
    /// <returns></returns>
    public static StepItem CreateNew(DateTime? startTime = null, string stepId = null)
    {
        StepItem item = new StepItem();
        item.StepId = stepId ?? LogIdMaker.GetNewId();
        item.StartTime = startTime.HasValue ? startTime.Value : DateTime.Now;
        item.Status = 200;
        return item;
    }


    /// <summary>
    /// 结束一个子任务日志块
    /// </summary>
    /// <param name="endTime"></param>
    public void End(DateTime? endTime = null)
    {
        DateTime end2 = endTime.HasValue ? endTime.Value : DateTime.Now;
        TimeSpan time = end2  - this.StartTime;
        this.Duration = (long)time.TotalMilliseconds;
    }

    /// <summary>
    /// 设置异常信息
    /// </summary>
    /// <param name="ex"></param>
    public void SetException(Exception ex)
    {
        if( ex != null ) {
            this.Status = ex.GetErrorCode();
            this.HasError = StatusCodeUtils.IsServerError(this.Status) ? 1 : 0;
            this.ExType = ex.GetType().FullName;
            this.ExMessage = ex.Message;
            this.ExceptionObject = ex;
        }
    }

    internal string GetLogText1()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.AppendLineRN($"[StepId]: {this.StepId}")
              .AppendLineRN($"[StepKind]: {this.StepKind}")
              .AppendLineRN($"[StepName]: {this.StepName}")
              .AppendLineRN($"[HasError]: {this.HasError}")
              .AppendLineRN($"[IsAsync]: {this.IsAsync}")
              .AppendLineRN($"[Status]: {this.Status}")
              .AppendLineRN($"[StartTime]: {this.StartTime.ToTime27String()}")
              .AppendLineRN($"[ExecuteTime]: {TimeSpan.FromMilliseconds(this.Duration)}");

            if( this.ExType.IsNullOrEmpty() == false ) {
                sb.AppendLineRN($"[ExceptionType]: {this.ExType}")
                  .AppendLineRN($"[ExceptionMessage]: {this.ExMessage}");
            }

            if( this.IsolationLevel.HasValue )
                sb.AppendLineRN($"[Transaction]: {this.IsolationLevel.Value}");

            sb.AppendLineRN(TextUtils.StepDetailSeparatedLine3);

            if( this.Detail.IsNullOrEmpty() == false ) {
                sb.AppendLineRN(this.Detail.SubstringN(LoggingLimit.OprLog.StepDetailMaxLen));
            }
            else  {
                sb.AppendLineRN(this.GetCmdxText().SubstringN(LoggingLimit.OprLog.StepDetailMaxLen));
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetCmdxText()
    {
        return this.Cmdx?.GetLogText() ?? string.Empty;
    }

    internal string GetLogText2()   // 单元测试专用
    {
        if( this.Detail.IsNullOrEmpty() )
            this.Detail = this.GetCmdxText();

        return this.ToJson(JsonStyle.Indented);
    }
        
}
