namespace ClownFish.Data;

/// <summary>
/// 定义数据库连接的必要属性成员接口
/// </summary>
public interface IDbConfig
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    DatabaseType DbType { get; set; }

    /// <summary>
    /// 数据库服务器地址
    /// </summary>
    string Server { get; set; }

    /// <summary>
    /// 服务监听端口。【仅当不是默认端口时指定】
    /// SQLSERVER默认端口：1433，MYSQL默认端口：3306
    /// </summary>
    int? Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    string Database { get; set; }

    /// <summary>
    /// 数据库的登录用户名
    /// </summary>
    string UserName { get; set; }

    /// <summary>
    /// 登录密码
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// 额外的连接字符串参数
    /// </summary>
    string Args { get; set; }
}
