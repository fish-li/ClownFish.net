﻿namespace ClownFish.Base.Exceptions;

/// <summary>
/// 验证参数时产生的异常
/// </summary>
public sealed class ValidationException2 : Exception, IErrorCode
{
    /// <summary>
    /// StatusCode, default value: 400
    /// </summary>
    public int StatusCode { get; set; } = 400;

    int IErrorCode.GetErrorCode() => this.StatusCode;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    public ValidationException2(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public ValidationException2(string message, Exception innerException) : base(message, innerException)
    {
    }

}
