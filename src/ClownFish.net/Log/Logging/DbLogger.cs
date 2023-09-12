namespace ClownFish.Log.Logging;

/// <summary>
/// DbLogger
/// </summary>
public static class DbLogger
{
    /// <summary>
    /// 
    /// </summary>
    public static void Init()
    {
        DbContextEvent.OnConnectionOpened += ConnectionOpened;
        DbContextEvent.OnAfterExecute += CommandAfterExecute;
        DbContextEvent.OnCommited += OnCommited;

#if NET6_0_OR_GREATER
        DbContextEvent.OnAfterExecuteBatch += BatchAfterExecute;
#endif
    }


    private static void ConnectionOpened(object sender, OpenConnEventArgs e)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;


        StepItem step = StepItem.CreateNew(e.StartTime);
        step.StepKind = StepKinds.SQLConn;
        step.StepName = e.IsAsync ? "OpenConnectionAsync" : "OpenConnection";
        step.IsAsync = e.IsAsync ? 1 : 0;
        step.SetException(e.Exception);

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append("ConnectionString: ").Append(e.DbContext.Connection.ConnectionString);

            string database = e.DbContext.GetChangeDatabase();
            if( database.HasValue() ) {
                sb.AppendLineRN().Append("Database: ").Append(database);
            }
            step.Detail = sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }


        step.End(e.EndTime);
        scope.AddStep(step);
    }

    private static void OnCommited(object sender, CommitTransEventArgs e)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        StepItem step = StepItem.CreateNew(e.StartTime);
        step.StepKind = StepKinds.SQLTrans;
        step.StepName = "SQL_Commit";
        step.SetException(e.Exception);
        step.Detail = "Database: " + e.DbContext.Connection.Database;

        step.End(e.EndTime);

        scope.AddStep(step);
    }

    private static void CommandAfterExecute(object sender, ExecuteCommandEventArgs e)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        StepItem step = StepItem.CreateNew(e.StartTime);
        step.StepKind = StepKinds.SQLExec;

        IsolationLevel? level = e.DbCommand.Transaction?.IsolationLevel;
        step.StepName = e.OperationName + GetTransFlag(level);
        step.IsAsync = e.IsAsync ? 1 : 0;
        step.SetException(e.Exception);

        step.Cmdx = e.DbCommand;
        step.IsolationLevel = level;

        step.End(e.EndTime);

        scope.AddStep(step);
    }

    internal static string GetTransFlag(IsolationLevel? level)
    {
        if( level.HasValue ) {
            if( level == IsolationLevel.ReadUncommitted )
                return "_NOLOCK";
            else
                return "_TRANS";
        }

        return "";
    }


#if NET6_0_OR_GREATER
    private static void BatchAfterExecute(object sender, ExecuteBatchEventArgs e)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        StepItem step = StepItem.CreateNew(e.StartTime);
        step.StepKind = StepKinds.SQLBatch;
        step.StepName = e.OperationName;
        step.IsAsync = e.IsAsync ? 1 : 0;
        step.SetException(e.Exception);

        step.Cmdx = e.Batch;

        step.End(e.EndTime);

        scope.AddStep(step);
    }
#endif

}
