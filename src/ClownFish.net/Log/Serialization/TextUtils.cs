namespace ClownFish.Log;

/// <summary>
/// 定义一些与日志文本相关的工具方法
/// </summary>
public static class TextUtils
{
    /// <summary>
    /// 步骤step之间的分隔行符
    /// </summary>
    public static readonly string StepSeparatedLine = "---de34e591f12843febb84a8274498bc00-";

    /// <summary>
    /// 一个步骤step内的片段分隔行符（老版本，不建议再使用）
    /// </summary>
    public static readonly string StepDetailSeparatedLineOld = "------------------------------------";

    /// <summary>
    /// 一个步骤step内的片段分隔行符
    /// </summary>
    public static readonly string StepDetailSeparatedLine = "------------------------------------3aea3d12823847bba74bc1594430afe1";


    /// <summary>
    /// 将一个对象转换成合适的日志文本形式
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static string GetLogText(this object value)
    {
        if( value == null )
            return string.Empty;

        if( value is string str )
            return str;

        try {
            if( value is DbCommand dbCommand )
                return dbCommand.ToLoggingText();
#if NETCOREAPP
            if( value is System.Net.Http.HttpRequestMessage request )
                return request.ToLoggingText();
#endif
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


    internal static string GetErrorLogText(this Exception ex)
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
