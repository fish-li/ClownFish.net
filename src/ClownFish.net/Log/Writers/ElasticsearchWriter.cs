using ClownFish.Http.Clients.Elastic;

namespace ClownFish.Log.Writers;

/// <summary>
/// 将Elasticsearch做为持久化目标的写入器
/// </summary>
internal sealed class ElasticsearchWriter : ILogWriter
{
    internal static readonly string IndexNameTimeFormat = Settings.GetSetting("ClownFish_Log_ES_IndexNameFormat", "-yyyyMMdd");
    private static readonly bool s_showError = Settings.GetBool("ClownFish_Log_ElasticsearchWriter_ShowError", 1);

    private SimpleEsClient _client;

    public void Init(LogConfiguration config, Type dataType)
    {
        InternalInit(LoggingOptions.EsSettingName);
    }

    internal bool InternalInit(string settingName)
    {
        EsConnOption opt = EsConnOption.Create(settingName, false);

        if( opt == null ) {
            Console2.Info($"##### ElasticsearchWriter 未能完成初始化，因为没有找到 {settingName} 连接配置参数！");
            return false;
        }

        _client = new SimpleEsClient(opt, IndexNameTimeFormat);

        Console2.Info(this.GetType().FullName + " Init OK, IndexNameFormat: " + IndexNameTimeFormat);
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
            if( s_showError ) {
                Console2.Warnning("ElasticsearchWriter.WriteList ERROR: " + ex1.Response);
            }
        }
        catch( Exception ex ) {
            if( s_showError ) {
                // 这里不显示完整的“调用堆栈”，是因为调用点已经非常明确，完全可以根据下面的“特征字符串”找到是这里发生的异常
                Console2.Warnning("ElasticsearchWriter.WriteList ERROR: " + ex.Message);
            }
        }

        ClownFishCounters.Logging.EsWriteCount.Add(list.Count);
    }
}
