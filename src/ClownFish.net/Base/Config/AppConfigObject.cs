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

        //========================================================================
        // 说明：由于历史原因，早期的Nebula采用的配置参数名称的风格是：x.y.z
        // 后来感觉不方便：1，不能双击全选，2，“不符合”环境变量的约定命名风格，
        // 所以后面采用新的命名风格：x_y_z
        // 但是为了向后兼容，配置API要支持：用 x_y_z 的风格去读取 x.y.z 的配置参数
        // 因此，在API实现时，会在内存中保存2种风格的配置参数对象
        //========================================================================

        _settings = new Dictionary<string, string>(config.AppSettings.Length, StringComparer.OrdinalIgnoreCase);
        foreach( var x in config.AppSettings ) {
            if( x.Key.HasValue() ) {
                _settings[x.Key] = x.Value ?? string.Empty;

                // 增加兼容KEY查找项
                string name2 = x.Key.GetConfName();
                if( x.Key != name2 )
                    _settings[name2] = x.Value ?? string.Empty;
            }
        }

        _conns = new Dictionary<string, ConnectionStringSetting>(config.ConnectionStrings.Length, StringComparer.OrdinalIgnoreCase);
        foreach( var x in config.ConnectionStrings ) {
            if( x.Name.HasValue() && x.ConnectionString.HasValue() ) {
                _conns[x.Name] = x;

                // 增加兼容KEY查找项
                string name2 = x.Name.GetConfName();
                if( x.Name != name2 )
                    _conns[name2] = x;
            }
        }

        _dbConfigs = new Dictionary<string, DbConfig>(config.DbConfigs.Length, StringComparer.OrdinalIgnoreCase);
        foreach( var x in config.DbConfigs ) {
            if( x.Name.HasValue() && x.Server.HasValue() ) {
                DbConfig dbConfig = x.ToDbConfig();
                _dbConfigs[x.Name] = dbConfig;

                // 增加兼容KEY查找项
                string name2 = x.Name.GetConfName();
                if( x.Name != name2 )
                    _dbConfigs[name2] = dbConfig;
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


}
