using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


//  命令行使用方法
//  =========================================
//  dotnet.exe ClownFish.CodeGen.dll  d:\path\bin\net5.0  xxx.EntityProxy.dll

// 用途：搜索指定目录下的所有DLL中的所有实体类型，生成对应的代理程序集。
// 第1个参数：包含DLL文件的路径，其中包含有实体程序集。
// 第2个参数：输出的程序集名称，此参数可选。
// 程序运行后，将生成一个 xxx.EntityProxy.dll 文件，如果文件已存在会先删除。

namespace ClownFish.CodeGen
{
    class Program
    {
        private static string s_dllDirectory;

        static void Main(string[] args)
        {
            if( args.Length < 1 ) {
                Console.WriteLine("缺少必要的运行参数。");
                Console.WriteLine("示例：");
                Console.WriteLine("dotnet.exe ClownFish.CodeGen.dll  d:\\path\\bin\\net5.0  xxx.EntityProxy.dll");
                System.Environment.Exit(-1);
                return;
            }

            //Console.WriteLine("调试消息：请在附加进程后，按ENTER继续，");
            //Console.ReadLine();

            // 读取命令行参数，并计算绝对路径
            string binPath = Path.GetFullPath(Path.Combine(System.Environment.CurrentDirectory, args[0]));
            string dllOutPath = args.Length >= 2
                                ? Path.GetFullPath(Path.Combine(binPath, args[1]))
                                : Path.GetFullPath(Path.Combine(binPath, "_cfd_default_EntityProxy.dll"));
            Console.WriteLine("BinPath: " + binPath);
            Console.WriteLine("OutFile: " + dllOutPath);


            if( Directory.Exists(binPath) == false ) {
                Console.WriteLine($"目录 {binPath} 不存在。");
                System.Environment.Exit(-2);
                return;
            }

            string clownfishdll = Path.Combine(binPath, "ClownFish.net.dll");
            if( File.Exists(clownfishdll) == false ) {
                Console.WriteLine($"文件 {clownfishdll} 不存在。");
                System.Environment.Exit(-2);
                return;
            }

            File.Delete(dllOutPath);
            File.Delete(dllOutPath + ".cs");
            File.Delete(dllOutPath + ".error");

            s_dllDirectory = binPath;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;


            DateTime startTime = DateTime.Now;
            // 生成代理类程序集
            try {
                ExecuteBatch(binPath, dllOutPath);
            }
            catch( Exception ex ) {
                Console.WriteLine(ex.ToString());
                System.Environment.Exit(-3);
            }
            finally {
                TimeSpan time = DateTime.Now - startTime;
                Console.WriteLine("本次生成代理程序集总耗时：" + time.ToString());
            }
        }


        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine($"加载依赖程序集: " + args.Name);

            string name = args.Name.Split(',')[0] + ".dll";
            string filePath = Path.Combine(s_dllDirectory, name);

            if( File.Exists(filePath) == false )
                return null;

            return Assembly.LoadFrom(filePath);
        }


        private static void ExecuteBatch(string binPath, string dllOutPath)
        {
            string clownfishdll = Path.Combine(binPath, "ClownFish.net.dll");
            Assembly asm = Assembly.LoadFrom(clownfishdll);

            Type type = asm.GetType("ClownFish.Data.CodeDom.CodeGenUtils", true);

            MethodInfo method = type.GetMethod("CompileEntityProxyAsm", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if( method == null )
                throw new ApplicationException("方法 CodeGenUtils.CompileEntityProxyAsm 没有找到");

            // 这里采用反射的方式去调用，主要是因为避免出现 ClownFish.net 版本不一致问题
            // 出问题场景：
            // - ClownFish.CodeGen 引用了 v1 版本的 ClownFish.net
            // - project11 引用了 v2 版本的 ClownFish.net
            // 在 ClownFish.CodeGen 运行时，ClownFish.net 将不会重复加载，继续使用 v1 版本，这样就会出现不兼容问题。

            method.Invoke(null, new object[] { binPath, dllOutPath, true });
        }

    }
}
