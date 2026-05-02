using ClownFish.Http.Clients.Elastic;

namespace ClownFish.Log.Writers;

internal sealed class OprlogEsWriter : ILogWriter
{
    private static readonly bool s_showError = Settings.GetBool("ClownFish_Log_OprlogEsWriter_ShowError", 1);

    private SimpleEsClient _clientAll;
    private SimpleEsClient _clientSlow;
    private SimpleEsClient _clientError;
    private bool _inited = false;


    public void Init(LogConfiguration config, Type dataType)
    {
        InternalInit(LoggingOptions.EsSettingName);
    }

    internal bool InternalInit(string settingName)
    {
        EsConnOption opt1 = EsConnOption.Create(settingName, false);

        if( opt1 == null ) {
            Console2.Info($"##### OprlogEsWriter 未能完成初始化，因为没有找到 {settingName} 连接配置参数！");
            return false;
        }

        _clientAll = new SimpleEsClient(opt1, ElasticsearchWriter.IndexNameTimeFormat);

        EsConnOption opt2 = EsConnOption.Create(settingName, true);
        _clientSlow = new SimpleEsClient(opt2, "xx");  // 写入数据时直接指定 index-name，所以第2个参数不起作用

        EsConnOption opt3 = EsConnOption.Create(settingName, true);
        _clientError = new SimpleEsClient(opt3, "xx");  // 写入数据时直接指定 index-name，所以第2个参数不起作用

        Console2.Info($"{this.GetType().FullName} Init OK, es url: {opt1.Url}, IndexNameFormat: {ElasticsearchWriter.IndexNameTimeFormat}");
        _inited = true;
        return true;
    }

    public void WriteList<T>(List<T> list) where T : class, IMsgObject
    {
        if( _inited == false || list.IsNullOrEmpty() )
            return;

        // 这个写入器只处理 OprLog 类型的数据
        if( typeof(T) != typeof(OprLog) )
            return;

        List<OprLog> list2 = list.Select(x => (OprLog)(object)x).ToList();
        WriteOprLogList(list2, _clientAll, null);

        // 由于 性能日志 和 异常日志 的数量不会太多，所以就一天生成一个索引，也不做配置参数了(主要是取名麻烦)
        string indexNamePostfix = DateTime.Now.ToString("-yyyyMMdd");

        List<OprLog> list3 = list2.Where(x => x.IsSlow == 1).ToList();
        WriteOprLogList(list3, _clientSlow, "slow-oprlog" + indexNamePostfix);

        List<OprLog> list4 = list2.Where(x => x.HasError == 1).ToList();
        WriteOprLogList(list4, _clientError, "error-oprlog" + indexNamePostfix);
    }


    private void WriteOprLogList(List<OprLog> list, SimpleEsClient client, string indexName)
    {
        try {
            client.WriteList(list, indexName);
        }
        catch( EsHttpException ex1 ) {
            if( s_showError ) {
                Console2.Warnning($"OprlogEsWriter.WriteList ERROR ({ex1.GetType().FullName}): {ex1.Response}");
            }
        }
        catch( RemoteWebException ex2 ) {
            if( s_showError ) {
                Console2.Warnning($"OprlogEsWriter.WriteList ERROR ({ex2.GetType().FullName}): {ex2.Message} \r\n [ResponseText]: {ex2.ResponseText}");
            }
        }
        catch( Exception ex ) {
            if( s_showError ) {
                // 这里不显示完整的“调用堆栈”，是因为调用点已经非常明确，完全可以根据下面的“特征字符串”找到是这里发生的异常
                Console2.Warnning($"OprlogEsWriter.WriteList ERROR ({ex.GetType().FullName}): {ex.Message}");
            }
        }

        ClownFishCounters.Logging.EsWriteCount.Add(list.Count);
    }
}

