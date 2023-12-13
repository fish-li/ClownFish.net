namespace ClownFish.Log.Writers;

// 使用方法：
// log.config:  <Writer Name="http" Type="ClownFish.Log.Writers.HttpJsonWriter, ClownFish.net" />
// app.config:  ClownFish_Log_WritersMap = InvokeLog=null;OprLog=http;*=null
// app.config:  HttpJsonWriter_Target_Url = http://xxxx.com/v20/api/log/save/{datatype}

internal class HttpJsonWriter : ILogWriter
{
    private string _url;

    private static readonly int s_batchSize = Settings.GetUInt("HttpJsonWriter_BatchSize", 5 * 1024 * 1024);

    private readonly StringBuilder _buffer = new StringBuilder(s_batchSize);

    public void Init(LogConfiguration config, WriterConfig section)
    {
        string url = Settings.GetSetting("HttpJsonWriter_Target_Url");

        if( url.IsNullOrEmpty() ) {
            Console2.Info("##### 由于没有配置 HttpJsonWriter_Target_Url 参数，HttpJsonWriter 将忽略所有调用！#####");
            return;
        }

        _url = url.AddUrlQueryArgs("app", EnvUtils.GetAppName());
        Console2.Info(this.GetType().FullName + " Init OK.");
    }

    internal void SetUrl(string url) => _url = url;

    void ILogWriter.Write<T>(List<T> list)
    {
        if( _url.IsNullOrEmpty() || list.IsNullOrEmpty() )
            return;

        string url = _url.Replace("{datatype}", typeof(T).Name);

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
            Console2.Warnning("HttpJsonWriter.WriteList ERROR: " + ex.ToString());
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

        httpOption.Headers.Add("Content-Type", "application/json-seq");
        httpOption.Headers.Add("Content-Encoding", "gzip");
        httpOption.Headers.Add("x-datatype", typeof(T).FullName);

        SendRequest(httpOption);
    }

    protected virtual void SendRequest(HttpOption httpOption)
    {
        httpOption.Send(HttpRetry.Create(2, 500));
    }
}
