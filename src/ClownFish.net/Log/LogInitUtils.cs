namespace ClownFish.Log;

internal static class LogInitUtils
{
    /// <summary>
    /// 按照默认方式初始化日志组件
    /// </summary>
    /// <param name="baseConfig">一个默认的配置，在合并时，它做为基础来源</param>
    /// <param name="addConfig">新增配置，合并时，它的参数将会覆盖baseConfig</param>
    public static LogConfiguration InitLogAsDefault(LogConfiguration baseConfig = null, LogConfiguration addConfig = null)
    {
        // 从程序集中加载默认的配置
        LogConfiguration config1 = baseConfig ?? LoadFromClownFishAssembly();

        // 加载应用程序中定制的配置文件
        LogConfiguration config2 = addConfig ?? LoadFile(ConfigFile.LogConfigFileName) ?? LoadFile("ClownFish.logconfig.ini") 
                                             ??  LoadFile(ConfigFile.AppConfigFileName);

        // 合并配置项，同名参数节点用config2覆盖config1
        LogConfiguration config3 = LogConfiguration.MegerConfig(config1, config2);


        // 初始化日志组件
        return InitLog(config3);
    }

    internal static LogConfiguration LoadFromClownFishAssembly()
    {
        // 从程序集中加载默认配置文件
        string ini = typeof(LogHelper).Assembly.ReadResAsText("ClownFish.ClownFish.logconfig.ini");
        return LogConfig.LoadFromIni(ini);
    }

    internal static LogConfiguration LoadFile(string filename)
    {
        string fileBody = ConfigFile.GetFile(filename);
        if( fileBody.IsNullOrEmpty() ) {
            return null;
        }

        return LogConfig.LoadFromIni(fileBody);
    }


    /// <summary>
    /// 初始化 ClownFish.Log
    /// </summary>
    /// <param name="filePath">ClownFish.Logconfig的完整路径</param>
    public static void InitLog(string filePath)
    {
        if( filePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filePath));

        if( LogConfig.IsInited )
            return;

        LogConfiguration config = LogConfig.LoadFromFile(filePath, true);
        InitLog(config);
    }


    /// <summary>
    /// 初始化 ClownFish.Log
    /// </summary>
    /// <param name="config"></param>
    public static LogConfiguration InitLog(LogConfiguration config)
    {
        if( config == null )
            throw new ArgumentNullException(nameof(config));


        // 允许重新指定写入器类型，例如：开发时写到XML文件，生产环境部署时统一写到ES
        string logWriterNames = Settings.GetSetting("ClownFish_Log_WritersMap");
        if( logWriterNames.HasValue() ) {
            Console2.Info("ClownFish_Log_WritersMap: " + logWriterNames);
            config.OverrideWriters(logWriterNames);
        }

        // 尝试本地参数中更新日志配置
        config.TryUpdateFromLocalSetting();

        ShowLogConfig(config);

        if( LogConfig.IsInited == false ) {
            LogConfig.Init(config);
        }

        return config;   // 返回值方便单元测试
    }

    internal static void ShowLogConfig(LogConfiguration config)
    {
        if( LocalSettings.GetBool("Show_ClownFish_LogConfig") ) {
            // 由于 Log_Config 的内容会做【合并】，所以这里显示【最终生效】的配置对象
            string configText = config.ToLoggingText();
            Console2.WriteLine("----------------------- ClownFish.Logconfig ----------------------------");
            Console2.WriteLine(configText);
            Console2.WriteLine("-------------------------------------------------------------------------");
        }
    }


}
