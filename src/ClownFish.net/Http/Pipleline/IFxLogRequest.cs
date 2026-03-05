namespace ClownFish.Http.Pipleline;


// https://learn.microsoft.com/zh-cn/previous-versions/aspnet/bb470252(v=vs.100)
// 日志这块，就不参考MS的设计了~~~

// 为了确保框架的日志功能不被外部模块干扰，ILogRequest 接口被设计成一个独立的【内部】接口，专门用于记录日志。
// 由于“用户代码”可以出现在 EndRequest 阶段中，甚至执行时间不确定，
// 为了能更好的记录性能相关信息，LogRequest 会放在 EndRequest 之后

internal interface IFxLogRequest
{
    void FxLogRequest(NHttpContext httpContext);
}
