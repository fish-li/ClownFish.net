namespace ClownFish.NRedis;

/// <summary>
/// Redis日志工具类
/// </summary>
public static class RedisLogger
{
    /// <summary>
    /// Init
    /// </summary>
    public static void Init()
    {
        RedisClientEvent.OnExecuteFinished += RedisClientEventOnExecuteFinished;
    }

    private static void RedisClientEventOnExecuteFinished(object sender, RedisClientEventArgs e)
    {
        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        StepItem step = StepItem.CreateNew(e.StartTime);
        step.StepKind = StepKinds.Redis;
        step.StepName = "Redis_" + e.Method.Name;
        step.IsAsync = e.Method.Name.EndsWith0("Async") ? 1 : 0; // 这里采用一种简化方法，最准确做法要检查方法的返回值。
        step.SetException(e.Exception);

        step.Cmdx = new RedisInvokeInfo {
            Method = e.Method,
            Arguments = e.Arguments,
        };

        step.End(e.EndTime);

        scope.AddStep(step);
    }
}


internal class RedisInvokeInfo : ILoggingObject
{
    public MethodInfo Method { get; set; }

    public object[] Arguments { get; set; }

    public string ToLoggingText()
    {
        if( this.Arguments.IsNullOrEmpty() )
            return string.Empty;

        StringBuilder sb = StringBuilderPool.Get();
        try {

            int i = 0;
            foreach( var p in this.Method.GetParameters() ) {

                if( sb.Length > 0 )
                    sb.Append(", ");

                object value = this.Arguments[i];
                string text = value == null
                                    ? "null"
                                    : (value.GetType() == typeof(string)
                                        ? ("\"" + value.ToString().SubstringN(LoggingLimit.RedisArgValueMaxLen) + "\"")
                                        : value.ToString2());

                sb.Append($"{p.Name}={text}");
                i++;
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }
}
