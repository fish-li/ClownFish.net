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
        //block.AppendLine("CLR SystemVersion: " + RuntimeEnvironment.GetSystemVersion());      // v4.0.30319

        block.AppendLine("OS Name: " + OsUtils.GetOsName());                      // Ubuntu 22.04.3 LTS
#if NET48_OR_GREATER || NET6_0_OR_GREATER
        // 说明，从 .net8 开始 RuntimeInformation.OSDescription 的取值在 Linux 实现中发生了改变，最后的结果和 OsUtils.GetOsName() 差不多
        block.AppendLine("OSDescription: " + RuntimeInformation.OSDescription);   // Linux 3.10.0-957.el7.x86_64 #1 SMP Thu Nov 8 23:39:32 UTC 2018
        block.AppendLine("FrameworkDescription: " + RuntimeInformation.FrameworkDescription);   // .NET 6.0.5
        block.AppendLine("OSArchitecture: " + RuntimeInformation.OSArchitecture);             // X64
        block.AppendLine("ProcessArchitecture: " + RuntimeInformation.ProcessArchitecture);   // X64
#endif
        block.AppendLine("RuntimeDirectory: " + RuntimeEnvironment.GetRuntimeDirectory());     // /usr/share/dotnet/shared/Microsoft.NETCore.App/6.0.5/
        block.AppendLine("SystemTempPath: " + Path.GetTempPath());                    //  /tmp/
        block.AppendLine("SystemDirectory: " + Environment.SystemDirectory);          // 没有内容
        block.AppendLine("CommandLine: " + Environment.CommandLine);                  //  /app/Nebula.Moon.dll
        block.AppendLine("EntryAssembly: " + AsmHelper.GetExeFilePath());             //  /app/Nebula.Moon.dll
        block.AppendLine("CurrentDirectory: " + Environment.CurrentDirectory);        // /app
        block.AppendLine("BaseDirectory: " + AppContext.BaseDirectory);     // /app

        block.AppendLine("Is64BitOperatingSystem: " + Environment.Is64BitOperatingSystem);
        block.AppendLine("Is64BitProcess: " + Environment.Is64BitProcess);
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
        DebugReportBlock block = new DebugReportBlock { Category = "GC Memory Information" };

        GCMemoryInfo info = GC.GetGCMemoryInfo();
        block.AppendLine($"TotalMemory: " + GC.GetTotalMemory(false).ToKString());
        block.AppendLine($"FragmentedBytes: " + info.FragmentedBytes.ToKString());
        block.AppendLine($"HeapSizeBytes: " + info.HeapSizeBytes.ToKString());
        block.AppendLine($"MemoryLoadBytes: " + info.MemoryLoadBytes.ToKString());
        block.AppendLine($"HighMemoryLoadThresholdBytes: " + info.HighMemoryLoadThresholdBytes.ToKString());
        block.AppendLine($"TotalAvailableMemoryBytes: " + info.TotalAvailableMemoryBytes.ToKString());

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


    public static DebugReportBlock GetEnvironmentVariables(bool autoFormat = true)
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Environment Variables", Order = 100 };

        int formatWidth = 0;

        if( autoFormat ) {
            foreach( var kv in EnvironmentVariables.GetAll() ) {
                if( kv.Key.Length > formatWidth )
                    formatWidth = kv.Key.Length;
            }

            formatWidth += 2;  // 多加2个空格
            formatWidth = 0 - formatWidth;   // 左对齐
        }

        (from x in EnvironmentVariables.GetAll()
         let line = SecurityLogUtils.GetEnvironmentVariableLine(x.Key, x.Value, formatWidth)
         orderby x.Key
         select line
         ).ToList().ForEach(x => block.AppendLine(x));

        return block;
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

    [UnconditionalSuppressMessage("SingleFile", "IL3000: Assembly.Location always returns an empty string for assemblies embedded in a single-file app")]
    public static DebugReportBlock GetAssemblyListInfo()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Load Assembly List", Order = 1004 };

        if( AsmHelper.IsSingleFileDeploy == false ) {
            try {
                int i = 1;
                (from asm in AsmHelper.GetLoadAssemblies()
                 let path = asm.Location
                 let asmVersion = asm.GetName().Version
                 let fileVersion = FileHelper.GetFileVersion(path)
                 let line = $"{path}; {asmVersion}; {fileVersion}"
                 orderby path
                 select line
                 ).ToList().ForEach(x => block.AppendLine($"{i++,4}: {x}"));
            }
            catch( Exception ex ) {
                // 曾经偶然出现过异常： 这个异常非常扯淡，因为 exe 启动时调用这段代码，居然说 “自身” 所在文件找不到 ！！！
                // System.IO.FileNotFoundException: 未能加载文件或程序集“TxClientW.exe”或它的某一个依赖项。系统找不到指定的文件。
                // 文件名:“TxClientW.exe” ---> System.IO.FileNotFoundException: 系统找不到指定的文件。 (异常来自 HRESULT:0x80070002)
                // 在 System.Reflection.AssemblyName.nGetFileInformation(String s)
                // 在 System.Reflection.AssemblyName.GetAssemblyName(String assemblyFile)
                // .......................
                // 当时的写法：let asmVersion = AssemblyName.GetAssemblyName(path).Version
                block.AppendLine(ex.ToString());
            }
        }

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
        DebugReportBlock block = new DebugReportBlock { Category = "ClownFish.AppConfig", Order = 100 };

        // 按照开发要求，app.config 中是不允许有敏感信息参数的，所以这里不做过滤，直接用XML展示
        block.AppendLine(appconfig.ToLoggingText());
        return block;
    }


    internal static DebugReportBlock GetDebugReportBlock(this LogConfiguration logconfig)
    {
        DebugReportBlock block = new DebugReportBlock { Category = "ClownFish.LogConfig", Order = 100 };

        block.AppendLine(logconfig.ToLoggingText());
        return block;
    }



    internal static DebugReportBlock GetStaticVariablesReportBlock(List<object> optionList)
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Runtime Static Variables" };
        block.AppendLine(" ");

        Dictionary<string, object> dict = new Dictionary<string, object>(optionList.Count);

        // 先获取各“小块”的标题
        foreach( var x in optionList.Where(a => a != null) ) {
            if( x is Type type ) {
                dict[type.FullName] = x;  // type
            }
            else if( x is Func<NameValue> func ) {
                NameValue nv = func.Invoke();
                dict[nv.Name] = nv;
            }
            else {
                dict[x.GetType().FullName] = x;  // instance
            }
        }

        // 按标题排序输出
        foreach( var x in dict.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase).Select(x => x.Value) ) {
            if( x is Type type ) {
                block.AppendLine($"------------------{type.FullName}--------------------------");
                AddFieldValues1(block, type);
            }
            else if( x is NameValue nv ) {
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

    [UnconditionalSuppressMessage("Trimming", "IL2070: optType.GetProperties")]
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

    [UnconditionalSuppressMessage("Trimming", "IL2075: optType.GetProperties")]
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
}
