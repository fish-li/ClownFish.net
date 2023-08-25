namespace ClownFish.Data;

/// <summary>
/// 一些与命令相关的扩展工具类
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// 设置当前要执行的命令对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static T SetCommand<T>(this T command, Action<DbCommand> action) where T : BaseCommand
    {
        // 说明：其实没有这个方法也可以，只是写代码就不够顺畅了

        // 例如：CPQuery.Create(...).SetCommand(...).ExecuteNonQuery();
        // 对应写法：
        // CPQuery query = CPQuery.Create(...);
        // query.Command = ..........
        // query.ExecuteNonQuery();

        if( action != null )
            action.Invoke(command.Command);
        return command;
    }


    /// <summary>
    /// 设置等待命令执行的时间（单位：秒）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    public static T SetTimeout<T>(this T command, int commandTimeout) where T : BaseCommand
    {
        command.Command.CommandTimeout = commandTimeout;
        return command;
    }



    internal static DbParameter[] CloneParameters(this DbCommand command)
    {
        if( command == null )
            throw new ArgumentNullException(nameof(command));

        if( command.Parameters.Count == 0 )
            return Empty.Array<DbParameter>();


        DbParameter[] parameters2 = new DbParameter[command.Parameters.Count];

        int i = 0;
        foreach( DbParameter src in command.Parameters ) {
            DbParameter newParam = null;
            ICloneable x = src as ICloneable;
            if( x != null ) {
                newParam = (DbParameter)x.Clone();
            }
            else {
                // 下面这段代码估计永远不会运行！
                newParam = CloneParameter(src, command);
            }
            parameters2[i++] = newParam;
        }
        return parameters2;
    }



    private static DbParameter CloneParameter(DbParameter src, DbCommand command)
    {
        DbParameter newParam = command.CreateParameter();
        newParam.ParameterName = src.ParameterName;
        newParam.DbType = src.DbType;
        newParam.Size = src.Size;
        newParam.Value = src.Value;
        newParam.Direction = src.Direction;
        return newParam;
    }


}
