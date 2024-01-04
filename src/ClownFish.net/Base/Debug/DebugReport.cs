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
    private static readonly List<DebugReportBlock> s_sysInfoList = new List<DebugReportBlock>(30);

    /// <summary>
    /// 类型相关信息，这部分数据可能会比较大！
    /// </summary>
    private static readonly List<DebugReportBlock> s_typeInfoList = new List<DebugReportBlock>(10);

    /// <summary>
    /// 
    /// </summary>
    public static readonly string HeaderText = @$"
=============================================================
{EnvUtils.GetAppName()}/{EnvUtils.AppRuntimeId}
=============================================================
".TrimStart();

    private static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {

                    AddSysInfoBlock(DebugReportBlocks.GetSystemInfo());
                    AddSysInfoBlock(AppConfig.GetDebugReportBlock());
                    AddSysInfoBlock(LogConfig.GetDebugReportBlock());
                    AddSysInfoBlock(NHttpApplication.Instance.GetDebugReportBlock());

                    AddSysInfoBlock(DebugReportBlocks.GetEnvironmentVariables());
                    AddSysInfoBlock(DebugReportBlocks.GetNetworkInfo());
                    AddSysInfoBlock(DebugReportBlocks.GetSomeOptionsInfo());
                    

                    AddTypeInfoBlock(ProxyLoader.EntityProxyAssemblyListReportBlock);
                    AddTypeInfoBlock(ProxyBuilder.CompileEntityListReportBlock);                                                
                    AddTypeInfoBlock(DebugReportBlocks.GetEntityProxyLoaderList());
                    AddTypeInfoBlock(DebugReportBlocks.GetAssemblyListInfo());

                    s_inited = true;
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="block"></param>
    public static void AddSysInfoBlock(DebugReportBlock block)
    {
        if( block == null )
            return;

        lock( s_sysInfoList ) {
            s_sysInfoList.Add(block);
        }
    }


    private static void AddTypeInfoBlock(DebugReportBlock block)
    {
        if( block == null )
            return;

        s_typeInfoList.Add(block);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<DebugReportBlock> GetStatusInfo()
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<DebugReportBlock> GetSysInfo()
    {
        Init();
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(30);
        return blocks.AddRange2(s_sysInfoList);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<DebugReportBlock> GetAsmInfo()
    {
        Init();
        List<DebugReportBlock> blocks = new List<DebugReportBlock>(30);
        return blocks.AddRange2(s_typeInfoList);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<DebugReportBlock> GetAllData()
    {
        Init();
        return GetStatusInfo().AddRange2(s_sysInfoList).AddRange2(s_typeInfoList);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blocks"></param>
    /// <returns></returns>
    public static string ToText(this List<DebugReportBlock> blocks)
    {
         StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.AppendLineRN(HeaderText);

            foreach( var b in blocks ) {
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
