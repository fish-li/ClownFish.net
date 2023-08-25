//namespace ClownFish.Log.Writers;

///// <summary>
///// 发送日志数据的客户端
///// </summary>
//internal sealed class HttpWriterClient
//{
//    private string _url = null;
//    private SerializeFormat _format = SerializeFormat.None;
//    private List<NameValue> _header = null;

//    private int _retryCount = 0;
//    private int _retryWaitMillisecond = 0;

//    /// <summary>
//    /// 队列中允许的最大长度，如果超过这个长度，队列就不接收消息。避免占用大量内存而导致OOM
//    /// </summary>
//    private static readonly int s_maxQueueLength = LocalSettings.GetSetting("ClownFish_Log_HttpWriterClient_MaxQueueLength").TryToUInt(1000);
//    /// <summary>
//    /// 每次上传后的休眠时间
//    /// </summary>
//    private static readonly int s_waitMillisecond = LocalSettings.GetSetting("ClownFish_Log_HttpWriterClient_WaitMillisecond").TryToUInt(500);

//    private readonly ObjectQueue _messageQueue = new ObjectQueue(s_maxQueueLength);

//    public readonly ValueCounter InCount = new ValueCounter(nameof(InCount));
//    public readonly ValueCounter OutCount = new ValueCounter(nameof(OutCount));
//    public readonly ValueCounter ErrCount = new ValueCounter(nameof(ErrCount));

//    private bool _end = false;  // 单元测试用的变量，用反射的方式修改。

//    private Thread _thread = null;
//    private bool _threadInited = false;
//    private readonly object _lock = new object();



//    [MethodImpl(MethodImplOptions.Synchronized)]
//    internal void Init(WriterConfig section, bool startThead)
//    {
//        string url = section.GetOptionValue("url");
//        if( string.IsNullOrEmpty(url) )
//            throw new LogConfigException("日志配置文件中，没有为HttpWriter指定url参数。");

//        string format = section.GetOptionValue("format");
//        _format = (string.IsNullOrEmpty(format) || format.Is("json")) ? SerializeFormat.Json : SerializeFormat.Xml;


//        _retryCount = section.GetOptionValue("retry-count").TryToUInt(10);
//        _retryWaitMillisecond = section.GetOptionValue("retry-wait-millisecond").TryToUInt(1000);

//        _header = ReadHttpArgs(section, "header:");

//        List<NameValue> queryString = ReadHttpArgs(section, "querystring:");
//        _url = url.AddUrlQueryArgs(queryString);

//        if( startThead )
//            StartThread();

//        Console2.Info("HttpWriterClient Init OK.");
//        _end = false;  // 不加这行的话，VS就提示这个变量要加 readonly，也很蛋疼~~
//    }


//    /// <summary>
//    /// 读取指定前缀的配置参数
//    /// </summary>
//    /// <param name="config"></param>
//    /// <param name="flag"></param>
//    /// <returns></returns>
//    private List<NameValue> ReadHttpArgs(WriterConfig config, string flag)
//    {
//        List<NameValue> list = new List<NameValue>();

//        foreach( var item in config.Options ) {
//            string key = item.Key;

//            if( key.StartsWith(flag, StringComparison.Ordinal) ) {

//                key = key.Substring(flag.Length);
//                if( key.Trim().Length == 0 )
//                    continue;

//                list.Add(new NameValue { Name = key, Value = item.Value });
//            }
//        }

//        if( list.Count == 0 )
//            return null;
//        else
//            return list;
//    }

    

//    /// <summary>
//    /// 添加一条消息到待发送队列
//    /// </summary>
//    /// <param name="message"></param>
//    public void AddMessage(object message)
//    {
//        if( message == null )
//            return;
        

//        _messageQueue.Enqueue(message);
//        InCount.Increment();
//    }


//    /// <summary>
//    /// 启动后台发送线程
//    /// </summary>
//    private void StartThread()
//    {
//        if( _threadInited == false ) {
//            lock( _lock ) {
//                if( _threadInited == false ) {

//                    using( ExecutionContext.SuppressFlow() ) {

//                        _thread = new Thread(ThreadMethod);
//                        _thread.IsBackground = true;
//                        _thread.Name = "HttpWriterClient";
//                        _thread.Start();
//                    }

//                    _threadInited = true;
//                }
//            }
//        }
//    }


//    private void ThreadMethod()
//    {
//        while( true ) {
//            SendMessageToServer();

//            if( _end )
//                return;

//            // 间隔 500 毫秒执行
//            Thread.Sleep(s_waitMillisecond);
//        }
//    }


//    private void SendMessageToServer()
//    {
//        while( true ) {
//            object message = _messageQueue.Dequeue();
//            if( message == null )
//                break;

//            try {
//                SendData(message);
//                OutCount.Increment();
//            }
//            catch {
//                ErrCount.Increment();
//                // 发送失败就忽略错误，不能让后台线程崩溃！
//            }
//        }

//    }

//    /// <summary>
//    /// 发送HTTP请求
//    /// </summary>
//    /// <param name="data"></param>
//    public void SendData(object data)
//    {
//        if( data == null )
//            return;

//        HttpOption httpOption = CreateHttpOption(data);
//        if( httpOption == null )
//            return;

//        Retry retry = Retry.Create(_retryCount, _retryWaitMillisecond);
//        httpOption.Send(retry);
//    }


//    /// <summary>
//    /// 创建HttpOption实例
//    /// </summary>
//    /// <param name="data"></param>
//    /// <returns></returns>
//    public HttpOption CreateHttpOption(object data)
//    {
//        //string url = _url.ConcatQueryStringArgs(HttpHeaders.DataType, data.GetType().Name);
        
//        HttpOption httpOption = new HttpOption();
//        httpOption.Url = _url;
//        httpOption.Method = "POST";
//        httpOption.Format = _format;
//        httpOption.Data = data;

//        if( _header != null && _header.Count > 0 ) {
//            foreach( var item in _header )
//                httpOption.Headers.Add(item.Name.UrlEncode(), item.Value.UrlEncode());
//        }

//        httpOption.Headers.Add(HttpHeaders.XRequest.DataType, data.GetType().Name.UrlEncode());
//        return httpOption;
//    }


//}
