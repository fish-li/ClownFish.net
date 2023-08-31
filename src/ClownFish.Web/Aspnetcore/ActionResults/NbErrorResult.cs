namespace ClownFish.Web.AspnetCore.ActionResults;

/// <summary>
/// NbErrorResult
/// </summary>
public sealed class NbErrorResult : ActionResult
{
    private readonly Exception _exception;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="ex"></param>
    public NbErrorResult(Exception ex)
    {
        if( ex == null) 
            throw new ArgumentNullException(nameof(ex));
        
        _exception = ex;
    }

    /// <summary>
    /// ExecuteResultAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        NHttpContext httpContextNetCore = new HttpContextNetCore(context.HttpContext);

        await httpContextNetCore.Http500Async(_exception);
    }

    /// <summary>
    /// ExecuteResult
    /// </summary>
    /// <param name="context"></param>
    public override void ExecuteResult(ActionContext context)
    {
        throw new NotImplementedException();
    }
}
