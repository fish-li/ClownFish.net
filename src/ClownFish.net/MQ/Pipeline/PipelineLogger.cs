namespace ClownFish.MQ.Pipeline;
#if NETCOREAPP

internal static class PipelineLogger
{
    public static void CreateOprLogScope<T>(this PipelineContext<T> context) where T: class
    {
        if( context.Handler.EnableLog == false )
            return;

        OprLogScope scope = OprLogScope.Start(context);
        context.SetOprLogScope(scope);

        OprLog log = scope.OprLog;
        log.RootId = context.ProcessId;
        log.OprKind = OprKinds.Msg;
        log.InSize = context.Request.BodyLen;

        Type handlerType = context.Handler.GetType();
        log.Module = handlerType.Namespace;
        log.Controller = handlerType.Name;
        log.Action = "HandleMessage";
        log.OprName = handlerType.Name + "/Execute";
        log.Url = context.GetTitle();

        log.RetryCount = context.RetryN;
    }

    public static void SaveLog<T>(this PipelineContext<T> context) where T : class
    {
        if( context.Handler.EnableLog == false )
            return;


        OprLogScope scope = context.OprLogScope;
        if( scope.IsNull == false ) {
            scope.SetException(context.LastException);
            try {
                scope.SaveOprLog(context);
            }
            catch( Exception ex ) {
                Console2.Error("消息管道写OprLog失败！", ex);
            }
        }
    }


}
#endif
