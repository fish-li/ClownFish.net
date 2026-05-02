using System.Diagnostics.CodeAnalysis;

namespace ClownFish.Web.Utils;

internal static class TracingUtils
{
    [UnconditionalSuppressMessage("Trimming", "IL2026: ReflectionUtils.CallStaticMethod")]
    public static void Init()
    {
        if( LoggingOptions.TracingEnabled == false ) {
            Console2.Info("########### 由于 LoggingOptions.TracingEnabled == false ，ClownFish.Tracing 性能监控将不会启用！");
            return;
        }

        DbLogger.Init();
        List<string> flags = new List<string>() { "DbLogger"};

        if( EFLogger.Init() ) {
            flags.Add("EFLogger");
        }

        HttpClientLogger2.Init();
        flags.Add("HttpClientLogger2");

        // AOT编译后，ClownFish.Email、ClownFish.Redis、ClownFish.Rabbit 这3个dll会被剔除掉，所以这里不调用它们的 Init 方法了

        if( ReflectionUtils.CallStaticMethod("ClownFish.Email.MailLogger, ClownFish.Email", "Init") == 1 )
            flags.Add("MailLogger");

        if( ReflectionUtils.CallStaticMethod("ClownFish.NRedis.RedisLogger, ClownFish.Redis", "Init") == 1 )
            flags.Add("RedisLogger");

        if( ReflectionUtils.CallStaticMethod("ClownFish.Rabbit.RabbitLogger, ClownFish.Rabbit", "Init") == 1 )
            flags.Add("RabbitLogger");

        Console2.Info($"ClownFish.Tracing 已启用监控模块: {string.Join('/', flags.ToArray())}");
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
