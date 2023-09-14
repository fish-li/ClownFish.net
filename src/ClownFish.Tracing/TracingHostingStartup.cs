using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

[assembly: HostingStartup(typeof(ClownFish.Tracing.TracingHostingStartup))]

namespace ClownFish.Tracing;

internal class TracingHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
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

        // 初始化核心部分
        TracingInitializer.Init();
        ShowStartInfo();
    }


    private static void ShowStartInfo()
    {
        Console2.WriteLine("==================================================================");
        Console2.WriteLine("ApplicationName : " + EnvUtils.GetAppName());
        Console2.WriteLine("AppRuntimeId    : " + EnvUtils.AppRuntimeId);
        Console2.WriteLine("AppStartTime    : " + EnvUtils.AppStartTime.ToTimeString());
        Console2.WriteLine("EntryAssembly   : " + Assembly.GetEntryAssembly().Location);
        Console2.WriteLine("EnvironmentName : " + EnvUtils.GetRuntimeEnvName() + "/" + EnvUtils.GetClusterName());
        Console2.WriteLine("ApplicationPath : " + AppContext.BaseDirectory);
        Console2.WriteLine("CurrentDirectory: " + Environment.CurrentDirectory);
        Console2.WriteLine("TempPath        : " + EnvUtils.GetTempPath());
        Console2.WriteLine("HostName        : " + EnvUtils.GetHostName());
        Console2.WriteLine("TimeZone        : " + MyTimeZone.CurrentTZ);
        Console2.WriteLine("CurrentCulture  : " + System.Globalization.CultureInfo.CurrentCulture?.Name);
        Console2.WriteLine("Version         : " + FileVersionInfo.GetVersionInfo(typeof(TracingHostingStartup).Assembly.Location).FileVersion);
        Console2.WriteLine("Framework  Name : " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        Console2.WriteLine("==================================================================");
    }
}
