namespace ClownFish.UnitTest.Http.Proxy;

/// <summary>
/// 
/// </summary>
public sealed class ProxyConfig
{
    /// <summary>
    /// 
    /// </summary>
    public List<ProxyItem> Items { get; set; }
}

/// <summary>
/// 
/// </summary>
public sealed class ProxyItem
{
    /// <summary>
    /// 
    /// </summary>
    public string Src { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Dest { get; set; }


    /// <summary>
    /// URL映射类型标记。
    /// 0：单一URL映射，
    /// 1：部分匹配
    /// 2：无条件匹配(转发所有请求)
    /// </summary>
    internal int TypeFlag { get; set; }


    internal ProxyItem Clone()
    {
        if( this.Src.IsNullOrEmpty() || this.Dest.IsNullOrEmpty() )
            return null;

        ProxyItem item = new ProxyItem {
            Dest = this.Dest.TrimEnd('/')
        };

        if( this.Src == "*" || this.Src == "/*" ) {
            item.Src = "*";
            item.TypeFlag = 2;
        }
        else if( this.Src.EndsWith0("/*") ) {
            item.Src = this.Src.Substring(0, this.Src.Length - 1);  //  "/aa/*"  =>  "/aa/"
            item.TypeFlag = 1;
        }
        else {
            item.Src = this.Src;
            item.TypeFlag = 0;
        }

        return item;
    }

}
