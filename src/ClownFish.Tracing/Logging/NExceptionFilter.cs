namespace ClownFish.Tracing.Logging;

public sealed class NExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Exception ex = context.Exception;
        if( ex != null ) {

            HttpPipelineContext pipelineContext = HttpPipelineContext.Get();
            if( pipelineContext != null ) {

                pipelineContext.SetException(ex);
                Console2.Debug("##### Execute: NExceptionFilter: " + pipelineContext.HttpContext.Request.FullPath);
            }            
        }
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        OnException(context);
        return Task.CompletedTask;
    }
}
