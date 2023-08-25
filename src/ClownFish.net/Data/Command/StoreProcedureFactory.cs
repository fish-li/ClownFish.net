namespace ClownFish.Data;

/// <summary>
/// StoreProcedure工厂
/// </summary>
public sealed class StoreProcedureFactory
{
    private readonly DbContext _dbContext;

    internal StoreProcedureFactory(DbContext dbContext)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        _dbContext = dbContext;
    }


    /// <summary>
    /// 根据存储过程名称和参数对象创建StoreProcedure实例
    /// </summary>
    /// <param name="spName"></param>
    /// <param name="parameterObject"></param>
    /// <returns></returns>
    public StoreProcedure Create(string spName, object parameterObject = null)
    {
        StoreProcedure sp = new StoreProcedure(_dbContext);
        sp.Init(spName, parameterObject);
        return sp;
    }

    /// <summary>
    /// 根据存储过程名称和参数对象创建StoreProcedure实例
    /// </summary>
    /// <param name="spName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public StoreProcedure Create(string spName, params DbParameter[] parameters)
    {
        StoreProcedure sp = new StoreProcedure(_dbContext);
        sp.Init(spName, parameters);
        return sp;
    }
}
