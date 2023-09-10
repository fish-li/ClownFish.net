namespace ClownFish.MQ.Pipeline;
#if NET6_0_OR_GREATER
internal static class PipelineUtils
{
    internal static void EnsureIsRootCode()
    {
        if( OprLogScope.Get().IsNull == false )
            throw new InvalidOperationException("消息订阅只能在程序初始化时启用！");
    }

    internal static void SafeCall<T>(Action<PipelineContext<T>> action, PipelineContext<T> context) where T: class
    {
        try {
            action(context);
        }
        catch( Exception ex ) {
            Console2.Error(ex);
        }
    }


    internal static async Task SafeCallAsync<T>(Func<PipelineContext<T>, Task> action, PipelineContext<T> context) where T : class
    {
        try {
            await action(context);
        }
        catch( Exception ex ) {
            Console2.Error(ex);
        }
    }



    internal static bool ExceptionIsNeedRetry(Exception lastException)
    {
        if( lastException == null )
            return false;


        // 这些异常不需要【重试】，因为重试的结果不会有变化。
        if( lastException is System.ComponentModel.DataAnnotations.ValidationException
            || lastException is ValidationException2
            || lastException is ArgumentException
            || lastException is InvalidOperationException
            || lastException is InvalidCodeException
            || lastException is MessageException
            || lastException is TaskCanceledException
            || lastException is DuplicateInsertException
            || lastException is ConfigurationErrorsException
            || lastException is NotImplementedException ) {

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
