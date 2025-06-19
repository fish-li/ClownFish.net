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


    /// <summary>
    /// 执行查询命令，将结果以 Multi-Json 格式写入到文件
    /// </summary>
    /// <param name="command"></param>
    /// <param name="maxRows">最大导出行数，正数时有效</param>
    /// <param name="outFilePath">一个文件路径，用于数据的写入</param>
    /// <returns>查询的数据结果行数</returns>
    public static int ExportToNdJson(this BaseCommand command, int maxRows, string outFilePath)
    {
        if( command == null )
            throw new ArgumentNullException(nameof(command));

        using FileStream file = new FileStream(outFilePath, FileMode.Create, FileAccess.Write);
        using StreamWriter writer = new StreamWriter(file, Encoding.UTF8);

        using( DbDataReader reader = command.ExecuteReader() ) {
            return reader.DbReaderToNdJson(maxRows, writer);
        }
    }


    /// <summary>
    /// 执行查询命令，将结果以 Multi-Json 格式写入到文件
    /// </summary>
    /// <param name="command"></param>
    /// <param name="maxRows">最大导出行数，正数时有效</param>
    /// <param name="outFilePath">一个文件路径，用于数据的写入</param>
    /// <returns>查询的数据结果行数</returns>
    public static async Task<int> ExportToNdJsonAsync(this BaseCommand command, int maxRows, string outFilePath)
    {
        if( command == null )
            throw new ArgumentNullException(nameof(command));

        using FileStream file = new FileStream(outFilePath, FileMode.Create, FileAccess.Write);
        using StreamWriter writer = new StreamWriter(file, Encoding.UTF8);

        using( DbDataReader reader = await command.ExecuteReaderAsync() ) {
            return reader.DbReaderToNdJson(maxRows, writer);
        }
    }


    /// <summary>
    /// 执行查询命令，将结果以 Multi-Json 格式写入到 TextWriter
    /// </summary>
    /// <param name="command"></param>
    /// <param name="maxRows">最大导出行数，正数时有效</param>
    /// <param name="writer"></param>
    /// <returns>查询的数据结果行数</returns>
    public static int ExportToNdJson(this BaseCommand command, int maxRows, TextWriter writer)
    {
        if( command == null )
            throw new ArgumentNullException(nameof(command));

        using( DbDataReader reader = command.ExecuteReader() ) {
            return reader.DbReaderToNdJson(maxRows, writer);
        }
    }


    /// <summary>
    /// 执行查询命令，将结果以 Multi-Json 格式写入到 TextWriter
    /// </summary>
    /// <param name="command"></param>
    /// <param name="maxRows">最大导出行数，正数时有效</param>
    /// <param name="writer"></param>
    /// <returns>查询的数据结果行数</returns>
    public static async Task<int> ExportToNdJsonAsync(this BaseCommand command, int maxRows, TextWriter writer)
    {
        if( command == null )
            throw new ArgumentNullException(nameof(command));

        using( DbDataReader reader = await command.ExecuteReaderAsync() ) {
            return reader.DbReaderToNdJson(maxRows, writer);
        }
    }

}
