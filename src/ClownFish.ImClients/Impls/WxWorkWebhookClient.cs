namespace ClownFish.ImClients.Impls;

/// <summary>
/// WxWorkWebhookClient
/// </summary>
public sealed class WxWorkWebhookClient : IWebhookClient
{
    private readonly string _webhookUrl;
    private readonly string _signKey;   // 目前不支持，以后也许会支持，所以先把它留着

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="webhookUrl"></param>
    /// <param name="signKey">此参数目前可以不传值</param>
    public WxWorkWebhookClient(string webhookUrl, string signKey = null)
    {
        if( webhookUrl.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(webhookUrl));

        _webhookUrl = webhookUrl;
        _signKey = signKey;
    }


    /// <summary>
    /// 服务端返回结构
    /// </summary>
    internal class SbResult : IShitResult
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

    // {"errcode":40008, "errmsg":"invalid message type, hint: [1644997756324054159813600], from ip: 113.57.152.182, more info at https://open.work.weixin.qq.com/devtool/query?e=40008"}



    /// <summary>
    /// 发送一条文本消息
    /// </summary>
    /// <param name="text"></param>
    public void SendText(string text)
    {
        if( text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(text);
        httpOption.ExecRPC<SbResult>();
    }


    /// <summary>
    /// 发送一条文本消息
    /// </summary>
    /// <param name="text"></param>
    public async Task SendTextAsync(string text)
    {
        if( text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(text);
        await httpOption.ExecRPCAsync<SbResult>();
    }

    private HttpOption SendText0(string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = _webhookUrl,
            Format = SerializeFormat.Json,
            Data = new {
                msgtype = "text",
                text = new {
                    content = text
                }
            }
        };
    }

    /// <summary>
    /// 发送一条Markdown消息
    /// </summary>
    /// <param name="text"></param>
    public void SendMarkdown(string text)
    {
        if( text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendMarkdown0(text);
        httpOption.ExecRPC<SbResult>();
    }


    /// <summary>
    /// 发送一条Markdown消息
    /// </summary>
    /// <param name="text"></param>
    public async Task SendMarkdownAsync(string text)
    {
        if( text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendMarkdown0(text);
        await httpOption.ExecRPCAsync<SbResult>();
    }

    private HttpOption SendMarkdown0(string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = _webhookUrl,
            Format = SerializeFormat.Json,
            Data = new {
                msgtype = "markdown",
                markdown = new {
                    content = text
                }
            }
        };
    }
}
