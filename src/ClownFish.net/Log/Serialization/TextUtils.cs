﻿namespace ClownFish.Log;

/// <summary>
/// 定义一些与日志文本相关的工具方法
/// </summary>
public static class TextUtils
{
    /// <summary>
    /// 多个step步骤 之间 的分隔行符
    /// </summary>
    public static readonly string StepSeparatedLine = "---de34e591f12843febb84a8274498bc00-";

    /// <summary>
    /// 一个step步骤 内 的片段分隔行符（老版本不建议使用，TODO：以后删除）
    /// </summary>
    public static readonly string StepDetailSeparatedLine1 = "------------------------------------";

    /// <summary>
    /// 一个step步骤 内 的片段分隔行符
    /// </summary>
    public static readonly string StepDetailSeparatedLine3 = "----b7YQJpLFTUK6KEaN1knyag";


    /// <summary>
    /// 将一个对象转换成合适的日志文本形式
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetLogText(this object value)
    {
        if( value == null )
            return string.Empty;

        if( value is string str )
            return str;

        try {
            if( value is DbCommand dbCommand )
                return dbCommand.ToLoggingText();

#if NET6_0_OR_GREATER
            if( value is DbBatch dbBatch )
                return dbBatch.ToLoggingText();
#endif
            if( value is ILoggingObject loggingObject )
                return loggingObject.ToLoggingText();

            if( value is ITextSerializer textSerializer )
                return textSerializer.ToText();

            return value.ToJson();
        }
        catch(Exception ex) {
            return $"## GetLogText({value.GetType().FullName}) ERROR: " + ex.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static string GetErrorLogText(this Exception ex)
    {
        if( ex == null )
            return string.Empty;

        try {
            return ex.ToString2();
        }
        catch(Exception ex2) {
            return $"## GetErrorLogText({ex.GetType().FullName}) ERROR: " + ex2.ToString();
        }
    }
}
