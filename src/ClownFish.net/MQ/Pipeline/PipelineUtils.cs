namespace ClownFish.MQ.Pipeline;
#if NETCOREAPP

/// <summary>
/// 消息管道工具类
/// </summary>
public static class PipelineUtils
{
    /// <summary>
    /// 确认当前代码是一个顶层的代码主体
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static void EnsureIsRootCode()
    {
        if( OprLogScope.Get().IsNull == false )
            throw new InvalidOperationException("消息订阅只能在程序初始化时启用！");
    }

    /// <summary>
    /// 安全的执行某个委托方法，如果出现异常会自动处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <param name="context"></param>
    public static void SafeCall<T>(Action<PipelineContext<T>> action, PipelineContext<T> context) where T: class
    {
        try {
            action(context);
        }
        catch( Exception ex ) {
            Console2.Error(ex);
        }
    }


    /// <summary>
    /// 安全的执行某个委托方法，如果出现异常会自动处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static async Task SafeCallAsync<T>(Func<PipelineContext<T>, Task> action, PipelineContext<T> context) where T : class
    {
        try {
            await action(context);
        }
        catch( Exception ex ) {
            Console2.Error(ex);
        }
    }


    /// <summary>
    /// 判断某个异常是否需要重试
    /// </summary>
    /// <param name="lastException"></param>
    /// <returns></returns>
    public static bool ExceptionIsNeedRetry(Exception lastException)
    {
        if( lastException == null )
            return false;


        // 这些异常不需要【重试】，因为重试的结果不会有变化。
        if( lastException is System.ComponentModel.DataAnnotations.ValidationException
            || lastException is ValidationException2
            || lastException is ClientDataException
            || lastException is ArgumentException
            || lastException is InvalidOperationException
            || lastException is InvalidDataException
            || lastException is InvalidCodeException
            || lastException is MessageException
            || lastException is TaskCanceledException
            || lastException is DuplicateInsertException
            || lastException is ForbiddenException
            || lastException is DatabaseNotFoundException
            || lastException is TenantNotFoundException
            || lastException is ConfigurationErrorsException
            || lastException is NotImplementedException
            || lastException is NotSupportedException ) {

            return false;
        }


        if( lastException is RemoteWebException wex ) {
            if( StatusCodeUtils.IsClientError(wex.StatusCode) ) {

                // 例如：http400这类异常，重试的结果不会有变化，所以不需要重试
                return false;
            }
        }


        if( lastException.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ) {

            // 如果是调用超时产生的异常，也不再重试，因为这类重试太占用时间，容易引起队列阻塞
            return false;
        }

        return true;
    }
}
#endif
