using ClownFish.Web.AspnetCore.ActionResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClownFish.Web.AspnetCore.Filters;

public sealed class SimpleResultFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // Action的返回结果类型可能是一些简单的数据类型，例如： int,long
        // 它们在Action结束后会包装成 ObjectResult，最后由JsonOutputFormatter 以 Content-Type: application/json; charset=utf-8 的形式输出，
        // 这样做就非常2B了，明明就是一个数字！
        // 所以这里要做的事情就是把这种简单数据结果以 Content-Type: text/plain 的形式输出

        if( context.Result is ObjectResult result
            && result.Value != null
            && result.StatusCode.GetValueOrDefault(200) == 200 ) {

            Type dataType = result.Value.GetType();
            if( IsSimpleType(dataType) ) {
                ProcessAsText(context, result, dataType);
            }
        }
    }


    private static bool IsSimpleType(Type t)
    {
        // 下面几种类型比较常用，所以就做特殊处理，实际上不止这几个类型
        return t == typeof(int)
            || t == typeof(long)
            || t == typeof(string)
            || t == typeof(Guid)
            || t == typeof(double)
            || t == typeof(decimal);
    }

    private void ProcessAsText(ResultExecutingContext context, ObjectResult result, Type dataType)
    {
        context.Result = new NbTextResult {
            Content = result.Value.ToString(),
            StatusCode = 200,
            ContentType = ResponseContentType.TextUtf8
        };

        if( context.HttpContext.Response.HasStarted == false ) {
            context.HttpContext.Response.Headers.Add("x-result-datatype", dataType.Name);
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }


}
