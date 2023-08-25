#if NET6_0_OR_GREATER

namespace ClownFish.Data;

/// <summary>
/// 执行SQL命令对应的事件参数
/// </summary>
public sealed class ExecuteBatchEventArgs : BaseEventArgs
{
    /// <summary>
    /// 操作ID
    /// </summary>
    public string OperationId { get; internal set; }

    /// <summary>
    /// 操作名称，例如：ExecuteNonQuery
    /// </summary>
    public string OperationName { get; internal set; }

    /// <summary>
    /// 当前正在执行的批命令（DbBatch实例）
    /// </summary>
    public DbBatch Batch { get; internal set; }

    /// <summary>
    /// 命令是否以异步方式执行
    /// </summary>
    public bool IsAsync { get; internal set; }
}



public static partial class DbContextEvent
{
    ///// <summary>
    ///// 批命令执行之前事件
    ///// </summary>
    //public static event EventHandler<ExecuteBatchEventArgs> OnBeforeExecuteBatch;

    /// <summary>
    /// 批命令执行之后事件
    /// </summary>
    public static event EventHandler<ExecuteBatchEventArgs> OnAfterExecuteBatch;



    //        internal static void BeforeExecuteBatch(DbContext dbContext, DbBatch batch, string operationId, string operationName, bool isAsync)
    //        {
    //            ExecuteBatchEventArgs e = null;

    //            EventHandler<ExecuteBatchEventArgs> handler = OnBeforeExecuteBatch;
    //            if( handler != null ) {
    //                if( e == null )
    //                    e = CreateBeforeExecuteBatchEventArgs(dbContext, batch, operationId, operationName, isAsync);
    //                handler(null, e);
    //            }

    //#if NETCOREAPP
    //            if( s_diagnosticSource.IsEnabled() ) {
    //                if( e == null )
    //                    e = CreateBeforeExecuteBatchEventArgs(dbContext, batch, operationId, operationName, isAsync);
    //                s_diagnosticSource.Write("BeforeExecuteBatch", e);
    //            }
    //#endif
    //        }


    //        private static ExecuteBatchEventArgs CreateBeforeExecuteBatchEventArgs(DbContext dbContext, DbBatch batch, string operationId, string operationName, bool isAsync)
    //        {
    //            ExecuteBatchEventArgs e = new ExecuteBatchEventArgs();
    //            e.DbContext = dbContext;
    //            e.OperationId = operationId;
    //            e.OperationName = operationName;
    //            e.Batch = batch;
    //            e.IsAsync = isAsync;
    //            e.StartTime = DateTime.Now;
    //            return e;
    //        }



    internal static void AfterExecuteBatch(DbContext dbContext, DbBatch batch, string operationId, string operationName, DateTime startTime, bool isAsync, Exception ex)
    {
        ExecuteBatchEventArgs e = null;

        EventHandler<ExecuteBatchEventArgs> handler = OnAfterExecuteBatch;
        if( handler != null ) {
            if( e == null )
                e = CreateAfterExecuteBatchEventArgs(dbContext, batch, operationId, operationName, startTime, isAsync, ex);
            handler(null, e);
        }

#if NET6_0_OR_GREATER
        if( s_diagnosticSource.IsEnabled() ) {
            if( e == null )
                e = CreateAfterExecuteBatchEventArgs(dbContext, batch, operationId, operationName, startTime, isAsync, ex);
            s_diagnosticSource.Write("AfterExecuteBatch", e);
        }
#endif
    }

    private static ExecuteBatchEventArgs CreateAfterExecuteBatchEventArgs(DbContext dbContext, DbBatch batch, string operationId, string operationName, DateTime startTime, bool isAsync, Exception ex)
    {
        ExecuteBatchEventArgs e = new ExecuteBatchEventArgs();
        e.DbContext = dbContext;
        e.OperationId = operationId;
        e.OperationName = operationName;
        e.Batch = batch;
        e.IsAsync = isAsync;

        e.StartTime = startTime;
        e.EndTime = DateTime.Now;
        e.Exception = ex;

        return e;
    }

}


#endif