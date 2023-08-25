namespace ClownFish.Base;

internal static class StatusCodeUtils
{
    // 注意：不要使用 650 以下的数字，说不定以后Web标准会使用

    public const int X651 = 651;  // BusinessLogicException

    public const int X670 = 670;  // VenusClientException

    public const int X680 = 680;  // Venus.ViewAppDataController.SendCommand(), 目标应用程序执行超时


    // 说明：状态码这个数字不能超过 3 位，否则会出现异常： 服务器提交了协议冲突. Section=ResponseStatusLine


    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsClientError(int status)
    {
        return status >= 400 && status < 500;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsServerError(int status)
    {
        return (status >= 500 && status < 600) || (status >= 700 && status < 800);
    }

    public static int GetStatusCodeForRemoteWebException(int serverStatus)
    {
        // 4xx 这类错误很特殊，它虽然表示是【客户端错误】，但这只是相当而言
        // 如果HTTP请求是当前服务端发起的，那么对于【最初的客户端】来说，异常仍然是服务端异常，所以下面就取一个特殊的状态码来表示

        // 如果是默认的处理方法，统一用 500，那么最后就不知道服务端当时的状态码是什么了，
        // 比如：服务端可能遇到的是 401, 403, 404， 这个时候用 500，就不知道服务端当时的错误原因了

        if( serverStatus >= 400 && serverStatus < 500 )
            return serverStatus + 300;
        else
            return serverStatus;
    }
}
