namespace ClownFish.ImClients.Impls;

/// <summary>
/// 钉钉-应用往聊天群推送消息-客户端工具类
/// </summary>
public class DingdingGroupChatClient : IGroupChatClient
{
    private readonly DingdingInternalClient _client;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="appSecret"></param>
    /// <param name="agentId"></param>
    public DingdingGroupChatClient(string appId, string appSecret, long agentId)
    {
        _client = new DingdingInternalClient(appId, appSecret, agentId);
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


        HttpOption httpOption = CreateChatGroup0(name, ownerId, userIds);
        CreateGroupResult result = _client.SetAuthorization(httpOption).ExecRPC<CreateGroupResult>();

        string chatId = result.ChatId;
        if( chatId.IsNullOrEmpty() )
            throw new InvalidOperationException("钉钉的服务端接口没有返回期望的数据：openConversationId 不存在！");

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


        HttpOption httpOption = CreateChatGroup0(name, ownerId, userIds);
        CreateGroupResult result = await _client.SetAuthorization(httpOption).ExecRPCAsync<CreateGroupResult>();

        string chatId = result.ChatId;
        if( chatId.IsNullOrEmpty() )
            throw new InvalidOperationException("钉钉的服务端接口没有返回期望的数据：openConversationId 不存在！");

        return chatId;
    }


    private HttpOption CreateChatGroup0(string name, string ownerId, string[] userIds)
    {
        // https://open.dingtalk.com/document/orgapp-server/create-group-session
        return new HttpOption {
            Method = "POST",
            Url = "https://oapi.dingtalk.com/chat/create",
            Format = SerializeFormat.Json,
            Data = new {
                name,
                owner = ownerId,
                useridlist = userIds,
                showHistoryType = 1,
                searchable = 1,
            }
        };
    }

    internal sealed class CreateGroupResult : ImSbResult
    {
        [JsonProperty("chatid")]
        public string ChatId { get; set; }

        [JsonProperty("openConversationId")]
        public string ConId { get; set; }
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public void SendFile(string chatId, byte[] fileBody, string fileName)
    {
        _client.SendFile(2, chatId, fileBody, fileName);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public async Task SendFileAsync(string chatId, byte[] fileBody, string fileName)
    {
        await _client.SendFileAsync(2, chatId, fileBody, fileName);
    }



    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public void SendFile(string chatId, string filePath)
    {
        _client.SendFile(2, chatId, filePath);
    }

    /// <summary>
    /// 发送一个文件
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public async Task SendFileAsync(string chatId, string filePath)
    {
        await _client.SendFileAsync(2, chatId, filePath);
    }

    /// <summary>
    /// 发送一张图片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="imageBody"></param>
    /// <param name="fileName"></param>
    public void SendImage(string chatId, byte[] imageBody, string fileName)
    {
        _client.SendImage(2, chatId, imageBody, fileName);
    }

    /// <summary>
    /// 发送一张图片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="imageBody"></param>
    /// <param name="fileName"></param>
    public async Task SendImageAsync(string chatId, byte[] imageBody, string fileName)
    {
        await _client.SendImageAsync(2, chatId, imageBody, fileName);
    }

    /// <summary>
    /// 发送一张图片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public void SendImage(string chatId, string filePath)
    {
        _client.SendImage(2, chatId, filePath);
    }

    /// <summary>
    /// 发送一张图片消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filePath"></param>
    public async Task SendImageAsync(string chatId, string filePath)
    {
        await _client.SendImageAsync(2, chatId, filePath);
    }

    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public void SendMarkdown(string chatId, string text)
    {
        _client.SendMarkdown(2, chatId, text);
    }


    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public async Task SendMarkdownAsync(string chatId, string text)
    {
        await _client.SendMarkdownAsync(2, chatId, text);
    }

    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public void SendText(string chatId, string text)
    {
        _client.SendText(2, chatId, text);
    }

    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="text"></param>
    public async Task SendTextAsync(string chatId, string text)
    {
        await _client.SendTextAsync(2, chatId, text);
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
        _client.SendCard(2, chatId, title, text, href);
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
        await _client.SendCardAsync(2, chatId, title, text, href);
    }

}
