namespace ClownFish.Http.Clients.RabbitMQ;

/// <summary>
/// 
/// </summary>
public static class RabbitmqUtils
{
    /// <summary>
    /// 设置队列类型
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="defaultQueueType"></param>
    /// <returns></returns>
    public static IDictionary<string, object> SetQueueType(IDictionary<string, object> arguments, string defaultQueueType)
    {
        if( defaultQueueType.HasValue() ) {
            if( arguments == null ) {
                arguments = new Dictionary<string, object>(1);
                arguments["x-queue-type"] = defaultQueueType;
            }
            else {
                // 仅当调用时没有指定时，这里才添加
                if( arguments.ContainsKey("x-queue-type") == false ) {
                    arguments["x-queue-type"] = defaultQueueType;
                }
            }
        }

        return arguments;
    }
}
