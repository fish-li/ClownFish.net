namespace ClownFish.Data;

/// <summary>
/// 实体对应的数据表操作类型
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class EntityTable<T> where T : Entity, new()
{
    private readonly DbContext _dbContext;

    internal EntityTable(DbContext dbContext)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        _dbContext = dbContext;
    }


    // 说明：代理对象从原实体继承而来，所以可以用Entity来引用，而且还实现了IEntityProxy接口
    private Entity _setProxy;
    private Entity _whereProxy;


    /// <summary>
    /// 设置实体要更新的字段
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public EntityTable<T> Set(Action<T> action)
    {
        if( action == null )
            throw new ArgumentNullException(nameof(action));
        if( _setProxy != null )
            throw new InvalidOperationException("不允许重复调用Set方法。");

        T entity = new T();
        _setProxy = (Entity)entity.CreateProxy(_dbContext);

        action((T)_setProxy);
        return this;
    }

    /// <summary>
    /// 根据赋值的属生成WHERE条件
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public EntityTable<T> Where(Action<T> action)
    {
        if( action == null )
            throw new ArgumentNullException(nameof(action));
        if( _whereProxy != null )
            throw new InvalidOperationException("不允许重复调用Where方法。");

        T entity = new T();
        _whereProxy = (Entity)entity.CreateProxy(_dbContext);

        action((T)_whereProxy);
        return this;
    }

    ///// <summary>
    ///// 根据赋值的属生成SELECT字段列表，对ToSingle/ToList方法有效
    ///// </summary>
    ///// <param name="action"></param>
    ///// <returns></returns>
    //public EntityTable<T> Select(Action<T> action)
    //{
    //	if( action == null )
    //		throw new ArgumentNullException(nameof(action));
    //	if (_selectProxy != null)
    //		throw new InvalidOperationException("不允许重复调用Select方法。");

    //	T entity = new T();
    //	_selectProxy = entity.GetProxy(_dbContext);

    //	action((T)_selectProxy);
    //	return this;
    //}

    private CPQuery GetInsertQuery()
    {
        if( _setProxy == null )
            throw new InvalidOperationException("请先调用Set方法。");

        CPQuery query = _setProxy.GetInsertQuery0();
        return query;
    }

    /// <summary>
    /// 根据指定的实体属性，生成INSERT语句，并执行数据库插入操作
    /// </summary>
    /// <returns></returns>
    public int Insert()
    {
        CPQuery query = GetInsertQuery();
        return query.ExecuteNonQuery();
    }

    /// <summary>
    /// 根据指定的实体属性，生成INSERT语句，并执行数据库插入操作
    /// </summary>
    /// <returns></returns>
    public async Task<int> InsertAsync()
    {
        CPQuery query = GetInsertQuery();
        return await query.ExecuteNonQueryAsync();
    }

    private CPQuery GetDeleteQuery()
    {
        if( _whereProxy == null )
            throw new InvalidOperationException("请先调用Where方法。");

        CPQuery where = _whereProxy.GetWhereQuery0();

        CPQuery query = _dbContext.CreateCPQuery()
                        + "DELETE FROM  " + _dbContext.GetObjectFullName(_whereProxy.GetTableName())
                        + where;
        return query;
    }

    /// <summary>
    /// 根据指定的实体属性，生成DELETE查询条件，并执行数据库插入操作
    /// </summary>
    /// <returns></returns>
    public int Delete()
    {
        CPQuery query = GetDeleteQuery();
        return query.ExecuteNonQuery();
    }

    /// <summary>
    /// 根据指定的实体属性，生成DELETE查询条件，并执行数据库插入操作
    /// </summary>
    /// <returns></returns>
    public async Task<int> DeleteAsync()
    {
        CPQuery query = GetDeleteQuery();
        return await query.ExecuteNonQueryAsync();
    }


    private CPQuery GeUpdateQuery()
    {
        if( _setProxy == null )
            throw new InvalidOperationException("请先调用Set方法。");
        if( _whereProxy == null )
            throw new InvalidOperationException("请先调用Where方法。");

        CPQuery update = _setProxy.GetUpdateQuery0(null);
        CPQuery where = _whereProxy.GetWhereQuery0();

        CPQuery query = update + where;
        return query;
    }

    /// <summary>
    /// 根据指定的实体属性，生成UPDATE的语句，并执行数据库插入操作
    /// </summary>
    /// <returns></returns>
    public int Update()
    {
        CPQuery query = GeUpdateQuery();
        return query.ExecuteNonQuery();
    }


    /// <summary>
    /// 根据指定的实体属性，生成UPDATE的语句，并执行数据库插入操作
    /// </summary>
    /// <returns></returns>
    public async Task<int> UpdateAsync()
    {
        CPQuery query = GeUpdateQuery();
        return await query.ExecuteNonQueryAsync();
    }


    /// <summary>
    /// 根据Where调用产生的查询条件获取单个实体对象
    /// </summary>
    /// <returns></returns>
    public T ToSingle()
    {
        CPQuery query = GetSelectQuery();
        return query.ToSingle<T>();
    }

    /// <summary>
    /// 根据Where调用产生的查询条件获取单个实体对象
    /// </summary>
    /// <returns></returns>
    public async Task<T> ToSingleAsync()
    {
        CPQuery query = GetSelectQuery();
        return await query.ToSingleAsync<T>();
    }

    /// <summary>
    /// 根据Where调用产生的查询条件获取实体对象列表
    /// </summary>
    /// <returns></returns>
    public List<T> ToList()
    {
        CPQuery query = GetSelectQuery();
        return query.ToList<T>();
    }


    /// <summary>
    /// 根据Where调用产生的查询条件获取实体对象列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<T>> ToListAsync()
    {
        CPQuery query = GetSelectQuery();
        return await query.ToListAsync<T>();
    }

    private CPQuery GetSelectQuery()
    {
        if( _whereProxy == null )
            throw new InvalidOperationException("请先调用Where方法。");

        CPQuery select = _dbContext.CreateCPQuery() + "SELECT * ";

        CPQuery where = _whereProxy.GetWhereQuery0();

        CPQuery query = select
                        + " FROM " + _dbContext.GetObjectFullName(_whereProxy.GetTableName())
                        + where;

        return query;
    }


}
