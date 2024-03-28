#if NETCOREAPP
using System.Net.Http;
using ClownFish.Log;

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
        int mode = LocalSettings.GetInt("ClownFish_HttpClient_MonitoringMode", 1);
        if( mode == 1 )
            DiagnosticListener.AllListeners.Subscribe(new HttpClientEventSubscriber());
        else
            HttpClientLogger.Init();
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

    public string ToLoggingText()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            this.Request.ToLoggingText(sb);

            if( this.Response != null) {
                sb.AppendLineRN(TextUtils.StepDetailSeparatedLine3);
                this.Response.ToLoggingText(sb);
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

        // 为了方便记录完整日志，确保 Reqest.Body 可多次读取
        TryReplaceContent(request);

        HttpClientEventData data = new HttpClientEventData {
            StartThreadId = Thread.CurrentThread.ManagedThreadId,
            StartTime = DateTime.Now,
            Request = request
        };
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

        string parentId = data.Request.GetHeader(HttpHeaders.XRequest.ParentId);
        // parentId格式：RequestId/OperationId，可参考：ClownFish.Log.Logging.HttpTraceUtils.SetClientRequest 方法
        var id2 = HttpTraceUtils.ParseParentIdHeader(parentId);
        string operationId = id2.OperationId.HasValue() ? id2.OperationId : Guid.NewGuid().ToString("N");

        StepItem step = StepItem.CreateNew(data.StartTime, operationId);
        step.StepKind = StepKinds.HttpRpc;

        // MS并没有将同步还是异步这个信息放在事件参数中，所以只能通过“是否切换线程”来判断是不是异步
        bool isAsync = Thread.CurrentThread.ManagedThreadId == data.StartThreadId;
        step.IsAsync = isAsync ? 0 : 1;
        step.StepName = isAsync ? "SendHttpAsync" : "SendHttp";

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

        //TODO: 为了能让HttpResponseSerializer永远可读取ResponseBody，需要修改 HttpResponseMessage.Content
        TryReplaceContent(data.Response);

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
            return 3;
        }

        // 如果参数不允许记录，或者根本没有 body，就忽略
        if( request.CanLogBody() ) {

            // 替换 body
            HttpContent content2 = CloneBody(request.Content);
            request.Content.Dispose();
            request.Content = content2;
            return 1;
        }
        else {
            return 2;
        }
    }

   

    internal static void TryReplaceContent(HttpResponseMessage response)
    {
        // 不能使用这段代码，因为有时候会出现异常：TODO: 以后再解决！
        // System.InvalidOperationException: The response is not fully buffered.
        //    at Azure.Response.get_Content()
        //    at Azure.AI.OpenAI.Embeddings.FromResponse(Response response)

        if( response == null )
            return;

        if( LoggingOptions.HttpClient.MustLogResponse == false )
            return;
               

        // 如果参数不允许记录，或者根本没有 body，就忽略
        if( response.CanLogBody() ) {

            // 替换 body
            HttpContent content2 = CloneBody(response.Content);
            response.Content.Dispose();
            response.Content = content2;
        }
    }

    internal static HttpContent CloneBody(HttpContent content)
    {
        MemoryStream ms = new MemoryStream();
        content.CopyTo(ms, null, CancellationToken.None);

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
