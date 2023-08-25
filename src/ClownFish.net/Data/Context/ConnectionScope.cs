namespace ClownFish.Data;

/// <summary>
/// 表示连接与事务的作用域。
/// 注意：不要在异步环境下使用这个类型。
/// </summary>
public sealed class ConnectionScope : IDisposable
{

    private static readonly AsyncLocal<ConnectionScope> s_localScope = new AsyncLocal<ConnectionScope>();

    private static ConnectionScope Current {
        get => s_localScope.Value;
        set => s_localScope.Value = value;
    }


    private bool _isNew;

    /// <summary>
    /// 当前using范围块之前的ConnectionScope实例
    /// </summary>
    private readonly ConnectionScope _lastInstance;

    /// <summary>
    /// DbContext实例引用
    /// </summary>
    public DbContext Context { get; private set; }





    #region 构造函数
    private ConnectionScope(DbContext context, bool isNew = true)
    {
        if( context == null )
            throw new ArgumentNullException("context");

        this._isNew = isNew;
        this.Context = context;

        if( isNew ) {
            // 保存前一个实例引用（有可能是 null）
            _lastInstance = Current;

            // 将当前实例设置为【当前】实例
            Current = this;
        }
    }


    /// <summary>
    /// 根据指定的数据库连接字符串创建ConnectionScope实例
    /// </summary>
    /// <param name="connectionName">如果连接配置名为Null，则使用每个有效的连接配置。</param>
    /// <returns></returns>
    public static ConnectionScope Create(string connectionName = null)
    {
        DbContext context = DbContext.Create(connectionName);
        return new ConnectionScope(context);
    }

    /// <summary>
    /// 根据指定的数据库连接字符串和数据库类型创建ConnectionScope实例
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static ConnectionScope Create(string connectionString, string providerName)
    {
        DbContext context = DbContext.Create(connectionString, providerName);
        return new ConnectionScope(context);
    }

    /// <summary>
    /// 尝试从当前上下文中获取已存在的ConnectionScope，
    /// 如果没有已存在的实例，会抛出异常。
    /// </summary>
    /// <returns></returns>
    public static ConnectionScope Get()
    {
        ConnectionScope current = Current;

        if( current == null )
            throw new InvalidProgramException("当前代码执行环境中没有创建ConnectionScope的实例");

        return new ConnectionScope(current.Context, false);
    }


    #endregion




    /// <summary>
    /// 实现IDisposable接口
    /// </summary>
    public void Dispose()
    {
        if( _isNew ) {

            // 恢复之前的【当前】实例
            Current = _lastInstance;

            if( this.Context != null ) {
                this.Context.Dispose();
                this.Context = null;
            }

            _isNew = false;
        }
    }


    #region DbContext 封装



    internal static DbContext GetCurrentDbConext()
    {
        ConnectionScope current = Current;

        if( current == null )
            throw new InvalidProgramException("当前代码执行环境中没有创建ConnectionScope的实例");

        return current.Context;
    }

    /// <summary>
    /// 开启数据库事务
    /// </summary>
    public void BeginTransaction()
    {
        Context.BeginTransaction();
    }


    /// <summary>
    /// 开启数据库事务
    /// </summary>
    /// <param name="isolationLevel"></param>
    public void BeginTransaction(IsolationLevel isolationLevel)
    {
        Context.BeginTransaction(isolationLevel);
    }



    /// <summary>
    /// 提交数据库事务
    /// </summary>
    public void Commit()
    {
        Context.Commit();
    }


    #endregion

}
