using ClownFish.Base.Config.Models;

namespace ClownFish.Base.Config;

internal static class AppConfigIni
{
    public static string ToIni(AppConfiguration config)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.AppendLineRN("[AppSettings]");
            foreach( var x in config.AppSettings ) {
                sb.AppendLineRN($"{x.Key}={x.Value}");
            }

            foreach( var x in config.ConnectionStrings ) {
                sb.AppendLineRN();
                sb.AppendLineRN($"[ConnectionString:{x.Name}]");
                sb.AppendLineRN($"ProviderName={x.ProviderName}");
                sb.AppendLineRN($"ConnectionString={x.ConnectionString}");
            }


            foreach( var x in config.DbConfigs ) {
                sb.AppendLineRN();
                sb.AppendLineRN($"[DbConfig:{x.Name}]");
                x.WriteToInit(sb);
            }
            sb.AppendLineRN();
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    public static AppConfiguration LoadFile(string filePath)
    {
        IniConfigData data = IniConfigFile.LoadFile(filePath, 200);
        return Load0(data);
    }


    public static AppConfiguration LoadText(string text)
    {
        IniConfigData data = IniConfigFile.LoadText(text);
        return Load0(data);
    }

    private static AppConfiguration Load0(IniConfigData data)
    {
        if( data == null )
            return new AppConfiguration();

        AppConfiguration config = new AppConfiguration();

        IniSection appsettings = data.GetSection("AppSettings");
        if( appsettings != null ) {
            config.AppSettings = (from kv in appsettings.Items
                                  select new AppSetting { Key = kv.Key, Value = kv.Value }
                                  ).ToArray();
        }

        config.ConnectionStrings = (from kv in data.Sections
                                    where kv.Key.StartsWith1("ConnectionString:")
                                    let css = IniToDbSetting(kv.Value)
                                    where css != null
                                    select css
                                    ).ToArray();

        config.DbConfigs = (from kv in data.Sections
                            where kv.Key.StartsWith1("DbConfig:")
                            let dbConfig = IniToDbConfig(kv.Value)
                            where dbConfig != null
                            select dbConfig
                            ).ToArray();

        return config;
    }

    private static ConnectionStringSetting IniToDbSetting(IniSection section)
    {
        ConnectionStringSetting css = new ConnectionStringSetting {
            Name = section.Name.Substring("ConnectionString:".Length),
            ConnectionString = section.GetValue("ConnectionString"),
            ProviderName = section.GetValue("ProviderName")
        };

        if( css.Name.IsNullOrEmpty() || css.ConnectionString.IsNullOrEmpty() )
            return null;

        return css;
    }

    private static DbConfig IniToDbConfig(IniSection section)
    {
        DbConfig dbConfig = section.GetObject<DbConfig>("");
        dbConfig.Name = section.Name.Substring("DbConfig:".Length);

        return dbConfig;
    }


}
