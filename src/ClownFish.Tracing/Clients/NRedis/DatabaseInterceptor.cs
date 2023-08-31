using Castle.DynamicProxy;

namespace ClownFish.Tracing;
internal class DatabaseInterceptor : IAsyncInterceptor // IInterceptor
{
    /// <summary>
    /// 实现 IInterceptor 接口
    /// </summary>
    /// <param name="invocation"></param>
    public void Intercept(IInvocation invocation)
    {
        DateTime start = DateTime.Now;

        try {
            invocation.Proceed();

            RedisClientEvent.AfterExecute(invocation, start);
        }
        catch(Exception ex ) {
            RedisClientEvent.AfterExecute(invocation, start, ex);
            throw;
        }
    }




    /// <summary>
    /// 同步调用场景，实现IAsyncInterceptor接口
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptSynchronous(IInvocation invocation)
    {
        Intercept(invocation);
    }


    /// <summary>
    /// 【异步/无结果】的调用场景，实现IAsyncInterceptor接口
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptAsynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
    }

    private async Task InternalInterceptAsynchronous(IInvocation invocation)
    {
        DateTime start = DateTime.Now;

        try {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;

            RedisClientEvent.AfterExecute(invocation, start);
        }
        catch(Exception ex ) {
            RedisClientEvent.AfterExecute(invocation, start, ex);
            throw;
        }
    }



    /// <summary>
    /// 【异步/有返回值】的调用场景，实现IAsyncInterceptor接口
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="invocation"></param>
    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
    }

    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
    {
        DateTime start = DateTime.Now;

        try {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            TResult result = await task;

            RedisClientEvent.AfterExecute(invocation, start);
            return result;
        }
        catch(Exception ex ) {
            RedisClientEvent.AfterExecute(invocation, start, ex);
            throw;
        }
    }


}
