using ClownFish.Http.Clients.Elastic;

namespace ClownFish.Log.Writers;

/// <summary>
/// 将Elasticsearch做为持久化目标的写入器
/// </summary>
internal sealed class ElasticsearchWriter : ILogWriter
{
    // 下面3个变量故意不加 readonly，允许在项目中直接修改
    internal static string ElasticsearchSettingName = "ClownFish_Log_Elasticsearch";
    internal static string IndexNameTimeFormat = Settings.GetSetting("ClownFish_Log_ES_IndexNameFormat", "-yyyyMMdd");  // 建议值：-yyyyMMdd-HH
    internal static int RequestTimeoutMs = Settings.GetUInt("ClownFish_Log_ES_TimeoutMs", 5_000);

    private SimpleEsClient _client;

    public void Init(LogConfiguration config, WriterConfig section)
    {
        InternalInit();
    }

    private bool InternalInit()
    {
        EsConnOption opt = EsConnOption.Create(ElasticsearchSettingName, false);

        if( opt == null ) {
            Console2.Info($"ElasticsearchWriter 不能初始化，因为没有找到 {ElasticsearchSettingName} 连接配置参数。");
            return false;
        }

        opt.TimeoutMs = ElasticsearchWriter.RequestTimeoutMs;
        opt.IndexNameTimeFormat = ElasticsearchWriter.IndexNameTimeFormat;
        _client = new SimpleEsClient(opt);

        Console2.Info(this.GetType().FullName + " Init OK.");
        return true;
    }

    public void Write<T>(List<T> list) where T : class, IMsgObject
    {
        if( _client == null )
            return;

        ClownFishCounters.Logging.EsWriteCount.Add(list.Count);

        _client.WriteList(list);
    }
}
