namespace ClownFish.ImClients;

/// <summary>
/// IWebhookClient
/// </summary>
public interface IWebhookClient
{
    /// <summary>
    /// 发送一条文本消息
    /// </summary>
    /// <param name="text"></param>
    void SendText(string text);

    /// <summary>
    /// 发送一条Markdown消息
    /// </summary>
    /// <param name="text"></param>
    void SendMarkdown(string text);




    /// <summary>
    /// 发送一条文本消息
    /// </summary>
    /// <param name="text"></param>
    Task SendTextAsync(string text);

    /// <summary>
    /// 发送一条Markdown消息
    /// </summary>
    /// <param name="text"></param>
    Task SendMarkdownAsync(string text);

}
