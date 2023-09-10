﻿namespace ClownFish.MQ.MMQ;

#if NET6_0_OR_GREATER

/// <summary>
/// 订阅者参数
/// </summary>
public sealed class MmqSubscriberArgs<T> where T : class
{
    /// <summary>
    /// 队列实例引用
    /// </summary>
    public MemoryMesssageQueue<T> Queue { get; set; }
    /// <summary>
    /// 订阅者数量
    /// </summary>
    public int SubscriberCount { get; set; } = 1;
    /// <summary>
    /// 消息处理失败后的重试次数
    /// </summary>
    public int RetryCount { get; set; }
    /// <summary>
    /// 消息处理失败后与下次重试之间的时间间隔，单位毫秒
    /// </summary>
    public int RetryWaitMilliseconds { get; set; }
}

#endif