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
        Console2.WriteLine("ApplicationName: ".PadRight(28) + ClownFishBehavior.Instance.GetApplicationName());
        Console2.WriteLine("HostName: ".PadRight(28) + ClownFishBehavior.Instance.GetHostName());
        Console2.WriteLine("EnvName: ".PadRight(28) + ClownFishBehavior.Instance.GetEnvName());
        Console2.WriteLine("Version: ".PadRight(28) + FileVersionInfo.GetVersionInfo(typeof(TracingInitializer).Assembly.Location).FileVersion);
        Console2.WriteLine("##### ClownFish.Tracing 初始化成功!");
        Console2.WriteLine("==================================================================");
    }
}
