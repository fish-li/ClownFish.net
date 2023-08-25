using System.Linq.Expressions;

namespace ClownFish.Data.Linq;

/// <summary>
/// 表示一个实体的LINQ查询
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class EntityQuery<T> : IOrderedQueryable<T>
{
    private readonly EntityLinqProvider _provider = null;
    private readonly Expression _expression = null;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="provider"></param>
    public EntityQuery(EntityLinqProvider provider)
    {
        if( provider == null )
            throw new ArgumentNullException(nameof(provider));

        _provider = provider;
        _expression = Expression.Constant(this);
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="expression"></param>
    public EntityQuery(EntityLinqProvider provider, Expression expression)
    {
        if( provider == null )
            throw new ArgumentNullException(nameof(provider));
        if( expression == null )
            throw new ArgumentNullException(nameof(expression));

        _provider = provider;
        _expression = expression;
    }
    /// <summary>
    /// 获取在执行与 System.Linq.IQueryable 的此实例关联的表达式目录树时返回的元素的类型。
    /// </summary>
    public Type ElementType {
        get {
            return typeof(T);
        }
    }
    /// <summary>
    /// 获取与 System.Linq.IQueryable 的实例关联的表达式目录树。
    /// </summary>
    public Expression Expression {
        get {
            return _expression;
        }
    }

    /// <summary>
    /// 获取与此数据源关联的查询提供程序。
    /// </summary>
    public IQueryProvider Provider {
        get {
            return _provider;
        }
    }

    /// <summary>
    /// 返回循环访问集合的枚举数。 query.ToList() 方法入口
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
        return _provider.Execute<IEnumerable<T>>(_expression).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        // 不实现弱类型版本
        throw new NotImplementedException();
    }
}
