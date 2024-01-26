namespace ClownFish.Base.Config.Models;

/// <summary>
/// 与 app.config 对应的实体类型，用于反序列读取配置文件。
/// </summary>
[XmlRoot("configuration")]
public sealed class AppConfiguration
{
    /// <summary>
    /// appSettings参数
    /// </summary>
    [XmlArray("appSettings")]
    [XmlArrayItem("add")]
    public AppSetting[] AppSettings { get; set; }


    /// <summary>
    /// connectionStrings参数
    /// </summary>
    [XmlArray("connectionStrings")]
    [XmlArrayItem("add")]
    public ConnectionStringSetting[] ConnectionStrings { get; set; }



    /// <summary>
    /// dbConfigs参数
    /// </summary>
    [XmlArray("dbConfigs")]
    [XmlArrayItem("add")]
    public XmlDbConfig[] DbConfigs { get; set; }




    internal void CorrectData()
    {
        if( this.AppSettings == null )
            this.AppSettings = new AppSetting[0];

        if( this.ConnectionStrings == null )
            this.ConnectionStrings = new ConnectionStringSetting[0];

        if( this.DbConfigs == null )
            this.DbConfigs = new XmlDbConfig[0];


        foreach( var x in this.ConnectionStrings ) {
            if( string.IsNullOrEmpty(x.ProviderName) )
                x.ProviderName = ClownFish.Data.DatabaseClients.SqlClient;
        }
    }


    /// <summary>
    /// 从文件中加载AppConfiguration实例
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    internal static AppConfiguration LoadFromFile(string filePath, bool checkExist = true)
    {
        if( filePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filePath));


        if( File.Exists(filePath) == false ) {
            if( checkExist )
                throw new FileNotFoundException("配置文件没有找到，filePath: " + filePath);
            else
                return null;
        }

        return XmlHelper.XmlDeserializeFromFile<AppConfiguration>(filePath);
    }


    /// <summary>
    /// 从XML文件中加载AppConfiguration实例
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    internal static AppConfiguration LoadFromXml(string xml)
    {
        if( xml.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(xml));

        return XmlHelper.XmlDeserialize<AppConfiguration>(xml);
    }

    /// <summary>
    /// 从System.Configuration.ConfigurationManager中加载AppConfiguration实例
    /// </summary>
    /// <returns></returns>
    internal static AppConfiguration LoadFromSysConfiguration()
    {
#if NETFRAMEWORK
        AppConfiguration config = new AppConfiguration();

        config.AppSettings = (from x in System.Configuration.ConfigurationManager.AppSettings.AllKeys
                              let s = new AppSetting { Key = x, Value = System.Configuration.ConfigurationManager.AppSettings[x] }
                              select s).ToArray();

        config.ConnectionStrings = (from x in System.Configuration.ConfigurationManager.ConnectionStrings.Cast<System.Configuration.ConnectionStringSettings>()
                                    let c = new ClownFish.Data.ConnectionStringSetting {
                                        Name = x.Name,
                                        ConnectionString = x.ConnectionString,
                                        ProviderName = x.ProviderName
                                    }
                                    select c
                                    ).ToArray();

        return config;
#else
        return null;
#endif
    }



}





