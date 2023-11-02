namespace ClownFish.WebClient;

/// <summary>
/// 定义HttpClient的扩展方法的工具类
/// </summary>
public static class HttpOptionExtensions
{
    /// <summary>
    /// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求，不读取HTTP响应
    /// </summary>
    /// <param name="option">HttpOption的实例，用于描述请求参数</param>
    /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
    /// <exception cref="RemoteWebException"></exception>
    public static void Send(this HttpOption option, Retry retry = null)
    {
        GetResult<ClownFish.Base.Void>(option, retry);
    }

    /// <summary>
    /// 根据指定的HttpOption参数，用【异步】方式发起一次HTTP请求，不读取HTTP响应
    /// </summary>
    /// <param name="option">HttpOption的实例，用于描述请求参数</param>
    /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
    /// <exception cref="RemoteWebException"></exception>
    public async static Task SendAsync(this HttpOption option, Retry retry = null)
    {
        await GetResultAsync<ClownFish.Base.Void>(option, retry);
    }




    /// <summary>
    /// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求
    /// </summary>
    /// <param name="option">HttpOption的实例，用于描述请求参数</param>
    /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
    /// <returns>以string方式返回服务端的响应内容</returns>
    /// <exception cref="RemoteWebException"></exception>
    public static string GetResult(this HttpOption option, Retry retry = null)
    {
        return GetResult<string>(option, retry);
    }

    /// <summary>
    /// 根据指定的HttpOption参数，用【异步】方式发起一次HTTP请求
    /// </summary>
    /// <param name="option">HttpOption的实例，用于描述请求参数</param>
    /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
    /// <returns>以string方式返回服务端的响应内容</returns>
    /// <exception cref="RemoteWebException"></exception>
    public async static Task<string> GetResultAsync(this HttpOption option, Retry retry = null)
    {
        return await GetResultAsync<string>(option, retry);
    }


#if NETFRAMEWORK
    private static ClownFish.WebClient.V1.HttpClient CreateClient(HttpOption option)
    {
        return new ClownFish.WebClient.V1.HttpClient(option);
    }
#else
    private static ClownFish.WebClient.V2.HttpClient2 CreateClient(HttpOption option)
    {
        return new ClownFish.WebClient.V2.HttpClient2(option);
    }
#endif

    private static T Send0<T>(HttpOption option)
    {
        // 用于单元测试场景，从“模拟结果”中直接返回
        object mockResult = HttpClientMockResults.GetMockResult(option.Id);
        if( mockResult != null )
            return (T)mockResult;


        var client = CreateClient(option);
        return client.Send<T>();
    }

    /// <summary>
    /// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求
    /// </summary>
    /// <typeparam name="T">返回值的类型参数，如果不需要结果请指定 ClownFish.Base.Void</typeparam>
    /// <param name="option">HttpOption的实例，用于描述请求参数</param>
    /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
    /// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
    /// <exception cref="RemoteWebException"></exception>
    public static T GetResult<T>(this HttpOption option, Retry retry = null)
    {
        if( option == null )
            throw new ArgumentNullException(nameof(option));

        if( option.Finished )
            throw new InvalidOperationException("ClownFish/HttpOption实例不允许重用！");

        
        ClownFishCounters.Concurrents.HttpCallCount.Increment();
        try {
            if( retry == null ) {
                return Send0<T>(option);
            }
            else {
                return retry.Run(() => {
                    return Send0<T>(option);
                });
            }
        }
        finally {
            option.Finished = true;
            ClownFishCounters.Concurrents.HttpCallCount.Decrement();
        }
    }


    private static Task<T> SendAsync0<T>(HttpOption option)
    {
        // 用于单元测试场景，从“模拟结果”中直接返回
        object mockResult = HttpClientMockResults.GetMockResult(option.Id);
        if( mockResult != null )
            return Task.FromResult((T)mockResult);


        var client = CreateClient(option);
        client.IsAsync = true;
        return client.SendAsync<T>();
    }

    /// <summary>
    /// 根据指定的HttpOption参数，用【异步】方式发起一次HTTP请求
    /// </summary>
    /// <typeparam name="T">返回值的类型参数，如果不需要结果请指定 ClownFish.Base.Void</typeparam>
    /// <param name="option">HttpOption的实例，用于描述请求参数</param>
    /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
    /// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
    /// <exception cref="RemoteWebException"></exception>
    public async static Task<T> GetResultAsync<T>(this HttpOption option, Retry retry = null)
    {
        if( option == null )
            throw new ArgumentNullException(nameof(option));

        if( option.Finished )
            throw new InvalidOperationException("ClownFish/HttpOption实例不允许重用！");


        ClownFishCounters.Concurrents.HttpCallCount.Increment();
        try {
            if( retry == null ) {
                return await SendAsync0<T>(option);
            }
            else {
                return await retry.RunAsync(async () => {
                    return await SendAsync0<T>(option);
                });
            }
        }
        finally {
            option.Finished = true;
            ClownFishCounters.Concurrents.HttpCallCount.Decrement();
        }
    }





}
