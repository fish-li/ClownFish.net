namespace ClownFish.Base.Config.Models;


/// <summary>
/// key/value 配置项
/// </summary>
public sealed class AppSetting
{
    /// <summary>
    /// key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// value
    /// </summary>
    public string Value { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"Key={Key}; Value={Value}";
    }
}


/// <summary>
/// 与 app.config 对应的实体类型，用于反序列读取配置文件。
/// </summary>
public sealed class AppConfiguration : ILoggingObject
{
    /// <summary>
    /// appSettings参数
    /// </summary>
    public AppSetting[] AppSettings { get; set; }


    /// <summary>
    /// connectionStrings参数
    /// </summary>
    public ConnectionStringSetting[] ConnectionStrings { get; set; }


    /// <summary>
    /// dbConfigs参数
    /// </summary>
    public DbConfig[] DbConfigs { get; set; }



    internal void CorrectData()
    {
        if( this.AppSettings == null )
            this.AppSettings = new AppSetting[0];

        if( this.ConnectionStrings == null )
            this.ConnectionStrings = new ConnectionStringSetting[0];

        if( this.DbConfigs == null )
            this.DbConfigs = new DbConfig[0];


        foreach( var x in this.ConnectionStrings ) {
            if( string.IsNullOrEmpty(x.ProviderName) )
                x.ProviderName = ClownFish.Data.DatabaseClients.SqlClient;
        }
    }

    /// <inheritdoc/>
    public string ToLoggingText()
    {
        return AppConfigIni.ToIni(this);
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

        if( filePath.EndsWithIgnoreCase(".ini") ) {
            return AppConfigIni.LoadFile(filePath);
        }

        if( EnvArgs0.IsAot == false ) {
            if( filePath.EndsWithIgnoreCase(".Appconfig") || filePath.EndsWithIgnoreCase(".App.config") ) {
                // app1.Appconfig   or  ClownFish.App.config 
                return AppConfigXml.LoadFile(filePath);
            }
        }

        throw new NotSupportedException("不支持的配置文件格式。filePath: " + filePath);
    }


    internal static AppConfiguration LoadFromString(string text, string textType)
    {
        if( textType.Is("ini") ) {
            return AppConfigIni.LoadText(text);
        }

        if( EnvArgs0.IsAot == false ) {
            if( textType.Is("xml") ) {
                return AppConfigXml.LoadXml(text);
            }
        }

        throw new NotSupportedException("不支持的配置文件格式。textType: " + textType);
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





