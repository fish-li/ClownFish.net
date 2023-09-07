namespace ClownFish.WebClient;

/// <summary>
/// 与HttpClient相关的事件通知类
/// </summary>
public static class HttpClientEvent
{
//#if NETCOREAPP
//    private static readonly DiagnosticListener s_diagnosticSource = new DiagnosticListener("ClownFish.HttpClientEvent");
//#endif

    /// <summary>
    /// 创建Request对象前将会引发此事件
    /// </summary>
    public static event EventHandler<BeforeCreateRequestEventArgs> OnCreateRequest;

    /// <summary>
    /// 在发送HTTP请求前将会引发此事件
    /// </summary>
    public static event EventHandler<BeforeSendEventArgs> OnBeforeSendRequest;

    /// <summary>
    /// 表示请求完成时触发的事件
    /// </summary>
    public static event EventHandler<RequestFinishedEventArgs> OnRequestFinished;

    internal static void BeforeCreateRequest(this BaseHttpClient client)
    {
        EventHandler<BeforeCreateRequestEventArgs> handler = OnCreateRequest;
        if( handler != null ) {

            BeforeCreateRequestEventArgs e = new BeforeCreateRequestEventArgs {
                OperationId = client.OperationId,
                HttpOption = client.HttpOption
            };
            handler(client, e);
        }
    }


    internal static void BeforeSend(this BaseHttpClient client)
    {
        BeforeSendEventArgs e = null;

        EventHandler<BeforeSendEventArgs> handler = OnBeforeSendRequest;
        if( handler != null ) {
            if( e == null )
                e = CreateBeforeSendEventArgs(client);
            handler(client, e);
        }

//#if NETCOREAPP
//        if( s_diagnosticSource.IsEnabled() ) {
//            if( e == null )
//                e = CreateBeforeSendEventArgs(client);
//            s_diagnosticSource.Write("OnBeforeSendRequest", e);
//        }
//#endif
    }

    private static BeforeSendEventArgs CreateBeforeSendEventArgs(BaseHttpClient client)
    {
        return new BeforeSendEventArgs {
            OperationId = client.OperationId,
            HttpOption = client.HttpOption,
            Request = client.Request
        };
    }


    internal static void RequestFinished(this BaseHttpClient client, HttpWebResponse response, Exception ex)
    {
        RequestFinishedEventArgs e = null;

        EventHandler<RequestFinishedEventArgs> handler = OnRequestFinished;
        if( handler != null ) {
            if( e == null )
                e = CreateRequestFinishedEventArgs(client, response, ex);

            try {
                handler(client, e);
            }
            catch {
                // 这里吃掉所有异常！
            }
        }

//#if NETCOREAPP
//        if( s_diagnosticSource.IsEnabled() ) {
//            if( e == null )
//                e = CreateRequestFinishedEventArgs(client, response, ex);

//            try {
//                s_diagnosticSource.Write("OnRequestFinished", e);
//            }
//            catch {
//                // 这里吃掉所有异常！
//            }
//        }
//#endif
    }

    private static RequestFinishedEventArgs CreateRequestFinishedEventArgs(BaseHttpClient client, HttpWebResponse response, Exception ex)
    {
        RequestFinishedEventArgs e = new RequestFinishedEventArgs();

        e.OperationId = client.OperationId;
        e.HttpOption = client.HttpOption;
        e.Request = client.Request;
        e.Response = response;

        e.StartTime = client.StartTime;
        e.EndTime = DateTime.Now;
        e.Exception = ex;

        return e;
    }



}
