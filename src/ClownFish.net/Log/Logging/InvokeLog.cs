namespace ClownFish.Log.Logging;

/// <summary>
/// 一条简单的执行统计信息（根据OprLog的数据创建）
/// </summary>
[Serializable]
[BatchWritable(BatchSize = 30)]
public sealed class InvokeLog : IMsgObject
{
    /// <summary>
    /// 当前应用程序的名称
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 操作ID，唯一不重复。对于有重试类操作，每次操作或者重试都生成一个ID
    /// </summary>
    public string ProcessId { get; set; }

    /// <summary>
    /// 操作类别。
    /// 100: HTTP请求, 200: 消息处理，300: 后台任务，400: 其它
    /// </summary>
    public int ActionType { get; set; }

    /// <summary>
    /// 代码执行主体(含框架部分)的开始执行时间
    /// </summary>
    public DateTime StartTime { get; set; }   

    /// <summary>
    /// 请求的执行时间
    /// </summary>
    public TimeSpan ExecuteTime { get; set; }

    /// <summary>
    /// 当前操作是否已达到性能阀值
    /// </summary>
    public int IsSlow { get; set; }

    /// <summary>
    /// 标记当前操作是不是一个【长任务】，
    /// 长任务不统计SUM服务的【总执行时间-SumTime】，因此也不计入服务的【平均响应时间-AvgTime】，对于每次请求仍然会计算【执行时间-ExecuteTime】
    /// </summary>
    public int IsLongTask { get; set; }

    /// <summary>
    /// 服务端响应状态码
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 操作的简要描述，例如可以是URL
    /// </summary>
    public string Title { get; set; }


    /// <summary>
    /// 指示当前操作是否出现异常
    /// </summary>
    public int HasError { get; set; }


    string IMsgObject.GetId() => this.ProcessId;
    DateTime IMsgObject.GetTime() => this.StartTime;

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.Title;
    }


}
