//namespace ClownFish.Log.Writers;

///// <summary>
///// 将日志记录以HTTP形式发送到Web服务器
///// </summary>
//internal sealed class HttpWriter : ILogWriter
//{
//    /// <summary>
//    /// HttpWriterClient实例
//    /// </summary>
//    private HttpWriterClient _client = null;


//    /// <summary>
//    /// 初始化
//    /// </summary>
//    /// <param name="config"></param>
//    /// <param name="section"></param>
//    [MethodImpl(MethodImplOptions.Synchronized)]
//    public void Init(LogConfiguration config, WriterConfig section)
//    {
//        // 避免重复调用
//        if( _client != null )
//            return;

//        HttpWriterClient client = new HttpWriterClient();
//        client.Init(section, true);

//        _client = client;
//    }

   


//    /// <summary>
//    /// 批量写入日志信息
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="list"></param>
//    public void Write<T>(List<T> list) where T : class, ITime
//    {
//        foreach(var info in list)
//            _client?.AddMessage(info);
//    }




//}
