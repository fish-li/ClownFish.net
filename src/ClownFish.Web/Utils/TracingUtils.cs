namespace ClownFish.Web.Utils;

internal static class TracingUtils
{
    public static void Init()
    {
        if( LoggingOptions.TracingEnabled == false )
            return;


        DbLogger.Init();
        EFLogger.Init();
        HttpClientLogger2.Init();

        ReflectionUtils.CallStaticMethod("ClownFish.Email.MailLogger, ClownFish.Email", "Init");
        ReflectionUtils.CallStaticMethod("ClownFish.NRedis.RedisLogger, ClownFish.Redis", "Init");
        ReflectionUtils.CallStaticMethod("ClownFish.Rabbit.RabbitLogger, ClownFish.Rabbit", "Init");
    }


    public static void CheckLogConfig()
    {
        string writesMap = Settings.GetSetting("ClownFish_Log_WritersMap");
        if( writesMap.IsNullOrEmpty() ) {
            Console2.Info("force set: ClownFish_Log_WritersMap => OprLog=http");
            MemoryConfig.AddSetting("ClownFish_Log_WritersMap", "OprLog=http");
        }
    }


    public static void SetResponseResult(this HttpPipelineContext pipelineContext, object mvcResult)
    {
        if( pipelineContext == null || mvcResult == null )
            return;

        if( pipelineContext.RespResult == null ) {

            // MVC 方法直接返回了对象，而不是 ActionResult 类型的对象
            if( mvcResult is ObjectResult obj ) {
                pipelineContext.RespResult = obj.Value ?? string.Empty;
            }
            else if( mvcResult is ContentResult txt ) {
                pipelineContext.RespResult = txt.Content ?? string.Empty;
            }
            else if( mvcResult is JsonResult json ) {
                pipelineContext.RespResult = json.Value ?? string.Empty;
            }
            // 忽略不能识别的结果
        }
    }

    // 说明（目前已知问题）： pipelineContext.RespResult  的支持是不完整的
    // 已在2类位置埋点：
    // 1, MVC-action 执行之后（调用上面的扩展方法）
    // 2, 调用 httpContext.HttpReply(...) 且传递的 body是 string

    // 其余3类场景不打算支持：
    // 1，没有有价值的 response body， 例如 MS定义的一些 ActionResult
    // 2，response body 可能不是文本
    // 3，采用 HttpWebResponse/HttpResponseMessage 来返回，此时流可能不支持重复读取
}
