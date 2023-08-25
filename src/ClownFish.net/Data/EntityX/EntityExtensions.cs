namespace ClownFish.Data;

/// <summary>
/// 实体插入相关扩展方法
/// </summary>
public static partial class EntityExtensions
{
    /// <summary>
    /// 将一个【实体对象】插入到数据库（快捷方式版本）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <param name="getNewId">插入后，是否需要返回新产生的自增列ID</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.Insert(entity, InsertOption.GetNewId) 来代替当前方法。")]
    public static long Insert<T>(this T entity, DbContext dbContext, bool getNewId) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        InsertOption flags = getNewId ? InsertOption.GetNewId : InsertOption.NoSet;
        return dbContext.Entity.Insert(entity, flags);
    }


    /// <summary>
    /// 将一个【实体对象】插入到数据库（快捷方式版本）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <param name="getNewId">插入后，是否需要返回新产生的自增列ID</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.InsertAsync(entity, InsertOption.GetNewId) 来代替当前方法。")]
    public static async Task<long> InsertAsync<T>(this T entity, DbContext dbContext, bool getNewId) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        InsertOption flags = getNewId ? InsertOption.GetNewId : InsertOption.NoSet;
        return await dbContext.Entity.InsertAsync(entity, flags);
    }


    /// <summary>
    /// 将一个【实体对象】插入到数据库（快捷方式版本），并检查是否为重复插入（需要唯一索引）
    /// </summary>
    /// <typeparam name="T">实体的类型参数</typeparam>
    /// <param name="entity">数据实体对象</param>
    /// <param name="dbContext">数据库连接对象</param>
    /// <returns>如果数据成功插入，返回true，如果数据行已存在，返回 false</returns>
    [Obsolete("建议使用 dbContext.Entity.Insert(entity, InsertOption.AllFields | InsertOption.IgnoreDuplicateError) 来代替当前方法。")]
    public static bool Insert2<T>(this T entity, DbContext dbContext) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        InsertOption flags = InsertOption.AllFields | InsertOption.IgnoreDuplicateError;
        long result = dbContext.Entity.Insert(entity, flags);
        return result >= 0;
    }


    /// <summary>
    /// 将一个【实体对象】插入到数据库（快捷方式版本），并检查是否为重复插入（需要唯一索引）
    /// </summary>
    /// <typeparam name="T">实体的类型参数</typeparam>
    /// <param name="entity">数据实体对象</param>
    /// <param name="dbContext">数据库连接对象</param>
    /// <returns>如果数据成功插入，返回true，如果数据行已存在，返回 false</returns>
    [Obsolete("建议使用 dbContext.Entity.InsertAsync(entity, InsertOption.AllFields | InsertOption.IgnoreDuplicateError) 来代替当前方法。")]
    public static async Task<bool> Insert2Async<T>(this T entity, DbContext dbContext) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        InsertOption flags = InsertOption.AllFields | InsertOption.IgnoreDuplicateError;
        long result = await dbContext.Entity.InsertAsync(entity, flags);
        return result >= 0;
    }


    /// <summary>
    /// 将一个【实体对象】的属性值（非NULL值）更新数据库（快捷方式版本）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.Update(entity) 来代替当前方法。")]
    public static int Update<T>(this T entity, DbContext dbContext) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        return dbContext.Entity.Update(entity);
    }



    /// <summary>
    /// 将一个【实体对象】的属性值（非NULL值）更新数据库（快捷方式版本）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.UpdateAsync(entity) 来代替当前方法。")]
    public static async Task<int> UpdateAsync<T>(this T entity, DbContext dbContext) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        return await dbContext.Entity.UpdateAsync(entity);
    }





    #region  过时的兼容方法       



    /// <summary>
    /// 将一个【实体对象】的属性值（非NULL值）更新数据库
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>        
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <param name="entity">实体对象</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.Update(entity) 来代替当前方法。")]
    public static int Update<T>(this DbContext dbContext, T entity) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        return dbContext.Entity.Update(entity);
    }



    /// <summary>
    /// 将一个【实体对象】的属性值（非NULL值）更新数据库
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>        
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <param name="entity">实体对象</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.UpdateAsync(entity) 来代替当前方法。")]
    public static async Task<int> UpdateAsync<T>(this DbContext dbContext, T entity) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        return await dbContext.Entity.UpdateAsync(entity);
    }


    /// <summary>
    /// 根据实体类型和对应的主键字段值，删除对应的数据表记录
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <param name="key">主键字段值</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.Delete<T>(key) 来代替当前方法。")]
    public static int Delete<T>(this DbContext dbContext, object key) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        return dbContext.Entity.Delete<T>(key);
    }


    /// <summary>
    /// 根据实体类型和对应的主键字段值，删除对应的数据表记录
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <param name="key">主键字段值</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.DeleteAsync<T>(key) 来代替当前方法。")]
    public static async Task<int> DeleteAsync<T>(this DbContext dbContext, object key) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        return await dbContext.Entity.DeleteAsync<T>(key);
    }

    /// <summary>
    /// 根据实体类型和对应的主键字段值，查询对应的实体对象
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <param name="key">主键字段值</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.GetByKey<T>(key) 来代替当前方法。")]
    public static T GetByKey<T>(this DbContext dbContext, object key) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        return dbContext.Entity.GetByKey<T>(key);
    }




    /// <summary>
    /// 根据实体类型和对应的主键字段值，查询对应的实体对象
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dbContext">DbContext实例，表示一个数据库连接对象</param>
    /// <param name="key">主键字段值</param>
    /// <returns></returns>
    [Obsolete("建议使用 dbContext.Entity.GetByKeyAsync<T>(key) 来代替当前方法。")]
    public static async Task<T> GetByKeyAsync<T>(this DbContext dbContext, object key) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        return await dbContext.Entity.GetByKeyAsync<T>(key);
    }



    #endregion

}
