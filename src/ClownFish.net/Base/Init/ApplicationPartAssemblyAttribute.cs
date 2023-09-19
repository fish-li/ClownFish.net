namespace ClownFish.Base;

/// <summary>
/// 标记某个程序集是应用程序的共享部分，它可以和主程序集一样被对待。
/// 它的一些公开类型将会自动注册，例如：Controller, BackgroundTask, HttpModule
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class ApplicationPartAssemblyAttribute : Attribute
{
}
