using ClownFish.WebClient;

namespace ClownFish.Web.Modules;

internal class ExecHttpUiModule : NHttpModule
{
    private static readonly string s_accessKey = Guid.NewGuid().ToString("N");

    private static readonly string s_html = typeof(ExecHttpUiModule).Assembly.ReadResAsText("ClownFish.Web.ExecHttp.html");

    public override void Init()
    {
        Console2.Info("ExecHttpUi accesss key: " + s_accessKey);
    }

    public override void BeginRequest(NHttpContext httpContext)
    {
        string path = httpContext.Request.Path;

        if( path == "/clownfish/ui/exechttp/page" ) {

            if( httpContext.Request.HttpMethod == "GET" ) {
                httpContext.PipelineContext.SetHttpHandler(ShowPageHandler.Instance);
                return;
            }
            else if( httpContext.Request.HttpMethod == "POST" ) {
                httpContext.PipelineContext.SetHttpHandler(SubmitHandler.Instance);
                return;
            }
        }
    }

    public sealed class ShowPageHandler : IAsyncNHttpHandler
    {
        public static readonly ShowPageHandler Instance = new ShowPageHandler();

        public async Task ProcessRequestAsync(NHttpContext httpContext)
        {
            string html = s_html.Replace("value='111'", "").Replace("value222", "").Replace("value333", "");
            await httpContext.HttpReplyAsync(html, ResponseContentType.HtmlUtf8);
        }
    }

    public sealed class SubmitHandler : IAsyncNHttpHandler
    {
        public static readonly SubmitHandler Instance = new SubmitHandler();

        public async Task ProcessRequestAsync(NHttpContext httpContext)
        {
            string accesskey = httpContext.Request.Form("accesskey");
            string httpcode = httpContext.Request.Form("httpcode");

            string result = await GetResult(accesskey, httpcode);

            string html = s_html.Replace("value='111'", $"value='{accesskey}'").Replace("value222", httpcode).Replace("value333", result);
            await httpContext.HttpReplyAsync(html, ResponseContentType.HtmlUtf8);
        }

        private async Task<string> GetResult(string accesskey, string httpcode)
        {
            if( accesskey != s_accessKey ) {
                return "accesskey is error!";
            }

            if( httpcode.IsNullOrEmpty() ) {
                return "httpcode is empty!";
            }

            try {
                HttpOption httpOption = HttpOption.FromRawText(httpcode);
                HttpResult<string> result = await httpOption.GetResultAsync<HttpResult<string>>();

                return result.ToAllText();
            }
            catch( Exception ex ) {
                return ex.ToString();
            }
        }
    }
}
