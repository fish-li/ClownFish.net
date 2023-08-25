using System.ComponentModel;

namespace ClownFish.Base.Config.Models;

/// <summary>
/// 数据库连接的配置类型
/// </summary>
public class XmlDbConfig
{
    /// <summary>
    /// 应用别名
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// 数据库类型，0，SQLSERVER，1，MYSQL，  2，PostgreSQL
    /// </summary>
    [XmlAttribute("dbType")]
    public DatabaseType DbType { get; set; }

    /// <summary>
    /// 数据库服务器地址
    /// </summary>
    [XmlAttribute("server")]
    public string Server { get; set; }

    /// <summary>
    /// 服务监听端口。【仅当不是默认端口时指定】
    /// SQLSERVER默认端口：1433，MYSQL默认端口：3306
    /// </summary>
    [XmlAttribute("port")]
    [DefaultValue(0)]
    public int Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [XmlAttribute("database")]
    public string Database { get; set; }

    /// <summary>
    /// 数据库的登录用户名
    /// </summary>
    [XmlAttribute("uid")]
    public string UserName { get; set; }

    /// <summary>
    /// 登录密码
    /// </summary>
    [XmlAttribute("pwd")]
    public string Password { get; set; }

    /// <summary>
    /// 额外的连接字符串参数
    /// </summary>
    [XmlAttribute("args")]
    public string Args { get; set; }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{DbType}/{Server}/{Database}";
    }

    internal DbConfig ToDbConfig()
    {
        return new DbConfig {
            Name = Name,
            DbType = DbType,
            Server = Server,
            Port = Port,
            Database = Database,
            UserName = UserName,
            Password = Password,
            Args = Args
        };
    }


}
