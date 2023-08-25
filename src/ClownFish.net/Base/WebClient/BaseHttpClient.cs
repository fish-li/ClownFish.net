namespace ClownFish.Base.WebClient;

/// <summary>
/// HttpClient基类
/// </summary>
internal abstract class BaseHttpClient
{
    static BaseHttpClient()
    {
        SysNetInitializer.Init();
    }


    internal string OperationId { get; private set; } = Guid.NewGuid().ToString("N");

    internal HttpOption HttpOption { get; private set; }

    internal DateTime StartTime { get; private set; } = DateTime.MinValue;

    internal bool IsAsync { get; set; }

    internal void SetStartTime()
    {
        // 执行重试的时候会再次进入这里

        //if( this.StartTime != DateTime.MinValue )
        //    throw new InvalidOperationException("不允许重复当前方法！");

        this.StartTime = DateTime.Now;
    }

#if NETFRAMEWORK
    /// <summary>
    /// HttpWebRequest 实例引用
    /// </summary>
    public System.Net.HttpWebRequest Request { get; protected set; }

#else
    /// <summary>
    /// HttpRequestMessage 实例引用
    /// </summary>
    public System.Net.Http.HttpRequestMessage Request { get; protected set; }

#endif


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="option"></param>
    protected BaseHttpClient(HttpOption option)
    {
        option.CheckInput();
        this.HttpOption = option;
    }

    public abstract T Send<T>();

    public abstract Task<T> SendAsync<T>();


    /// <summary>
    /// 从HttpWebResponse读取结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <returns></returns>
    protected T GetResult<T>(HttpWebResponse response)
    {
        if( response == null )
            throw new ArgumentNullException("response");


        if( typeof(T) == typeof(ClownFish.Base.Void) )
            return (T)(object)ClownFish.Base.Void.Value;

        using( ResponseReader reader = new ResponseReader(response, this.HttpOption.AutoDecompressResponse) ) {
            return reader.Read<T>();
        }
    }

}





