using System.ComponentModel;

namespace ClownFish.Base.Config.Models;

/// <summary>
/// 与 app.config 对应的实体类型，用于反序列读取配置文件。
/// </summary>
[XmlRoot("configuration")]
public sealed class XmlAppConfiguration
{
    /// <summary>
    /// appSettings参数
    /// </summary>
    [XmlArray("appSettings")]
    [XmlArrayItem("add")]
    public XmlAppSetting[] AppSettings { get; set; }


    /// <summary>
    /// connectionStrings参数
    /// </summary>
    [XmlArray("connectionStrings")]
    [XmlArrayItem("add")]
    public XmlConnectionStringSetting[] ConnectionStrings { get; set; }

    /// <summary>
    /// dbConfigs参数
    /// </summary>
    [XmlArray("dbConfigs")]
    [XmlArrayItem("add")]
    public XmlDbConfig[] DbConfigs { get; set; }



    internal AppConfiguration ToAppConfiguration()
    {
        AppConfiguration config = new AppConfiguration();

        if( this.AppSettings != null && this.AppSettings.Length > 0 ) {
            config.AppSettings = this.AppSettings
                .Where(x => x.Key.HasValue())
                .Select(item => item.ToAppSetting())
                .ToArray();
        }
        if( this.ConnectionStrings != null && this.ConnectionStrings.Length > 0 ) {
            config.ConnectionStrings = this.ConnectionStrings
                .Where(x => x.Name.HasValue())
                .Select(item => item.ToConnectionStringSetting())
                .ToArray();
        }
        if( this.DbConfigs != null && this.DbConfigs.Length > 0 ) {
            config.DbConfigs = this.DbConfigs
                .Where(x => x.Name.HasValue() && x.Server.HasValue())
                .Select(item => item.ToDbConfig())
                .ToArray();
        }

        return config;
    }
}


/// <summary>
/// key/value 配置项
/// </summary>
public sealed class XmlAppSetting
{
    /// <summary>
    /// key
    /// </summary>
    [XmlAttribute("key")]
    public string Key { get; set; }

    /// <summary>
    /// value
    /// </summary>
    [XmlAttribute("value")]
    public string Value { get; set; }

    internal AppSetting ToAppSetting()
    {
        return new AppSetting {
            Key = this.Key,
            Value = this.Value
        };
    }
}


/// <summary>
/// 数据库连接配置类型
/// </summary>
public sealed class XmlConnectionStringSetting
{
    /// <summary>
    /// 数据库连接名称
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }


    /// <summary>
    /// 连接字符串
    /// </summary>
    [XmlAttribute("connectionString")]
    public string ConnectionString { get; set; }


    /// <summary>
    /// 数据驱动的提供者名称
    /// </summary>
    [XmlAttribute("providerName")]
    public string ProviderName { get; set; }


    internal ConnectionStringSetting ToConnectionStringSetting()
    {
        return new ConnectionStringSetting {
            Name = this.Name,
            ConnectionString = this.ConnectionString,
            ProviderName = this.ProviderName
        };
    }
}



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
    [XmlIgnore]
    public DatabaseType DbType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [XmlAttribute("dbType")]
    public string DbTypeString {
        get => DbType.ToString();
        set {
            // 尝试解析为数值
            if( int.TryParse(value, out int intValue) ) {
                DbType = (DatabaseType)intValue;
            }
            else {
                // 如果无法解析为数值，尝试按枚举名称解析
                DbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), value);
            }
        }
    }


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
        return $"Name={Name};DbType={DbType};Server={Server};Port={Port};Database={Database};UserName={UserName};Password={Password}";
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
