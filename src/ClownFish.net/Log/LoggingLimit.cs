namespace ClownFish.Log;

/// <summary>
/// 日志相关的一些限制阀值，用于控制产生日志的长度
/// </summary>
internal static class LoggingLimit
{
    /// <summary>
    /// 调用Redis API时，每个参数值的最大长度
    /// </summary>
    public static int RedisArgValueMaxLen { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_RedisArgValueMaxLen", 128);

    /// <summary>
    /// 各种 Request/Response Body 所允许的最大长度。超出部分将被截断。
    /// </summary>
    public static int HttpBodyMaxLen { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_HttpBodyMaxLen", 1024 * 4);



    internal static class OprLog
    {
        /// <summary>
        /// OprLog.Text, Text2, Text3, Text4, Text5, CtxData, Addition 字段 允许的最大长度。
        /// </summary>
        public static int TextnMaxLen { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_OprLog_TextnMaxLen", 1024 * 2);
        

        /// <summary>
        /// 一次顶层调用(HttpAction/MessageHandler/BackgroundTask)过程中 StepItem列表 的最大长度
        /// </summary>
        public static int StepsMaxCount { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_OprLog_StepsMaxCount", 100);

        /// <summary>
        /// 每个 StepItem.Detail 字段 允许的最大长度
        /// </summary>
        public static int StepDetailMaxLen { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_OprLog_StepDetailMaxLen", 1024 * 6);

        /// <summary>
        /// OprLog.Detail 字段 允许的最大长度。
        /// </summary>
        public static int DetailsMaxLen { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_OprLog_DetailsMaxLen", 1024 * 200);

        /// <summary>
        /// 一次顶层调用(HttpAction/MessageHandler/BackgroundTask)过程中 Logs列表 的最大长度
        /// </summary>
        public static int LogsMaxCount { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_OprLog_LogsMaxCount", 100);

        /// <summary>
        /// Log(string message) 参数中 message 允许的最大长度。
        /// </summary>
        public static int LogsTextMaxLen { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_OprLog_LogsTextMaxLen", 500);
    }


    /// <summary>
    /// SQL相关的限制值
    /// </summary>
    internal static class SQL
    {
        /// <summary>
        /// 每条SQL语句最大允许的长度
        /// </summary>
        public static int CommandTextMaxLen { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_SQL_CommandTextMaxLen", 1024 * 1);

        /// <summary>
        /// 每个SQL命令最大允许的参数数量
        /// </summary>
        public static int ParametersMaxCount { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_SQL_ParametersMaxCount", 32);

        /// <summary>
        /// 每个SQL命令参数值转成字符串后的最大长度
        /// </summary>
        public static int ParamValueMaxLen { get; set; } = LocalSettings.GetInt("ClownFish_LogLimit_SQL_ParamValueMaxLen", 128);
    }
}
