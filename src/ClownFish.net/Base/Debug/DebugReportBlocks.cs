using System.Runtime.InteropServices;
using ClownFish.Base.Config.Models;

namespace ClownFish.Base;


internal static class DebugReportBlocks
{
    public static DebugReportBlock GetLoggingCounters()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Logging Counters" };

        block.AppendLine("MaxCacheQueueLength: " + ClownFish.Log.LoggingOptions.MaxCacheQueueLength.ToString());

        block.AppendLine("WriteCount: " + ClownFishCounters.Logging.WriteCount.Get().ToWString());
        block.AppendLine("InQueueCount: " + ClownFishCounters.Logging.InQueueCount.Get().ToWString());
        block.AppendLine("GiveupCount: " + ClownFishCounters.Logging.GiveupCount.Get().ToWString());

        block.AppendLine("QueueFlushCount: " + ClownFishCounters.Logging.QueueFlushCount.Get().ToWString());
        block.AppendLine("WriterErrorCount: " + ClownFishCounters.Logging.WriterErrorCount.Get().ToWString());
        block.AppendLine("FatalErrorCount: " + ClownFishCounters.Logging.FatalErrorCount.Get().ToWString());

        block.AppendLine("XmlWriteCount: " + ClownFishCounters.Logging.XmlWriteCount.Get().ToWString());
        block.AppendLine("JsonWriteCount: " + ClownFishCounters.Logging.JsonWriteCount.Get().ToWString());
        block.AppendLine("Json2WriteCount: " + ClownFishCounters.Logging.Json2WriteCount.Get().ToWString());
        block.AppendLine("EsWriteCount: " + ClownFishCounters.Logging.EsWriteCount.Get().ToWString());
        block.AppendLine("Rabbit2WriteCount: " + ClownFishCounters.Logging.Rabbit2WriteCount.Get().ToWString());
        block.AppendLine("RabbitWriteCount: " + ClownFishCounters.Logging.RabbitWriteCount.Get().ToWString());
        block.AppendLine("KafkaWriteCount: " + ClownFishCounters.Logging.KafkaWriteCount.Get().ToWString());

        return block;
    }


    public static DebugReportBlock GetCacheStatus()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Cache Status" };

        block.AppendLine("AppCache.Count: " + AppCache.GetCount().ToString());

#if NETCOREAPP
        block.AppendLine("MsHttpClientCache.Count: " + ClownFish.WebClient.V2.MsHttpClientCache.GetCount().ToString());
#endif

        return block;
    }


    public static DebugReportBlock GetSystemInfo()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "System Information", Order = 100 };

        //block.AppendLine("Runtime Version: " + Environment.Version);                                // 6.0.5
        block.AppendLine("FrameworkDescription: " + RuntimeInformation.FrameworkDescription);   // .NET 6.0.5
        //block.AppendLine("CLR SystemVersion: " + RuntimeEnvironment.GetSystemVersion());      // v4.0.30319

        block.AppendLine("OS Name: " + OsUtils.GetOsName());                      // Ubuntu 22.04.3 LTS
        block.AppendLine("OSDescription: " + RuntimeInformation.OSDescription);   // Linux 3.10.0-957.el7.x86_64 #1 SMP Thu Nov 8 23:39:32 UTC 2018            

        block.AppendLine("RuntimeDirectory: " + RuntimeEnvironment.GetRuntimeDirectory());     // /usr/share/dotnet/shared/Microsoft.NETCore.App/6.0.5/
        block.AppendLine("SystemTempPath: " + Path.GetTempPath());                    //  /tmp/
        block.AppendLine("SystemDirectory: " + Environment.SystemDirectory);          // 没有内容
        block.AppendLine("CommandLine: " + Environment.CommandLine);                  //  /app/Nebula.Moon.dll
        block.AppendLine("EntryAssembly: " + AsmHelper.GetEntryAssembly().Location);  //  /app/Nebula.Moon.dll
        block.AppendLine("CurrentDirectory: " + Environment.CurrentDirectory);        // /app
        block.AppendLine("AppDomain.BaseDirectory: " + AppContext.BaseDirectory);     // /app

        block.AppendLine("Is64BitOperatingSystem: " + Environment.Is64BitOperatingSystem);
        block.AppendLine("Is64BitProcess: " + Environment.Is64BitProcess);
        block.AppendLine("OSArchitecture: " + RuntimeInformation.OSArchitecture);             // X64
        block.AppendLine("ProcessArchitecture: " + RuntimeInformation.ProcessArchitecture);   // X64

        block.AppendLine("MachineName: " + Environment.MachineName);
        block.AppendLine("ProcessorCount: " + Environment.ProcessorCount);
        block.AppendLine("UserDomainName: " + Environment.UserDomainName);
        block.AppendLine("UserName: " + Environment.UserName);

        return block;
    }

#if NETCOREAPP

    public static DebugReportBlock GetThreadPoolInfo()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "ThreadPool Information" };

        ThreadPool.GetMinThreads(out int minWorker, out int minIOCP);
        ThreadPool.GetMaxThreads(out int maxWorker, out int maxIOCP);
        ThreadPool.GetAvailableThreads(out int availableWorker, out int availableIOCP);

        block.AppendLine("Min Worker Threads: " + minWorker.ToString());
        block.AppendLine("Max Worker Threads: " + maxWorker.ToString());
        block.AppendLine("Available Worker Threads: " + availableWorker.ToString());
        block.AppendLine("--------------------------------------------");

        block.AppendLine("Min CompletionPort Threads: " + minIOCP.ToString());
        block.AppendLine("Max CompletionPort Threads: " + maxIOCP.ToString());
        block.AppendLine("Available CompletionPort Threads: " + availableIOCP.ToString());

        block.AppendLine("--------------------------------------------");
        block.AppendLine("ThreadPool.ThreadCount: " + ThreadPool.ThreadCount.ToString());
        block.AppendLine("ThreadPool.PendingWorkItemCount: " + ThreadPool.PendingWorkItemCount.ToString());
        block.AppendLine("Environment.ProcessorCount: " + Environment.ProcessorCount.ToString());

        return block;
    }


    public static DebugReportBlock GetGCInfo()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "GC Information" };

        GCMemoryInfo info = GC.GetGCMemoryInfo();
        block.AppendLine($"TotalMemory（当前认为要分配的字节数）: " + GC.GetTotalMemory(false).ToKString());
        block.AppendLine($"FragmentedBytes（上次垃圾收集发生时的总碎片）: " + info.FragmentedBytes.ToKString());
        block.AppendLine($"HeapSizeBytes（上次垃圾收集发生时的堆总大小）: " + info.HeapSizeBytes.ToKString());
        block.AppendLine($"MemoryLoadBytes（上次垃圾收集发生时的内存负载）: " + info.MemoryLoadBytes.ToKString());
        block.AppendLine($"HighMemoryLoadThresholdBytes（上次垃圾收集发生时的高内存负载阈值）: " + info.HighMemoryLoadThresholdBytes.ToKString());
        block.AppendLine($"TotalAvailableMemoryBytes（上次垃圾收集发生时垃圾收集器要使用的可用内存总数）: " + info.TotalAvailableMemoryBytes.ToKString());

        // 获取进程的内存占用，目前有3个方法：MemoryLoadBytes, WorkingSet, docker stats
        // 以下是实际的数据（来自一个测试程序）
        // MemoryLoadBytes: 303,625,666
        // WorkingSet:      314,122,240
        // docker stats:    241.9MiB
        // 很显然，用.NET自身的方式获取的结果都偏大！！

        //block.AppendLine("--------------------------------------------");
        //block.AppendLine("Environment.WorkingSet : " + Environment.WorkingSet.ToKString());

        return block;
    }
#endif


    public static DebugReportBlock GetEnvironmentVariables()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Environment Variables", Order = 100 };

        (from x in EnvironmentVariables.GetAll()
         let line = GetEnvironmentVariableLine(x.Key, x.Value)
         orderby x.Key
         select line
         ).ToList().ForEach(x => block.AppendLine(x));

        return block;
    }

    private static readonly HashSet<string> s_hideEnvNames = LocalSettings.GetSetting("DebugReport_HideEnvNames").SplitToHashSet();

    private static string GetEnvironmentVariableLine(string key, string value)
    {
        if( key.EndsWithIgnoreCase("ConnectionString") )
            return $"{key}: {ConnectionStringUtils.HidePwd(value)}";
        else if( key.EndsWithIgnoreCase("Password") )
            return $"{key}: ********";
        else if( s_hideEnvNames.Contains(key) )
            return $"{key}: ********";
        else
            return $"{key}: {value}";
    }



    public static DebugReportBlock GetEntityProxyLoaderList()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Entity/Loader List", Order = 1003 };

        List<Type> types = EntityProxyFactory.GetEntityTypes();

        int i = 1;
        foreach( Type t in types.OrderBy(x => x.FullName) ) {
            block.AppendLine($"{i++,4}: {t.FullName}");
            //block.AppendLine("Type: " + t.FullName);
            //block.AppendLine("    Proxy: " + EntityProxyFactory.GetProxy(t)?.FullName);
            //block.AppendLine("    Loader: " + DataLoaderFactory.GetLoaderType(t)?.FullName);
        }
        return block;
    }


    public static DebugReportBlock GetAssemblyListInfo()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Load Assembly List", Order = 1004 };

        int i = 1;
        (from asm in AsmHelper.GetLoadAssemblies()
         let path = asm.Location
         let asmVersion = AssemblyName.GetAssemblyName(path).Version
         let fileVersion = FileVersionInfo.GetVersionInfo(path).FileVersion
         let line = $"{path}; {asmVersion}; {fileVersion}"
         orderby path
         select line
         ).ToList().ForEach(x => block.AppendLine($"{i++,4}: {x}"));

        return block;
    }


    public static DebugReportBlock GetDebugReportBlock(this NHttpApplication httpApplication)
    {
        if( httpApplication == null )
            return null;

        DebugReportBlock block = new DebugReportBlock { Category = nameof(NHttpApplication), Order = 100 };
        block.AppendLine($"HttpModules:");

        int i = 1;
        foreach( var x in httpApplication.GetModules() ) {
            block.AppendLine($"{i++,3}: {x.GetType().FullName}, order: {x.Order}");
        }

        return block;
    }

    internal static DebugReportBlock GetDebugReportBlock(this AppConfiguration appconfig)
    {
        DebugReportBlock block = new DebugReportBlock { Category = AppConfig.ClownFishAppconfig, Order = 100 };

        // 按照开发要求，app.config 中是不允许有敏感信息参数的，所以这里不做过滤，直接用XML展示
        block.AppendLine(appconfig.ToXml2());
        return block;
    }

    internal static DebugReportBlock GetDebugReportBlock(this LogConfiguration logconfig)
    {
        DebugReportBlock block = new DebugReportBlock { Category = LogConfig.ConfigFileName, Order = 100 };

        block.AppendLine(logconfig.ToXml2());
        return block;
    }


    internal static void AddFieldValues1(DebugReportBlock block, Type optType)
    {
        PropertyInfo[] ps = optType.GetProperties(BindingFlags.Static | BindingFlags.Public);
        FieldInfo[] fs = optType.GetFields(BindingFlags.Static | BindingFlags.Public);

        foreach( PropertyInfo p in ps ) {
            object value = p.GetValue(null, null);
            block.AppendLine($"{p.Name} = {value.ToString2()}");
        }

        foreach( FieldInfo f in fs ) {
            object value = f.GetValue(null);
            block.AppendLine($"{f.Name} = {value.ToString2()}");
        }
    }

    internal static void AddFieldValues2(DebugReportBlock block, object opt)
    {
        Type optType = opt.GetType();

        PropertyInfo[] ps = optType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        FieldInfo[] fs = optType.GetFields(BindingFlags.Instance | BindingFlags.Public);

        foreach( PropertyInfo p in ps ) {
            object value = p.GetValue(opt, null);
            block.AppendLine($"{p.Name} = {value.ToString2()}");
        }

        foreach( FieldInfo f in fs ) {
            object value = f.GetValue(opt);
            block.AppendLine($"{f.Name} = {value.ToString2()}");
        }
    }


    public static DebugReportBlock GetStaticVariablesReportBlock()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Runtime Static Variables" };

        foreach( var x in DebugReport.OptionList.Where(a=>a != null) ) {
            if( x is Type type ) {
                block.AppendLine($"------------------{type.FullName}--------------------------");
                AddFieldValues1(block, type);
            }
            else if( x is Func<NameValue> func ) {
                NameValue nv = func.Invoke();
                block.AppendLine($"------------------{nv.Name}--------------------------");
                block.AppendLine(nv.Value);
            }
            else {
                block.AppendLine($"------------------{x.GetType().FullName}--------------------------");
                AddFieldValues2(block, x);
            }
            block.AppendLine(" ");
        }
        return block;
    }

}
