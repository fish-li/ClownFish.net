namespace ClownFish.Data;

/// <summary>
/// 数据库连接配置类型
/// </summary>
[Serializable]
public sealed class ConnectionStringSetting
{
    /// <summary>
    /// 连接名称
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

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Name={Name}\nProviderName={ProviderName}\nConnectionString={ConnectionString}";
    }

    ///// <summary>
    ///// 构造方法
    ///// </summary>
    //public ConnectionStringSetting() { }


    ///// <summary>
    ///// 构造方法
    ///// </summary>
    ///// <param name="name"></param>
    ///// <param name="connectionString"></param>
    ///// <param name="providerName"></param>
    //public ConnectionStringSetting(string name, string connectionString, string providerName)
    //{

    //    this.Name = name;
    //    this.ConnectionString = connectionString;
    //    this.ProviderName = providerName;
    //}


    //internal void Validate()
    //{
    //    if( this.Name.IsNullOrEmpty() )
    //        throw new ArgumentNullException(nameof(Name));

    //    if( this.ConnectionString.IsNullOrEmpty() )
    //        throw new ArgumentNullException(nameof(ConnectionString));
    //}


    internal ConnectionStringSetting Clone()
    {
        return new ConnectionStringSetting {
            Name = Name,
            ConnectionString = ConnectionString,
            ProviderName = ProviderName
        };
    }
}