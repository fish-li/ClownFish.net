namespace ClownFish.Log.Logging;

/// <summary>
/// HttpClientLogger
/// </summary>
internal static class HttpClientLogger
{
    internal static void Init()
    {
        HttpClientEvent.OnRequestFinished += HttpClientOnRequestFinished;
    }


    private static void HttpClientOnRequestFinished(object sender, RequestFinishedEventArgs e)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        HttpPipelineContext httpPipeline = HttpPipelineContext.Get();
        if( httpPipeline != null && httpPipeline.HttpContext.IsTransfer )
            return;

        BaseHttpClient client = (BaseHttpClient)sender;

        StepItem step = StepItem.CreateNew(e.StartTime, e.OperationId);
        step.StepKind = StepKinds.HttpRpc;

        step.StepName = client.IsAsync ? "SendHttpAsync" : "SendHttp";
        step.IsAsync = client.IsAsync ? 1 : 0;
        step.SetException(e.Exception);
        step.Cmdx = e;

        step.End(e.EndTime);

        scope.AddStep(step);
    }



}
