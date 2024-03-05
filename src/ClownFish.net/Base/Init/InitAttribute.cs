namespace ClownFish.Base;

/// <summary>
/// 指示某个类型在启动时自动初始化（调用 static Init 方法）
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class InitAttribute : Attribute
{
    /// <summary>
    /// 初始化调用的方法名称，默认值：Init
    /// </summary>
    public string MethodName { get; set; }


    internal static void ExecuteAll()
    {
        // 搜索业务程序集
        List<Type> list = (from asm in AppPartUtils.GetApplicationPartAsmList()
                            from x in asm.GetPublicTypes()
                            let a = x.GetCustomAttribute<InitAttribute>()
                            where a != null
                            select x).ToList();


        foreach( var type in list ) {
            InitAttribute attr = type.GetCustomAttribute<InitAttribute>();
            string methodName = attr.MethodName.IsNullOrEmpty() ? "Init" : attr.MethodName;

            MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if( method != null ) {
                Console2.Info($"Execute: {type.FullName}.{methodName}()");
                try {
                    method.InvokeAndLog(null, null);
                }
                catch( TargetInvocationException ex ) {
                    throw ex.InnerException;   // 将原始异常抛出来
                }
            }
        }
    }
}
