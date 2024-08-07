using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

[assembly: HostingStartup(typeof(ClownFish.Tracing.TracingHostingStartup))]

namespace ClownFish.Tracing;

internal class TracingHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        TracingInitializer.ConfigureWebHost(builder);

        // 初始化核心部分
        TracingInitializer.Init();
    }
}
