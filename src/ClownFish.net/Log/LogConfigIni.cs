using ClownFish.Base.Config;

namespace ClownFish.Log;

internal static class LogConfigIni
{
    public static string ToIni(LogConfiguration config)
    {
        StringBuilder sb = StringBuilderPool.Get();

        try {
            sb.AppendLineRN("[Logging]");
            sb.AppendLineRN($"Enable={config.Enable.ToString2()}");
            sb.AppendLineRN($"TimerPeriod={config.TimerPeriod}");
            if( config.Performance != null ) {
                config.Performance.WriteToInit(sb);
            }
            if( config.File != null ) {
                config.File.WriteToInit(sb);
            }
            sb.AppendLineRN();

            sb.AppendLineRN("[Logging.Writers]");
            if( config.Writers.HasValue() ) {
                foreach( var w in config.Writers ) {
                    sb.AppendLineRN(w.ToString());
                }
            }
            sb.AppendLineRN();

            sb.AppendLineRN("[Logging.DataTypes]");
            if( config.Types.HasValue() ) {
                foreach( var t in config.Types ) {
                    sb.AppendLineRN(t.ToString());
                }
            }
            sb.AppendLineRN();
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    public static LogConfiguration LoadIni(string ini)
    {
        if( ini.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(ini));

        IniConfigData data = IniConfigFile.LoadText(ini);

        IniSection logging = data.GetSection("Logging");
        IniSection writers = data.GetSection("Logging.Writers");
        IniSection dataTypes = data.GetSection("Logging.DataTypes");

        LogConfiguration config = new LogConfiguration();

        if( logging == null && writers == null && dataTypes == null )
            return config;


        if( logging != null ) {
            config.Enable = logging.GetValue("Enable", "1").TryToBool();
            config.TimerPeriod = logging.GetValue("TimerPeriod", "100").TryToInt();
            config.Performance = logging.GetObject<PerformanceConfig>("Performance");
            config.File = logging.GetObject<FileConfig>("File");
        }

        if( writers != null ) {
            config.Writers = (from kv in writers.Items
                              select new WriterConfig { Name = kv.Key, Type = kv.Value }
                              ).ToArray();
        }

        if( dataTypes != null ) {
            config.Types = (from kv in dataTypes.Items
                            select new TypeItemConfig { DataType = kv.Key, Writers = kv.Value }
                            ).ToArray();
        }
        return config;
    }

}