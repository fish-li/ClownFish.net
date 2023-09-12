﻿namespace ClownFish.NRedis;

/// <summary>
/// 订阅者参数
/// </summary>
public sealed class RedisSubscriberArgs
{
    /// <summary>
    /// Redis的连接配置名称
    /// </summary>
    public string SettingName { get; set; }
    /// <summary>
    /// Channel名称
    /// </summary>
    public string Channel { get; set; }
    ///// <summary>
    ///// 订阅者数量
    ///// </summary>
    //public int SubscriberCount { get; set; } = 1;
    /// <summary>
    /// 消息处理失败后的重试次数
    /// </summary>
    public int RetryCount { get; set; }
    /// <summary>
    /// 消息处理失败后与下次重试之间的时间间隔，单位毫秒
    /// </summary>
    public int RetryWaitMilliseconds { get; set; }

    /// <summary>
    /// 最大允许的消息长度。如果超过这个值，消息会直接丢弃。
    /// </summary>
    public int MaxMessageLength { get; set; } = 16 * 1024;
}
