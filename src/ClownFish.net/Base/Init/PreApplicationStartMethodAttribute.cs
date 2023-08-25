namespace ClownFish.Base;

/// <summary>
/// 提供对应用程序启动的扩展支持。
/// </summary>
/// <remarks>
///  说明：
///  ASP.NET也有一个同名的类型，即：System.Web.PreApplicationStartMethodAttribute
///  那个类型的初始化执行时间比较早，而且不能由我们的代码来决定何时启动初始化，例如：
///  如果使用那个类型，在初始化时不能调用 BuildManager.GetReferencedAssemblies(); （会抛异常）
///  
///  使用当前这个类型，可以由我们决定什么时候开始初始化，例如：
///  我们可以在Global.asax的Application_Start中调用AppInitializer.Start()来触发初始化。
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class PreApplicationStartMethodAttribute : Attribute
{

    /// <summary>
    /// 初始化 PreApplicationStartMethodAttribute 类的新实例。
    /// </summary>
    /// <param name="type">一个描述启动方法的类型的对象。</param><param name="methodName">没有返回值的空参数签名。</param>
    public PreApplicationStartMethodAttribute(Type type, string methodName)
    {
        if( type == null )
            throw new ArgumentNullException("type");

        if( string.IsNullOrEmpty(methodName) )
            throw new ArgumentNullException("methodName");

        Type = type;
        MethodName = methodName;
    }

    /// <summary>
    /// 获取关联启动方法所返回的类型。
    /// </summary>
    /// <returns>
    /// 一个描述启动方法的类型的对象。
    /// </returns>
    public Type Type { get; private set; }

    /// <summary>
    /// 获取关联的启动方法。
    /// </summary>
    /// <returns>
    /// 一个字符串，其中包含关联启动方法的名称。
    /// </returns>
    public string MethodName { get; private set; }
}
