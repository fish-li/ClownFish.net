namespace ClownFish.Log.Configuration;

/// <summary>
/// 配置日志的配置信息
/// </summary>
public sealed class PerformanceConfig
{
    /// <summary>
    /// HTTP请求执行的阀值时间，单位：毫秒
    /// </summary>
    [XmlAttribute]
    public int HttpExecute { get; set; } = int.MinValue;


    /// <summary>
    /// 消息订阅处理执行的阀值时间，单位：毫秒
    /// </summary>
    [XmlAttribute]
    public int HandleMessage { get; set; } = int.MinValue;


    // 上面2个属性默认值取 int.MinValue 原因：
    // 如果某个应用程序不需要计算慢请求，可以设置 HttpExecute="0" or HttpExecute="-1"
    // 因此，HttpExecute的默认值不能是 0，否则在 LogInitUtils.MegerConfig(...) 方法中没法知道要不要覆盖!
    // 毕竟 int 类型的默认值就是 0，根本没办法区分，所以为了让 MegerConfig 方法的代码好写，就取了 int.MinValue 这个默认值
    // 另外，如果用 int? 会更麻烦，所以就这样了~~


    internal void CheckOrSetDefault()
    {
        //if( this.HttpExecute <= 0 )
        //    this.HttpExecute = 100;

        //if( this.HandleMessage <= 0 )
        //    this.HandleMessage = 100;
    }

}
