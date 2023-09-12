namespace ClownFish.ImClients.Impls;

/// <summary>
/// DingdingWebhookClient
/// </summary>
public sealed class DingdingWebhookClient : IWebhookClient
{
    private readonly string _webhookUrl;
    private readonly string _signKey;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="webhookUrl"></param>
    /// <param name="signKey"></param>
    public DingdingWebhookClient(string webhookUrl, string signKey)
    {
        if( webhookUrl.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(webhookUrl));

        _webhookUrl = webhookUrl;
        _signKey = signKey;
    }
    private static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds() * 1000;
    }

    private string GenSign(long timestamp)
    {
        string s2 = timestamp.ToString() + "\n" + _signKey;
        byte[] b2 = Encoding.UTF8.GetBytes(s2);

        byte[] encKeyBytes = Encoding.UTF8.GetBytes(_signKey);
        using( HMACSHA256 hmac = new HMACSHA256(encKeyBytes) ) {
            byte[] hashValue = hmac.ComputeHash(b2);
            return Convert.ToBase64String(hashValue);
        }
    }


    private string GetUrl()
    {
        if( _signKey.IsNullOrEmpty() )
            return _webhookUrl;


        long timestamp = GetTimestamp();
        string sign = GenSign(timestamp);
        return $"{_webhookUrl}&timestamp={timestamp}&sign={sign.UrlEncode()}";
    }


    /// <summary>
    /// 发送一条文本消息
    /// </summary>
    /// <param name="text"></param>
    public void SendText(string text)
    {
        if( text.IsNullOrEmpty() )
            return;

        HttpOption httpOption = SendText0(text);
        httpOption.ExecRPC<ImSbResult>();
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
        await httpOption.ExecRPCAsync<ImSbResult>();
    }

    private HttpOption SendText0(string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(),
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
        httpOption.ExecRPC<ImSbResult>();
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
       await httpOption.ExecRPCAsync<ImSbResult>();
    }


    private HttpOption SendMarkdown0(string text)
    {
        return new HttpOption {
            Method = "POST",
            Url = GetUrl(),
            Format = SerializeFormat.Json,
            Data = new {
                msgtype = "markdown",
                markdown = new {
                    title = DingdingUtils.GetMarkdonwTitle(text),
                    text = text
                }
            }
        };
    }


}
