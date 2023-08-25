#if NETCOREAPP

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;


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

        // 解析代码，生成语法树结构
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

        // 获取当前加载的所有程序集，用于编译时引用
        MetadataReference[] refs =
            (from x in AsmHelper.GetLoadAssemblies()
             let path = x.Location
             where path.IndexOfIgnoreCase("VisualStudio") ==  -1        // 排除 MsTest 相关程序集
             select MetadataReference.CreateFromFile(path)).ToArray();

        // 说明：如果不加 where 过滤，会出现下面的异常
        // (29,2): error CS0433: The type 'SerializableAttribute' exists in both 
        //        'Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' 
        //         and 'System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e'

        // 根据异常去查找DLL，发现有很多个 Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices 这样的文件，
        // 但是存在这样一个文件：C:\VS\Shared\Packages\MSTest.TestAdapter.1.2.0\build\netcoreapp1.0\Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.dll
        // 狗日的，它里面的确包含一个 System.SerializableAttribute

        // 补充说明下：在开发时，查看 SerializableAttribute ，它定义在 System.Runtime.dll中
        // 狗日的 .net core 真是太乱 !



        // 创建编译单元
        var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(dllOutPath))
            .WithOptions(new CSharpCompilationOptions(
                    Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary,
                    usings: null,
                    optimizationLevel: OptimizationLevel.Debug,
                    checkOverflow: false,
                    allowUnsafe: true,
                    platform: Platform.AnyCpu,
                    warningLevel: 4,
                    // don't support XML file references in interactive (permissions & doc comment includes)
                    xmlReferenceResolver: null
                ))
            .AddReferences(refs)
            .AddSyntaxTrees(syntaxTree);

        // 生成程序集的 DLL文件
        EmitResult eResult = compilation.Emit(dllOutPath);

        string errorFilePath = dllOutPath + ".error";

        if( eResult.Success ) {
            RetryFile.WriteAllText(errorFilePath, "Compile successed!");
        }
        else {
            SaveError(eResult, errorFilePath);
            throw new InvalidOperationException("编译实体代理程序集失败，原因可查看错误文件：" + errorFilePath);
        }

        return Assembly.LoadFrom(dllOutPath);
    }


    internal static void SaveTempCode(string code, string codeFilePath)
    {
        if( File.Exists(codeFilePath) )
            RetryFile.Delete(codeFilePath);

        RetryFile.WriteAllText(codeFilePath, code, Encoding.UTF8);
    }

    internal static void SaveError(EmitResult eResult, string errorFilePath)
    {
        StringBuilder sb = new StringBuilder();

        foreach( var x in eResult.Diagnostics )
            sb.AppendLineRN(x.ToString());

        RetryFile.WriteAllText(errorFilePath, sb.ToString());
    }


}

#endif
