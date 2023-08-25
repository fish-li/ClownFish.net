namespace ClownFish.Data;

/// <summary>
/// 用于定义不同类型数据库客户端差异行为的基类
/// </summary>
public abstract class BaseClientProvider
{
    /// <summary>
    /// 当前客户端支持数据库类别
    /// </summary>
    public abstract DatabaseType DatabaseType { get; }

    /// <summary>
    /// DbProviderFactory实例
    /// </summary>
    public abstract DbProviderFactory ProviderFactory { get; }

    /// <summary>
    /// 获取一个标识符的完整形式。用于从实体生成SQL时，且需要支持特殊名称而包含定界符的全名称。
    /// 拿SQLSERVER来说，传入 xxx 则应该返回 [xxx]
    /// 默认行为：return name;
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual string GetObjectFullName(string name)
    {
        return name;
    }

    /// <summary>
    /// 获取与数据库类型匹配的命令集合中参数名称。
    /// 默认行为：return "@" + name;
    /// </summary>
    /// <param name="name"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public virtual string GetParamterName(string name, DbContext dbContext = null)
    {
        return "@" + name;
    }

    /// <summary>
    /// 获取与数据库类型匹配的SQL语句中的参数名称。
    /// 默认行为：return "@" + name;
    /// </summary>
    /// <param name="name"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public virtual string GetParamterPlaceholder(string name, DbContext dbContext = null)
    {
        return "@" + name;
    }

    /// <summary>
    /// 判断异常是不是由于 【重复INSERT】 导致的异常，这类异常一般是由于违反数据库的唯一索引而触发的。
    /// 默认行为：return false;
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public virtual bool IsDuplicateInsertException(Exception ex)
    {
        return false;
    }


    /// <summary>
    /// 生成分页查询所需的部分语句
    /// </summary>
    /// <param name="query"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    public virtual CPQuery SetPagedQuery(CPQuery query, int skip, int take)
    {
        return query;
    }


    /// <summary>
    /// 根据query参数生成分页查询所需的Page2Query结构。
    /// 此方法用于调用 BaseCommand.ToPageList 方法时生成2个查询命令。
    /// 默认行为：throw new NotImplementedException(); 如果需要调用分页操作则需要实现这个方法。
    /// </summary>
    /// <param name="query"></param>
    /// <param name="pagingInfo"></param>
    /// <returns></returns>
    public virtual Page2Query GetPagedCommand(BaseCommand query, PagingInfo pagingInfo)
    {
        throw new NotImplementedException();
    }


    


    /// <summary>
    /// 切换数据库，默认行为：调用ADO.NET的实现，即：use xxxx; 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="databaseName"></param>
    public virtual void ChangeDatabase(DbContext dbContext, string databaseName)
    {
        dbContext.Connection.ChangeDatabase(databaseName);

        // 达梦：dbContext.CPQuery.Create("set schema " + databaseName).ExecuteNonQuery();
    }

    /// <summary>
    /// 执行SQL前的准备阶段会被调用。
    /// 默认行为：什么也不做。
    /// </summary>
    /// <param name="command"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public virtual void PrepareCommand(DbCommand command, DbContext dbContext)
    {
        // 默认什么也不做
    }


    /// <summary>
    /// 获取一个新的查询对象，它能够获取最近INSERT操作后产生的自增字段的值
    /// </summary>
    /// <param name="query">正在执行的查询</param>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual CPQuery GetNewIdQuery(CPQuery query, object entity)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// 生成一个有序GUID
    /// </summary>
    /// <returns></returns>
    public virtual Guid NewSeqGuid()
    {
        return GuidHelper.CreateSeqGuid(SequentialGuidType.AsString);
    }
}
