namespace ClownFish.ImClients.Impls;

/// <summary>
/// 企业微信-应用往聊天群推送消息-客户端工具类
/// </summary>
public class WxworkGroupChatClient : IGroupChatClient
{
    private readonly WxworkInternalClient _client;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="corpId"></param>
    /// <param name="appSecret"></param>
    /// <param name="agentId"></param>
    public WxworkGroupChatClient(string corpId, string appSecret, long agentId)
    {
        _client = new WxworkInternalClient(corpId, appSecret, agentId);
    }

    /// <summary>
    /// 创建聊天群
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ownerId"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    public string CreateChatGroup(string name, string ownerId, string[] userIds)
    {
        if( name.IsNullOrEmpty() || ownerId.IsNullOrEmpty() || userIds.IsNullOrEmpty() )
            throw new ArgumentNullException();

        // 群ID采用事先分配的方式创建
        string chatId = Guid.NewGuid().ToString("N");

        HttpOption httpOption = CreateChatGroup0(chatId, name, ownerId, userIds);
        _client.SendRequest<ImSbResult>(httpOption);

        return chatId;
    }


    /// <summary>
    /// 创建聊天群
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ownerId"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    public async Task<string> CreateChatGroupAsync(string name, string ownerId, string[] userIds)
    {
        if( name.IsNullOrEmpty() || ownerId.IsNullOrEmpty() || userIds.IsNullOrEmpty() )
            throw new ArgumentNullException();

        // 群ID采用事先分配的方式创建
        string chatId = Guid.NewGuid().ToString("N");

        HttpOption httpOption = CreateChatGroup0(chatId, name, ownerId, userIds);
        await _client.SendRequestAsync<ImSbResult>(httpOption);

        return chatId;
    }


    private HttpOption CreateChatGroup0(string chatId, string name, string ownerId, string[] userIds)
    {
        // https://developer.work.weixin.qq.com/document/path/90245
        return new HttpOption {
            Method = "POST",
            Url = "https://qyapi.weixin.qq.com/cgi-bin/appchat/create",
            Format = SerializeFormat.Json,
            Data = new {
                name,
                owner = ownerId,
                userlist = userIds,
                chatid = chatId,
            }
        };
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public void SendFile(string chatId, byte[] fileBody, string fileName)
    {
        _client.SendFile(PushMsgType.GroupMsg, chatId, fileBody, fileName);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public async Task SendFileAsync(string chatId, byte[] fileBody, string fileName)
    {
        await _client.SendFileAsync(PushMsgType.GroupMsg, chatId, fileBody, fileName);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public void SendFile(string chatId, string filePath)
    {
        _client.SendFile(PushMsgType.GroupMsg, chatId, filePath);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public async Task SendFileAsync(string chatId, string filePath)
    {
        await _client.SendFileAsync(PushMsgType.GroupMsg, chatId, filePath);
    }

    /// <summary>
    /// 发送一张图片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="imageBody"></param>
    /// <param name="fileName"></param>
    public void SendImage(string chatId, byte[] imageBody, string fileName)
    {
        _client.SendImage(PushMsgType.GroupMsg, chatId, imageBody, fileName);
    }

    /// <summary>
    /// 发送一张图片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="imageBody"></param>
    /// <param name="fileName"></param>
    public async Task SendImageAsync(string chatId, byte[] imageBody, string fileName)
    {
        await _client.SendImageAsync(PushMsgType.GroupMsg, chatId, imageBody, fileName);
    }


    /// <summary>
    /// 发送一张图片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public void SendImage(string chatId, string filePath)
    {
        _client.SendImage(PushMsgType.GroupMsg, chatId, filePath);
    }

    /// <summary>
    /// 发送一张图片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public async Task SendImageAsync(string chatId, string filePath)
    {
        await _client.SendImageAsync(PushMsgType.GroupMsg, chatId, filePath);
    }

    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public void SendMarkdown(string chatId, string text)
    {
        _client.SendMarkdown(PushMsgType.GroupMsg, chatId, text);
    }

    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public async Task SendMarkdownAsync(string chatId, string text)
    {
        await _client.SendMarkdownAsync(PushMsgType.GroupMsg, chatId, text);
    }

    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public void SendText(string chatId, string text)
    {
        _client.SendText(PushMsgType.GroupMsg, chatId, text);
    }

    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public async Task SendTextAsync(string chatId, string text)
    {
        await _client.SendTextAsync(PushMsgType.GroupMsg, chatId, text);
    }


    /// <summary>
    /// 发送一条卡片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="href"></param>
    public void SendCard(string chatId, string title, string text, string href)
    {
        _client.SendCard(PushMsgType.GroupMsg, chatId, title, text, href);
    }

    /// <summary>
    /// 发送一条卡片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="href"></param>
    /// <returns></returns>
    public async Task SendCardAsync(string chatId, string title, string text, string href)
    {
        await _client.SendCardAsync(PushMsgType.GroupMsg, chatId, title, text, href);
    }
}
