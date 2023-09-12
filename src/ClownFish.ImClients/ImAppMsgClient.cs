namespace ClownFish.ImClients;

/// <summary>
/// IM应用-给-单个用户-发送消息-的客户端
/// </summary>
public sealed class ImAppMsgClient : IAppMsgClient
{
    private readonly IAppMsgClient _client;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="settingName"></param>
    public ImAppMsgClient(string settingName)
    {
        ImAppAuthConfig opt = Settings.GetSetting<ImAppAuthConfig>(settingName, true);

        opt.Validate();
        _client = CreateClient(opt);
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="opt"></param>
    public ImAppMsgClient(ImAppAuthConfig opt)
    {
        if( opt == null )
            throw new ArgumentNullException(nameof(opt));

        opt.Validate();
        _client = CreateClient(opt);
    }

    private static IAppMsgClient CreateClient(ImAppAuthConfig opt)
    {
        ImType clientType = opt.ImType;

        if( clientType == ImType.WxWork )
            return new WxworkAppMsgClient(opt.AppId, opt.AppSecret, opt.AgentId);

        if( clientType == ImType.DingDing )
            return new DingdingAppMsgClient(opt.AppId, opt.AppSecret, opt.AgentId);

        if( clientType == ImType.FeiShu )
            return new FeishuAppMsgClient(opt.AppId, opt.AppSecret);

        throw new ArgumentOutOfRangeException(nameof(clientType));
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public void SendFile(string userId, byte[] fileBody, string fileName)
    {
        _client.SendFile(userId, fileBody, fileName);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public async Task SendFileAsync(string userId, byte[] fileBody, string fileName)
    {
        await _client.SendFileAsync(userId, fileBody, fileName);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    public void SendFile(string userId, string filePath)
    {
        _client.SendFile(userId, filePath);
    }


    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    public async Task SendFileAsync(string userId, string filePath)
    {
        await _client.SendFileAsync(userId, filePath);
    }

    /// <summary>
    /// 发送一张图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="imageBody"></param>
    /// <param name="filename"></param>
    public void SendImage(string userId, byte[] imageBody, string filename)
    {
        _client.SendImage(userId, imageBody, filename);
    }

    /// <summary>
    /// 发送一张图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="imageBody"></param>
    /// <param name="filename"></param>
    public async Task SendImageAsync(string userId, byte[] imageBody, string filename)
    {
        await _client.SendImageAsync(userId, imageBody, filename);
    }

    /// <summary>
    /// 发送一张图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    public void SendImage(string userId, string filePath)
    {
        _client.SendImage(userId, filePath);
    }

    /// <summary>
    /// 发送一张图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    public async Task SendImageAsync(string userId, string filePath)
    {
        await _client.SendImageAsync(userId, filePath);
    }


    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public void SendMarkdown(string userId, string text)
    {
        _client.SendMarkdown(userId, text);
    }


    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public async Task SendMarkdownAsync(string userId, string text)
    {
        await _client.SendMarkdownAsync(userId, text);
    }


    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public void SendText(string userId, string text)
    {
        _client.SendText(userId, text);
    }

    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public async Task SendTextAsync(string userId, string text)
    {
        await _client.SendTextAsync(userId, text);
    }


    /// <summary>
    /// 发送一条卡片消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="href"></param>
    public void SendCard(string userId, string title, string text, string href)
    {
        _client.SendCard(userId, title, text, href);
    }

    /// <summary>
    /// 发送一条卡片消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="href"></param>
    /// <returns></returns>
    public async Task SendCardAsync(string userId, string title, string text, string href)
    {
        await _client.SendCardAsync(userId, title, text, href);
    }

}
