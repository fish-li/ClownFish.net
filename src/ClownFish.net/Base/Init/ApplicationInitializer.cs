namespace ClownFish.Base;

/// <summary>
/// 应用程序始化工具类
/// </summary>
public static class ApplicationInitializer
{
    /// <summary>
    /// 执行所有初始化方法。 
    /// 初始化方法入口由 [PreApplicationStartMethod]/[Init]/AppInitializer.Init() 指定
    /// </summary>
    public static void Execute()
    {
        PreApplicationStartMethodAttribute.ExecuteAll();

        InitAttribute.ExecuteAll();

        ExecuteAppInit();
    }


    private static void ExecuteAppInit()
    {
        List<Type> list = (from asm in AppPartUtils.GetApplicationPartAsmList()
                           from x in asm.GetPublicTypes()
                           where x.Name == "AppInitializer"
                           select x).ToList();

        foreach( Type type in list ) {
            MethodInfo method = type.GetMethod("Init", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

            if( method != null ) {
                Console2.Info($"Execute {type.FullName}.Init()");
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
