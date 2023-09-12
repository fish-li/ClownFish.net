namespace ClownFish.ImClients;

/// <summary>
/// 一些默认参数
/// </summary>
public static class ImDefault
{
    /// <summary>
    /// 重试次数，默认值：3
    /// </summary>
    public static int Count = 3;

    /// <summary>
    /// 重试的间隔时间，单位：毫秒，默认值：300
    /// </summary>
    public static int WaitMillisecond = 300;


    /// <summary>
    /// CreateRetry
    /// </summary>
    /// <returns></returns>
    public static Retry CreateRetry()
    {
        return HttpRetry.Create(Count, WaitMillisecond);
    }

}
