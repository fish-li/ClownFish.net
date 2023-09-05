using ClownFish.Web.AspnetCore.ActionResults;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ClownFish.Web.AspnetCore.Filters;

public sealed class StatusCodeFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if( context.Result is IStatusCodeActionResult result2 ) {

            if( result2.StatusCode == 400 ) {
                context.Result = CreateResult400(context);
            }

            if( result2.StatusCode == 415 ) {
                context.Result = CreateResult415();
            }
        }
    }

    private IActionResult CreateResult400(ResultExecutingContext context)
    {
        string message = null;

        // 由 ValidationException 引发的异常，这里把它转换成内部统一的异常结构，便于前端使用
        if( context.Result is BadRequestObjectResult result2 && result2.Value is ValidationProblemDetails details ) {
            message = details.GetSummary();
        }
        else {
            if( context.ModelState != null && context.ModelState.IsValid == false ) {
                message = context.ModelState.GetSummary();
            }
        }

        // 如果不能获取到错误描述消息，就不做转换处理
        if( message.IsNullOrEmpty() )
            return context.Result;

        ErrorPageModel model = new ErrorPageModel {
            Message = message,
            ExceptionType = typeof(System.ComponentModel.DataAnnotations.ValidationException).FullName,
            RequestId = HttpPipelineContext.Get()?.ProcessId,
            StatusCode = 400
        };

        return new NbTextResult {
            StatusCode = 400,
            ContentType = ResponseContentType.JsonUtf8,
            Content = model.ToJson(JsonStyle.Indented)
        };
    }

    private IActionResult CreateResult415()
    {
        ErrorPageModel model = new ErrorPageModel {
            Message = "Unsupported Media Type",
            ExceptionType = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.UnsupportedContentTypeException).FullName,
            StatusCode = 415
        };

        return new NbTextResult {
            StatusCode = 415,
            ContentType = ResponseContentType.JsonUtf8,
            Content = model.ToJson(JsonStyle.Indented)
        };
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}


internal static class ModelStateExtensions
{
    /// <summary>
    /// 获取验证消息提示并格式化提示
    /// </summary>
    public static string GetSummary(this ValidationProblemDetails details, string separator = "\r\n")
    {
        if( details.Detail.IsNullOrEmpty() == false )
            return details.Detail;

        StringBuilder sb = StringBuilderPool.Get();

        try {
            foreach( var item in details.Errors ) {
                foreach( string message in item.Value ) {
                    if( sb.Length > 0 )
                        sb.Append(separator);

                    sb.Append(message);
                }
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    /// <summary>
    /// 获取验证消息提示并格式化提示
    /// </summary>
    public static string GetSummary(this ModelStateDictionary modelState, string separator = "\r\n")
    {
        StringBuilder sb = StringBuilderPool.Get();

        try {
            foreach( var item in modelState ) {

                ModelStateEntry state = item.Value;
                if( state.Errors.IsNullOrEmpty() == false ) {

                    foreach( ModelError err in state.Errors ) {
                        string message = err.ErrorMessage ?? err.Exception?.Message;

                        if( message.HasValue() ) {
                            if( sb.Length > 0 )
                                sb.Append(separator);

                            sb.Append(message);
                        }
                    }
                }
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }





}

