using ClownFish.Base.Xml;
using ClownFish.Log.Configuration;
using Microsoft.AspNetCore.Builder;

namespace ClownFish.Tracing;

public static class TracingInitializer
{
    private static bool s_inited = false;
    private static readonly object s_lock = new object();

    public static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {

                    Init0();
                    s_inited = true;
                }
            }
        }
    }

    private static void Init0()
    {
        if( EnvironmentVariables.Get("ClownFish_Tracing_Enable") == "0" ) {
            Console2.Info("########################### 由于设置 ClownFish_Tracing_Enable=0 ，ClownFish.Tracing 将不会启用！");
            return;
        }

        ClownFishInit.InitBase();

        if( LoggingOptions.TracingEnabled == false ) {
            Console2.Info("########################### 由于 LoggingOptions.TracingEnabled == false ，ClownFish.Tracing 将不会启用！");
            return;
        }

        if( LogConfig.IsInited ) {
            Console2.Info("########################### ClownFish.Log 日志组件已经提前初始化，ClownFish.Tracing 产生的日志数据可能会丢失！！");
            // 这里允许当前继续执行，因为有可能当前程序的 ClownFish.Log.Config 已包含了 ClownFish.Tracing.ClownFish.Log.config 中的内容
        }

        InitLog();

        TracingUtils.Init();

        AspnetcoreLogger.Init();

        ShowStartInfo();
    }

    private static void InitLog()
    {      
        // 从程序集中加载默认配置文件
        string xml = typeof(TracingInitializer).Assembly.ReadResAsText("ClownFish.Tracing.ClownFish.Log.config");
        LogConfiguration config = XmlHelper.XmlDeserialize<LogConfiguration>(xml);

        // 尝试本地参数中更新日志配置
        config.TryUpdateFromLocalSetting();

        // 日志的数据类型和写入器的配置已经在 Nebula.Tracing.ClownFish.Log.config 指定好了，
        // 所有的 OprLog 使用 HttpJsonWriter，将日志发送到 Nebula.LogGate 服务来接收
        // 所以就注释了下面的代码。

        //// 允许重新指定写入器类型，例如：开发时写到XML文件，生产环境部署时统一写到ES
        //string logWriterNames = Settings.GetSetting("Nebula_Log_WritersMap");
        //config.OverrideWriters(logWriterNames);


        if( LocalSettings.GetBool("Show_ClownFish_Tracing_Log_Config") ) {
            // 由于 Log_Config 的内容会做【合并】，所以这里显示【最终生效】的配置对象
            string configXml = XmlHelper.XmlSerialize(config, Encoding.UTF8);
            Console2.WriteLine("------------------- ClownFish_Tracing_Log_Config ------------------------");
            Console2.WriteLine(configXml);
            Console2.WriteLine("-------------------------------------------------------------------------");
        }

        ClownFish.Log.LogConfig.Init(config);

        // 在这里模式下，日志只保留 Oprlog，它们发送 Nebula.LogGate
        // InvokeLog 即使产生也会被丢弃，所以就设置下面的开关，明确指出不需要产生InvokeLog
        MemoryConfig.AddSetting("ClownFish_Log_InvokeLogEnable", "0");
    }


    private static void ShowStartInfo()
    {
        Console2.WriteLine("##### Nebula.Tracing 初始化成功!");
        Console2.WriteLine("==================================================================");
        Console2.WriteLine("ApplicationName        : " + EnvUtils.GetAppName());
        Console2.WriteLine("AppRuntimeId           : " + EnvUtils.AppRuntimeId);
        Console2.WriteLine("AppStartTime           : " + EnvUtils.AppStartTime.ToTime23String());
        Console2.WriteLine("EntryAssembly          : " + AsmHelper.GetExeFilePath());
        Console2.WriteLine("CLUSTER_ENVIRONMENT    : " + EnvUtils.GetClusterName());
        Console2.WriteLine("RUNTIME_ENVIRONMENT    : " + EnvUtils.GetRunEnv());
        Console2.WriteLine("ApplicationPath        : " + AppContext.BaseDirectory);
        Console2.WriteLine("CurrentDirectory       : " + Environment.CurrentDirectory);
        Console2.WriteLine("TempPath               : " + EnvUtils.GetTempPath());
        Console2.WriteLine("HostName               : " + EnvUtils.GetHostName());
        Console2.WriteLine("TimeZone               : " + MyTimeZone.CurrentTZ);
        Console2.WriteLine("CurrentCulture         : " + System.Globalization.CultureInfo.CurrentCulture?.Name);
        Console2.WriteLine("Version                : " + AsmHelper.GetFileVersion(typeof(TracingHostingStartup)));
        Console2.WriteLine("Framework  Name        : " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        Console2.WriteLine("==================================================================");
    }


    public static void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => {

            services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options => {
                options.AllowSynchronousIO = true;
                options.AddServerHeader = false;
            });
            services.Configure<IISServerOptions>(options => {
                options.AllowSynchronousIO = true;
            });


            // 为了能捕获到Action中出现的异常，只能用 Filter 的方式，所以这里注册一个全局异常过滤器
            services.Configure<MvcOptions>(opt => {
                opt.Filters.Add<NExceptionFilter>(int.MaxValue);
            });
        });
    }

}
