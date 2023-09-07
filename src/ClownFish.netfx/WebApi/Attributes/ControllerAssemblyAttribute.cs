namespace ClownFish.WebApi;

/// <summary>
/// 指示包含Controller的程序集
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class ControllerAssemblyAttribute : Attribute
{
}
