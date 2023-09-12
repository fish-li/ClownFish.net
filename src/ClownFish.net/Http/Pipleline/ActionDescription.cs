namespace ClownFish.Http.Pipleline;

/// <summary>
/// 当前正在执行Action描述信息
/// </summary>
public sealed class ActionDescription
{
    /// <summary>
    /// Action的方法信息
    /// </summary>
    public MethodInfo MethodInfo { get; private set; }

    /// <summary>
    /// ControllerType
    /// </summary>
    public Type ControllerType { get; private set; }

    // 上面2个属性应该在 URL 路由期间确定下来，属于必填项。
    // Controller实例 允许 在执行时构造。

    /// <summary>
    /// Controller instance，允许在框架外部实例化
    /// </summary>
    public object Controller { get; set; }


    /// <summary>
    /// 构造方法。
    /// 此方法允许不指定 Controller 实例。
    /// </summary>
    /// <param name="controllerType"></param>
    /// <param name="method"></param>
    public ActionDescription(Type controllerType, MethodInfo method)
    {
        if( controllerType == null )
            throw new ArgumentNullException(nameof(controllerType));
        if( method == null )
            throw new ArgumentNullException(nameof(method));

        this.ControllerType = controllerType;
        this.MethodInfo = method;

        // this.Controller =  这个方法允许不指定 Controller
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="method"></param>
    /// <param name="controllerType"></param>
    public ActionDescription(object controller, MethodInfo method, Type controllerType = null)
    {
        if( controller == null )
            throw new ArgumentNullException(nameof(controller));
        if( method == null )
            throw new ArgumentNullException(nameof(method));

        this.Controller = controller;
        this.ControllerType = controllerType ?? controller.GetType();
        this.MethodInfo = method;
    }




    /// <summary>
    /// 从将要执行的方法或者类型查找特定的标记
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetActionAttribute<T>() where T : Attribute
    {
        T attr = this.MethodInfo.GetMyAttribute<T>();

        if( attr == null )
            attr = this.ControllerType.GetMyAttribute<T>();

        return attr;
    }

}
