#if NETFRAMEWORK

using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace ClownFish.Data.CodeDom;

internal static class CodeCompilerHelper
{
    public static Assembly CompileCode(string code, string dllOutPath)
    {
        if( string.IsNullOrEmpty(code) )
            throw new ArgumentNullException(nameof(code));
        if( string.IsNullOrEmpty(dllOutPath) )
            throw new ArgumentNullException(nameof(dllOutPath));

        // 先保存代码，用于出问题时排查
        SaveTempCode(code, dllOutPath + ".cs");

        // 准备编译器参数
        CompilerParameters cp = new CompilerParameters();
        cp.GenerateExecutable = false;

        if( dllOutPath == null )
            cp.GenerateInMemory = true;
        else
            cp.OutputAssembly = dllOutPath;
        

        // 添加编译必须的程序集引用
        Assembly[] assemblies = AsmHelper.GetLoadAssemblies();
        foreach( Assembly assembly in assemblies ) {
            if( assembly.IsDynamic )
                continue;

            // 在单元测试环境，下面代码会遇到一个问题 ：
            // 已导入一个具有相同标识“Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a”的程序集。请尝试移除其中一个重复的引用。
            // 已导入一个具有相同标识“Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a”的程序集。请尝试移除其中一个重复的引用。
            // 已导入一个具有相同标识“Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.Interface, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a”的程序集。请尝试移除其中一个重复的引用。
            // 已导入一个具有相同标识“Microsoft.VisualStudio.TestPlatform.TestFramework, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a”的程序集。请尝试移除其中一个重复的引用。

            if( assembly.Location.IndexOfIgnoreCase("VisualStudio") > 0 )
                continue;

            cp.ReferencedAssemblies.Add(assembly.Location);
        }


        // 创建编译器实例并编译代码
        CSharpCodeProvider csProvider = new CSharpCodeProvider();
        CompilerResults cr = csProvider.CompileAssemblyFromSource(cp, code);

        string errorFilePath = dllOutPath + ".error";

        if( cr.Errors == null || cr.Errors.HasErrors == false ) {
            RetryFile.WriteAllText(errorFilePath, "Compile successed!");
        }
        else {
            SaveError(cr, errorFilePath);
            throw new InvalidOperationException("编译实体代理程序集失败，原因可查看错误文件：" + errorFilePath);
        }

        // 获取编译结果，它是编译后的程序集
        return cr.CompiledAssembly;
    }


    internal static void SaveTempCode(string code, string codeFilePath)
    {
        if( File.Exists(codeFilePath) )
            RetryFile.Delete(codeFilePath);

        RetryFile.WriteAllText(codeFilePath, code, Encoding.UTF8);
    }

    internal static void SaveError(CompilerResults cr, string errorFilePath)
    {
        StringBuilder sb = new StringBuilder();

        foreach( CompilerError error in cr.Errors )
            sb.AppendLine(error.ErrorText);


        try {
            RetryFile.WriteAllText(errorFilePath, sb.ToString(), Encoding.UTF8);
        }
        catch {  /* 输出最近一次运行时运行的异常，方便调试程序，忽略写文件出现的异常。 */
        }
    }

    
}


#endif