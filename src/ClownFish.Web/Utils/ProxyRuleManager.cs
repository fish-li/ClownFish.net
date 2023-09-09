namespace ClownFish.Web.Utils;

/// <summary>
/// 反向代理规则管理类
/// </summary>
public sealed class ProxyRuleManager
{
    private ProxyMapRule _rules;

    internal ProxyMapRule Rules => _rules;


    /// <summary>
    /// Init
    /// </summary>
    public bool Init(string filename)
    {
        ProxyMapRule rule = LoadRule(filename);
        if( rule == null )
            return false;

        if( LocalSettings.GetBool("ProxyRuleManager_ShowConfig") ) {
            Console2.WriteSeparatedLine();
            Console2.WriteLine(rule.ToXml2());
            Console2.WriteSeparatedLine();
        }

        // 忽略无效的规则
        rule.Rules = rule.Rules.Where(x => x.TypeFlag > 0).ToList();

        if( rule.Rules.Count == 0 )
            return false;

        _rules = rule;
        return true;
    }


    internal static ProxyMapRule LoadRule(string filename)
    {
        if( filename.HasValue() ) {
            string xmlText = ConfigFile.GetFile(filename);
            if( xmlText.HasValue() ) {
                Console2.Info("Load ProxyRule file：" + filename);
                return LoadRuleXml(xmlText);
            }
        }

        return null;
    }


    internal static ProxyMapRule LoadRuleXml(string xmlText)
    {
        // 反序列化，获取配置对象
        ProxyMapRule rule = xmlText.FromXml<ProxyMapRule>();

        // 将【通配符】转成正则表达式
        // 例如：http://*.abc.com/  这种URL就需要用正则表达式
        foreach( var x in rule.Rules ) {

            if( x.Src.IsNullOrEmpty() || x.Dest.IsNullOrEmpty() ) {
                // 直接忽略无效的规则，不抛异常了
                x.TypeFlag = 0;
                continue;
            }

            if( x.Src == "*" ) {
                x.TypeFlag = 5;
            }
            else if( x.Src.Contains("[any]") ) {
                if( x.Dest.Contains("[any]") == false ) {
                    x.Dest += "[any]"; // 不抛异常了，直接放在最后面，最后的结果是否正确也不管了~~
                }
                string newUrl = x.Src.Replace(".", "\\.").Replace("[any]", "(?<any>.*)").Replace("*", "(.*)");
                x.SrcRegex = new Regex(newUrl, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
                x.TypeFlag = x.Src.StartsWith('/') ? 3 : 4;
            }
            else {
                x.TypeFlag = x.Src.StartsWith('/') ? 1 : 2;
            }

            // 允许URL中包含服务名称的配置参数，例如：dest="{configServiceUrl}/[any]"
            x.Dest = ExpandUrl(x.Dest);
        }
        return rule;
    }


    internal static string ExpandUrl(string url)
    {
        if( url.IsNullOrEmpty() )
            return url;

        if( url[0] == '{' ) {
            int p = url.IndexOf('}');
            if( p > 0 ) {

                // URL 可能是这种格式的："{DemoService_Url}/v20/api/xxxxxxxxxxxxxx"

                string name = url.Substring(1, p - 1);

                // 从配置文件中查找
                string rootUrl = Settings.GetSetting(name);

                if( rootUrl.IsNullOrEmpty() )
                    throw new InvalidOperationException("展开URL时，没有在配置参数中找到参数项：" + name);

                return Urls.Combine(rootUrl, url.Substring(p + 1));
            }
        }

        return url;
    }



    /// <summary>
    /// 计算当前请求的反向代理地址
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GetProxyDestUrl(NHttpRequest request)
    {
        if( _rules == null )
            throw new InvalidOperationException("没有加载任何转发规则，不能执行计算！");

        foreach( var x in _rules.Rules ) {

            if( x.TypeFlag == 5 ) {
                return Urls.Combine(x.Dest, request.PathAndQuery);
            }

            if( x.TypeFlag == 1 && x.Src.Is(request.Path) ) {
                return x.Dest + request.Query;
            }
            if( x.TypeFlag == 2 && x.Src.Is(request.FullPath) ) {
                return x.Dest + request.Query;
            }
            if( x.TypeFlag == 3 || x.TypeFlag == 4 ) {
                string url = x.TypeFlag == 3 ? request.PathAndQuery : request.FullUrl;
                Match m = x.SrcRegex.Match(url);
                if( m.Success ) {
                    string matchUrl = m.Groups["any"].Value;
                    return x.Dest.Replace("[any]", matchUrl);
                }
            }
        }

        return null;
    }
}




/// <summary>
/// ProxyMapRule
/// </summary>
[XmlRoot]
public sealed class ProxyMapRule
{
    /// <summary>
    /// Rules
    /// </summary>
    [XmlArrayItem("rule")]
    public List<ProxyMapRuleItem> Rules { get; set; }
}


/// <summary>
/// ProxyMapRuleItem
/// </summary>
public sealed class ProxyMapRuleItem
{
    /// <summary>
    /// SrcPrefix
    /// </summary>
    [XmlAttribute("src")]
    public string Src { get; set; }

    /// <summary>
    /// DestPrefix
    /// </summary>
    [XmlAttribute("dest")]
    public string Dest { get; set; }


    /// <summary>
    /// URL映射类型标记。
    /// 0：无效规则
    /// 1：一对一URL映射，无host部分
    /// 2：一对一URL映射，包含host部分
    /// 3：正则匹配映射，无host部分
    /// 4：正则匹配映射，包含host部分
    /// 5：无条件匹配(转发所有请求)
    /// </summary>
    internal int TypeFlag { get; set; }


    internal Regex SrcRegex;
}
