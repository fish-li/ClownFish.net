using ClownFish.Http.Clients.Elastic;

namespace ClownFish.Log.Writers;

/// <summary>
/// 将Elasticsearch做为持久化目标的写入器
/// </summary>
internal sealed class ElasticsearchWriter : ILogWriter
{
    private static readonly string s_settingName = "ClownFish_Log_Elasticsearch";
    private static readonly string s_indexNameTimeFormat = Settings.GetSetting("ClownFish_Log_ES_IndexNameFormat", "-yyyyMMdd-HH");

    private SimpleEsClient _client;

    public void Init(LogConfiguration config, WriterConfig section)
    {
        InternalInit(s_settingName);
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

        Console2.Info(this.GetType().FullName + " Init OK.");
        return true;
    }

    public void Write<T>(List<T> list) where T : class, IMsgObject
    {
        if( _client == null )
            return;

        _client.WriteList(list);

        ClownFishCounters.Logging.EsWriteCount.Add(list.Count);
    }
}
