namespace ClownFish.Data;

/// <summary>
/// 定义一些扩展方法
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// 获取一个标识符的包含限定符的完整名称。
    /// 例如：标识符 name 的完整名称是 [name] 或者 `name` 或者 "name" ，具体限定符由数据库的类型决定。
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static string GetObjectFullName(this DbContext dbContext, string name)
    {
        // 绝大多数情况下，添加【定界符】没有意义，只会浪费性能！
        // 所以没有明确要求，就不自动添加。
        if( dbContext.EnableDelimiter == false )
            return name;

        return dbContext.ClientProvider.GetObjectFullName(name);
    }

    /// <summary>
    /// 执行一个委托，里面可以包含操作数据库操作。
    /// 如果执行结束且没有异常发生，返回 true，
    /// 如果执行过程中出现异常，且异常为数据库重复插入异常，则返回 false，
    /// 如果执行过程中出现其它异常，异常会重新抛出。
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action">表示一个执行过程，它应该包含一个数据库的插入操作。</param>
    /// <returns></returns>
    internal static bool ExecuteAndIgnoreDuplicateInsertException(this DbContext dbContext, Action action)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));
        if( action == null )
            throw new ArgumentNullException(nameof(action));


        try {
            action();
            return true;
        }
        catch( Exception ex ) {

            if( IsDuplicateInsert(dbContext, ex) ) {
                // 忽略这种异常
                return false;
            }
            else {
                throw;
            }
        }
    }


    /// <summary>
    /// 执行一个委托，里面可以包含操作数据库操作。
    /// 如果执行结束且没有异常发生，返回 true，
    /// 如果执行过程中出现异常，且异常为数据库重复插入异常，则返回 false，
    /// 如果执行过程中出现其它异常，异常会重新抛出。
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="action">表示一个执行过程，它应该包含一个数据库的插入操作。</param>
    /// <returns></returns>
    internal static async Task<bool> ExecuteAndIgnoreDuplicateInsertExceptionAsync(this DbContext dbContext, Func<Task> action)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));
        if( action == null )
            throw new ArgumentNullException(nameof(action));


        try {
            await action();
            return true;
        }
        catch( Exception ex ) {

            if( IsDuplicateInsert(dbContext, ex) ) {
                // 忽略这种异常
                return false;
            }
            else {
                throw;
            }
        }
    }


    /// <summary>
    /// 判断一个数据库异常是否由于重复插入导致的，由唯一索引控制。
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static bool IsDuplicateInsert(this DbContext dbContext, Exception ex)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));
        if( ex == null )
            throw new ArgumentNullException(nameof(ex));

        // 说明：
        // 1，基于ADO.NET的要求，数据库操作异常都应该是 DbException 的子类（由各驱动实现）
        // 2，在实际数据库操作过程出现异常，ex的类型将会是 DbExceuteException

        DbException dbException = ex as DbException;
        if( dbException == null )
            dbException = ex.GetBaseException() as DbException;

        if( dbException == null )
            return false;

        return dbContext.ClientProvider.IsDuplicateInsertException(dbException);
    }

}
