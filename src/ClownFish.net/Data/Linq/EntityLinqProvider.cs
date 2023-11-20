using System.Linq.Expressions;

namespace ClownFish.Data.Linq;

/// <summary>
/// 实体的LINQ查询的提供者
/// </summary>
public sealed class EntityLinqProvider : IQueryProvider
{
    private readonly DbContext _dbContext;
    private readonly Type _entityType;
    private readonly string _table;

    internal EntityLinqProvider(DbContext dbContext, Type entityType, string table = null)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        _dbContext = dbContext;
        _entityType = entityType;
        _table = table.IsNullOrEmpty() ? dbContext.GetObjectFullName(entityType.GetDbTableName()) : dbContext.GetObjectFullName(table);
    }


    /// <summary>
    /// 构造一个 System.Linq.IQueryable 对象，该对象可计算指定表达式目录树所表示的查询。
    /// </summary>
    /// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
    /// <returns></returns>
    public IQueryable CreateQuery(Expression expression)
    {
        // 不实现弱类型版本
        throw new NotImplementedException();
    }

    /// <summary>
    /// 构造一个 System.Linq.IQueryable`1 对象，该对象可计算指定表达式目录树所表示的查询。
    /// </summary>
    /// <typeparam name="TElement">返回的 System.Linq.IQueryable`1 的元素的类型</typeparam>
    /// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
    /// <returns></returns>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new EntityQuery<TElement>(this, expression);
    }

    /// <summary>
    /// 执行指定表达式目录树所表示的查询。
    /// </summary>
    /// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
    /// <returns></returns>
    public object Execute(Expression expression)
    {
        // 不实现弱类型版本
        throw new NotImplementedException();
    }

    /// <summary>
    /// 执行指定表达式目录树所表示的强类型查询。【所有==同步==数据访问方法的入口】
    /// </summary>
    /// <typeparam name="TResult">执行查询所生成的值的类型</typeparam>
    /// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
    /// <returns></returns>
    public TResult Execute<TResult>(Expression expression)
    {
        LinqParser sqlParser = new LinqParser(_dbContext, _entityType, expression, _table);
        sqlParser.Translator();
        object result = sqlParser.ExecuteCommand();

        return (TResult)(object)result;
    }

    /// <summary>
    /// 执行指定表达式目录树所表示的强类型查询。【所有==异步步==数据访问方法的入口】
    /// </summary>
    /// <typeparam name="TResult">执行查询所生成的值的类型</typeparam>
    /// <param name="expression">表示 LINQ 查询的表达式目录树。</param>
    /// <returns></returns>
    public async Task<TResult> ExecuteAsync<TResult>(Expression expression)
    {
        LinqParser sqlParser = new LinqParser(_dbContext, _entityType, expression, _table);
        sqlParser.Translator();
        object result = await sqlParser.ExecuteCommandAsync();

        return (TResult)(object)result;
    }


}
