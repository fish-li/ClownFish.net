#if NETCOREAPP
using System.Net.Http;
using ClownFish.Log;
using ClownFish.WebClient.V2;

namespace ClownFish.Log.Logging;

// 参考： System.Net.Http.DiagnosticsHandler.SendAsyncCore

/// <summary>
/// 
/// </summary>
public static class HttpClientLogger2
{
    /// <summary>
    /// 
    /// </summary>
    public static void Init()
    {
        int mode = LocalSettings.GetInt("ClownFish_HttpClient_TraceMode", 2);
        if( mode == 2 ) {
            DiagnosticListener.AllListeners.Subscribe(new HttpClientEventSubscriber());
        }
        if( mode == 1 ) {
            HttpClientLogger.Init();
        }
    }
}



internal class HttpClientEventSubscriber : IObserver<DiagnosticListener>
{
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(DiagnosticListener listener)
    {
        switch( listener.Name ) {
            case "HttpHandlerDiagnosticListener":
                listener.Subscribe(new HttpClientEventObserver());
                break;
        }
    }
}

internal class HttpClientEventData : ILoggingObject
{
    public int StartThreadId { get; set; }
    public DateTime StartTime { get; set; }
    public Exception Exception { get; set; }
    public HttpRequestMessage Request { get; set; }
    public HttpResponseMessage Response { get; set; }

    public StreamContent RequestContent { get; set; }
    public StreamContent ResponseContent { get; set; }

    public string ToLoggingText()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            this.Request.ToLoggingText(this.RequestContent, false, sb);

            if( this.Response != null ) {
                sb.AppendLineRN(TextUtils.StepDetailSeparatedLine3);
                this.Response.ToLoggingText(this.ResponseContent, false, sb);
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }
}

internal class HttpClientEventObserver : IObserver<KeyValuePair<string, object>>
{
    private static readonly AsyncLocal<HttpClientEventData> s_local = new AsyncLocal<HttpClientEventData>();

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(KeyValuePair<string, object> kvp)
    {
        if( kvp.Key == "System.Net.Http.Request" ) {
            BeforeSend(kvp.Value);
            return;
        }

        if( kvp.Key == "System.Net.Http.Exception" ) {
            OnError(kvp.Value);
            return;
        }

        if( kvp.Key == "System.Net.Http.Response" ) {
            AfterSend(kvp.Value);
            return;
        }
    }

    private void BeforeSend(object eventData)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        HttpRequestMessage request = eventData.Get<HttpRequestMessage>("Request");
        HttpTraceUtils.SetTraceHeader(request, scope);


        // 如果当前请求是一个反向代理转发，就不记录日志了
        HttpPipelineContext httpPipeline = HttpPipelineContext.Get();
        if( httpPipeline != null && httpPipeline.HttpContext.IsTransfer )
            return;

        HttpClientEventData data = new HttpClientEventData {
            StartThreadId = Thread.CurrentThread.ManagedThreadId,
            StartTime = DateTime.Now,
            Request = request
        };

        // 为了记录完整日志，确保 Reqest.Body 可多次读取，需要修改 HttpRequestMessage.Content
        if( TryReplaceContent(request) is 1 or 3 ) {
            // 记录日志时， HttpRequestMessage 可能被 dispose
            // 所以这里增加一个引用，供写日志时访问
            data.RequestContent = (StreamContent)request.Content;
        }

        s_local.Value = data;
    }

    private void OnError(object eventData)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        HttpClientEventData data = s_local.Value;
        if( data == null )
            return;

        data.Exception = eventData.Get<Exception>("Exception");
    }


    private void AfterSend(object eventData)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        HttpClientEventData data = s_local.Value;
        if( data == null )
            return;

        // 释放引用
        s_local.Value = null;

        DateTime endTime = DateTime.Now;

        string parentId = data.Request.GetHeaderFirstValue(HttpHeaders.XRequest.ParentId);
        // parentId格式：RequestId/OperationId，可参考：ClownFish.Log.Logging.HttpTraceUtils.SetClientRequest 方法
        var id2 = HttpTraceUtils.ParseParentIdHeader(parentId);
        string operationId = id2.OperationId.HasValue() ? id2.OperationId : Guid.NewGuid().ToString("N");

        StepItem step = StepItem.CreateNew(data.StartTime, operationId);
        step.StepKind = StepKinds.HttpRpc;

        // MS并没有将同步还是异步这个信息放在事件参数中，所以只能通过“是否切换线程”来判断是不是异步
        bool isAsync = Thread.CurrentThread.ManagedThreadId != data.StartThreadId;
        step.IsAsync = isAsync ? 1 : 0;
        step.StepName = data.Request.GetOptionValue<string>(LoggingKeys.HttpOptionId) ?? (isAsync ? "SendHttpAsync" : "SendHttp");

        if( data.Exception != null ) {
            step.SetException(data.Exception);
        }
        else {
            // DiagnosticsHandler有个非常坑爹的设计，它捕获了 OperationCanceledException 异常
            // 在里面只是设置 taskStatus=TaskStatus.Canceled; 并没有引发事件
            // 所以需要在结束时判断是否有OperationCanceledException异常发生

            TaskStatus taskStatus = eventData.Get<TaskStatus>("RequestTaskStatus");
            if( taskStatus == TaskStatus.Canceled ) {
                // ......... TND，这个异常拿不到 ！！
                step.Status = 500;
                step.HasError = 1;
                step.ExType = typeof(OperationCanceledException).FullName;
                step.ExMessage = "调用超时.";
            }
        }

        data.Response = eventData.Get<HttpResponseMessage>("Response");

        // 注意：有些请求即使是以 HTTP 500 来响应的，
        // 但是对于 .net bcl DiagnosticsHandler.SendAsyncCore() 方法来说，它并没有出现异常，
        // 所以没有引发"System.Net.Http.Exception"事件
        // 在后面处理响应时，ClownFish.WebClient.V2.HttpClient2 会主动检测并抛出异常，所以这里要做一个相同的判断
        if( step.Status == 200 && data.Response.StatusCode != HttpStatusCode.OK ) {
            step.Status = (int)data.Response.StatusCode;
            step.HasError = data.Response.IsSuccessStatusCode ? 0 : 1;
            if( step.HasError == 1 ) {
                step.ExType = typeof(WebException).FullName;
                step.ExMessage = HttpClient2.CreateExceptionMessage(data.Response);
            }
        }

        // 为了能让HttpResponseSerializer永远可读取ResponseBody，需要修改 HttpResponseMessage.Content
        if( TryReplaceContent(data.Response) == 1 ) {
            // 注意：在写日志时 HttpClientEventData.ToLoggingText()，response.Content 已变成 System.Net.Http.EmptyContent
            // 所以这里增加一个引用，供写日志时访问
            data.ResponseContent = (StreamContent)data.Response.Content;
        }

        step.Cmdx = data;

        step.End(endTime);

        scope.AddStep(step);
    }



    internal static int TryReplaceContent(HttpRequestMessage request)
    {
        if( request == null )
            return 0;

        if( LoggingOptions.HttpClient.MustLogRequest == false )
            return -1;

        // 如果 body 本身就是 MemoryStream，那就不需要替换了
        if( request.Content.BodyIsMemoryStream() ) {
            return request.CanLogBody(request.Content) ? 3 : 4;
        }

        // 如果body满足日志记录条件，就创建一个副本并替换 body
        if( request.CanLogBody(request.Content) ) {

            StreamContent content2 = CloneBody(request.Content);
            request.Content.Dispose();
            request.Content = content2;
            return 1;
        }
        else {
            // 如果参数不允许记录，或者根本没有 body，就忽略
            return 2;
        }
    }



    internal static int TryReplaceContent(HttpResponseMessage response)
    {
        // 不能使用这段代码，因为有时候会出现异常：TODO: 以后再解决！
        // System.InvalidOperationException: The response is not fully buffered.
        //    at Azure.Response.get_Content()
        //    at Azure.AI.OpenAI.Embeddings.FromResponse(Response response)

        if( response == null )
            return 0;

        if( LoggingOptions.HttpClient.MustLogResponse == false )
            return -1;


        // 如果body满足日志记录条件，就创建一个副本并替换 body
        if( response.CanLogBody(response.Content) ) {

            StreamContent content2 = CloneBody(response.Content);
            response.Content.Dispose();
            response.Content = content2;
            return 1;
        }
        else {
            // 如果参数不允许记录，或者根本没有 body，就忽略
            return 2;
        }
    }

    internal static StreamContent CloneBody(HttpContent content)
    {
        MemoryStream ms = new MemoryStream();

#if NET6_0_OR_GREATER
        content.CopyTo(ms, null, CancellationToken.None);
#else
        content.CopyToAsync(ms).GetAwaiter().GetResult();
#endif

        ms.Position = 0;
        StreamContent content2 = new StreamContent(ms);

        foreach( KeyValuePair<string, IEnumerable<string>> kvp in content.Headers ) {
            foreach( var value in kvp.Value ) {
                content2.Headers.TryAddWithoutValidation(kvp.Key, value);
            }
        }

        return content2;
    }

}
#endif
