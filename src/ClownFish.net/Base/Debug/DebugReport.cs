using ClownFish.Data.CodeDom;
using ClownFish.Tasks;

namespace ClownFish.Base;

/// <summary>
/// 运行时诊断报告，由Venus提供页面支持
/// </summary>
public static class DebugReport
{
    /// <summary>
    /// 查看状态数据
    /// </summary>
    private static readonly List<Func<DebugReportBlock>> s_statusInfoCbList = new List<Func<DebugReportBlock>>(32);

    /// <summary>
    /// 查看系统信息
    /// </summary>
    private static readonly List<DebugReportBlock> s_sysInfoList = new List<DebugReportBlock>(5);

    /// <summary>
    /// 查看配置参数
    /// </summary>
    private static readonly List<DebugReportBlock> s_configList = new List<DebugReportBlock>(5);

    /// <summary>
    /// 查看参数变量
    /// </summary>
    private static readonly List<object> s_optionList = new(20);

    /// <summary>
    /// 查看程序集信息
    /// </summary>
    private static readonly List<DebugReportBlock> s_asmInfoList = new List<DebugReportBlock>(5);


    /// <summary>
    /// 注册一个包含“配置参数”定义的类型，用于在 “查看参数变量” 时展示其中的属性和字段
    /// </summary>
    /// <param name="optionType"></param>
    public static void RegisterOptionsType(Type optionType)
    {
        if( optionType != null ) {
            lock( s_optionList ) {
                s_optionList.Add(optionType);
            }
        }
    }

    /// <summary>
    /// 注册一个包含“配置参数”定义的对象实例，用于在 “查看参数变量” 时展示其中的属性和字段
    /// </summary>
    /// <param name="optionObject"></param>
    public static void RegisterOptionsObject(object optionObject)
    {
        if( optionObject != null ) {
            lock( s_optionList ) {
                s_optionList.Add(optionObject);
            }
        }
    }

    /// <summary>
    /// 注册回调委托，用于在 “查看参数变量” 时执行
    /// </summary>
    /// <param name="cb"></param>
    public static void RegisterOptionsCallback(Func<NameValue> cb)
    {
        if( cb != null ) {
            lock( s_optionList ) {
                s_optionList.Add(cb);
            }
        }
    }


    /// <summary>
    /// 注册回调委托，用于在 “查看状态数据” 时执行
    /// </summary>
    /// <param name="callback"></param>
    public static void RegisterStatusInfoCallback(Func<DebugReportBlock> callback)
    {
        if( callback != null ) {
            lock( s_statusInfoCbList ) {
                s_statusInfoCbList.Add(callback);
            }
        }
    }

    /// <summary>
    /// 注册一个报告片段，用于在 “查看系统信息” 时展示
    /// </summary>
    /// <param name="block"></param>
    public static void RegisterSysInfoBlock(DebugReportBlock block)
    {
        if( block != null ) {
            lock( s_sysInfoList ) {
                s_sysInfoList.Add(block);
            }
        }
    }

    /// <summary>
    /// 注册一个报告片段，用于在 “查看配置参数” 时展示
    /// </summary>
    /// <param name="block"></param>
    public static void RegisterConfigDataBlock(DebugReportBlock block)
    {
        if( block != null ) {
            lock( s_configList ) {
                s_configList.Add(block);
            }
        }
    }

    /// <summary>
    /// 注册一个报告片段，用于在 “查看程序集信息” 时展示
    /// </summary>
    /// <param name="block"></param>
    public static void RegisterAssemblyInfoBlock(DebugReportBlock block)
    {
        if( block != null ) {
            lock( s_asmInfoList ) {
                s_asmInfoList.Add(block);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static readonly string HeaderText = @$"
=============================================================
{EnvUtils.GetAppName()}/{EnvUtils.AppRuntimeId}
=============================================================
".TrimStart();

#if NET9_0_OR_GREATER
    private static readonly Lock s_lock = new Lock();
#else
    private static readonly object s_lock = new object();
#endif
    private static bool s_inited = false;


    /// <summary>
    /// Init
    /// </summary>
    public static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {

                    RegisterSysInfoBlock(DebugReportBlocks.GetSystemInfo());
                    RegisterSysInfoBlock(NHttpApplication.Instance.GetDebugReportBlock());

                    RegisterConfigDataBlock(DebugReportBlocks.GetEnvironmentVariables());
                    RegisterConfigDataBlock(MemoryConfig.GetDebugReportBlock());
                    RegisterConfigDataBlock(AppConfig.GetDebugReportBlock());
                    RegisterConfigDataBlock(LogConfig.GetDebugReportBlock());

                    RegisterAssemblyInfoBlock(ProxyLoader.EntityProxyAssemblyListReportBlock);
                    RegisterAssemblyInfoBlock(ProxyBuilder.CompileEntityListReportBlock);
                    RegisterAssemblyInfoBlock(DebugReportBlocks.GetEntityProxyLoaderList());
                    RegisterAssemblyInfoBlock(DebugReportBlocks.GetAssemblyListInfo());

                    RegisterOptionsType(typeof(LoggingOptions));
                    RegisterOptionsType(typeof(LoggingOptions.Http));
                    RegisterOptionsType(typeof(LoggingOptions.HttpClient));
                    RegisterOptionsType(typeof(LoggingLimit));
                    RegisterOptionsType(typeof(LoggingLimit.OprLog));
                    RegisterOptionsType(typeof(LoggingLimit.SQL));
                    RegisterOptionsType(typeof(HttpClientDefaults));
                    RegisterOptionsType(typeof(CacheOption));
                    RegisterOptionsType(typeof(ClownFishOptions));
                    RegisterOptionsType(typeof(ClownFishPubOptions));

#if NETCOREAPP
                    RegisterStatusInfoCallback(DebugReportBlocks.GetThreadPoolInfo);
                    RegisterStatusInfoCallback(DebugReportBlocks.GetGCInfo);
                    RegisterStatusInfoCallback(MemoryStreamPool.GetStatus);
#endif
                    RegisterStatusInfoCallback(DebugReportBlocks.GetLoggingCounters);
                    RegisterStatusInfoCallback(DebugReportBlocks.GetCacheStatus);

                    s_inited = true;
                }
            }
        }
    }


    /// <summary>
    /// 获取某个部分报告
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetReport(string name)
    {
        Init();

        return name switch {
            "ALL" => GetAllData().ToText(),
            "StatusInfo" => GetStatusInfo().ToText(),
            "SysInfo" => GetSysInfo().ToText(),
            "AsmInfo" => GetAsmInfo().ToText(),
            "ConfigInfo" => GetConfigInfo().ToText(),
            "StaticVariables" => GetStaticVariables().ToText(),
            _ => "_NULL_"
        };
    }



    internal static List<DebugReportBlock> GetStatusInfo()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(30);

        foreach( var cb in s_statusInfoCbList ) {
            DebugReportBlock block = cb.Invoke();
            if( block != null ) {
                blocks.Add(block);
            }
        }

        return blocks;
    }

    internal static List<DebugReportBlock> GetSysInfo()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(10);
        return blocks.AddRange2(s_sysInfoList);
    }

    internal static List<DebugReportBlock> GetConfigInfo()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(5);
        return blocks.AddRange2(s_configList);
    }

    internal static List<DebugReportBlock> GetStaticVariables()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(1);
        DebugReportBlock block = DebugReportBlocks.GetStaticVariablesReportBlock(s_optionList);
        blocks.Add(block);
        return blocks;
    }


    internal static List<DebugReportBlock> GetAsmInfo()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(5);
        return blocks.AddRange2(s_asmInfoList);
    }


    internal static List<DebugReportBlock> GetAllData()
    {
        return GetStatusInfo().AddRange2(s_sysInfoList).AddRange2(s_configList).AddRange2(s_asmInfoList);
    }

    internal static string ToText(this List<DebugReportBlock> blocks)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.AppendLineRN(HeaderText);

            foreach( var b in blocks.Where(x => x != null).OrderBy(x => x.Order) ) {
                b.GetText(sb);
                sb.AppendLine("\r\n");
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    /// <summary>
    /// Write all info to DebugReport.txt
    /// </summary>
    public static void WriteAllToFile()
    {
        if( LocalSettings.GetBool("ClownFish_CreateDebugReport_AtAppStartup") ) {

            // 获取所有的诊断信息，并写入到临时文件中
            string text = DebugReport.GetReport("ALL");
            string filePath = Path.Combine(EnvUtils.GetTempPath(), "DebugReport.txt");
            RetryFile.WriteAllText(filePath, text);
        }
    }
}
