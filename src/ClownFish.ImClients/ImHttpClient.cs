namespace ClownFish.ImClients;

/// <summary>
/// ImHttpClient
/// </summary>
public static class ImHttpClient
{
    private static readonly bool s_debug = LocalSettings.GetInt("ClownFish_ImHttpClient_Debug_Enabled", 0) == 1;

    /// <summary>
    /// 执行HTTP调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="httpOption"></param>
    /// <returns></returns>
    /// <exception cref="ShitResultException"></exception>
    public static T ExecRPC<T>(this HttpOption httpOption) where T : IShitResult
    {
        HttpResult<string> httpResult = null;
        try {
            httpResult = httpOption.GetResult<HttpResult<string>>(ImDefault.CreateRetry());
        }
        catch( RemoteWebException ex ) {
            if( s_debug ) {
                Console2.ShowHTTP(httpOption, ex.Result, false);
            }
            throw new ShitResultException(ex.Message, httpOption, httpResult, null);
        }

        T result = default;

        if( typeof(T) == typeof(ImShitResult) ) {
            result = (T)(object)new ImShitResult(httpOption, httpResult);
        }
        else {
            result = httpResult.Result.FromJson<T>();
        }

        bool success = (result.ErrCode == 0);

        if( s_debug ) {
            Console2.ShowHTTP(httpOption, httpResult, success);
        }

        if( success == false )
            throw new ShitResultException(result.ErrMsg, httpOption, httpResult, result);

        return result;
    }


    /// <summary>
    /// 执行HTTP调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="httpOption"></param>
    /// <returns></returns>
    /// <exception cref="ShitResultException"></exception>
    public static async Task<T> ExecRPCAsync<T>(this HttpOption httpOption) where T : IShitResult
    {
        HttpResult<string> httpResult = null;
        try {
            httpResult = await httpOption.GetResultAsync<HttpResult<string>>(ImDefault.CreateRetry());
        }
        catch( RemoteWebException ex ) {
            if( s_debug ) {
                Console2.ShowHTTP(httpOption, ex.Result, false);
            }
            throw new ShitResultException(ex.Message, httpOption, httpResult, null);
        }

        T result = default;

        if( typeof(T) == typeof(ImShitResult) ) {
            result = (T)(object)new ImShitResult(httpOption, httpResult);
        }
        else {
            result = httpResult.Result.FromJson<T>();
        }

        bool success = (result.ErrCode == 0);

        if( s_debug ) {
            Console2.ShowHTTP(httpOption, httpResult, success);
        }

        if( success == false )
            throw new ShitResultException(result.ErrMsg, httpOption, httpResult, result);

        return result;
    }

          


}
