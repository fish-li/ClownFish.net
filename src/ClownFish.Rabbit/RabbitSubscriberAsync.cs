using RabbitMQ.Client.Events;

namespace ClownFish.Rabbit;

/// <summary>
/// RabbitMQ消息订阅管道
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class RabbitSubscriberAsync<T> : IBasicConsumer, IAsyncBasicConsumer where T : class
{
    private readonly RabbitSubscriberArgs _args;
    private readonly AsyncMessagePipeline<T> _pipeline;

    private RabbitConnection _connection;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="args"></param>
    internal RabbitSubscriberAsync(AsyncBaseMessageHandler<T> handler, RabbitSubscriberArgs args)
    {
        _args = args;
        _pipeline = new AsyncMessagePipeline<T>(handler, args.RetryCount, args.RetryWaitMilliseconds);
    }


    /// <summary>
    /// 开始订阅消息
    /// </summary>
    public void Start()
    {
        //连接到RabbitMQ服务器
        _connection = new RabbitConnection(_args.SettingName, "Subscriber-" + _args.QueueName);
        
        // 支持异步消费
        _connection.DispatchConsumersAsync = true;

        // 告诉Rabbit服务，订阅客户端一次只能处理一条消息
        _connection.GetChannel().BasicQos(0, 1, false);

        string consumerTag = $"{EnvUtils.GetAppName()}({EnvUtils.GetHostName()})"; // 显示在 Queues/Consumers 表格中

        //设置消息的订阅者。 消息采用回调方式处理
        bool autoAck = false;
        IDictionary<string, object> arguments = new Dictionary<string, object>();
        arguments["x-cancel-on-ha-failover"] = true;  // https://www.rabbitmq.com/ha.html#cancellation

        _connection.GetChannel().BasicConsume(_args.QueueName, autoAck, consumerTag, arguments, this);

        ClownFishInit.AppExitToken.Register(OnAppEnd);
    }


    private void OnAppEnd()
    {
        if( _connection != null ) {
            _connection.Dispose();
            _connection = null;
        }

        Console2.WriteLine("Application exit, stop RabbitSubscriber: " + _pipeline.HandlerInstance.GetType().FullName);
    }


    private async Task HandleMessage(RabbitMessage message)
    {
        if( ClownFishInit.AppExitToken.IsCancellationRequested )
            return;

        MqRequest request = message.CreateMqRequest<T>(_args.MaxMessageLength);

        try {
            await _pipeline.PushMessage(request);
        }
        catch( Exception ex ) {
            OnError("处理消息失败！", ex);
        }

        SendAck(message.DeliveryTag);
    }
    

    private void SendAck(ulong deliveryTag)
    {
        if( _connection == null )  // OnAppEnd
            return;

        try {
            //确认消息已处理
            _connection.GetChannel().BasicAck(deliveryTag, false);
        }
        catch( Exception ex ) {
            OnError("消息确认失败！", ex);
        }
    }




    private void OnError(string subject, Exception ex)
    {
        Console2.Error($"RabbitSubscriber {subject} 。", ex);
    }


    #region IAsyncBasicConsumer 接口

    IModel IAsyncBasicConsumer.Model => _connection.GetChannel();

    event AsyncEventHandler<ConsumerEventArgs> IAsyncBasicConsumer.ConsumerCancelled {
        add {
            throw new NotImplementedException();
        }

        remove {
            throw new NotImplementedException();
        }
    }

    Task IAsyncBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
        string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
    {

        RabbitMessage message = new RabbitMessage(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
        return HandleMessage(message);
    }

    Task IAsyncBasicConsumer.HandleBasicCancel(string consumerTag)
    {
        // do nothing
        return Task.CompletedTask;
    }

    Task IAsyncBasicConsumer.HandleBasicCancelOk(string consumerTag)
    {
        // do nothing
        return Task.CompletedTask;
    }

    Task IAsyncBasicConsumer.HandleBasicConsumeOk(string consumerTag)
    {
        // do nothing
        return Task.CompletedTask;
    }

    Task IAsyncBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
    {
        // do nothing
        return Task.CompletedTask;
    }


    #endregion




    #region IBasicConsumer 接口

    // 下面的成员永远不会被调用，原因是要实现 IBasicConsumer 这个接口标记。

    IModel IBasicConsumer.Model => throw new NotImplementedException();

    event EventHandler<ConsumerEventArgs> IBasicConsumer.ConsumerCancelled {
        add {
            throw new NotImplementedException();
        }

        remove {
            throw new NotImplementedException();
        }
    }

    void IBasicConsumer.HandleBasicCancel(string consumerTag)
    {
        throw new NotImplementedException();
    }

    void IBasicConsumer.HandleBasicCancelOk(string consumerTag)
    {
        throw new NotImplementedException();
    }

    void IBasicConsumer.HandleBasicConsumeOk(string consumerTag)
    {
        throw new NotImplementedException();
    }

    void IBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, 
        string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
    {
        throw new NotImplementedException();
    }

    void IBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
    {
        throw new NotImplementedException();
    }

    #endregion

}
