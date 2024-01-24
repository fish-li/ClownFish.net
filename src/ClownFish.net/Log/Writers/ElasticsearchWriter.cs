using ClownFish.Http.Clients.Elastic;

namespace ClownFish.Log.Writers;

/// <summary>
/// 将Elasticsearch做为持久化目标的写入器
/// </summary>
internal sealed class ElasticsearchWriter : ILogWriter
{
    private static readonly string s_indexNameTimeFormat = Settings.GetSetting("ClownFish_Log_ES_IndexNameFormat", "-yyyyMMdd");

    private SimpleEsClient _client;

    public void Init(LogConfiguration config, WriterConfig section)
    {
        InternalInit(LoggingOptions.EsSettingName);
    }

    internal bool InternalInit(string settingName)
    {
        EsConnOption opt = EsConnOption.Create(settingName, false);

        if( opt == null ) {
            Console2.Info($"ElasticsearchWriter 不能初始化，因为没有找到 {settingName} 连接配置参数。");
            return false;
        }

        opt.IndexNameTimeFormat = s_indexNameTimeFormat;
        _client = new SimpleEsClient(opt);

        Console2.Info(this.GetType().FullName + " Init OK, IndexNameFormat: " + s_indexNameTimeFormat);
        return true;
    }

    public void WriteList<T>(List<T> list) where T : class, IMsgObject
    {
        if( _client == null )
            return;

        try {
            _client.WriteList(list);
        }
        catch( EsHttpException ex1 ) {
            Console2.Warnning("ElasticsearchWriter.WriteList ERROR: " + ex1.Response);
        }
        catch( Exception ex ) {
            // 这里不显示完整的“调用堆栈”，是因为调用点已经非常明确，完全可以根据下面的“特征字符串”找到是这里发生的异常
            Console2.Warnning("ElasticsearchWriter.WriteList ERROR: " + ex.Message);
        }

        ClownFishCounters.Logging.EsWriteCount.Add(list.Count);
    }
}
