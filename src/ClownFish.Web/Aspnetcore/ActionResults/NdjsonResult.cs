namespace ClownFish.Web.Aspnetcore.ActionResults;

public sealed class NdjsonResult : ActionResult
{
    private readonly ICollection _list;

    public NdjsonResult(ICollection list)
    {
        _list = list;
    }

    /// <summary>
    /// ExecuteResultAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        NHttpContext httpContextNetCore = HttpPipelineContext.Get2().HttpContext;
        await httpContextNetCore.HttpGzipNdjsonReply(_list);
    }

    /// <summary>
    /// ExecuteResult
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void ExecuteResult(ActionContext context)
    {
        throw new NotImplementedException();
    }

}
