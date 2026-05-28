using ClownFish.Http.Clients.RabbitMQ;
using ClownFish.MQ;

namespace ClownFish.Log.Writers;

/// <summary>
/// 将RabbitMQ做为持久化目标的写入器
/// </summary>
internal sealed class RabbitHttpWriter : ILogWriter
{
    private static readonly bool s_showError = Settings.GetBool("ClownFish_Log_RabbitHttpWriter_ShowError", 1);

    private RabbitHttpClient _client;

    public void Init(LogConfiguration config, Type dataType)
    {
        InternalInit(dataType, LoggingOptions.RabbitSettingName);
    }

    internal int InternalInit(Type dataType, string settingName)
    {
        string configValue = Settings.GetSetting(settingName);

        if( configValue.IsNullOrEmpty() ) {
            Console2.Info($"##### RabbitHttpWriter 未能完成初始化，因为没有找到 {settingName} 的连接配置参数！");
            return -1;
        }

        RabbitOption option = configValue.ToObject<RabbitOption>();
        if( option.Server.IsNullOrEmpty() ) {
            Console2.Info($"##### RabbitHttpWriter 未能完成初始化，因为连接配置参数 {settingName} 的 Server 为空！");
            return -2;
        }


        // 创建客户端连接
        _client = new RabbitHttpClient(option);

        // 触发连接打开
        _client.TestConnection();

        // 为每种日志的数据类型创建对应的队列
        AutoCreateQueue(dataType);

        Console2.Info($"{this.GetType().FullName} Init OK, conn-config: {option}, ShowError: {s_showError}");
        return 1;
    }

    private void AutoCreateQueue(Type dataType)
    {
        string queue = dataType.GetQueueName();
        string bindingKey = queue;
        _client.CreateQueueBind(queue, null, bindingKey, null);
    }

    //private void AutoCreateQueue(LogConfiguration config)
    //{
    //    // 检查每种数据类型，判断它们有没有要求写入到Rabbit
    //    foreach( var item in config.Types ) {

    //        // for example:  <Type DataType="xxxxxx" Writers="Json,Rabbit" />
    //        if( item.Writers.ToArray2().Contains("RabbitHttp", StringComparer.OrdinalIgnoreCase) ) {

    //            string queue = item.TypeObject.GetQueueName();
    //            string bindingKey = queue;
    //            _client.CreateQueueBind(queue, null, bindingKey, null);
    //        }
    //    }
    //}



    /// <summary>
    /// 批量写入日志信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public void WriteList<T>(List<T> list) where T : class, IMsgObject
    {
        if( _client == null )
            return;

        BatchWritableAttribute attr = typeof(T).GetMyAttribute<BatchWritableAttribute>();
        if( attr != null ) {
            string routingKey = typeof(T).GetQueueName();

            SendMessage0(list, null, routingKey);
        }
        else {
            foreach( T x in list ) {
                SendMessage0(x);
            }
        }

        ClownFishCounters.Logging.Rabbit2WriteCount.Add(list.Count);
    }

    private void SendMessage0(object data, string exchange = null, string routingKey = null)
    {
        try {
            _client.SendMessage(data, exchange, routingKey);
        }
        catch( Exception ex ) {
            if( s_showError ) {
                // 这里不显示完整的“调用堆栈”，是因为调用点已经非常明确，完全可以根据下面的“特征字符串”找到是这里发生的异常
                Console2.Warnning("RabbitHttpWriter.SendMessage0 ERROR: " + ex.Message);
            }
        }
    }
}
