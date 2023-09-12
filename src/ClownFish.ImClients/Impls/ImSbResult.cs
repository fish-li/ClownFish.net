namespace ClownFish.ImClients.Impls;

/// <summary>
/// 服务端返回结构
/// </summary>
public class ImSbResult : IShitResult
{
    /// <summary>
    /// ErrCode
    /// </summary>
    public int ErrCode { get; set; }
    /// <summary>
    /// ErrMsg
    /// </summary>
    public string ErrMsg { get; set; }
}

// {"errcode":400105,"errmsg":"unsupported msgtype"}


/// <summary>
/// 服务端返回结构
/// </summary>
public class FsSbResult : IShitResult
{
    /// <summary>
    /// ErrCode
    /// </summary>
    [JsonProperty("code")]
    public int ErrCode { get; set; }

    /// <summary>
    /// ErrMsg
    /// </summary>
    [JsonProperty("msg")]
    public string ErrMsg { get; set; }
}

// 异常时：{"code":10208,"data":{},"msg":"msg type text1 not support"}
// 正常时：{"Extra":null,"StatusCode":0,"StatusMessage":"success"}


