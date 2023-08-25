<%@ Page Language="C#"  %>
<script runat="server">

    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);

        this.Response.ContentType = "text/plain";
        this.Response.AppendHeader("Content-Encoding", "gzip");
        this.Response.Filter = new System.IO.Compression.GZipStream(this.Response.Filter, System.IO.Compression.CompressionMode.Compress);

        string text = @"
HttpClient 和生存期管理
每次对 IHttpClientFactory 调用 CreateClient 都会返回一个新 HttpClient 实例。 每个命名客户端都创建一个 HttpMessageHandler。 工厂管理 HttpMessageHandler 实例的生存期。
IHttpClientFactory 将工厂创建的 HttpMessageHandler 实例汇集到池中，以减少资源消耗。 新建 HttpClient 实例时，可能会重用池中的 HttpMessageHandler 实例（如果生存期尚未到期的话）。
由于每个处理程序通常管理自己的基础 HTTP 连接，因此需要池化处理程序。 创建超出必要数量的处理程序可能会导致连接延迟。 部分处理程序还保持连接无期限地打开，这样可以防止处理程序对 DNS（域名系统）更改作出反应。
处理程序的默认生存期为两分钟。 可在每个命名客户端上重写默认值：
C#

复制
public void ConfigureServices(IServiceCollection services)
{           
    services.AddHttpClient(""extendedhandlerlifetime"")
        .SetHandlerLifetime(TimeSpan.FromMinutes(5));

    // Remaining code deleted for brevity.
HttpClient 实例通常可视为无需处置的 .NET 对象 。 处置既取消传出请求，又保证在调用 Dispose 后无法使用给定的 HttpClient 实例。 IHttpClientFactory 跟踪和处置 HttpClient 实例使用的资源。
保持各个 HttpClient 实例长时间处于活动状态是在 IHttpClientFactory 推出前使用的常见模式。 迁移到 IHttpClientFactory 后，就无需再使用此模式。
IHttpClientFactory 的替代项
通过在启用了 DI 的应用中使用 IHttpClientFactory，可避免：
通过共用 HttpMessageHandler 实例，解决资源耗尽问题。
通过定期循环 HttpMessageHandler 实例，解决 DNS 过时问题。
此外，还有其他方法使用生命周期长的 SocketsHttpHandler 实例来解决上述问题。
在应用启动时创建 SocketsHttpHandler 的实例，并在应用的整个生命周期中使用它。
根据 DNS 刷新时间，将 PooledConnectionLifetime 配置为适当的值。
根据需要，使用 new HttpClient(handler, disposeHandler: false) 创建 HttpClient 实例。
上述方法使用 IHttpClientFactory 解决问题的类似方式解决资源管理问题。
SocketsHttpHandler 在 HttpClient 实例之间共享连接。 此共享可防止套接字耗尽。
SocketsHttpHandler 会根据 PooledConnectionLifetime 循环连接，避免出现 DNS 过时问题。
Cookie
共用 HttpMessageHandler 实例将导致共享 CookieContainer 对象。 意外的 CookieContainer 对象共享通常会导致错误的代码。 对于需要 Cookie 的应用，请考虑执行以下任一操作：
禁用自动 Cookie 处理
避免 IHttpClientFactory

";
        for( int i = 0; i < 100; i++ )
            this.Response.Write(text);
    }

</script>