namespace ClownFish.Data.CodeDom;

/// <summary>
/// 用于生成并编译实体的代理类的工具类
/// </summary>
internal static class ProxyBuilder
{
    private static bool s_inited = false;

    internal static DebugReportBlock CompileEntityListReportBlock { get; private set; }


    /// <summary>
    /// 自动搜索当前程序加载的所有实体类型，并为它们编译生成代理类型及注册。
    /// 如果已存在代理程序集，会直接加载，不会再次编译。
    /// 
    /// 注意：
    /// 1、搜索实体时只搜索用EntityAssemblyAttribute标记的程序集！
    /// 2、这个方法应该在初始化时被调用一次（再次调用会引发异常）
    /// 3、当前方法采用同步单线程方式运行，如果实体过多的大型项目，建议使用工具提前生成代理类型。
    /// </summary>
    /// <param name="dllOutPath"></param>
    /// <param name="useAttrFilter">
    /// 仅搜索标记为EntityAssemblyAttribute的程序集，
    /// 设置为true将会加快搜索速度，
    /// 但是要求包含实体类型的程序集用 [assembly: ClownFish.Data.EntityAssembly] 标记出来</param>
    /// <returns>返回处理了哪些实体类型（通常情况下不需要接收返回值，除非需要排错）。</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static List<Type> CompileAllEntityProxy(string dllOutPath, bool useAttrFilter)
    {
        if( string.IsNullOrEmpty(dllOutPath) )
            throw new ArgumentNullException(nameof(dllOutPath));

        if( s_inited )
            throw new InvalidOperationException("CompileAllEntityProxy方法不允许重复调用。");


        if( RetryFile.Exists(dllOutPath) )
            RetryFile.Delete(dllOutPath);

        // add to debug repowrt

        // 获取所有已生成的实体代理类型编译结果
        List<EntityCompileResult> existCompileResult = ProxyLoader.SearchExistEntityCompileResult();
        ProxyLoader.RegisterCompileResult(existCompileResult);


        // 搜索所有需要编译的实体类型
        List<Type> listTypes = SearchAllEntityTypes(existCompileResult, useAttrFilter);
        LogDebugInfo(listTypes);


        // 编译代理类型
        List<EntityCompileResult> compileResult = Compile(listTypes.ToArray(), dllOutPath);
        ProxyLoader.RegisterCompileResult(compileResult);

        s_inited = true;
        return listTypes;
    }



    private static void LogDebugInfo(List<Type> listTypes)
    {
        DebugReportBlock block = new DebugReportBlock { Category = "Compile Entity List", Order = 1001 };

        if( listTypes.HasValue() ) {

            int i = 1;
            foreach( Type t in listTypes )
                block.AppendLine($"{i++,4}: {t.FullName}");
        }

        CompileEntityListReportBlock = block;
    }



    /// <summary>
    /// 查找当前进程加载的程序集中所有的实体类型
    /// </summary>
    /// <param name="existCompileResult"></param>
    /// <param name="useAttrFilter"></param>
    /// <returns></returns>
    internal static List<Type> SearchAllEntityTypes(List<EntityCompileResult> existCompileResult, bool useAttrFilter)
    {
        List<Type> listTypes = new List<Type>();
        List<Assembly> entityAsmList = null;

        if( useAttrFilter ) {
            // 仅搜索标记为EntityAssemblyAttribute的程序集
            entityAsmList = AsmHelper.GetAssemblyList<EntityAssemblyAttribute>();
        }
        else {
            // 先获取进程加载的所有程序集
            Assembly[] allAsm = AsmHelper.GetLoadAssemblies(true);

            entityAsmList = new List<Assembly>(allAsm.Length);

            // 排除用EntityProxyAssemblyAttribute标记过的程序集（实体代理程序集）
            foreach( Assembly asm in allAsm ) {
                if( asm.GetCustomAttribute<EntityProxyAssemblyAttribute>() == null )
                    entityAsmList.Add(asm);
            }
        }

        // 获取所有已生成的实体代理类型
        foreach( Assembly asm in entityAsmList ) {
            Type[] types = GetAssemblyEntityTypes(asm);

            // 排除已经有代理类型的实体（已经有代理了，就不用再生成了）
            foreach( Type t in types ) {
                if( existCompileResult.FindIndex(x => x.EntityType == t) == -1 )
                    listTypes.Add(t);
            }
        }

        return listTypes;
    }


    /// <summary>
    /// 查找程序集中所有数据实体（从ClownFish.Data.Entity继承）
    /// </summary>
    /// <param name="asm"></param>
    /// <returns></returns>
    internal static Type[] GetAssemblyEntityTypes(Assembly asm)
    {
        if( asm == null )
            throw new ArgumentNullException(nameof(asm));

        List<Type> list = new List<Type>();

        foreach( Type t in asm.GetPublicTypes() ) {
            if( t.IsSubclassOf(TypeList.Entity) ) {

                // 排除抽象类
                if( t.IsAbstract )
                    continue;

                // 封闭类可以生成Loader类型，但是不能生成代理，为了简化判断逻辑，所以【暂时】排除。
                // 封闭类排除后 ，从数据库加载实体时，将使用反射版本。
                if( t.IsSealed )
                    continue;

                // 代理类继承于实体，所以需要排除
                if( typeof(IEntityProxy).IsAssignableFrom(t) )
                    continue;

                list.Add(t);
            }
        }

        return list.ToArray();
    }



    /// <summary>
    /// 生成并编译指定类型的代理类型
    /// </summary>
    /// <param name="entityTypes"></param>
    /// <param name="dllOutPath"></param>
    /// <returns></returns>
    internal static List<EntityCompileResult> Compile(Type[] entityTypes, string dllOutPath)
    {
        if( entityTypes == null )
            throw new ArgumentNullException(nameof(entityTypes));

        if( entityTypes.Length == 0 )
            return new List<EntityCompileResult>(0);


        StringBuilder sb = new StringBuilder(1024 * 64);
        sb.AppendLine(EntityGenerator.UsingCodeBlock);
        EntityGenerator.LoadAssembly();

        List<EntityCompileResult> result = new List<EntityCompileResult>(entityTypes.Length);

        for( int i = 0; i < entityTypes.Length; i++ ) {
            EntityGenerator g = new EntityGenerator();
            string code = g.GetCode(entityTypes[i]);
            sb.AppendLine(code);

            EntityCompileResult cr = new EntityCompileResult();
            cr.ProxyName = g.NameSpace + "." + g.ProxyClassName;
            cr.LoaderName = g.NameSpace + "." + g.DataLoaderClassName;
            result.Add(cr);
        }


        // 编译有可能会抛出异常，这里不处理。
        string allCode = sb.ToString();
        Assembly asm = CodeCompilerHelper.CompileCode(allCode, dllOutPath);

        foreach( var cr in result ) {
            cr.ProxyType = asm.GetType(cr.ProxyName);
            cr.LoaderType = asm.GetType(cr.LoaderName);
            cr.LoaderInstnace = cr.LoaderType.FastNew();       // 创建加载器的实例，供后面注册使用
        }

        return result;
    }

}
