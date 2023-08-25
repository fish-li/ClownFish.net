namespace ClownFish.Data;

/// <summary>
/// 应用库的连接信息
/// </summary>
public sealed class DbConfig : IDbConfig
{
    /// <summary>
    /// id
    /// </summary>
    [DbColumn(PrimaryKey = true)]
    public int Id { get; set; }

    /// <summary>
    /// 应用别名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 数据库类型，0，SQLSERVER，1，MYSQL，  2，PostgreSQL
    /// </summary>
    public DatabaseType DbType { get; set; }

    /// <summary>
    /// 数据库服务器地址
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    /// 服务监听端口。【仅当不是默认端口时指定】
    /// SQLSERVER默认端口：1433，MYSQL默认端口：3306
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// 数据库的登录用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 登录密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 额外的连接字符串参数
    /// </summary>
    public string Args { get; set; }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{DbType}/{Server}/{Database}";
    }

    //internal void Validate()
    //{
    //    if( this.Name.IsNullOrEmpty() )
    //        throw new ArgumentNullException(nameof(Name));

    //    if( this.Server.IsNullOrEmpty() )
    //        throw new ArgumentNullException(nameof(Server));
    //}

    internal DbConfig Clone()
    {
        return new DbConfig {
            Id = Id,
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


    /// <summary>
    /// 从字符串中构造DbConfig对象
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DbConfig Parse(string value)
    {
        if( value.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(value));

        return value.ToObject<DbConfig>();
    }
}
