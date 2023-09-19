namespace ClownFish.Base;

/// <summary>
/// 封装了Exception的一些扩展方法
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// 获取异常嵌套链上所有的错误消息
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static string GetAllMessages(this Exception ex)
    {
        if( ex == null )
            throw new ArgumentNullException(nameof(ex));

        StringBuilder sb = StringBuilderPool.Get();
        try {
            Exception current = ex;
            while( current != null ) {

                if( sb.Length > 0 )
                    sb.Append("\r\n==>");

                sb.Append($"({current.GetType().FullName}) {current.Message}");
                current = current.InnerException;
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    /// <summary>
    /// 一个回调委托，用于GetErrorCode方法执行时，遇到一个不能识别的异常时发生回调
    /// </summary>
    public static Func<Exception, int?> GetErrorCodeCallbackFunc;


    /// <summary>
    /// 获取异常对应的状态码
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static int GetErrorCode(this Exception ex)
    {
        if( ex == null )
            return 200;

        if( ex is System.ComponentModel.DataAnnotations.ValidationException )
            return 400;

#if NET6_0_OR_GREATER
        if( ex is System.Net.Http.HttpRequestException hex && hex.StatusCode.HasValue )
            return (int)hex.StatusCode.Value;
#endif

        if( ex is IErrorCode ex2 )
            return ex2.GetErrorCode();


        if( GetErrorCodeCallbackFunc != null ) {
            int? result = GetErrorCodeCallbackFunc(ex);
            if( result.HasValue )
                return result.Value;
        }

        return 500;
    }



}
