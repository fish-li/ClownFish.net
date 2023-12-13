using System.Runtime.InteropServices;

namespace ClownFish.Base;


internal static class DebugReportBlocks
{
    /// <summary>
    ///  
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static DebugReportBlock GetSomeOptionsInfo()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Some Options" };

        block.AppendLine("CacheOption.AppCacheSeconds: " + CacheOption.AppCacheSeconds.ToString());
        block.AppendLine("CacheOption.ExpirationScanFrequency: " + CacheOption.ExpirationScanFrequency.ToString());
        block.AppendLine("--------------------------------------------");

        block.AppendLine("HttpClientDefaults.HttpClientTimeout: " + HttpClientDefaults.HttpClientTimeout.ToString());        
        block.AppendLine("HttpClientDefaults.HttpClientCacheSeconds: " + HttpClientDefaults.HttpClientCacheSeconds.ToString());
        block.AppendLine("HttpClientDefaults.HttpProxyTimeout: " + HttpClientDefaults.HttpProxyTimeout.ToString());
        block.AppendLine("HttpClientDefaults.RabbitHttpClientTimeout: " + HttpClientDefaults.RabbitHttpClientTimeout.ToString());
        block.AppendLine("HttpClientDefaults.EsHttpClientTimeout: " + HttpClientDefaults.EsHttpClientTimeout.ToString());
        block.AppendLine("HttpClientDefaults.HttpJsonWriterTimeout: " + HttpClientDefaults.HttpJsonWriterTimeout.ToString());

        return block;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static DebugReportBlock GetCacheStatus()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Cache Status" };

        block.AppendLine("AppCache.Count: " + AppCache.GetCount().ToString());

#if NETCOREAPP
        block.AppendLine("MsHttpClientCache.Count: " + ClownFish.WebClient.V2.MsHttpClientCache.GetCount().ToString());
#endif

        return block;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static DebugReportBlock GetSystemInfo()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "System Information", Order = 100 };

        //block.AppendLine("CLR Version: " + Environment.Version);                                // 6.0.5
        block.AppendLine("FrameworkDescription: " + RuntimeInformation.FrameworkDescription);   // .NET 6.0.5
        //block.AppendLine("CLR SystemVersion: " + RuntimeEnvironment.GetSystemVersion());      // v4.0.30319

        //block.AppendLine("OS Name: " + SystemHelper.GetOsName());                 // Linux
        //block.AppendLine("OSVersion: " + Environment.OSVersion);                // Unix 3.10.0.957
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static DebugReportBlock GetNetworkInfo()
    {
        // 如果是在容器中部署，网络信息就没什么参考价值了
        if( EnvUtils.IsInDocker )
            return null;

        DebugReportBlock block = new DebugReportBlock { Category = "Network Information", Order = 100 };

        try {
            var x = SystemHelper.GetCurrentNetworkInfo();
            block.AppendLine($"MAC Address: " + x.GetMac());
            block.AppendLine($"IP Address: " + x.GetIPv4());
        }
        catch( Exception ex ) {
            block.AppendLine(ex.Message);
        }

        return block;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
}
