using ClownFish.Base.Config.Models;

namespace ClownFish.Base;

internal class AppConfigObject
{
    private readonly AppConfiguration _config;

    internal AppConfiguration GetConfiguration() => _config;

    private readonly Dictionary<string, string> _settings;
    private readonly Dictionary<string, ConnectionStringSetting> _conns;
    private readonly Dictionary<string, DbConfig> _dbConfigs;

    public AppConfigObject(AppConfiguration config)
    {
        if( config == null )
            throw new ArgumentNullException(nameof(config));

        config.CorrectData();
        _config = config;


        // 构造字典，用于快速查找

        _settings = new Dictionary<string, string>(config.AppSettings.Length, StringComparer.OrdinalIgnoreCase);
        foreach( var x in config.AppSettings ) {
            if( x.Key.HasValue() ) {
                _settings[x.Key] = x.Value ?? string.Empty;
            }
        }

        _conns = new Dictionary<string, ConnectionStringSetting>(config.ConnectionStrings.Length, StringComparer.OrdinalIgnoreCase);
        foreach( var x in config.ConnectionStrings ) {
            if( x.Name.HasValue() && x.ConnectionString.HasValue() ) {
                _conns[x.Name] = x;
            }
        }

        _dbConfigs = new Dictionary<string, DbConfig>(config.DbConfigs.Length, StringComparer.OrdinalIgnoreCase);
        foreach( var x in config.DbConfigs ) {
            if( x.Name.HasValue() && x.Server.HasValue() ) {
                _dbConfigs[x.Name] = x;
            }
        }
    }


    public string GetSetting(string name)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException(nameof(name));

        return _settings.TryGet(name);
    }


    public ConnectionStringSetting GetConnectionString(string name)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException(nameof(name));

        return _conns.TryGet(name)?.Clone();
    }


    public DbConfig GetDbConfig(string name)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException(nameof(name));

        return _dbConfigs.TryGet(name)?.Clone();
    }


    internal void AddDbConfig(string name, DbConfig dbConfig)
    {
        _dbConfigs[name] = dbConfig;
    }

    internal void AddConnectionString(string name, ConnectionStringSetting connConf)
    {
        _conns[name] = connConf;
    }
}
