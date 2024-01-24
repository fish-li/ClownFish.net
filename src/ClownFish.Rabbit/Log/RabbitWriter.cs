using ClownFish.Log.Writers;

namespace ClownFish.Rabbit.Log;
/// <summary>
/// 将日志记录到RabbitMQ的写入器
/// </summary>
internal sealed class RabbitWriter : ILogWriter
{
    private bool _inited;
    private RabbitClient _client;

    public void Init(LogConfiguration config, WriterConfig section)
    {
        InternalInit(config);
    }

    private void InternalInit(LogConfiguration config)
    {
        string configValue = Settings.GetSetting(LoggingOptions.RabbitSettingName);

        if( configValue.IsNullOrEmpty() ) {
            Console2.Info($"RabbitWriter 不能初始化，因为没有找到 {LoggingOptions.RabbitSettingName} 的连接配置参数。");
            return;
        }

        RabbitOption option = configValue.ToObject<RabbitOption>();
        if( option.Server.IsNullOrEmpty() ) {
            Console2.Info($"RabbitWriter 不能初始化，因为连接配置参数 {LoggingOptions.RabbitSettingName} 的 Server 为空。");
            return;
        }


        // 创建客户端连接
        _client = new RabbitClient(option, "ClownFish_Log_RabbitWriter");

        // 触发连接打开
        _client.TestConnection();

        // 为每种日志的数据类型创建对应的队列
        AutoCreateQueue(config);

        Console2.Info(this.GetType().FullName + " Init OK, config: " + option.ToString());
        _inited = true;
    }


    private void AutoCreateQueue(LogConfiguration config)
    {
        // 检查每种数据类型，判断它们有没有要求写入到Rabbit
        foreach( var item in config.Types ) {

            // for example:  <Type DataType="xxxxxx" Writers="Json,Rabbit" />
            if( item.Writers.ToArray2().Contains("Rabbit", StringComparer.OrdinalIgnoreCase) ) {
                _client.CreateQueueBind(item.GetDataTypeTypeObject());
            }
        }
    }



    /// <summary>
    /// 批量写入日志信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public void WriteList<T>(List<T> list) where T : class, IMsgObject
    {
        if( _inited == false )
            return;

        // 将10条InvokeLog合并在一起发到队列，以LIST方式发送
        BatchWritableAttribute attr = typeof(T).GetMyAttribute<BatchWritableAttribute>();
        if( attr != null ) {
            BatchWrite(list, _client, attr.BatchSize.Min(10));
        }
        else {
            foreach( T x in list ) {
                _client.SendMessage(x);
            }
        }

        ClownFishCounters.Logging.RabbitWriteCount.Add(list.Count);
    }


    private void BatchWrite<T>(List<T> list, RabbitClient client, int batchSize)
    {
        string routingKey = typeof(T).GetQueueName();

        if( list.Count <= batchSize ) {
            client.SendMessage(list.ToJson(), null, routingKey);
        }
        else {
            List<List<T>> listlist = list.SplitList(int.MaxValue, batchSize);

            foreach( List<T> listX in listlist ) {
                client.SendMessage(listX.ToJson(), null, routingKey);
            }
        }
    }


}
