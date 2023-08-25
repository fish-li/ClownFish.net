namespace ClownFish.Data;

/// <summary>
/// 执行数据库操作的基类
/// </summary>
public abstract class BaseCommand
{
    /// <summary>
    /// DbContext实例
    /// </summary>
#pragma warning disable IDE1006 // 命名样式
    protected readonly DbContext _context;

    /// <summary>
    /// DbCommand实例
    /// </summary>
    protected readonly DbCommand _command;
#pragma warning restore IDE1006 // 命名样式


    /// <summary>
    /// 内部DbContext实例引用
    /// </summary>
    public DbContext Context { get { return _context; } }

    /// <summary>
    /// 内部DbCommand实例引用
    /// </summary>
    public virtual DbCommand Command { get { return _command; } }

    ///// <summary>
    ///// 用于SQL语句的参数占位符，命令名称的计算
    ///// </summary>
    //internal ParaNameBuilder ParaNameBuilder { get; private set; }

    /// <summary>
    /// 标记当前对象已经执行了一次操作任务。
    /// 由于性能日志的设计需要，DbCommand 是不允许重用的，所以增加 Finished 来检测是否重用
    /// </summary>
    private bool _finished;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="context"></param>
    protected BaseCommand(DbContext context)
    {
        if( context == null )
            throw new ArgumentNullException(nameof(context));
    
        _context = context;
        _command = context.Connection.CreateCommand();
    }


    #region Execute 方法


    /// <summary>
    /// 开始执行数据库操作前要处理的额外操作
    /// </summary>
    protected virtual void BeforeExecute(DbCommand command)
    {
        // add logging
    }

    private T Execute<T>(string operationName, Func<DbCommand, T> func)
    {
        if( _finished )
            throw new InvalidOperationException("ClownFish/DataCommand实例不允许重用！");

        // 打开数据库连接
        _context.OpenConnection();

        _context.ClientProvider.PrepareCommand(this.Command, _context);

        // 设置命令的连接以及事务对象
        DbCommand command = this.Command;
        command.Connection = _context.Connection;

        if( _context.Transaction != null )
            command.Transaction = _context.Transaction;

        // 触发执行 前 事件
        string operationId = Guid.NewGuid().ToString("N");
        DbContextEvent.BeforeExecute(this, operationId, operationName, false);
        DateTime start = DateTime.Now;

        this.BeforeExecute(command);

        try {
            // 执行数据库操作
            T result = func(command);

            // 触发执行 后 事件
            DbContextEvent.AfterExecute(this, operationId, operationName, start, false, null);

            return result;
        }
        catch( Exception ex ) {
            // 触发 异常 事件
            DbContextEvent.AfterExecute(this, operationId, operationName, start, false, ex);

            // 重新抛出一个特定的异常，方便异常日志中记录command信息。
            throw new DbExceuteException(ex, command);
        }
        finally {
            _finished = true;
        }
    }


    private async Task<T> ExecuteAsync<T>(string operationName, Func<DbCommand, Task<T>> func)
    {
        if( _finished )
            throw new InvalidOperationException("ClownFish/DataCommand实例不允许重用！");

        // 打开数据库连接
        await _context.OpenConnectionAsync();

        _context.ClientProvider.PrepareCommand(this.Command, _context);

        // 设置命令的连接以及事务对象
        DbCommand command = this.Command;
        command.Connection = _context.Connection;

        if( _context.Transaction != null )
            command.Transaction = _context.Transaction;

        // 触发执行 前 事件
        string operationId = Guid.NewGuid().ToString("N");
        DbContextEvent.BeforeExecute(this, operationId, operationName, true);
        DateTime start = DateTime.Now;

        this.BeforeExecute(command);

        try {
            // 执行数据库操作
            T result = await func(command);

            // 触发执行 后 事件
            DbContextEvent.AfterExecute(this, operationId, operationName, start, true, null);

            return result;
        }
        catch( Exception ex ) {
            // 触发 异常 事件
            DbContextEvent.AfterExecute(this, operationId, operationName, start, true, ex);

            // 重新抛出一个特定的异常，方便异常日志中记录command信息。
            throw new DbExceuteException(ex, command);
        }
        finally {
            _finished = true;
        }
    }



    /// <summary>
    /// 执行命令，并返回影响函数
    /// </summary>
    /// <returns>影响行数</returns>
    public int ExecuteNonQuery()
    {
        string operationName = null;

        if( this.Command.CommandText.StartsWithIgnoreCase("INSERT") )
            operationName = "SQL_INSERT";
        else if( this.Command.CommandText.StartsWithIgnoreCase("UPDATE") )
            operationName = "SQL_UPDATE";
        else if( this.Command.CommandText.StartsWithIgnoreCase("DELETE") )
            operationName = "SQL_DELETE";
        else
            operationName = "SQL_EXECUTE";

        return Execute<int>(operationName,
                cmd => cmd.ExecuteNonQuery()
                );
    }


    /// <summary>
    /// 执行命令，并返回影响函数
    /// </summary>
    /// <returns>影响行数</returns>
    public async Task<int> ExecuteNonQueryAsync()
    {
        string operationName = null;

        if( this.Command.CommandText.StartsWithIgnoreCase("INSERT") )
            operationName = "SQL_INSERT_Async";
        else if( this.Command.CommandText.StartsWithIgnoreCase("UPDATE") )
            operationName = "SQL_UPDATE_Async";
        else if( this.Command.CommandText.StartsWithIgnoreCase("DELETE") )
            operationName = "SQL_DELETE_Async";
        else
            operationName = "SQL_EXECUTE_Async";

        return await ExecuteAsync<int>(operationName,
                async cmd => await cmd.ExecuteNonQueryAsync()
                );
    }



    /// <summary>
    /// 执行查询，以DataTable形式返回结果
    /// </summary>
    /// <param name="tableName">DataTable的表名</param>
    /// <returns>查询结构的数据集</returns>
    public DataTable ToDataTable(string tableName = null)
    {
        if( string.IsNullOrEmpty(tableName) )
            tableName = "_tb";

        return Execute<DataTable>(nameof(ToDataTable),
                cmd => {
                    DataTable table = new DataTable(tableName);
                    DbDataAdapter da = _context.Factory.CreateDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(table);

                    return table;
                });
    }


    /// <summary>
    /// 执行查询，以DataTable形式返回结果
    /// </summary>
    /// <param name="tableName">DataTable的表名</param>
    /// <returns>查询结构的数据集</returns>
    public async Task<DataTable> ToDataTableAsync(string tableName = null)
    {
        if( string.IsNullOrEmpty(tableName) )
            tableName = "_tb";

        return await ExecuteAsync<DataTable>(nameof(ToDataTableAsync),
                async cmd => {
                    DataSet ds = new DataSet();
                    ds.EnforceConstraints = false;  // 禁用约束检查

                    DataTable table = new DataTable(tableName);
                    ds.Tables.Add(table);

                    using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {
                        table.Load(reader);
                    }

                    return table;
                });
    }



    /// <summary>
    /// 执行查询，以DataSet形式返回结果
    /// </summary>
    /// <returns>数据集</returns>
    public DataSet ToDataSet()
    {
        return Execute<DataSet>(nameof(ToDataSet),
            cmd => {
                DataSet ds = new DataSet();

                DbDataAdapter adapter = _context.Factory.CreateDataAdapter();
                adapter.SelectCommand = cmd;

                adapter.Fill(ds);
                for( int i = 0; i < ds.Tables.Count; i++ ) {
                    ds.Tables[i].TableName = "_tb" + i.ToString();
                }
                return ds;
            });
    }


    /// <summary>
    /// 执行查询，以DataSet形式返回结果
    /// </summary>
    /// <returns>数据集</returns>
    public async Task<DataSet> ToDataSetAsync()
    {
        return await ExecuteAsync<DataSet>(nameof(ToDataSetAsync),
            async cmd => {
                DataSet ds = new DataSet();
                ds.EnforceConstraints = false;  // 禁用约束检查

                using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {

                    int index = 0;
                    while( true ) {
                        DataTable table = new DataTable("_tb" + (index++).ToString());
                        ds.Tables.Add(table);
                        table.Load(reader);

                        // 上面代码中隐含着一个调用: reader.NextResult()，遗憾的是，它是个同步调用！
                        // 所以，就不需要像下面这样再调用了，否则还会出现异常

                        //if( await reader.NextResultAsync() == false )
                        //	break;

                        if( reader.IsClosed )
                            break;
                    }
                }

                return ds;
            });
    }


    /// <summary>
    /// 执行命令，返回DbDataReader对象实例
    /// </summary>
    /// <returns>DbDataReader实例</returns>
    public DbDataReader ExecuteReader()
    {
        return Execute<DbDataReader>(nameof(ExecuteReader),
            cmd => cmd.ExecuteReader()
            );
    }


    /// <summary>
    /// 执行命令，返回DbDataReader对象实例
    /// </summary>
    /// <returns>DbDataReader实例</returns>
    public async Task<DbDataReader> ExecuteReaderAsync()
    {
        return await ExecuteAsync<DbDataReader>(nameof(ExecuteReaderAsync),
            async cmd => await cmd.ExecuteReaderAsync()
            );
    }


    internal static T ToScalar<T>(object obj)
    {
        if( obj == null || DBNull.Value.Equals(obj) )
            return default(T);

        if( obj is T )
            return (T)obj;

        Type targetType = typeof(T);


        // 有时候获取结果时，虽然字段的数据类型不是 string，但是就是希望以 string 形式返回
        // 例如以下使用场景，
        // sql = "select RowGuid from table1 where aa=2"
        // List<string> list = CPQuery.Create(sql).ToScalarList<string>();

        if( targetType == TypeList._string )
            return (T)(object)obj.ToString();

        //单测走不到
        //if( targetType == typeof(object) ) 
        //	return (T)obj;

        return (T)Convert.ChangeType(obj, targetType);
    }



    /// <summary>
    /// 执行命令，返回第一行第一列的值
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <returns>结果集的第一行,第一列</returns>
    public T ExecuteScalar<T>()
    {
        return Execute<T>(nameof(ExecuteScalar),
            cmd => ToScalar<T>(cmd.ExecuteScalar())
            );
    }


    /// <summary>
    /// 执行命令，返回第一行第一列的值
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <returns>结果集的第一行,第一列</returns>
    public async Task<T> ExecuteScalarAsync<T>()
    {
        return await ExecuteAsync<T>(nameof(ExecuteScalarAsync),
            async cmd => ToScalar<T>(await cmd.ExecuteScalarAsync())
            );
    }


    /// <summary>
    /// 执行命令，并返回第一列的值列表
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <returns>结果集的第一列集合</returns>
    public List<T> ToScalarList<T>()
    {
        return Execute<List<T>>(nameof(ToScalarList),
            cmd => {
                List<T> list = new List<T>();
                using( DbDataReader reader = cmd.ExecuteReader() ) {
                    while( reader.Read() ) {
                        list.Add(ToScalar<T>(reader[0]));
                    }
                    return list;
                }
            });
    }


    /// <summary>
    /// 执行命令，并返回第一列的值列表
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <returns>结果集的第一列集合</returns>
    public async Task<List<T>> ToScalarListAsync<T>()
    {
        return await ExecuteAsync<List<T>>(nameof(ToScalarListAsync),
            async cmd => {
                List<T> list = new List<T>();
                using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {
                    while( reader.Read() ) {
                        list.Add(ToScalar<T>(reader[0]));
                    }
                    return list;
                }
            });
    }



    /// <summary>
    /// 执行命令，将结果集转换为实体列表
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>实体集合</returns>
    public List<T> ToList<T>() where T : class, new()
    {
        return Execute<List<T>>(nameof(ToList),
            cmd => {
                using( DbDataReader reader = cmd.ExecuteReader() ) {
                    IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
                    return loader.ToList(reader);
                }
            });
    }


    /// <summary>
    /// 执行命令，将结果集转换为实体列表
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>实体集合</returns>
    public async Task<List<T>> ToListAsync<T>() where T : class, new()
    {
        return await ExecuteAsync<List<T>>(nameof(ToListAsync),
            async cmd => {
                using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {
                    IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
                    return loader.ToList(reader);
                }
            });
    }


    /// <summary>
    /// 执行命令，将结果集的第一行转换为实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>实体</returns>
    public T ToSingle<T>() where T : class, new()
    {
        return Execute<T>(nameof(ToSingle),
            cmd => {
                using( DbDataReader reader = cmd.ExecuteReader() ) {
                    IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
                    return loader.ToSingle(reader);
                }
            });
    }

    /// <summary>
    /// 执行命令，将结果集的第一行转换为实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T> ToSingleAsync<T>() where T : class, new()
    {
        return await ExecuteAsync<T>(nameof(ToSingleAsync),
            async cmd => {
                using( DbDataReader reader = await cmd.ExecuteReaderAsync() ) {
                    IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
                    return loader.ToSingle(reader);
                }
            });
    }



    /// <summary>
    /// 执行查找命令，生成分页结果，将结果集转换为实体列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pagingInfo"></param>
    /// <returns></returns>
    public List<T> ToPageList<T>(PagingInfo pagingInfo) where T : class, new()
    {
        if( this is StoreProcedure )
            throw new NotImplementedException();  // 存储过程不支持将一个命令分裂成 List+Count 这种模式

        var queries = this.Context.ClientProvider.GetPagedCommand(this, pagingInfo);

        List<T> list = queries.ListQuery.ToList<T>();

        if( queries.CountQuery != null )
            pagingInfo.TotalRows = queries.CountQuery.ExecuteScalar<int>();

        return list;
    }


    /// <summary>
    /// 执行查找命令，生成分页结果，将结果集转换为实体列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pagingInfo"></param>
    /// <returns></returns>
    public async Task<List<T>> ToPageListAsync<T>(PagingInfo pagingInfo) where T : class, new()
    {
        if( this is StoreProcedure )
            throw new NotImplementedException();  // 存储过程不支持将一个命令分裂成 List+Count 这种模式

        var queries = this.Context.ClientProvider.GetPagedCommand(this, pagingInfo);

        List<T> list = await queries.ListQuery.ToListAsync<T>();

        if( queries.CountQuery != null )
            pagingInfo.TotalRows = await queries.CountQuery.ExecuteScalarAsync<int>();

        return list;
    }


    /// <summary>
    /// 执行查找命令，生成分页结果
    /// </summary>
    /// <param name="pagingInfo"></param>
    /// <returns></returns>
    public DataTable ToPageTable(PagingInfo pagingInfo)
    {
        if( this is StoreProcedure )
            throw new NotImplementedException();  // 存储过程不支持将一个命令分裂成 List+Count 这种模式

        var queries = this.Context.ClientProvider.GetPagedCommand(this, pagingInfo);

        DataTable table = queries.ListQuery.ToDataTable();

        if( queries.CountQuery != null )
            pagingInfo.TotalRows = queries.CountQuery.ExecuteScalar<int>();

        return table;
    }


    /// <summary>
    /// 执行查找命令，生成分页结果
    /// </summary>
    /// <param name="pagingInfo"></param>
    /// <returns></returns>
    public async Task<DataTable> ToPageTableAsync(PagingInfo pagingInfo)
    {
        if( this is StoreProcedure )
            throw new NotImplementedException();  // 存储过程不支持将一个命令分裂成 List+Count 这种模式

        var queries = this.Context.ClientProvider.GetPagedCommand(this, pagingInfo);

        DataTable table = await queries.ListQuery.ToDataTableAsync();

        if( queries.CountQuery != null )
            pagingInfo.TotalRows = await queries.CountQuery.ExecuteScalarAsync<int>();

        return table;
    }



    #endregion

}
