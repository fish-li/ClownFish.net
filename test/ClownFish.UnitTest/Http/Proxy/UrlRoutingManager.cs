namespace ClownFish.UnitTest.Http.Proxy;

internal static class UrlRoutingManager
{
    private static bool s_enable = false;

    /// <summary>
    /// 保存【完整URL】的映射关系
    /// </summary>
    private static readonly Dictionary<string, string> s_urlMaps = new Dictionary<string, string>(128, StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// 除 【完整URL映射关系】之外的配置
    /// </summary>
    private static readonly List<ProxyItem> s_list = new List<ProxyItem>(128);

    internal static void ClearRules()   // 供测试使用
    {
        s_urlMaps.Clear();
        s_list.Clear();
        s_enable = false;
    }

    /// <summary>
    /// 注册URL转发规则
    /// </summary>
    /// <param name="config">URL转发规则</param>
    public static void RegisterRules(ProxyConfig config)
    {
        if( config == null || config.Items.IsNullOrEmpty() )
            return;

        if( s_enable )
            throw new InvalidOperationException("当前方法不允许多次调用。");


        List<ProxyItem> list = new List<ProxyItem>();

        foreach( var x in config.Items ) {

            ProxyItem item = x.Clone();
            if( item == null )
                continue;

            if( item.TypeFlag == 0 )
                s_urlMaps[x.Src] = x.Dest;
            else
                list.Add(item);
        }

        if( s_urlMaps.Count > 0 || list.Count > 0 ) {
            s_list.AddRange(list.OrderBy(x => x.TypeFlag));
            s_enable = true;
        }
    }



    /// <summary>
    /// 获取要转发的目标地址
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GetDestUrl(NHttpRequest request)
    {
        if( s_enable == false )
            return null;

        string path = request.Path;

        string destUrl = s_urlMaps.TryGet(path);
        if( destUrl.IsNullOrEmpty() == false ) {
            return destUrl + request.Query;
        }


        foreach( var x in s_list ) {
            if( x.TypeFlag == 2 ) {
                return x.Dest + request.RawUrl;
            }

            if( x.TypeFlag == 1 ) {
                if( path.StartsWithIgnoreCase(x.Src) ) {
                    return x.Dest + request.RawUrl;
                }
            }

            //else if( path.Is(x.Src) ) {  // TypeFlag == 0
            //    return x.Dest + request.Query;
            //}
        }

        return null;
    }
}
