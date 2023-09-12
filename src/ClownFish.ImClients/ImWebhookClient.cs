namespace ClownFish.ImClients;

/// <summary>
/// 采用-IM-WebHook-方式给-聊天群-发送消息-的客户端
/// </summary>
public sealed class ImWebhookClient : IWebhookClient
{
    private readonly IWebhookClient _client;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="imType"></param>
    /// <param name="webhookUrl"></param>
    /// <param name="signKey"></param>
    public ImWebhookClient(ImType imType, string webhookUrl, string signKey)
    {
        _client = CreateClient(imType, webhookUrl, signKey);
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="settingName"></param>
    public ImWebhookClient(string settingName)
    {
        ImWebHookConfig opt = Settings.GetSetting<ImWebHookConfig>(settingName, true);
        opt.Validate();

        _client = CreateClient(opt.ImType, opt.WebHookUrl, opt.SignKey);
    }


    private static IWebhookClient CreateClient(ImType clientType, string webhookUrl, string signKey)
    {
        if( clientType == ImType.WxWork )
            return new WxWorkWebhookClient(webhookUrl, signKey);

        if( clientType == ImType.DingDing )
            return new DingdingWebhookClient(webhookUrl, signKey);

        if( clientType == ImType.FeiShu )
            return new FeishuWebhookClient(webhookUrl, signKey);

        throw new ArgumentOutOfRangeException(nameof(clientType));
    }

    /// <summary>
    /// 发送一条文本消息
    /// </summary>
    /// <param name="text"></param>
    public void SendText(string text)
    {
        _client.SendText(text);
    }

    /// <summary>
    /// 发送一条文本消息
    /// </summary>
    /// <param name="text"></param>
    public async Task SendTextAsync(string text)
    {
        await _client.SendTextAsync(text);
    }

    /// <summary>
    /// 发送一条Markdown消息
    /// </summary>
    /// <param name="text"></param>
    public void SendMarkdown(string text)
    {
        _client.SendMarkdown(text);
    }

    /// <summary>
    /// 发送一条Markdown消息
    /// </summary>
    /// <param name="text"></param>
    public async Task SendMarkdownAsync(string text)
    {
        await _client.SendMarkdownAsync(text);
    }
}
