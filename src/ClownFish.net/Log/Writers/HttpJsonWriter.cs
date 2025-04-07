namespace ClownFish.Log.Writers;

// 使用方法：
// log.config:  <Writer Name="http" Type="ClownFish.Log.Writers.HttpJsonWriter, ClownFish.net" />
// app.config:  ClownFish_Log_WritersMap = InvokeLog=null;OprLog=http;*=null
// app.config:  Nebula_LogGate_Url = http://nebula-loggate-svc   // 这里需要有个服务端来接收日志数据

internal class HttpJsonWriter : ILogWriter
{
    private string _url;
    private string _urlOprlog;
    private string _urlNebulaLog;

    private static readonly int s_batchSize = Settings.GetUInt("ClownFish_Log_HttpJsonWriter_BatchSize", 3 * 1024 * 1024);
    private static readonly bool s_showError = Settings.GetBool("ClownFish_Log_HttpJsonWriter_ShowError", 1);

    private readonly StringBuilder _buffer = new StringBuilder(s_batchSize);

    void ILogWriter.Init(LogConfiguration config, WriterConfig section)
    {
        string configValue = Settings.GetSetting("Nebula_LogGate_Url")   // 优先采用 Nebula.LogGate 做为服务端接收日志数据
                             ?? Settings.GetSetting("HttpJsonWriter_Target_Url");   // 兼容以前的老参数名称

        if( InitUrl(configValue) == 0 ) {
            Console2.Info("##### 由于没有配置 Nebula_LogGate_Url 参数，HttpJsonWriter 将忽略所有调用！#####");
            return;
        }

        Console2.Info(this.GetType().FullName + " Init OK, upload url: " + _url);
    }

    internal int InitUrl(string configValue)
    {
        string url = (configValue ?? "").TrimEnd('/');

        if( url.IsNullOrEmpty() ) {
            return 0;
        }

        // 允许只配置一个站点根网址，这里就补全完整的调用地址
        if( url.EndsWith1("/{datatype}") == false )
            url = url + "/v20/api/loggate/save/{datatype}";

        _url = url.AddUrlQueryArgs("app", EnvUtils.GetAppName());

        // 下面2个地址使用的机率非常大，所以用2个变量来缓存结果，避免反复调用 string.Replace
        _urlOprlog = _url.Replace("{datatype}", "OprLog");
        _urlNebulaLog = _url.Replace("{datatype}", "NebulaLog");
        return 1;
    }

    internal void SetUrl(string url) => _url = url;


    private string GetInvokeUrl(string dataType)
    {
        return dataType switch {
            "OprLog" => _urlOprlog,
            "NebulaLog" => _urlNebulaLog,
            _ => _url.Replace("{datatype}", dataType)  // TODO: 以后可以优化下，减少不必要的字符串替换
        };
    }

    void ILogWriter.WriteList<T>(List<T> list)
    {
        if( _url.IsNullOrEmpty() || list.IsNullOrEmpty() )
            return;

        string url = GetInvokeUrl(typeof(T).Name);        

        // 如果网络中断，或者服务端挂了，整个数据包就一起丢弃，避免无用的重试。
        try {
            // 按照指定大小，将列表中的元素先做JSON序列化，然后再拼接成一个字符串
            DataSpliter<T> spliter = new DataSpliter<T>(list, s_batchSize, _buffer);

            while( true ) {
                string jsonlPart = spliter.GetNextPart();
                if( jsonlPart.IsNullOrEmpty() ) {
                    break;
                }

                SendBatch<T>(jsonlPart, url);
            }

            ClownFishCounters.Logging.HttpJsonWriteCount.Add(list.Count);
        }
        catch( Exception ex ) {
            if( s_showError ) {
                Console2.Warnning("HttpJsonWriter.WriteList ERROR: " + ex.ToString());
            }
        }
        finally {
            _buffer.Clear();
        }
    }

    private void SendBatch<T>(string jsonlPart, string url)
    {
        byte[] jsonGzip = jsonlPart.ToGzip();

        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = url,
            Format = SerializeFormat.None,
            Data = jsonGzip,
            Timeout = HttpClientDefaults.HttpJsonWriterTimeout
        };

        // 说明：json lines 还没有形成【技术标准】，
        // 有些人建议使用 application/jsonl 例如：https://jsonlines.org/
        // 有些人建议使用 application/json-seq 例如：https://www.atatus.com/glossary/jsonl/
        // 有些人建议使用 application/x-ndjson 例如：http://ndjson.org/，  https://github.com/jshttp/mime-db/issues/224

        // 2024-07-22 补充：按 ChatGPT 说法：
        // application/json-seq:  是一个较为宽泛的媒体类型，可能在实际使用中有不同的实现方式。
        // application/x-ndjson:  是一种更为具体和标准化的格式，要求每个 JSON 对象用换行符分隔，适合于流式处理。
        // 因此后者的定义更明确，这里就采用后者

        httpOption.Headers.Add("Content-Type", RequestContentType.JsonLines);
        httpOption.Headers.Add("Content-Encoding", "gzip");
        httpOption.Headers.Add("x-datatype", typeof(T).FullName);

        SendRequest(httpOption);
    }

    protected virtual void SendRequest(HttpOption httpOption)
    {
        string returnId = Guid.NewGuid().ToString("N");
        httpOption.Headers.Add("x-returnid", returnId);

        try {
            HttpJsonWriterExt.OnSendRequest?.Invoke(httpOption);

            //httpOption.Send(HttpRetry.Create(2, 500));

            HttpResult<string> result = httpOption.GetResult<HttpResult<string>>(HttpRetry.Create(2, 500));

            if( result.Result.TryToInt() < 1 ) {    // 上传日志失败了
                Console2.Info("HttpJsonWriter上传日志时，服务端返回: " + result.Result);
            }

            string[] values = result.Headers.GetValues("x-returnid");
            if( values.IsNullOrEmpty() || values.FirstOrDefault() != returnId )
                throw new InvalidOperationException("日志服务端没有按照约定的方式返回，或者请求没有到达日志服务端(被防火墙拦截)！");
        }
        catch( Exception ex ) {
            if( s_showError ) {
                // 这里不显示完整的“调用堆栈”，是因为调用点已经非常明确，完全可以根据下面的“特征字符串”找到是这里发生的异常
                Console2.Warnning("HttpJsonWriter.SendRequest ERROR: " + ex.Message);
            }
        }
    }
}



/// <summary>
/// HttpJsonWriter的扩展支持类
/// </summary>
public static class HttpJsonWriterExt
{
    /// <summary>
    /// 发送HTTP请求的事件委托。
    /// 典型使用场景：添加“身份凭证”
    /// </summary>
    public static Action<HttpOption> OnSendRequest;
}
