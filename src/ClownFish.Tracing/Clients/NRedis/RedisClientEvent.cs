using Castle.DynamicProxy;

namespace ClownFish.Tracing;

public static class RedisClientEvent
{
    //private static readonly DiagnosticListener s_diagnosticSource = new DiagnosticListener("ClownFish.RedisClientEvent");

    /// <summary>
    /// 每当执行一次Redis调用后触发的事件
    /// </summary>
    public static event EventHandler<RedisClientEventArgs> OnExecuteFinished;



    internal static void AfterExecute(IInvocation invocation, DateTime start, Exception ex = null)
    {
        RedisClientEventArgs e = null;

        EventHandler<RedisClientEventArgs> handler = OnExecuteFinished;
        if( handler != null ) {
            if( e == null )
                e = CreateRedisClientEventArgs(invocation, start, ex);
            handler(invocation.InvocationTarget, e);
        }


        //if( s_diagnosticSource.IsEnabled() ) {
        //    if( e == null )
        //        e = CreateRedisClientEventArgs(invocation, start, ex);
        //    s_diagnosticSource.Write("OnExecuteFinished", e);
        //}

    }


    private static RedisClientEventArgs CreateRedisClientEventArgs(IInvocation invocation, DateTime start, Exception ex)
    {
        return new RedisClientEventArgs {
            Method = invocation.Method,
            Arguments = invocation.Arguments,
            StartTime = start,
            EndTime = DateTime.Now,
            Exception = ex
        };
    }
}
