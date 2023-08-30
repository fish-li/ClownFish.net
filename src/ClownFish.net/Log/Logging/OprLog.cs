namespace ClownFish.Log.Logging;

/// <summary>
/// 表示一条操作日志
/// </summary>
[Serializable]
public partial class OprLog : IMsgObject
{
    /// <summary>
    /// 操作ID，唯一不重复。对于有重试类操作，每次操作或者重试都生成一个ID
    /// </summary>
    public string OprId { get; set; }

    /// <summary>
    /// 操作类别，可使用OprKinds已预定义的名称。
    /// </summary>
    public string OprKind { get; set; }

    /// <summary>
    /// 操作名称，例如：ExecuteNonQuery  or  StringSet
    /// </summary>
    public string OprName { get; set; }


    #region 链路字段

    /// <summary>
    /// parent OprId
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// 日志的顶层ID，3大执行主体（HTTP请求/消息处理/后台任务）在开始执行时会创建一个顶层ID
    /// </summary>
    public string RootId { get; set; }

    #endregion

    #region 基本状态

    /// <summary>
    /// 标记当前操作是不是一个【长任务】，
    /// 长任务不统计SUM服务的【总执行时间-SumTime】，因此也不计入服务的【平均响应时间-AvgTime】，对于每次请求仍然会计算【执行时间-ExecuteTime】
    /// </summary>
    public int IsLongTask { get; set; }

    /// <summary>
    /// 开始执行时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 操作的执行状态。 
    /// 对于HTTP请求，就用响应的状态码来填充，非WEB类的操作，200成功，500失败
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 本次操作的持续执行时间，单位：毫秒
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 指示当前操作是否超出性能阀值。
    /// </summary>
    public int IsSlow { get; set; }


    /// <summary>
    /// 对于HTTP请求来说，就是 Request.ContentLength
    /// 对于消息处理来说，就是消息byte[]长度
    /// </summary>
    public long InSize { get; set; }

    /// <summary>
    /// 对于HTTP请求来说，就是 Response.ContentLength
    /// 其它场景不赋值。
    /// </summary>
    public long OutSize { get; set; }

    /// <summary>
    /// 已重试次数
    /// </summary>
    public int RetryCount { get; set; }


    /// <summary>
    /// OprDetails中StepItem的数量
    /// </summary>
    public int StepCount { get; set; }

    #endregion

    #region 业务相关数据

    /// <summary>
    /// 
    /// </summary>
    public string HttpMethod { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string UserAgent { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string HttpRef { get; set; }


    /// <summary>
    /// 
    /// </summary>
    public string TenantId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string UserCode { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string UserRole { get; set; }



    /// <summary>
    /// 业务ID，例如：工作流的流程ID
    /// </summary>
    public string BizId { get; set; }
    /// <summary>
    /// 业务操作名称，例如：某个工作流的节点名称
    /// </summary>
    public string BizName { get; set; }

        

    /// <summary>
    /// 一级模块名称
    /// </summary>
    public string Module { get; set; }
    /// <summary>
    /// 二级模块名称
    /// </summary>
    public string Controller { get; set; }
    /// <summary>
    /// 三级模块名称
    /// </summary>
    public string Action { get; set; }

    #endregion

    #region 异常相关数据

    /// <summary>
    /// 指示当前操作是否出现异常，属于冗余字段，方便查询。
    /// </summary>
    public int HasError { get; set; }
    /// <summary>
    /// 异常类型
    /// </summary>
    public string ExType { get; set; }
    /// <summary>
    /// 异常消息
    /// </summary>
    public string ExMessage { get; set; }
    /// <summary>
    /// 可理解为 ex.ToString()
    /// </summary>
    public string ExAll { get; set; }

    #endregion

    #region 大文本

    /// <summary>
    /// 在性能日志中记录所有耗时的操作
    /// </summary>
    public string OprDetails { get; set; }


    /// <summary>
    /// 引发当前操作执行的请求数据，【可以为空】
    /// 例如：HttpRequest, Message
    /// </summary>
    public string Request { get; set; }

    /// <summary>
    /// 当前操作的响应数据，【可以为空】
    /// </summary>
    public string Response { get; set; }

    /// <summary>
    /// 上下文相关数据
    /// </summary>
    public string CtxData { get; set; }

    /// <summary>
    /// 附加信息
    /// </summary>
    public string Addition { get; set; }

    /// <summary>
    /// 包含多行的 time:message 格式文本
    /// </summary>
    public string Logs { get; set; }

    #endregion

    #region 扩展字段

    /// <summary>
    /// Text1，预留字段，具体含意由应用程序决定
    /// </summary>
    public string Text1 { get; set; }

    /// <summary>
    /// Text2，预留字段，具体含意由应用程序决定
    /// </summary>
    public string Text2 { get; set; }

    /// <summary>
    /// Text3，预留字段，具体含意由应用程序决定
    /// </summary>
    public string Text3 { get; set; }

    /// <summary>
    /// Text4，预留字段，具体含意由应用程序决定
    /// </summary>
    public string Text4 { get; set; }

    /// <summary>
    /// Text5，预留字段，具体含意由应用程序决定
    /// </summary>
    public string Text5 { get; set; }

    #endregion


    #region 部署环境相关数据

    /// <summary>
    /// 应用程序名称
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 应用程序类别，取 (AppStartup.RuntimeStatus.StartupMode + 10)
    /// </summary>
    public int AppKind { get; set; }

    /// <summary>
    /// 机器名称
    /// </summary>
    public string HostName { get; set; }
    /// <summary>
    /// 环境名称
    /// </summary>
    public string EnvName { get; set; }

    #endregion


    string IMsgObject.GetId() => this.OprId;

    DateTime IMsgObject.GetTime() => this.StartTime;



}
