using ClownFish.Data.CodeDom;
using ClownFish.Tasks;

namespace ClownFish.Base;

/// <summary>
/// 运行时诊断报告
/// </summary>
public static class DebugReport
{
    private static readonly object s_lock = new object();
    private static bool s_inited = false;
    
    /// <summary>
    /// 系统环境信息
    /// </summary>
    internal static readonly List<DebugReportBlock> SysInfoList = new List<DebugReportBlock>(5);

    /// <summary>
    /// 配置参数
    /// </summary>
    internal static readonly List<DebugReportBlock> ConfigList = new List<DebugReportBlock>(5);

    /// <summary>
    /// 类型相关信息
    /// </summary>
    internal static readonly List<DebugReportBlock> TypeInfoList = new List<DebugReportBlock>(5);


    /// <summary>
    /// 配置参数对象清单，允许：Type, object
    /// </summary>
    public static readonly List<object> OptionList = new(20);


    /// <summary>
    /// 
    /// </summary>
    public static readonly string HeaderText = @$"
=============================================================
{EnvUtils.GetAppName()}/{EnvUtils.AppRuntimeId}
=============================================================
".TrimStart();

    /// <summary>
    /// Init
    /// </summary>
    public static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {

                    SysInfoList.Add(DebugReportBlocks.GetSystemInfo());
                    SysInfoList.Add(NHttpApplication.Instance.GetDebugReportBlock());
                    SysInfoList.Add(DebugReportBlocks.GetNetworkInfo());

                    ConfigList.Add(DebugReportBlocks.GetEnvironmentVariables());
                    ConfigList.Add(MemoryConfig.GetDebugReportBlock());
                    ConfigList.Add(AppConfig.GetDebugReportBlock());
                    ConfigList.Add(LogConfig.GetDebugReportBlock());

                    TypeInfoList.Add(ProxyLoader.EntityProxyAssemblyListReportBlock);
                    TypeInfoList.Add(ProxyBuilder.CompileEntityListReportBlock);
                    TypeInfoList.Add(DebugReportBlocks.GetEntityProxyLoaderList());
                    TypeInfoList.Add(DebugReportBlocks.GetAssemblyListInfo());

                    OptionList.Add(typeof(LoggingOptions));
                    OptionList.Add(typeof(LoggingOptions.Http));
                    OptionList.Add(typeof(LoggingOptions.HttpClient));
                    OptionList.Add(typeof(LoggingLimit));
                    OptionList.Add(typeof(LoggingLimit.OprLog));
                    OptionList.Add(typeof(LoggingLimit.SQL));
                    OptionList.Add(typeof(HttpClientDefaults));
                    OptionList.Add(typeof(CacheOption));

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
#if NETCOREAPP
        blocks.Add(DebugReportBlocks.GetThreadPoolInfo());
        blocks.Add(DebugReportBlocks.GetGCInfo());
        blocks.Add(BackgroundTaskManager.GetReportBlock());
#endif

        blocks.Add(DebugReportBlocks.GetLoggingCounters());
        blocks.Add(DebugReportBlocks.GetCacheStatus());

        return blocks;
    }

    internal static List<DebugReportBlock> GetSysInfo()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(10);
        return blocks.AddRange2(SysInfoList);
    }

    internal static List<DebugReportBlock> GetConfigInfo()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(5);
        return blocks.AddRange2(ConfigList);
    }

    internal static List<DebugReportBlock> GetStaticVariables()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(1);
        DebugReportBlock block = DebugReportBlocks.GetStaticVariablesReportBlock();
        blocks.Add(block);
        return blocks;
    }


    internal static List<DebugReportBlock> GetAsmInfo()
    {
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(5);
        return blocks.AddRange2(TypeInfoList);
    }


    internal static List<DebugReportBlock> GetAllData()
    {
        return GetStatusInfo().AddRange2(SysInfoList).AddRange2(ConfigList).AddRange2(TypeInfoList);
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


}
