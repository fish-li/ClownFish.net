namespace ClownFish.Data.CodeDom;

// *****************************
// 注意：这个类型的  【命名空间，类型名称，方法名称】，都不能改名，因为它们被 ClownFish.CodeGen 在反射调用。
// *****************************


internal static class CodeGenUtils
{
    internal static int CompileEntityProxyAsm(string binPath, string dllSaveFilePath, bool deleteTempFile)
    {
        // 先删除遗留文件
        RetryFile.Delete(dllSaveFilePath);
        RetryFile.Delete(dllSaveFilePath + ".cs");
        RetryFile.Delete(dllSaveFilePath + ".error");


        List<Type> listTypes = SearchEntityTypes(binPath);

        if( listTypes.Count == 0 ) {
            Console2.WriteLine("没有找到实体类型。");
            return 0;
        }

        List<EntityCompileResult> compileResult = ProxyBuilder.Compile(listTypes.ToArray(), dllSaveFilePath);
        Console2.WriteLine($"已为 {compileResult.Count} 个实体生成代理和加载器类型。");

        if( deleteTempFile ) {
            RetryFile.Delete(dllSaveFilePath + ".cs");
            RetryFile.Delete(dllSaveFilePath + ".error");
        }

        return compileResult.Count;
    }

    

    internal static List<Type> SearchEntityTypes(string binPath)
    {
        string[] files = Directory.GetFiles(binPath, "*.dll", SearchOption.TopDirectoryOnly);

        List<Assembly> asmList = new List<Assembly>();
        List<Type> listTypes = new List<Type>();

        foreach( string file in files ) {

            string filename = Path.GetFileName(file);

            // 排除一些不可能的程序集
            if( filename.StartsWith("System.", StringComparison.OrdinalIgnoreCase) )
                continue;
            if( filename.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) )
                continue;

            try {
                Assembly asm = Assembly.LoadFrom(file);

                if( asm.GetCustomAttribute<EntityAssemblyAttribute>() != null ) {
                    asmList.Add(asm);
                }
            }
            catch { // 有些DLL可能就不是.NET程序集，所以是存在加载异常的，这里就直接忽略。
            }
        }

        // 获取所有已生成的实体代理类型
        foreach( Assembly asm in asmList ) {
            Type[] types = ProxyBuilder.GetAssemblyEntityTypes(asm);
            listTypes.AddRange(types);
        }

        return listTypes;
    }


}
