namespace ClownFish.Data;

/// <summary>
/// CPQuery工厂类
/// </summary>
public sealed class CPQueryFactory
{
    private readonly DbContext _dbContext;

    internal CPQueryFactory(DbContext dbContext)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        _dbContext = dbContext;
    }


    /// <summary>
    /// 根据指定的参数化SQL和参数对象创建CPQuery实例
    /// </summary>
    /// <param name="parameterizedSQL"></param>
    /// <param name="argsObject"></param>
    /// <returns></returns>
    public CPQuery Create(string parameterizedSQL, object argsObject = null)
    {
        CPQuery query = new CPQuery(_dbContext);
        query.Init(parameterizedSQL, argsObject);
        return query;
    }

    /// <summary>
    /// 根据指定的参数化SQL和参数对象创建CPQuery实例
    /// </summary>
    /// <param name="parameterizedSQL"></param>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public CPQuery Create(string parameterizedSQL, Hashtable dictionary)
    {
        CPQuery query = new CPQuery(_dbContext);
        query.Init(parameterizedSQL, dictionary);
        return query;
    }

    /// <summary>
    /// 根据指定的参数化SQL和参数对象创建CPQuery实例
    /// </summary>
    /// <param name="parameterizedSQL"></param>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public CPQuery Create(string parameterizedSQL, IDictionary<string, object> dictionary)
    {
        CPQuery query = new CPQuery(_dbContext);
        query.Init(parameterizedSQL, dictionary);
        return query;
    }

    /// <summary>
    /// 根据指定的参数化SQL和参数对象创建CPQuery实例
    /// </summary>
    /// <param name="parameterizedSQL"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public CPQuery Create(string parameterizedSQL, params DbParameter[] parameters)
    {
        CPQuery query = new CPQuery(_dbContext);
        query.Init(parameterizedSQL, parameters);
        return query;
    }

}
