namespace ClownFish.Web.Attributes;

/// <summary>
/// 标记某个类型是一个 API控制器类，API控制器类中的【公共实例方法】如果有 [ApiAction("/url....")] 修饰则会被当作Action来处理
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WebApiAttribute : Attribute
{
    // 目前没有任何属性，纯粹是一个标记属性
}
