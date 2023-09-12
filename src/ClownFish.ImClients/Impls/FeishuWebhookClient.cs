namespace ClownFish.ImClients.Impls;

/// <summary>
/// 飞书-Webhook消息推送-客户端工具类
/// </summary>
public sealed class FeishuWebhookClient : IWebhookClient
{
    private readonly string _webhookUrl;
    private readonly string _signKey;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="webhookUrl"></param>
    /// <param name="signKey"></param>
    public FeishuWebhookClient(string webhookUrl, string signKey)
    {
        if( webhookUrl.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(webhookUrl));

        _webhookUrl = webhookUrl;
        _signKey = signKey;
    }
    private static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    private string GenSign(long timestamp)
    {
        if( _signKey.IsNullOrEmpty() )
            return null;

        string s2 = timestamp.ToString() + "\n" + _signKey;
        byte[] b2 = Encoding.UTF8.GetBytes(s2);

        using( HMACSHA256 hmac = new HMACSHA256(b2) ) {
            byte[] hashValue = hmac.ComputeHash(Empty.Array<byte>());
            return Convert.ToBase64String(hashValue);
        }
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
        httpOption.ExecRPC<FsSbResult>();
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
       await  httpOption.ExecRPCAsync<FsSbResult>();
    }

    private HttpOption SendText0(string text)
    {
        long timestamp = GetTimestamp();
        string sign = GenSign(timestamp);

        return new HttpOption {
            Method = "POST",
            Url = _webhookUrl,
            Format = SerializeFormat.Json,
            Data = new {
                timestamp = timestamp,
                sign = sign,
                msg_type = "text",
                content = new {
                    text = text
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
        httpOption.ExecRPC<FsSbResult>();
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
       await  httpOption.ExecRPCAsync<FsSbResult>();
    }


    private HttpOption SendMarkdown0(string text)
    {
        long timestamp = GetTimestamp();
        string sign = GenSign(timestamp);

        return new HttpOption {
            Method = "POST",
            Url = _webhookUrl,
            Format = SerializeFormat.Json,
            Data = new {
                timestamp = timestamp,
                sign = sign,
                msg_type = "interactive",
                card = new {
                    config = new {
                        wide_screen_mode = true,
                        enable_forward = true,
                    },
                    elements = new object[] {
                        new {
                            tag = "markdown",
                            content = text
                        },
                    }
                }
            }
        };
    }

}
