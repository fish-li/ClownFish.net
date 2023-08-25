using ClownFish.Base.Config.Models;

namespace ClownFish.Data;

/// <summary>
/// 获取数据库连接的工具类
/// </summary>
public static class ConnectionManager
{
    internal static ConnectionInfo GetFirstConnection()
    {
        ConnectionStringSetting setting = AppConfig.GetConfigObject().GetConfiguration().ConnectionStrings.FirstOrDefault();
        if( setting == null )
            throw new InvalidOperationException("没有在配置文件中注册数据库连接！");

        return new ConnectionInfo(setting);
    }

    /// <summary>
    /// 根据名称获取对应的连接信息
    /// </summary>
    /// <param name="name">连接字符串的名称</param>
    /// <param name="ifNotFoundThrowException">找到匹配的对象时，是否要抛出异常</param>
    /// <returns></returns>
    public static ConnectionInfo GetConnection(string name, bool ifNotFoundThrowException = true)
    {
        ConnectionStringSetting setting = AppConfig.GetConnectionString(name);

        if( setting == null ) {
            if( ifNotFoundThrowException )
                throw new ArgumentOutOfRangeException("指定的数据库连接名称没有注册：" + name);
            else
                return null;
        }
        else {
            return new ConnectionInfo(setting);
        }
    }



    /// <summary>
    /// 根据名称获取对应的连接信息
    /// </summary>
    /// <param name="name">连接字符串的名称</param>
    /// <param name="ifNotFoundThrowException">找到匹配的对象时，是否要抛出异常</param>
    /// <returns></returns>
    public static DbConfig GetDbConfig(string name, bool ifNotFoundThrowException = true)
    {
        DbConfig dbConfig = AppConfig.GetDbConfig(name);
        if( dbConfig == null ) {
            if( ifNotFoundThrowException )
                throw new ArgumentOutOfRangeException("指定的数据库连接名称没有注册：" + name);
            else
                return null;
        }
        else {
            return dbConfig;
        }
    }
}
