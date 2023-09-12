namespace ClownFish.ImClients;

/// <summary>
/// IM内部应用发送消息的客户端接口
/// </summary>
public interface IAppMsgClient
{
    /// <summary>
    /// 发送一条文本消息到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    void SendText(string userId, string text);

    /// <summary>
    /// 发送一条Markdown消息到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    void SendMarkdown(string userId, string text);

    /// <summary>
    /// 发送一个图片到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="imageBody"></param>
    /// <param name="filename"></param>
    void SendImage(string userId, byte[] imageBody, string filename);

    /// <summary>
    /// 发送一个图片到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    void SendImage(string userId, string filePath);

    /// <summary>
    /// 发送一个文件到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    void SendFile(string userId, byte[] fileBody, string fileName);

    /// <summary>
    /// 发送一个文件到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    void SendFile(string userId, string filePath);





    /// <summary>
    /// 发送一条文本消息到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    Task SendTextAsync(string userId, string text);

    /// <summary>
    /// 发送一条Markdown消息到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    Task SendMarkdownAsync(string userId, string text);

    /// <summary>
    /// 发送一个图片到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="imageBody"></param>
    /// <param name="filename"></param>
    Task SendImageAsync(string userId, byte[] imageBody, string filename);

    /// <summary>
    /// 发送一个图片到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    Task SendImageAsync(string userId, string filePath);

    /// <summary>
    /// 发送一个文件到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    Task SendFileAsync(string userId, byte[] fileBody, string fileName);

    /// <summary>
    /// 发送一个文件到某个用户
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    Task SendFileAsync(string userId, string filePath);


    /// <summary>
    /// 发送一条卡片消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="href"></param>
    void SendCard(string userId, string title, string text, string href);

    /// <summary>
    /// 发送一条卡片消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="href"></param>
    Task SendCardAsync(string userId, string title, string text, string href);
}
