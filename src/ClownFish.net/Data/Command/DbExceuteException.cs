namespace ClownFish.Data;

// CA2237	将 [Serializable] 添加到 'DbExceuteException'，原因是此类型实现了 ISerializable。
// 由于DbExceuteException包含了DbCommand，而DbCommand没有标记为可序列化，因此只能禁止CA2237的检查。

/// <summary>
/// 表示在数据访问执行过程中发生的异常。
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237")]
public sealed class DbExceuteException : Exception
{
    /// <summary>
    /// SQL执行时关联的命令对象。
    /// </summary>
    public DbCommand Command { get; private set; }
    // 这个对象不可序列化


    /// <summary>
    /// 当前连接的连接字符串
    /// </summary>
    public string ConnectionString { get; private set; }


    /// <summary>
    /// 初始化 <see cref="DbExceuteException"/>对象。
    /// </summary>
    /// <param name="innerException">当前异常对象。</param>
    /// <param name="command"><see cref="DbCommand"/>的实例。</param>
    public DbExceuteException(Exception innerException, DbCommand command)
    : base(innerException.Message, innerException)
    {
        //if( innerException == null )
        //    throw new ArgumentNullException("innerException");

        if( command == null )
            throw new ArgumentNullException("command");

        Command = command;

        if( command.Connection != null )
            this.ConnectionString = command.Connection.ConnectionString;
    }

}
