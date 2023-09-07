namespace ClownFish.UnitTest.Http.Pipleline.Test;

public class TestHandler1 : IAsyncNHttpHandler
{
    public string RequestBodyText { get; private set; }

    public byte[] RequestBodyBytes { get; private set; }

    public Dictionary<string, string> RequestValues { get; private set; }

    public List<NameValue> QuerySting { get; private set; }

    public List<NameValue> Form { get; private set; }

    public List<NameValue> Headers { get; private set; }

    public List<NameValue> Cookies { get; private set; }

    public string LoggingText { get; private set; }


    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        ReadRequest(httpContext.Request);

        string flag = httpContext.Request.Header("x-ThrowException");
        if( flag.HasValue() )
            throw new ApplicationException(flag);

        await SetResponse(httpContext.Response);
    }


    private void ReadRequest(NHttpRequest request)
    {
        request.LogRequestBody = true;

        this.RequestBodyText = request.ReadBodyAsText();
        this.RequestBodyBytes = request.ReadBodyAsBytes();

        this.RequestValues = new Dictionary<string, string> {
            { "HttpMethod", request.HttpMethod},
            { "RootUrl", request.RootUrl},
            { "Path", request.Path},
            { "Query", request.Query},
            { "RawUrl", request.RawUrl},
            { "FullPath", request.FullPath},
            { "FullUrl", request.FullUrl},
            { "ContentType", request.ContentType},
            { "UserAgent", request.UserAgent},

            { "IsHttps", request.IsHttps.ToString()},
            { "IsAuthenticated", request.HttpContext.IsAuthenticated.ToString()},
            { "ContentLength", request.ContentLength.ToString()},
            { "HasBody", request.HasBody.ToString()},
            { "LogRequestBody", request.LogRequestBody.ToString()},

            { "GetBodyTextLength", request.GetBodyTextLength().ToString()},
            { "GetEncoding", request.GetEncoding().ToString()},
            { "ToString", request.ToString()},
        };

        this.QuerySting = new List<NameValue>();
        foreach( string name in request.QueryStringKeys ) {
            string value = request.QueryString(name);
            this.QuerySting.Add(new NameValue(name, value));
        }

        this.Form = new List<NameValue>();
        foreach( string name in request.FormKeys ) {
            string value = request.Form(name);
            this.Form.Add(new NameValue(name, value));
        }


        this.Headers = new List<NameValue>();
        //foreach( string name in request.HeaderKeys ) {
        //    string[] values = request.GetHeaders(name);
        //    foreach( string value in values ) {
        //        this.Headers.Add(new NameValue(name, value));
        //    }
        //}
        request.AccessHeaders((name, value) => this.Headers.Add(new NameValue(name, value)));

        this.Cookies = new List<NameValue>();
        foreach( string name in request.CookieKeys ) {
            string value = request.Cookie(name);
            this.Cookies.Add(new NameValue(name, value));
        }
        
        this.LoggingText = request.ToLoggingText();
    }

    private async Task SetResponse(NHttpResponse response)
    {
        response.StatusCode = 503;
        response.ContentType = "text1";
        response.ContentEncoding = Encoding.Unicode;

        response.SetHeader("x-name1", "xxxxxxx");
        response.SetCookie2("c1", "x1");
        response.SetCookie("c2", "x2");

        response.ClearHeaders();


        response.SetCookie2("c1", "566d8eb50aa248debfe90b861fe7f0ac");
        response.SetCookie("c2", "59a086b737e546bfba325baef3792d0f");

        response.SetHeader("x-name1", "a39b9f287a0641c49e0a020800e2d65c");
        response.SetHeaders("x-name2", new string[] { "2c975c858d4c486ca", "709caba2956f713" });


        response.Write(string.Empty.GetBytes());
        response.Write(Empty.Array<byte>());

        response.Write("body-5e5435023e3a464196b1c23903e2aacf".GetBytes());
        await response.WriteAsync("body-56e1819f58de4d0896e7974dc5aff6c5".GetBytes());

        byte[] b1 = Encoding.UTF8.GetBytes("bb-c7e3b60201f544ef96110fa3625250e1");
        byte[] b2 = Encoding.UTF8.GetBytes("bb-b1de7f5eb895451bb9d59de9134b841a");

        response.Write(b1);
        await response.WriteAsync(b2);

        // 这个操作会出现异常
        response.SetHeader("x-name3", "xxxx");
    }
}
