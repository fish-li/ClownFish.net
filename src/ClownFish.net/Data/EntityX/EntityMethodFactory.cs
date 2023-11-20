using System.Linq.Expressions;
using ClownFish.Data.Linq;

namespace ClownFish.Data;

/// <summary>
/// 提供实体操作的入口类
/// </summary>
public sealed class EntityMethodFactory
{
    private readonly DbContext _dbContext;

    internal EntityMethodFactory(DbContext dbContext)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        _dbContext = dbContext;
    }

    /// <summary>
    /// 创建与实体相关的EntityTable实例，开始数据库操作
    /// </summary>
    /// <typeparam name="T">实体的类型参数</typeparam>
    /// <returns>与实体相关的EntityTable实例</returns>
    internal EntityTable<T> From<T>() where T : Entity, new()
    {
        return new EntityTable<T>(_dbContext);
    }

    /// <summary>
    /// 开始LINQ查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="table">需要查询的数据表名称。通常场景下可不指定，【仅当】一个实体类型对应多个数据表时才需要指定。</param>
    /// <returns></returns>
    public EntityQuery<T> Query<T>(string table = null) where T : Entity, new()
    {
        EntityLinqProvider provider = new EntityLinqProvider(_dbContext, typeof(T), table);
        return new EntityQuery<T>(provider);
    }


    /// <summary>
    /// 创建与实体相关的代理对象。 CreateProxy方法的别名。
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">初始实体，如果为null表示新增，否则基于指定的对象来创建实体代理副本</param>
    /// <returns>与实体相关的代理对象</returns>
    public T BeginEdit<T>(T entity = null) where T : Entity, new()
    {
        return CreateProxy(entity);
    }


    /// <summary>
    /// 创建实体的代理对象。
    /// 实体代理对象可感知属性的所有变更情况，提供动态SQL生成能力，
    /// 实体代理对象提供了简便的 Insert/Update/Delete 方法。
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">初始实体，如果为null表示新增，否则基于指定的对象来创建实体代理副本</param>
    /// <returns></returns>
    /// <example>
    /// var product = dbContext.Entity.CreateProxy(product);
    /// product.ProductId = 123;
    /// product.ProductName = "xxxx";
    /// product.Insert() or Update() or Delete()
    /// </example>
    public T CreateProxy<T>(T entity = null) where T : Entity, new()
    {
        if( entity == null )
            entity = new T();

        return (T)entity.CreateProxy(_dbContext);
    }



    #region  Insert 方法


    /// <summary>
    /// 将一个实体插入到数据库
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="flags">Insert操作控制参数</param>
    /// <returns></returns>
    public long Insert<T>(T entity, InsertOption flags = InsertOption.NoSet) where T : Entity, new()
    {
        CPQuery query = flags.HasFlag(InsertOption.AllFields)
                        ? _dbContext.CPQuery.Create(EntityCudUtils.GetInsertSQL(entity, _dbContext), entity)
                        : EntityCudUtils.GetInsertQuery(entity, _dbContext);

        if( query == null )
            return 0;

        bool getNewId = flags.HasFlag(InsertOption.GetNewId);

        if( flags.HasFlag(InsertOption.IgnoreDuplicateError) == false ) {
            return EntityCudUtils.ExecuteInsert(query, getNewId, entity);
        }


        try {
            return EntityCudUtils.ExecuteInsert(query, getNewId, entity);
        }
        catch( Exception ex ) {
            if( _dbContext.IsDuplicateInsert(ex) ) {
                return -1;
            }
            else {
                throw;
            }
        }
    }



    /// <summary>
    /// 将一个实体插入到数据库
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="flags">Insert操作控制参数</param>
    /// <returns></returns>
    public async Task<long> InsertAsync<T>(T entity, InsertOption flags = InsertOption.NoSet) where T : Entity, new()
    {
        CPQuery query = flags.HasFlag(InsertOption.AllFields)
                        ? _dbContext.CPQuery.Create(EntityCudUtils.GetInsertSQL(entity, _dbContext), entity)
                        : EntityCudUtils.GetInsertQuery(entity, _dbContext);

        if( query == null )
            return 0;

        bool getNewId = flags.HasFlag(InsertOption.GetNewId);

        if( flags.HasFlag(InsertOption.IgnoreDuplicateError) == false ) {
            return await EntityCudUtils.ExecuteInsertAsync(query, getNewId, entity);
        }


        try {
            return await EntityCudUtils.ExecuteInsertAsync(query, getNewId, entity);
        }
        catch( Exception ex ) {
            if( _dbContext.IsDuplicateInsert(ex) ) {
                return -1;
            }
            else {
                throw;
            }
        }
    }

    #endregion


    #region  Update 方法

    /// <summary>
    /// 用当前实体的属性值（非NULL值）更新数据库
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns></returns>
    public int Update<T>(T entity) where T : Entity, new()
    {
        T proxy = EntityCudUtils.CreateUpdateProxy(entity, _dbContext);
        return proxy.Update();
    }


    /// <summary>
    /// 用当前实体的属性值（非NULL值）更新数据库
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns></returns>
    public async Task<int> UpdateAsync<T>(T entity) where T : Entity, new()
    {
        T proxy = EntityCudUtils.CreateUpdateProxy(entity, _dbContext);
        return await proxy.UpdateAsync();
    }


    private CPQuery GetUpdateQuery<T>(Action<T> setAction, Expression<Func<T, bool>> where) where T : Entity, new()
    {
        if( setAction == null )
            throw new ArgumentNullException(nameof(setAction));
        if( where == null )
            throw new ArgumentNullException(nameof(where));

        T proxy = _dbContext.Entity.CreateProxy<T>();
        setAction(proxy);

        CPQuery query = proxy.GetUpdateQuery0(null);
        if( query == null )
            throw new InvalidOperationException("没有任何 set 操作！");

        query.AppendSql("\r\nWHERE ");

        WhereParase whereParse = new WhereParase(_dbContext, query);
        whereParse.Visit(where);

        if( query.Command.CommandText.EndsWith0("\r\nWHERE ") )
            throw new InvalidOperationException("不允许执行没有任何 查询条件 的 UPDATE ！");

        return query;
    }

    /// <summary>
    /// 更新数据表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="setAction"></param>
    /// <param name="where"></param>
    /// <returns></returns>
    public int Update<T>(Action<T> setAction, Expression<Func<T, bool>> where) where T : Entity, new()
    {
        CPQuery query = GetUpdateQuery<T>(setAction, where);

        return query.ExecuteNonQuery();
    }

    /// <summary>
    /// 更新数据表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="setAction"></param>
    /// <param name="where"></param>
    /// <returns></returns>
    public async Task<int> UpdateAsync<T>(Action<T> setAction, Expression<Func<T, bool>> where) where T : Entity, new()
    {
        CPQuery query = GetUpdateQuery<T>(setAction, where);

        return await query.ExecuteNonQueryAsync();
    }


    #endregion


    #region  Delete 方法


    private CPQuery GetDeleteQuery<T>(Expression<Func<T, bool>> where) where T : Entity, new()
    {
        if( where == null )
            throw new ArgumentNullException(nameof(where));

        string tableName = typeof(T).GetDbTableName();
        CPQuery query = _dbContext.CreateCPQuery()
                        + "DELETE FROM " + _dbContext.GetObjectFullName(tableName) + "\r\nWHERE ";

        WhereParase whereParse = new WhereParase(_dbContext, query);
        whereParse.Visit(where);

        if( query.Command.CommandText.EndsWith0("\r\nWHERE ") )
            throw new InvalidOperationException("不允许执行没有任何 查询条件 的 DELETE ！");

        return query;
    }


    /// <summary>
    /// 删除数据表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="where"></param>
    /// <returns></returns>
    public int Delete<T>(Expression<Func<T, bool>> where) where T : Entity, new()
    {
        CPQuery query = GetDeleteQuery<T>(where);

        return query.ExecuteNonQuery();
    }

    /// <summary>
    /// 删除数据表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="where"></param>
    /// <returns></returns>
    public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> where) where T : Entity, new()
    {
        CPQuery query = GetDeleteQuery<T>(where);

        return await query.ExecuteNonQueryAsync();
    }


    /// <summary>
    /// 根据实体类型和对应的主键字段值，删除对应的数据表记录
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="key">主键字段值</param>
    /// <returns></returns>
    public int Delete<T>(object key) where T : Entity, new()
    {
        T proxy = EntityCudUtils.CreateDeleteProxy<T>(_dbContext, key);
        return proxy.Delete();
    }


    /// <summary>
    /// 根据实体类型和对应的主键字段值，删除对应的数据表记录
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="key">主键字段值</param>
    /// <returns></returns>
    public async Task<int> DeleteAsync<T>(object key) where T : Entity, new()
    {
        T proxy = EntityCudUtils.CreateDeleteProxy<T>(_dbContext, key);
        return await proxy.DeleteAsync();
    }


    #endregion


    #region  GetByKey 方法

    /// <summary>
    /// 根据实体类型和对应的主键字段值，查询对应的实体对象
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="key">主键字段值</param>
    /// <returns></returns>
    public T GetByKey<T>(object key) where T : Entity, new()
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        return From<T>()
                    .Where(x => EntityCudUtils.SetKeyValue(x, key))
                    .ToSingle();
    }



    /// <summary>
    /// 根据实体类型和对应的主键字段值，查询对应的实体对象
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="key">主键字段值</param>
    /// <returns></returns>
    public async Task<T> GetByKeyAsync<T>(object key) where T : Entity, new()
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        return await From<T>()
                    .Where(x => EntityCudUtils.SetKeyValue(x, key))
                    .ToSingleAsync();
    }

    #endregion



}
