namespace ClownFish.Web.Utils;

/// <summary>
/// 框架内部使用的扩展方法
/// </summary>
public static class HttpHeaderUtils
{
    public static void DeleteUselessHeaders(HttpRequest httpRequest)
    {
        // 删除一些无用的请求头
        httpRequest.Headers.Remove("x-b3-sampled");
        httpRequest.Headers.Remove("x-b3-spanid");
        httpRequest.Headers.Remove("x-b3-traceid");
        httpRequest.Headers.Remove("x-b3-parentspanid");
        httpRequest.Headers.Remove("x-envoy-attempt-count");
        httpRequest.Headers.Remove("x-envoy-peer-metadata-id");
        httpRequest.Headers.Remove("x-envoy-peer-metadata");
        httpRequest.Headers.Remove("x-envoy-decorator-operation");
        httpRequest.Headers.Remove("x-envoy-external-address");
        httpRequest.Headers.Remove("x5-uuid");
        httpRequest.Headers.Remove("x-real-ip");
        httpRequest.Headers.Remove("eagleeye-rpcid");
        httpRequest.Headers.Remove("eagleeye-traceid");
        httpRequest.Headers.Remove("x-forwarded-cluster");
        //httpRequest.Headers.Remove("x-forwarded-proto");  // 这个不能删除！可搜索代码：GetCookieOptions
        httpRequest.Headers.Remove("x-forwarded-client-cert");
        httpRequest.Headers.Remove("wl-proxy-client-ip");
        httpRequest.Headers.Remove("web-server-type");
        httpRequest.Headers.Remove("x-true-ip");
        httpRequest.Headers.Remove("sec-ch-ua");
        httpRequest.Headers.Remove("sec-ch-ua-mobile");
        httpRequest.Headers.Remove("sec-ch-ua-platform");
        httpRequest.Headers.Remove("sec-fetch-site");
        httpRequest.Headers.Remove("sec-fetch-mode");
        httpRequest.Headers.Remove("sec-fetch-dest");
    }
}
