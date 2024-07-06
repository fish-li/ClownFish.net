namespace ClownFish.Web.Aspnetcore;

/// <summary>
/// 标记 Controller 只允许在 dev/test 环境下调用
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class OnlyTestEnvAttribute : Attribute
{
    internal static bool CurrentIsAllow(ActionDescription action)
    {
        if( EnvUtils.IsProdEnv && action.ControllerType.GetMyAttribute<OnlyTestEnvAttribute>() != null )
            return false;

        return true;
    }
}
