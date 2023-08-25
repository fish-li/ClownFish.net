namespace ClownFish.Data;

/// <summary>
/// 数据访问的上下文信息
/// </summary>
public sealed class DbContext : IDisposable
{
    #region 数据成员定义


    private bool _useTransaction;
    private IsolationLevel? _isolationLevel;
    private string _changeDatabase;
    private bool _isDisposed = false;
    private bool _isOpened = false;
    private readonly bool _autoCloseConnection = true;

    /// <summary>
    /// 当前实例的连接信息
    /// </summary>
    internal ConnectionInfo ConnectionInfo { get; private set; }

    internal BaseClientProvider ClientProvider { get; private set; }

    /// <summary>
    /// 获取当前实例内部引用的DbProviderFactory
    /// </summary>
    public DbProviderFactory Factory => ClientProvider.ProviderFactory;

    /// <summary>
    /// 获取当前连接对应的数据库类别
    /// </summary>
    public DatabaseType DatabaseType => ClientProvider.DatabaseType;


    private DbConnection _connection;
    /// <summary>
    /// 内部使用的连接对象
    /// </summary>
    public DbConnection Connection {
        get {
            if( _isDisposed )
                throw new InvalidOperationException("数据库连接已释放，不能再执行任何操作！");

            if( _connection == null ) {
                _connection = this.Factory.CreateConnection();
                _connection.ConnectionString = ConnectionInfo.ConnectionString;
            }
            return _connection;
        }
    }

    /// <summary>
    /// 当前连接上的事务对象
    /// </summary>
    public DbTransaction Transaction { get; private set; }


    /// <summary>
    /// 是否在生成SQL时使用【定界符】。
    /// 当我们创建的表名或者字段名与数据库关键词同名时，可启用此参数将“名称”包裹起来。
    /// 默认值：false （减少不必要的拼接提升性能）
    /// </summary>
    public bool EnableDelimiter { get; set; }


    #endregion

    #region 外挂方法的工厂对象引用

    private CPQueryFactory _factoryCPQuery;
    private XmlCommandFactory _factoryXmlCommand;
    private StoreProcedureFactory _factoryStoreProcedure;
    private EntityMethodFactory _entiryMethod;

    /// <summary>
    /// CPQuery工厂实例引用
    /// </summary>
    public CPQueryFactory CPQuery {
        get {
            if( _factoryCPQuery == null )
                _factoryCPQuery = new CPQueryFactory(this);
            return _factoryCPQuery;
        }
    }

    /// <summary>
    /// XmlCommand工厂实例引用
    /// </summary>
    public XmlCommandFactory XmlCommand {
        get {
            if( _factoryXmlCommand == null )
                _factoryXmlCommand = new XmlCommandFactory(this);
            return _factoryXmlCommand;
        }
    }

    /// <summary>
    /// StoreProcedure工厂实例引用
    /// </summary>
    public StoreProcedureFactory StoreProcedure {
        get {
            if( _factoryStoreProcedure == null )
                _factoryStoreProcedure = new StoreProcedureFactory(this);
            return _factoryStoreProcedure;
        }
    }


    /// <summary>
    /// EntityMethodFactory实例引用
    /// </summary>
    public EntityMethodFactory Entity {
        get {
            if( _entiryMethod == null )
                _entiryMethod = new EntityMethodFactory(this);
            return _entiryMethod;
        }
    }


#if NET6_0_OR_GREATER
    private DbBatchFactory _dbBatchFactory;

    /// <summary>
    /// DbBatchFactory实例引用
    /// </summary>
    public DbBatchFactory Batch {
        get {
            if( _dbBatchFactory == null )
                _dbBatchFactory = new DbBatchFactory(this);
            return _dbBatchFactory;
        }
    }
#endif

    #endregion

    #region 构造函数

    internal DbContext(ConnectionInfo connectionInfo, DbConnection connection = null)
    {
        this.ClientProvider = DbClientFactory.GetProvider(connectionInfo.ProviderName);
        this.ConnectionInfo = connectionInfo;

        // connection 采用延迟方式创建，可以尽量减少对底层连接的占用时间

        if( connection != null ) {
            _connection = connection;
            _autoCloseConnection = false;
        }
    }

    /// <summary>
    /// 【不推荐使用】使用默认的连接字符串创建DbContext实例
    /// </summary>
    /// <returns></returns>
    public static DbContext Create()
    {
        ConnectionInfo connectionInfo = ConnectionManager.GetFirstConnection();
        return new DbContext(connectionInfo);
    }

    /// <summary>
    /// 根据指定的数据库连接名称创建DbContext实例，连接名称需要在connectionStrings中配置
    /// </summary>
    /// <param name="connectionName"></param>
    /// <returns></returns>
    public static DbContext Create(string connectionName)
    {
        if( connectionName.IsNullOrEmpty() ) {
            ConnectionInfo connectionInfo1 = ConnectionManager.GetFirstConnection();
            return new DbContext(connectionInfo1);
        }


        ConnectionInfo connectionInfo2 = ConnectionManager.GetConnection(connectionName, false);
        if( connectionInfo2 != null ) {
            return new DbContext(connectionInfo2);
        }


        DbConfig dbConfig = ConnectionManager.GetDbConfig(connectionName, true);
        return new DbContext(new ConnectionInfo(dbConfig));
    }

    /// <summary>
    /// 根据指定的数据库连接字符串和数据库类型创建DbContext实例
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static DbContext Create(string connectionString, string providerName)
    {
        ConnectionInfo connectionInfo = new ConnectionInfo(connectionString, providerName);
        return new DbContext(connectionInfo);
    }

    /// <summary>
    /// 根据连接参数创建DbContext实例
    /// </summary>
    /// <param name="dbConfig"></param>
    /// <returns></returns>
    public static DbContext Create(DbConfig dbConfig)
    {
        if( dbConfig == null)
            throw new ArgumentNullException(nameof(dbConfig));

        return new DbContext(new ConnectionInfo(dbConfig));
    }

    /// <summary>
    /// 根据指定的数据库连接对象和数据库类型创建DbContext实例
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static DbContext Create(DbConnection connection, string providerName)
    {
        if( connection == null )
            throw new ArgumentNullException(nameof(connection));
        if( providerName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(providerName));

        // 这里就不校验 providerName 是否与 connection 匹配了，
        // 例如：如果 connection is MySqlConnection , providerName == "System.Data.SqlClient" 也不管 ！
        // 因为不好做：DbConnection 可能的类型种类太多了，现在 mssql, mysql 都有2套实现，未来还不确定会有不会第3种

        // 注意：这种方式得到的连接字符串有时候是不包含密码部分的，在这里也就无所谓了，反正不用它来打开连接。
        string connectionString = connection.ConnectionString;

        ConnectionInfo connectionInfo = new ConnectionInfo(connectionString, providerName);
        return new DbContext(connectionInfo, connection);
    }


    #endregion


    #region 基础操作


    /// <summary>
    /// 实现IDisposable接口，关闭连接
    /// </summary>
    public void Dispose()
    {
        _isDisposed = true;

        if( _connection != null && _autoCloseConnection ) {
            try {
                _connection.Dispose();
            }
            catch { /*  忽略所有异常   */ }
            finally {
                _connection = null;
            }
        }

        if( _isOpened ) {
            ClownFish.Base.ClownFishCounters.RealtimeStatus.SqlConnCount.Decrement();
            _isOpened = false;
        }
    }

    /// <summary>
    /// 同步方式打开数据库连接
    /// </summary>
    public void OpenConnection()
    {
        if( this.Connection.State == ConnectionState.Open )
            return;

        // 如果连接池的连接不够用了，此时获取连接是个很慢的操作
        DateTime now = DateTime.Now;
        try {
            this.Connection.Open();

            _isOpened = true;
            ClownFish.Base.ClownFishCounters.RealtimeStatus.SqlConnCount.Increment();

            DbContextEvent.ConnectionOpened(this, now, false, null);
        }
        catch( Exception ex ) {
            DbContextEvent.ConnectionOpened(this, now, false, ex);
            throw;
        }

        InitConnection();
    }

    /// <summary>
    /// 异步方式打开数据库连接
    /// </summary>
    /// <returns></returns>
    public async Task OpenConnectionAsync()
    {
        if( this.Connection.State == ConnectionState.Open )
            return;

        DateTime now = DateTime.Now;
        try {
            await this.Connection.OpenAsync();

            _isOpened = true;
            ClownFish.Base.ClownFishCounters.RealtimeStatus.SqlConnCount.Increment();

            DbContextEvent.ConnectionOpened(this, now, true, null);
        }
        catch( Exception ex ) {
            DbContextEvent.ConnectionOpened(this, now, true, ex);
            throw;
        }

        InitConnection();
    }

    /// <summary>
    /// 用于连接创建后的初始化
    /// </summary>
    private void InitConnection()
    {
        if( this._connection == null )      // 确信连接已创建
            throw new InvalidOperationException("数据库连接打开失败！");


        // 当使用 MySqlConnector 时，必须先切换数据库，再打开事务，否则会出现异常
        // The transaction associated with this command is not the connection's active transaction; see https://fl.vu/mysql-trans


        // 切换数据库
        if( string.IsNullOrEmpty(_changeDatabase) == false ) {
            this.ClientProvider.ChangeDatabase(this, _changeDatabase);
            _changeDatabase = null; // 清除变量
        }

        // 开启事务
        if( _useTransaction ) {
            _useTransaction = false;    // 清除变量

            if( _isolationLevel.HasValue )
                this.Transaction = this._connection.BeginTransaction(_isolationLevel.Value);
            else
                this.Transaction = this._connection.BeginTransaction();
        }
    }

    /// <summary>
    /// 为打开的连接更改当前数据库。
    /// </summary>
    /// <param name="databaseName"></param>
    public void ChangeDatabase(string databaseName)
    {
        // 如果连接还没有创建，就用变量来保存要切换的数据库，等待连接创建后再切换
        if( this._connection == null )
            _changeDatabase = databaseName;
        else
            // 连接存在就直接切换
            this.ClientProvider.ChangeDatabase(this, databaseName);
    }

    internal string GetChangeDatabase()
    {
        return _changeDatabase;
    }


    /// <summary>
    /// 开启数据库事务，并指定事务的隔离级别
    /// </summary>
    /// <param name="isolationLevel"></param>
    public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if( this._connection == null ) {
            _useTransaction = true;
            _isolationLevel = isolationLevel;
        }
        else {
            if( this.Transaction != null )
                throw new InvalidOperationException("当前上下文中已存在打开的事务，请不要重复开启事务。");

            this.Transaction = this._connection.BeginTransaction(isolationLevel);
        }
    }



    /// <summary>
    /// 提交数据库事务
    /// </summary>
    public void Commit()
    {
        if( this.Transaction != null ) {

            DateTime now = DateTime.Now;
            try {
                this.Transaction.Commit();
                this.Transaction = null;
                DbContextEvent.Commit(this, now, null);
            }
            catch( Exception ex ) {
                DbContextEvent.Commit(this, now, ex);
                throw;
            }
        }
        else
            throw new InvalidOperationException("没有开启事务不能执行Commit操作。");
    }


    ///// <summary>
    ///// 回滚事务
    ///// </summary>
    //public void Rollback()
    //{
    //	if( this.Transaction != null ) {
    //		this.Transaction.Rollback();
    //		this.Transaction = null;
    //		DbContextEvent.FireOnRollback(this);
    //	}
    //	else
    //		throw new InvalidOperationException("没有开启事务不能执行Rollback操作。");
    //}

    /// <summary>
    /// 创建一个常用的命令参数
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="value"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    internal DbParameter CreateParameter(DbType dbType, object value, int? size = null)
    {
        DbParameter param = this.Factory.CreateParameter();
        param.DbType = dbType;

        if( size.HasValue )
            param.Size = size.Value;

        if( value != null )
            param.Value = value;
        else
            param.Value = DBNull.Value;
        return param;
    }

    /// <summary>
    /// 创建一个用于输出的命令参数
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="size"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    internal DbParameter CreateOutParameter(DbType dbType, object value = null, int? size = null)
    {
        DbParameter param = CreateParameter(dbType, value, size);
        param.Direction = ParameterDirection.Output;
        return param;
    }


    /// <summary>
    /// 创建一个与DbContext有关的CPQuery实例
    /// </summary>
    /// <returns></returns>
    internal CPQuery CreateCPQuery()
    {
        return this.CPQuery.Create(null);
    }


    /// <summary>
    /// 生成一个有序GUID
    /// </summary>
    /// <returns></returns>
    public Guid NewSeqGuid()
    {
        return this.ClientProvider.NewSeqGuid();
    }

    #endregion




}
