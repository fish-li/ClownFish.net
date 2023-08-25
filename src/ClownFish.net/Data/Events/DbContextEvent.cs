namespace ClownFish.Data;

/// <summary>
/// 与数据库访问相关的事件通知类
/// </summary>
public static partial class DbContextEvent
{
#if NETCOREAPP
		private static readonly DiagnosticListener s_diagnosticSource = new DiagnosticListener("ClownFish.DALEvent");
#endif

    /// <summary>
    /// 连接打开事件
    /// </summary>
    public static event EventHandler<OpenConnEventArgs> OnConnectionOpened;

    /// <summary>
    /// 命令执行之前事件
    /// </summary>
    public static event EventHandler<ExecuteCommandEventArgs> OnBeforeExecute;

    /// <summary>
    /// 命令执行之后事件
    /// </summary>
    public static event EventHandler<ExecuteCommandEventArgs> OnAfterExecute;

    /// <summary>
    /// 提交事务事件
    /// </summary>
    public static event EventHandler<CommitTransEventArgs> OnCommited;


    internal static void ConnectionOpened(DbContext dbContext, DateTime startTime, bool isAsync, Exception ex)
    {
        OpenConnEventArgs e = null;

        EventHandler<OpenConnEventArgs> handler = OnConnectionOpened;
        if( handler != null ) {
            if( e == null )
                e = CreateOpenConnEventArgs(dbContext, startTime, isAsync, ex);
            handler(null, e);
        }

#if NETCOREAPP
			if( s_diagnosticSource.IsEnabled() ) {
				if( e == null )
					e = CreateOpenConnEventArgs(dbContext, startTime, isAsync, ex);
				s_diagnosticSource.Write("ConnectionOpened", e);
        }
#endif
    }
    private static OpenConnEventArgs CreateOpenConnEventArgs(DbContext dbContext, DateTime startTime, bool isAsync, Exception ex)
    {
        OpenConnEventArgs e = new OpenConnEventArgs();
        e.DbContext = dbContext;
        e.Connection = dbContext.Connection;
        e.IsAsync = isAsync;

        e.StartTime = startTime;
        e.EndTime = DateTime.Now;
        e.Exception = ex;
        return e;
    }



    internal static void BeforeExecute(BaseCommand command, string operationId, string operationName, bool async)
    {
        ExecuteCommandEventArgs e = null;

        EventHandler<ExecuteCommandEventArgs> handler = OnBeforeExecute;
        if( handler != null ) {
            if( e == null )
                e = CreateBeforeExecuteCommandEventArgs(command, operationId, operationName, async);
            handler(null, e);
        }

//#if NETCOREAPP
//        if( s_diagnosticSource.IsEnabled() ) {
//            if( e == null )
//                e = CreateBeforeExecuteCommandEventArgs(command, operationId, operationName, async);
//            s_diagnosticSource.Write("BeforeExecute", e);
//        }
//#endif
    }

    private static ExecuteCommandEventArgs CreateBeforeExecuteCommandEventArgs(BaseCommand command, string operationId, string operationName, bool async)
    {
        ExecuteCommandEventArgs e = new ExecuteCommandEventArgs();
        e.DbContext = command.Context;
        e.OperationId = operationId;
        e.OperationName = operationName;
        e.Command = command;
        e.IsAsync = async;
        e.StartTime = DateTime.Now;
        return e;
    }




    internal static void AfterExecute(BaseCommand command, string operationId, string operationName, DateTime startTime, bool isAsync, Exception ex)
    {
        ExecuteCommandEventArgs e = null;

        EventHandler<ExecuteCommandEventArgs> handler = OnAfterExecute;
        if( handler != null ) {
            if( e == null )
                e = CreateAfterExecuteCommandEventArgs(command, operationId, operationName, startTime, isAsync, ex);
            handler(null, e);
        }

#if NETCOREAPP
			if( s_diagnosticSource.IsEnabled() ) {
				if( e == null )
					e = CreateAfterExecuteCommandEventArgs(command, operationId, operationName, startTime, isAsync, ex);
				s_diagnosticSource.Write("AfterExecute", e);
			}
#endif
    }

    private static ExecuteCommandEventArgs CreateAfterExecuteCommandEventArgs(BaseCommand command, string operationId, string operationName, DateTime startTime, bool isAsync, Exception ex)
    {
        ExecuteCommandEventArgs e = new ExecuteCommandEventArgs();
        e.DbContext = command.Context;
        e.OperationId = operationId;
        e.OperationName = operationName;
        e.Command = command;
        e.IsAsync = isAsync;

        e.StartTime = startTime;
        e.EndTime = DateTime.Now;
        e.Exception = ex;

        return e;
    }




    internal static void Commit(DbContext dbContext, DateTime startTime, Exception ex)
    {
        CommitTransEventArgs e = null;

        EventHandler<CommitTransEventArgs> handler = OnCommited;
        if( handler != null ) {
            if( e == null )
                e = CreateCommitTransEventArgs(dbContext, startTime, ex);
            handler(null, e);
        }

#if NETCOREAPP
			if( s_diagnosticSource.IsEnabled() ) {
				if( e == null )
					e = CreateCommitTransEventArgs(dbContext, startTime, ex);
				s_diagnosticSource.Write("OnCommit", e);
			}
#endif
    }

    private static CommitTransEventArgs CreateCommitTransEventArgs(DbContext dbContext, DateTime startTime, Exception ex)
    {
        CommitTransEventArgs e = new CommitTransEventArgs();
        e.DbContext = dbContext;

        e.StartTime = startTime;
        e.EndTime = DateTime.Now;
        e.Exception = ex;

        return e;
    }
}




