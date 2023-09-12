namespace ClownFish.ImClients;

/// <summary>
/// IM内部应用参与群聊的接口
/// </summary>
public interface IGroupChatClient
{
    /// <summary>
    /// 创建聊天群
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ownerId"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    string CreateChatGroup(string name, string ownerId, string[] userIds);

    /// <summary>
    /// 发送一条文本消息到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    void SendText(string chatId, string text);

    /// <summary>
    /// 发送一条Markdown消息到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    void SendMarkdown(string chatId, string text);

    /// <summary>
    /// 发送一个图片到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="imageBody"></param>
    /// <param name="fileName"></param>
    void SendImage(string chatId, byte[] imageBody, string fileName);

    /// <summary>
    /// 发送一个图片到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    void SendImage(string chatId, string filePath);

    /// <summary>
    /// 发送一个文件到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    void SendFile(string chatId, byte[] fileBody, string fileName);

    /// <summary>
    /// 发送一个文件到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    void SendFile(string chatId, string filePath);




    /// <summary>
    /// 创建聊天群
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ownerId"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    Task<string> CreateChatGroupAsync(string name, string ownerId, string[] userIds);

    /// <summary>
    /// 发送一条文本消息到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    Task SendTextAsync(string chatId, string text);

    /// <summary>
    /// 发送一条Markdown消息到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    Task SendMarkdownAsync(string chatId, string text);

    /// <summary>
    /// 发送一个图片到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="imageBody"></param>
    /// <param name="fileName"></param>
    Task SendImageAsync(string chatId, byte[] imageBody, string fileName);

    /// <summary>
    /// 发送一个图片到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    Task SendImageAsync(string chatId, string filePath);

    /// <summary>
    /// 发送一个文件到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    Task SendFileAsync(string chatId, byte[] fileBody, string fileName);

    /// <summary>
    /// 发送一个文件到聊天群
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    Task SendFileAsync(string chatId, string filePath);


    /// <summary>
    /// 发送一条卡片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="href"></param>
    void SendCard(string chatId, string title, string text, string href);

    /// <summary>
    /// 发送一条卡片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="href"></param>
    Task SendCardAsync(string chatId, string title, string text, string href);
}
