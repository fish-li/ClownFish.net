namespace ClownFish.Tasks;
#if NET6_0_OR_GREATER
/// <summary>
/// BackgroundTask相关的日志工具类
/// </summary>
internal static class BackgroundTaskLogger
{
    /// <summary>
    /// 开启一个日志监控范围
    /// </summary>
    /// <returns></returns>
    public static void CreateOprLogScope(this BgTaskExecuteContext context)
    {
        if( context == null )
            throw new ArgumentNullException(nameof(context));

        if( context.Executor.EnableLog == false )
            return;

        OprLogScope scope = OprLogScope.Start(context);
        context.SetOprLogScope(scope);

        OprLog log = scope.OprLog;
        log.RootId = context.ProcessId;
        log.OprKind = OprKinds.BTask;
 
        Type executorType = context.Executor.GetType();
        log.Module = executorType.Namespace;
        log.Controller = executorType.Name;
        log.Action = "ExecuteTask";
        log.OprName = executorType.Name + "/Execute";
        log.Url = context.GetTitle();
    }

    /// <summary>
    /// 写入日志(InvokeLog+OprLog)，如果过程中出现异常会直接吃掉。
    /// </summary>
    /// <param name="context"></param>
    public static void SaveLog(this BgTaskExecuteContext context)
    {
        if( context.Executor.EnableLog == false )
            return;


        OprLogScope scope = context.OprLogScope;
        if( scope.IsNull == false ) {
            scope.SetException(context.LastException);
            try {
                scope.SaveOprLog(context);
            }
            catch( Exception ex ) {
                Console2.Error("BackgroundTask写OprLog失败", ex);
            }
        }
    }


}
#endif
