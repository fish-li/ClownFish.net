﻿namespace ClownFish.ImClients.Impls;

/// <summary>
/// 飞书-应用给用户推送消息-客户端工具类
/// </summary>
public class FeishuAppMsgClient : IAppMsgClient
{
    private readonly FeishuInternalClient _client;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="appSecret"></param>
    public FeishuAppMsgClient(string appId, string appSecret)
    {
        _client = new FeishuInternalClient(appId, appSecret);
    }


    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public void SendText(string userId, string text)
    {
        _client.SendText(1, userId, text);
    }

    /// <summary>
    /// 发送一条 文本 消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public async Task SendTextAsync(string userId, string text)
    {
        await _client.SendTextAsync(1, userId, text);
    }

    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public void SendMarkdown(string userId, string text)
    {
        _client.SendMarkdown(1, userId, text);
    }

    /// <summary>
    /// 发送一条 Markdown 消息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public async Task SendMarkdownAsync(string userId, string text)
    {
        await _client.SendMarkdownAsync(1, userId, text);
    }


    /// <summary>
    /// 发送一个 文件
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    public void SendFile(string userId, string filePath)
    {
        _client.SendFile(1, userId, filePath);
    }

    /// <summary>
    /// 发送一个 文件
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    public async Task SendFileAsync(string userId, string filePath)
    {
        await _client.SendFileAsync(1, userId, filePath);
    }

    /// <summary>
    /// 发送一个 文件
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public void SendFile(string userId, byte[] fileBody, string fileName)
    {
        _client.SendFile(1, userId, fileBody, fileName);
    }


    /// <summary>
    /// 发送一个 文件
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileBody"></param>
    /// <param name="fileName"></param>
    public async Task SendFileAsync(string userId, byte[] fileBody, string fileName)
    {
        await _client.SendFileAsync(1, userId, fileBody, fileName);
    }


    /// <summary>
    /// 发送一个 图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    public void SendImage(string userId, string filePath)
    {
        _client.SendImage(1, userId, filePath);
    }


    /// <summary>
    /// 发送一个 图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filePath"></param>
    public async Task SendImageAsync(string userId, string filePath)
    {
        await _client.SendImageAsync(1, userId, filePath);
    }

    /// <summary>
    /// 发送一个 图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="imageBody"></param>
    /// <param name="filename"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SendImage(string userId, byte[] imageBody, string filename)
    {
        _client.SendImage(1, userId, imageBody, filename);
    }


    /// <summary>
    /// 发送一个 图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="imageBody"></param>
    /// <param name="filename"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SendImageAsync(string userId, byte[] imageBody, string filename)
    {
        await _client.SendImageAsync(1, userId, imageBody, filename);
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
        _client.SendCard(1, userId, title, text, href);
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
        await _client.SendCardAsync(1, userId, title, text, href);
    }

}
