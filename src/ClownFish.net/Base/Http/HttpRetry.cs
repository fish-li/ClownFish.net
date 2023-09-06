using System.Net.Sockets;

namespace ClownFish.Base;

/// <summary>
/// 创建发出HTTP请求时的重试策略工具类
/// </summary>
public static class HttpRetry
{
    /// <summary>
    /// 获取用于发送HTTP请求的重试策略。
    /// 重试条件：当发生 “网络不通” 或者 “HTTP 502,503” 时。
    /// </summary>
    /// <param name="count">重试次数，默认值：7</param>
    /// <param name="milliseconds">重试的间隔毫秒数，默认值：1000</param>
    /// <returns></returns>
    public static Retry Create(int count = 0, int milliseconds = 0)
    {
        if( count <= 0 )
            count = Retry.Default.Count;

        if( milliseconds <= 0 )
            milliseconds = Retry.Default.WaitMillisecond;

        return Retry.Create(count, milliseconds)
                    .Filter<RemoteWebException>(NeedRetry);
    }


    /// <summary>
    /// 判断异常是否需要重试
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    internal static bool NeedRetry(RemoteWebException ex)
    {
        int status = ex.StatusCode;
        if( status == 500 || status == 502 || status == 503 || status == 504 )
            return true;

        Exception baseException = ex.GetBaseException();

        if( baseException is SocketException || baseException is IOException )
            return true;

#if NETCOREAPP
        if( baseException is System.Net.Http.HttpRequestException )
            return true;
#endif

        return false;
    }

}
