using ClownFish.Http.Clients.RabbitMQ;
using ClownFish.MQ;

namespace ClownFish.Log.Writers;

/// <summary>
/// 将RabbitMQ做为持久化目标的写入器
/// </summary>
internal sealed class RabbitHttpWriter : ILogWriter
{
    private RabbitHttpClient _client;    

    public void Init(LogConfiguration config, WriterConfig section)
    {
        InternalInit(config, RabbitOption.DefaultSettingName);
    }


    internal int InternalInit(LogConfiguration config, string settingName)
    {
        string configValue = Settings.GetSetting(settingName);

        if( configValue.IsNullOrEmpty() ) {
            Console2.Info($"RabbitHttpWriter 不能初始化，因为没有找到 {settingName} 的连接配置参数。");
            return -1;
        }

        RabbitOption option = configValue.ToObject<RabbitOption>();
        if( option.Server.IsNullOrEmpty() ) {
            Console2.Info($"RabbitHttpWriter 不能初始化，因为连接配置参数 {settingName} 的 Server 为空。");
            return -2;
        }


        // 创建客户端连接
        _client = new RabbitHttpClient(option);

        // 触发连接打开
        _client.TestConnection();

        // 为每种日志的数据类型创建对应的队列
        AutoCreateQueue(config);

        Console2.Info(this.GetType().FullName + " Init OK.");
        return 1;
    }


    private void AutoCreateQueue(LogConfiguration config)
    {
        // 检查每种数据类型，判断它们有没有要求写入到Rabbit
        foreach( var item in config.Types ) {

            // for example:  <Type DataType="xxxxxx" Writers="Json,Rabbit" />
            if( item.Writers.ToArray2().Contains("RabbitHttp", StringComparer.OrdinalIgnoreCase) ) {

                string queue = item.TypeObject.GetQueueName();
                string bindingKey = queue;
                _client.CreateQueueBind(queue, null, bindingKey, null);
            }
        }
    }



    /// <summary>
    /// 批量写入日志信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public void Write<T>(List<T> list) where T : class, IMsgObject
    {
        if( _client == null )
            return;

        // 将10条InvokeLog合并在一起发到队列，以LIST方式发送
        BatchWritableAttribute attr = typeof(T).GetMyAttribute<BatchWritableAttribute>();
        if( attr != null ) {
            BatchWrite(list, attr.BatchSize.Min(10));
        }
        else {
            foreach( T x in list ) {
                _client.SendMessage(x);
            }
        }

        ClownFishCounters.Logging.Rabbit2WriteCount.Add(list.Count);
    }


    private void BatchWrite<T>(List<T> list, int batchSize)
    {
        string routingKey = typeof(T).GetQueueName();

        if( list.Count <= batchSize ) {
            _client.SendMessage(list, null, routingKey);
        }
        else {
            List<List<T>> listlist = list.SplitList(int.MaxValue, batchSize);

            foreach( List<T> listX in listlist ) {
                _client.SendMessage(listX, null, routingKey);
            }
        }
    }

}
