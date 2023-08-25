namespace ClownFish.Data;

/// <summary>
/// 
/// </summary>
public class BaseEventArgs : EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public DbContext DbContext { get; internal set; }

    /// <summary>
    /// 
    /// </summary>
    public Exception Exception { get; internal set; }


    /// <summary>
    /// StartTime
    /// </summary>
    public DateTime StartTime { get; internal set; }

    /// <summary>
    /// EndTime
    /// </summary>
    public DateTime EndTime { get; internal set; }
}



/// <summary>
/// 打开数据库连接对应的事件参数
/// </summary>
public sealed class OpenConnEventArgs : BaseEventArgs
{
    /// <summary>
    /// 当前打开的数据库连接
    /// </summary>
    public DbConnection Connection { get; internal set; }
    /// <summary>
    /// 连接是否以异步方式打开
    /// </summary>
    public bool IsAsync { get; internal set; }
}



/// <summary>
/// 提交事务对应的事件参数
/// </summary>
public sealed class CommitTransEventArgs : BaseEventArgs
{
}

/// <summary>
/// 执行SQL命令对应的事件参数
/// </summary>
public sealed class ExecuteCommandEventArgs : BaseEventArgs
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
    /// 当前正在执行的数据库命令（CPQuery/XmlCommand/StoreProcedure）
    /// </summary>
    public BaseCommand Command { get; internal set; }

    /// <summary>
    /// 当前正在执行的数据库命令（DbCommand实例）
    /// </summary>
    public DbCommand DbCommand => this.Command.Command;
    /// <summary>
    /// 命令是否以异步方式执行
    /// </summary>
    public bool IsAsync { get; internal set; }
}



