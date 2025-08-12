using System.Net;
using Microsoft.Extensions.Primitives;

namespace ClownFish.Web.Utils;

internal static class IpBlackList
{
    private static readonly Dictionary<string, ValueCounter> s_dict;
    static IpBlackList()
    {
        if( ClownFishWebOptions.IpBlackList.Count == 0 )
            return;

        s_dict = new Dictionary<string, ValueCounter>(ClownFishWebOptions.IpBlackList.Count);
        foreach( string ip in ClownFishWebOptions.IpBlackList ) {
            s_dict[ip] = new ValueCounter(ip);
        }

        DebugReport.RegisterStatusInfoCallback(GetStatus);
    }

    /// <summary>
    /// 检查当前请求的IP是否为黑名单范围之内
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="logCounter">如果IP为黑名单范围，是否记录到计数器</param>
    /// <returns></returns>
    public static bool Check(HttpContext httpContext, bool logCounter)
    {
        if( ClownFishWebOptions.IpBlackList.Count == 0 )
            return false;

        string clientIP = GetClientIP(httpContext);
        if( clientIP.IsNullOrEmpty() )
            return false;

        bool result = ClownFishWebOptions.IpBlackList.Contains(clientIP);
        if( result && logCounter ) {
            ValueCounter counter = s_dict[clientIP];
            counter.Increment();
        }
        return result;
    }


    public static string GetClientIP(HttpContext httpContext)
    {
        string clientIP = null;

        if( httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues value) ) {
            clientIP = value.FirstOrDefault();

            if( clientIP.HasValue() ) {
                string[] items = clientIP.Split(',');
                clientIP = items.First().Trim();
            }
        }

        if( clientIP.IsNullOrEmpty() ) {
            IPAddress ip = httpContext.Connection.RemoteIpAddress;
            if( ip != null ) {
                clientIP = ip.MapToIPv4().ToString();
            }
        }
        return clientIP;
    }


    internal static DebugReportBlock GetStatus()
    {
        if( s_dict == null ) {
            return null;
        }

        DebugReportBlock block = new DebugReportBlock { Category = "HttpRequest Ip BlackList" };
        foreach( var item in s_dict ) {
            block.AppendLine($"{item.Key}: {item.Value.Get().ToWString()}");
        }
        return block;
    }
}
