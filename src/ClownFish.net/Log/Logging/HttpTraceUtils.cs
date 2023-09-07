namespace ClownFish.Log.Logging;

/// <summary>
/// HttpTraceUtils
/// </summary>
public static class HttpTraceUtils
{
    /// <summary>
    /// 请求开始时调用，确定 RootId, ParentId
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="scope"></param>
    internal static void SetRootParent(NHttpContext httpContext, OprLogScope scope)
    {
        if( scope != null ) {
            string parentId = httpContext.Request.Header(HttpHeaders.XRequest.ParentId) ?? string.Empty;
            string rootId = httpContext.Request.Header(HttpHeaders.XRequest.RootId);

            // 如果RootId为空，表示当前请求是一个【顶层请求】，此时分配一个RootId
            if( rootId.IsNullOrEmpty() ) {
                rootId = httpContext.PipelineContext.ProcessId;
            }

            scope.OprLog.RootId = rootId;
            scope.OprLog.ParentId = parentId;
        }
    }

    // ParentId 字段有2种取值可能：
    // 1，为 null，表示是一个顶层的请求
    // 2，oprLogId/clientId  格式，之所有这里要包含2个ID，主要是因为 clientId 所在的日志做为子表嵌入在 OprDetails 字段中，
    //    而且有可能整个OprDetails没有内容，所以如果 ParentId=clientId 会出现找到父结节问题，但是oprLogId所在的日志是一定存在。


#if NETFRAMEWORK
    internal static void SetTraceHeader(BaseHttpClient client)
    {
        System.Net.HttpWebRequest request = (client as ClownFish.WebClient.V1.HttpClient).Request;

        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull == false  ) {

            if( request.Headers.AllKeys.Contains(HttpHeaders.XRequest.RootId) == false ) {

                request.Headers.Add(HttpHeaders.XRequest.RootId, scope.OprLog.RootId);
                request.Headers.Add(HttpHeaders.XRequest.ParentId, scope.OprLog.OprId + "/" + client.OperationId);
            }
        }
    }
#else
    /// <summary>
    /// HttpClient 发送请求时调用，在请求头中指定2个链路头
    /// </summary>
    /// <param name="client"></param>
    internal static void SetTraceHeader(BaseHttpClient client)
    {
        System.Net.Http.HttpRequestMessage request = (client as ClownFish.WebClient.V2.HttpClient2).Request;

        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull == false ) {
            SetTraceHeader(request, scope, client.OperationId);
        }
    }

    internal static void SetTraceHeader(System.Net.Http.HttpRequestMessage request, OprLogScope scope, string operationId = null)
    {
        if( scope.IsNull == false && request.Headers.Contains(HttpHeaders.XRequest.RootId) == false ) {

            operationId = operationId ?? Guid.NewGuid().ToString("N");
            request.Headers.TryAddWithoutValidation(HttpHeaders.XRequest.RootId, scope.OprLog.RootId);
            request.Headers.TryAddWithoutValidation(HttpHeaders.XRequest.ParentId, scope.OprLog.OprId + "/" + operationId);
            // 说明，scope.OprLog.OprId == RequestId  == httpContext.PipelineContext.ProcessId
        }
    }
#endif


    // ParentId 请求头的说明：
    // 发送HTTP请求时，ParentId头不仅仅是当前请求ID，还包含当次客户端ID信息，这样才能形成正确的树形结构

    private static readonly char[] s_separator = new char[] { '/' };

    /// <summary>
    /// 解析 parentid 请求头。
    /// 期望格式：RequestId/OperationId，如果格式不是预期，则将所有内容认为是 RequestId
    /// </summary>
    /// <param name="parentIdHeaderValue"></param>
    /// <returns></returns>
    public static (string RequestId, string OperationId) ParseParentIdHeader(string parentIdHeaderValue)
    {
        if( parentIdHeaderValue.IsNullOrEmpty() )
            return (parentIdHeaderValue, null);

        // parentId格式：RequestId/OperationId，可参考上面的 SetClientRequest 方法
        string[] id2 = parentIdHeaderValue.Split(s_separator, StringSplitOptions.RemoveEmptyEntries);

        if( id2.Length == 2 )
            return (id2[0], id2[1]);
        else
            return (parentIdHeaderValue, string.Empty);
    }

    


    

}
