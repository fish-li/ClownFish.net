namespace ClownFish.Tasks;
#if NETCOREAPP
/// <summary>
/// BackgroundTask的运行状态
/// </summary>
public sealed class BgTaskStatus
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; }

    /// <summary>
    /// 任务类别。 0：同步，1：异步
    /// </summary>
    public int Kind { get; set; }

    /// <summary>
    /// 运行状态。 -1：未开始, 0: 等待中，1：运行中，2：已停止
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 最近运行状态。 -1：未开始, 0：成功， 1：运行中，2：失败
    /// </summary>
    public int LastStatus { get; set; }

    /// <summary>
    /// 获取后台任务的状态显示文本
    /// </summary>
    /// <returns></returns>
    public string GetStatusText()
    {
        return this.Status switch {
            -1 => "未开始",
            0 => "等待中",
            1 => "运行中",
            2 => "已停止",
            _ => "未知"
        };
    }


    /// <summary>
    /// 获取后台任务的最后一次执行状态
    /// </summary>
    /// <returns></returns>
    public string GetLastStatusText()
    {
        return this.LastStatus switch {
            -1 => "未开始",
            0 => "成功",
            1 => "运行中",
            2 => "失败",
            _ => "未知"
        };
    }


    /// <summary>
    /// 是否已启用日志
    /// </summary>
    public bool EnableLog { get; set; }

    /// <summary>
    /// 触发器
    /// </summary>
    public int SleepSeconds { get; set; }

    /// <summary>
    /// 触发器
    /// </summary>
    public string CronValue { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    public long ExecuteCount { get; set; }

    /// <summary>
    /// 失败次数
    /// </summary>
    public long ErrorCount { get; set; }

    /// <summary>
    /// 最近启动时间
    /// </summary>
    public long LastRunTime { get; set; }        

    /// <summary>
    /// 下次启动时间
    /// </summary>
    public long NextRunTime { get; set; }
}
#endif
