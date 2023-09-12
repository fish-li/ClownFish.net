namespace ClownFish.Rabbit;

/// <summary>
/// Rabbit日志工具类
/// </summary>
public static class RabbitLogger
{
    /// <summary>
    /// Init
    /// </summary>
    public static void Init()
    {
        RabbitClientEvent.OnSendMessage += RabbitClientEventOnSendMessage;
    }

    private static void RabbitClientEventOnSendMessage(object sender, SendRabbitMessageEventArgs e)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        StepItem step = StepItem.CreateNew(e.StartTime);
        step.StepKind = StepKinds.Rabbit;
        step.StepName = "SendRabbitMessage";
        step.SetException(e.Exception);
        step.Detail = $"server: {e.Server}\nexchange: {e.Exchange}\nrouting: {e.RoutingKey}\ndatatype: {e.Data?.GetType()?.FullName}\nlength: {e.DataLen}";

        step.End(e.EndTime);

        scope.AddStep(step);
    }
}
