namespace ClownFish.WebClient;

/// <summary>
/// 为HttpClient设置模拟调用结果
/// </summary>
public static class HttpClientMockResults
{
    private static readonly TSafeDictionary<string, ResultObject> s_dict = new TSafeDictionary<string, ResultObject>(128);

    private class ResultObject
    {
        public bool AutoDelete { get; set; }
        public object Result { get; set; }
    }

    /// <summary>
    /// 为HttpClient设置模拟调用结果
    /// </summary>
    /// <param name="httpOptionId"></param>
    /// <param name="result"></param>
    /// <param name="autoDelete"></param>
    public static void SetMockResult(string httpOptionId, object result, bool autoDelete = true)
    {
        if( httpOptionId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(httpOptionId));

        if( result == null )
            throw new ArgumentNullException(nameof(result));

        s_dict[httpOptionId] = new ResultObject { AutoDelete = autoDelete , Result = result };
    }


    /// <summary>
    /// 清除所有模拟结果
    /// </summary>
    public static void Clear()
    {
        s_dict.Clear();
    }


    internal static object GetMockResult(string httpOptionId)
    {
        if( EnvUtils.IsProdEnv || httpOptionId.IsNullOrEmpty() )
            return null;

        ResultObject resultObject = s_dict.TryGet(httpOptionId);

        if( resultObject == null )
            return null;

        if( resultObject.AutoDelete ) {
            s_dict.TryRemove(httpOptionId, out ResultObject xx);
        }

        return resultObject.Result;
    }
}
