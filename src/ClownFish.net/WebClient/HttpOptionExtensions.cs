namespace ClownFish.WebClient;

/// <summary>
/// 定义HttpClient的扩展方法的工具类
/// </summary>
public static class HttpOptionExtensions
{
    // 说明：为什么允许 HttpOption 参数为 NULL ？
    // 按理说，调用当前类型的扩展方法时，HttpOption 参数为 NULL 是可以 throw new ArgumentException(nameof(option));
    // 但是在有些场景下，会设计一些静态方法来构造 HttpOption 对象，然后再调用下面这些扩展方法，形成一个链式调用，
    // 然而，在构造HttpOption对象时，发现条件不满足，希望“退出”整个调用过程。

    // 典型示例代码：TxHttp.Post("url", data).SendAsync();  它的业务意义是：将采集数据data上传到云端。
    // 这里的data是由另一个方法返回的，有可能是 null ，原因可能是，业务条件不满足，或者采集目标不存在，之类的，
    // TxHttp.Post(...) 用来构造 HttpOption 对象，此时它发现 data == null，就希望忽略后面的链式调用。
    // 当然了，你可以说：可以在调用 TxHttp.Post(...) 之前先检查 data 参数，如果为null就不执行所有调用，
    // 确实也可以，但是代码量会增加。
    // 当这些场景比较多的时候，这些“样板”代码会非常多，显得很不美观，也增加了代码复杂度！
    // 对于这些场景来说，采集到数据就上传，没有采集到数据就忽略，可以减少不必要的判断，并简化代码。

    // 如果不使用这种“链式调用”，HttpOption 参数是直接 new 出来的，那么这里允许为NULL，也不受影响，
    // 所以，2025-03-06 决定：下面这些扩展方法允许 HttpOption 参数为 NULL


    /// <summary>
    /// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求，不读取HTTP响应
    /// </summary>
    /// <param name="option">HttpOption的实例，用于描述请求参数</param>
    /// <param name="retry">提供一个Retry实例，用于指示如何执行重试。如果此参数为NULL则不启用重试</param>
    /// <exception cref="RemoteWebException"></exception>
    public static void Send(this HttpOption option, Retry retry = null)
    {
        if( option == null )
            return;        

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
        if( option == null )
            return;

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
        if( option == null )
            return null;

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
        if( option == null )
            return null;

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
            return default(T);

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
            return default(T);

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
