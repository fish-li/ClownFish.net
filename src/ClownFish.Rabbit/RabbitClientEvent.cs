namespace ClownFish.Rabbit;

internal static class RabbitClientEvent
{
    //private static readonly DiagnosticListener s_diagnosticSource = new DiagnosticListener("Nebula.RabbitClientEvent");

    /// <summary>
    /// 每次发送一条消息后触发
    /// </summary>
    public static event EventHandler<SendRabbitMessageEventArgs> OnSendMessage;

    internal static void SendMessage(RabbitClient client, string server, string exchange, string routingKey, object data, int dataLen, DateTime start, Exception ex)
    {
        SendRabbitMessageEventArgs e = null;

        EventHandler<SendRabbitMessageEventArgs> handler = OnSendMessage;
        if( handler != null ) {
            if( e == null )
                e = CreateSendMessageEventArgs(server, exchange, routingKey, data, dataLen, start, ex);
            handler(client, e);
        }


        //if( s_diagnosticSource.IsEnabled() ) {
        //    if( e == null )
        //        e = CreateSendMessageEventArgs(server, exchange, routingKey, data, dataLen, start, ex);
        //    s_diagnosticSource.Write("OnSendMessage", e);
        //}

    }

    private static SendRabbitMessageEventArgs CreateSendMessageEventArgs(string server, string exchange, string routingKey, object data, int dataLen, DateTime start, Exception ex)
    {
        return new SendRabbitMessageEventArgs {
            Server = server,
            Exchange = exchange,
            RoutingKey = routingKey,
            Data = data,
            DataLen = dataLen,
            StartTime = start,
            EndTime = DateTime.Now,
            Exception = ex
        };
    }


}



