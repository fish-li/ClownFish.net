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
        EsConnOption opt = GetConnOption1() ?? GetConnOption2();

        if( opt == null ) {
            Console2.Info($"ElasticsearchWriter 不能初始化，因为没有找到 {ElasticsearchSettingName} 连接配置参数。");
            return false;
        }

        _client = new SimpleEsClient(opt);

        Console2.Info(this.GetType().FullName + " Init OK.");
        return true;
    }

    private EsConnOption GetConnOption1()
    {
        return Settings.GetSetting<EsConnOption>(ElasticsearchSettingName, false);
    }

    private EsConnOption GetConnOption2()
    {
        DbConfig dbConfig = DbConnManager.GetAppDbConfig(ElasticsearchSettingName, false);

        if( dbConfig == null )
            return null;

        return new EsConnOption {
            Server = dbConfig.Server,
            UserName = dbConfig.UserName,
            Password = dbConfig.Password,
            TimeoutMs = RequestTimeoutMs,
            IndexNameTimeFormat = IndexNameTimeFormat,
        };
    }

    public void Write<T>(List<T> list) where T : class, IMsgObject
    {
        if( _client == null )
            return;

        ClownFishCounters.Logging.EsCount.Add(list.Count);

        _client.WriteList(list);
    }
}
