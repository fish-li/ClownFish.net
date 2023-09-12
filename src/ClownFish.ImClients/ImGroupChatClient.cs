namespace ClownFish.ImClients;

/// <summary>
/// IM应用-给-聊天群-发送消息-的客户端
/// </summary>
public sealed class ImGroupChatClient : IGroupChatClient
{
    private readonly IGroupChatClient _client;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="settingName"></param>
    public ImGroupChatClient(string settingName)
    {
        ImAppAuthConfig opt = Settings.GetSetting<ImAppAuthConfig>(settingName, true);

        opt.Validate();
        _client = CreateClient(opt);
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="opt"></param>
    public ImGroupChatClient(ImAppAuthConfig opt)
    {
        if( opt == null )
            throw new ArgumentNullException(nameof(opt));

        opt.Validate();
        _client = CreateClient(opt);
    }

    private static IGroupChatClient CreateClient(ImAppAuthConfig opt)
    {
        ImType clientType = opt.ImType;

        if( clientType == ImType.WxWork )
            return new WxworkGroupChatClient(opt.AppId, opt.AppSecret, opt.AgentId);

        if( clientType == ImType.DingDing )
            return new DingdingGroupChatClient(opt.AppId, opt.AppSecret, opt.AgentId);

        if( clientType == ImType.FeiShu )
            return new FeishuGroupChatClient(opt.AppId, opt.AppSecret);

        throw new ArgumentOutOfRangeException(nameof(clientType));
    }

    /// <summary>
    /// 创建一个聊天群
    /// </summary>
    /// <param name="name">聊天群名称</param>
    /// <param name="ownerId">群主的userId</param>
    /// <param name="userIds">群成员的useId列表</param>
    /// <returns></returns>
    public string CreateChatGroup(string name, string ownerId, string[] userIds)
    {
        return _client.CreateChatGroup(name, ownerId, userIds);
    }

    /// <summary>
    /// 创建一个聊天群
    /// </summary>
    /// <param name="name">聊天群名称</param>
    /// <param name="ownerId">群主的userId</param>
    /// <param name="userIds">群成员的useId列表</param>
    /// <returns></returns>
    public async Task<string> CreateChatGroupAsync(string name, string ownerId, string[] userIds)
    {
        return await _client.CreateChatGroupAsync(name, ownerId, userIds);
    }



    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public void SendFile(string chatId, byte[] fileBody, string fileName)
    {
        _client.SendFile(chatId, fileBody, fileName);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public async Task SendFileAsync(string chatId, byte[] fileBody, string fileName)
    {
        await _client.SendFileAsync(chatId, fileBody, fileName);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public void SendFile(string chatId, string filePath)
    {
        _client.SendFile(chatId, filePath);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public async Task SendFileAsync(string chatId, string filePath)
    {
        await _client.SendFileAsync(chatId, filePath);
    }

    /// <summary>
    /// 发送一张图片
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="imageBody"></param>
    /// <param name="fileName"></param>
    public void SendImage(string chatId, byte[] imageBody, string fileName)
    {
        _client.SendImage(chatId, imageBody, fileName);
    }

    /// <summary>
    /// 发送一张图片
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="imageBody"></param>
    /// <param name="fileName"></param>
    public async Task SendImageAsync(string chatId, byte[] imageBody, string fileName)
    {
        await _client.SendImageAsync(chatId, imageBody, fileName);
    }


    /// <summary>
    /// 发送一张图片
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public void SendImage(string chatId, string filePath)
    {
        _client.SendImage(chatId, filePath);
    }

    /// <summary>
    /// 发送一张图片
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public async Task SendImageAsync(string chatId, string filePath)
    {
        await _client.SendImageAsync(chatId, filePath);
    }

    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public void SendMarkdown(string chatId, string text)
    {
        _client.SendMarkdown(chatId, text);
    }

    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public async Task SendMarkdownAsync(string chatId, string text)
    {
        await _client.SendMarkdownAsync(chatId, text);
    }

    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public void SendText(string chatId, string text)
    {
        _client.SendText(chatId, text);
    }

    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public async Task SendTextAsync(string chatId, string text)
    {
        await _client.SendTextAsync(chatId, text);
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
        _client.SendCard(chatId, title, text, href);
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
        await _client.SendCardAsync(chatId, title, text, href);
    }
}
