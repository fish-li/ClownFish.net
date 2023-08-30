#if NETCOREAPP

namespace ClownFish.Log.Logging;

internal static class EFLogger
{
    internal static void Init()
    {
        DiagnosticListener.AllListeners.Subscribe(new EFEventSubscriber());
    }
}


internal class EFEventSubscriber : IObserver<DiagnosticListener>
{
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(DiagnosticListener listener)
    {
        switch( listener.Name ) {
            case "Microsoft.EntityFrameworkCore":
                listener.Subscribe(new EFEventObserver());
                break;
        }
    }
}

internal class EFEventObserver : IObserver<KeyValuePair<string, object>>
{
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(KeyValuePair<string, object> kvp)
    {
        if( kvp.Key == "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpened") {
            //var eventData = (ConnectionEndEventData)kvp.Value;
            ConnectionOpened(kvp.Value, false);
            return;
        }

        if( kvp.Key == "Microsoft.EntityFrameworkCore.Database.Connection.ConnectionError" ) {
            //var eventData = (ConnectionErrorEventData)kvp.Value;
            ConnectionOpened(kvp.Value, true);
            return;
        }

        if( kvp.Key == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted" ) {
            //var eventData = (CommandExecutedEventData)kvp.Value;
            AfterExecute(kvp.Value, false);
            return;
        }

        if( kvp.Key == "Microsoft.EntityFrameworkCore.Database.Command.CommandError" ) {
            //var eventData = (CommandErrorEventData)kvp.Value;
            AfterExecute(kvp.Value, true);
            return;
        }

        if( kvp.Key == "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionCommitted" ) {
            //var eventData = (TransactionEndEventData)kvp.Value;
            OnCommit(kvp.Value, false);
            return;
        }

        if( kvp.Key == "Microsoft.EntityFrameworkCore.Database.Transaction.TransactionError" ) {
            //var eventData = (TransactionErrorEventData)kvp.Value;
            OnCommit(kvp.Value, true);
            return;
        }
    }

    //private void ConnectionOpened(ConnectionEndEventData eventData, Exception ex)
    private void ConnectionOpened(object eventData, bool hasError)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        DateTime startTime = eventData.Get<DateTimeOffset>("StartTime").DateTime.ToLocalTime();
        StepItem step = StepItem.CreateNew(startTime);
        step.StepKind = StepKinds.SQLConn;
        step.StepName = "EF_OpenConnection";
        step.IsAsync = eventData.Get<bool>("IsAsync") ? 1 : 0;

        if( hasError ) {
            step.SetException(eventData.Get<Exception>("Exception"));
        }

        DbConnection connection = eventData.Get<DbConnection>("Connection");
        step.Detail = "ConnectionString: " + ConnectionStringUtils.HidePwd(connection.ConnectionString);

        step.End(startTime.Add(eventData.Get<TimeSpan>("Duration")));
        scope.AddStep(step);
    }

    //private void AfterExecute(CommandEndEventData eventData, Exception ex)
    private void AfterExecute(object eventData, bool hasError)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        DateTime startTime = eventData.Get<DateTimeOffset>("StartTime").DateTime.ToLocalTime();
        StepItem step = StepItem.CreateNew(startTime);
        step.StepKind = StepKinds.SQLExec;

        // 基本上所有的CURD 都是采用 ExecuteReader 的方式实现的，所以这种情况下分析SQL语句
        DbCommand command = eventData.Get<DbCommand>("Command");
        step.StepName = GetStepName(command);

        step.IsAsync = eventData.Get<bool>("IsAsync") ? 1 : 0;

        if( hasError ) {
            step.SetException(eventData.Get<Exception>("Exception"));
        }

        // 在请求结束时就读不到内容了，只能先把内容读出来~~~
        step.IsolationLevel = command.Transaction?.IsolationLevel;
        step.Cmdx = command.ToLoggingText();        

        step.End(startTime.Add(eventData.Get<TimeSpan>("Duration")));

        scope.AddStep(step);
    }

    private static readonly Regex s_regex1 = new Regex(@"^set\s+\w+\s+[^;]+;\s*(?<name>\w+)\s+", RegexOptions.IgnoreCase| RegexOptions.Compiled);
    private static readonly Regex s_regex2 = new Regex(@"^\s*(?<name>\w+)\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private string GetStepName(DbCommand command)
    {
        string sql = command.CommandText;
        string transFlag = DbLogger.GetTransFlag(command.Transaction?.IsolationLevel);

        if( sql.StartsWithIgnoreCase("SET ") ) {
            // EF 在生成 INSERT, UPDATE, DELETE 时，可能会添加设置，所以为了能知道到底执行的是什么操作，就要去掉这个设置
            // 可能会出现的内容   "SET NOCOUNT ON;"  or   "SET AUTOCOMMIT = 1;"
            Match m = s_regex1.Match(sql);
            return GetStepName(m, transFlag);
        }
        else {
            Match m = s_regex2.Match(sql);
            return GetStepName(m, transFlag);
        }
    }

    private string GetStepName(Match m, string transFlag)
    {
        if( m.Success ) {
            string name = m.Groups["name"].Value.ToUpper();
            return name switch {
                "SELECT" => "EF_SQL_SELECT" + transFlag,
                "INSERT" => "EF_SQL_INSERT" + transFlag,
                "UPDATE" => "EF_SQL_UPDATE" + transFlag,
                "DELETE" => "EF_SQL_DELETE" + transFlag,
                _ => "EF_SQL_EXECUTE" + transFlag
            };
        }
        return "EF_SQL_EXECUTE" + transFlag;
    }

    //private void OnCommit(TransactionEndEventData eventData, Exception ex)
    private void OnCommit(object eventData, bool hasError)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        DateTime startTime = eventData.Get<DateTimeOffset>("StartTime").DateTime.ToLocalTime();
        StepItem step = StepItem.CreateNew(startTime);
        step.StepKind = StepKinds.SQLTrans;
        step.StepName = "EF_SQL_Commit";

        if( hasError ) {
            step.SetException(eventData.Get<Exception>("Exception"));
        }

        DbTransaction trans = eventData.Get<DbTransaction>("Transaction");
        string database = trans?.Connection?.Database;
        if( database.HasValue() ) {
            step.Detail = "Database: " + database;
        }

        step.End(startTime.Add(eventData.Get<TimeSpan>("Duration")));

        scope.AddStep(step);
    }
}
#endif
