namespace ClownFish.Data.CodeDom;

internal static class ProxyLoader
{
    internal static DebugReportBlock EntityProxyAssemblyListReportBlock { get; private set; }

    /// <summary>
    /// 加载所有已存在的实体代理对应的实体类型。
    /// 说明：已经存在的代理类型是用工具提前生成好的。
    /// </summary>
    /// <returns></returns>
    internal static List<EntityCompileResult> SearchExistEntityCompileResult()
    {
        // 工具生成的程序集一定会使用EntityProxyAssemblyAttribute，所以用它做过滤
        List<Assembly> proxyAsmList = AsmHelper.GetAssemblyList<EntityProxyAssemblyAttribute>();
        LogDebugInfo(proxyAsmList);


        List<EntityCompileResult> list = new List<EntityCompileResult>(1024);
        
        foreach( Assembly asm in proxyAsmList ) {

            foreach( Type t in asm.GetPublicTypes() ) {

                // 工具会为每个实体类型生成二个辅助类型：XxxxxProxy, XxxxxLoader
                // XxxxxLoader，实体加载器类型，用于从数据结果中加载实体列表
                // XxxxxProxy，实体代理类型，用于跟踪实体数据成员的修改情况，并用于生成INSERT/UPDATE/DELETE语句
                // 如果实体是密封类型，将不会生成实体代理类，但是实体加载器类型一定会生成
                // 所以，在查找时，先找【实体加载器类型】，并根据EntityAdditionAttribute来查找配对的【实体代理类型】

                // 【实体加载器类型】的样例代码请参考 \test\ClownFish.Data.UnitTest\AutoCode1.cs
                if( t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(BaseDataLoader<>) ) {

                    EntityCompileResult cr = new EntityCompileResult();
                    cr.EntityType = t.BaseType.GetGenericArguments()[0];

                    cr.LoaderType = t;
                    cr.LoaderInstnace = cr.LoaderType.FastNew();        // 创建加载器的实例，供后面注册使用

                    // 使用.NET原生版本GetCustomAttribute，因为不需要缓存，请不要修改成 GetMyAttribute<>()
                    EntityAdditionAttribute a = t.GetCustomAttribute<EntityAdditionAttribute>();
                    if( a != null ) // 允许没有代理类（实体就是封闭类型）
                        cr.ProxyType = a.ProxyType;

                    list.Add(cr);
                }
            }
        }

        return list;
    }


    private static void LogDebugInfo(List<Assembly> proxyAsmList)
    {
        DebugReportBlock block = new DebugReportBlock { Category = "EntityProxy Assembly List", Order = 1002 };

        if( proxyAsmList.HasValue() ) {
            foreach( Assembly asm in proxyAsmList )
                block.AppendLine(asm.Location);
        }

        EntityProxyAssemblyListReportBlock = block;
    }




    /// <summary>
    /// 注册实体代理类型
    /// </summary>
    /// <param name="result"></param>
    internal static void RegisterCompileResult(List<EntityCompileResult> result)
    {
        if( result == null || result.Count == 0 )
            return;

        // 【实体代理类型】允许为空，所以要过滤
        List<Type> proxyTypes = (from x in result where x.ProxyType != null select x.ProxyType).ToList();
        List<object> loaderInstances = (from x in result select x.LoaderInstnace).ToList();

        EntityProxyFactory.BatchRegister(proxyTypes);
        DataLoaderFactory.BatchRegister(loaderInstances);
    }






}
