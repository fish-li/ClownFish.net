﻿//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using ClownFish.Base;
//using ClownFish.Data.CodeDom;


////  命令行使用方法
////  =========================================
////  1、搜索指定的DLL中的实体类，生成代理程序集
////  dotnet.exe ClownFish.Data.ProxyGen.dll  d:\path\demo.dll

////  2、搜索指定目录下的所有DLL中的所有实体类型，生成代理程序集
////  dotnet.exe ClownFish.Data.ProxyGen.dll  d:\path

//// 命令行参数可以是一个程序集文件的路径，也可以是一个目录，建议采用完整路径，
//// 如果参数是目录，将查找目录下的所有实体程序集，并为每个实体程序集生成代理程序集。
//// 程序执行后，将生成一个 xxx.EntityProxy.dll 文件（与实体程序集相同的目录），如果文件已存在将会更新。




//namespace ClownFish.Data
//{
//    /// <summary>
//    /// 提供一个工具类，用于封装实体代码的生成过程。
//    /// 
//    /// 如果需要一个独立的“实体代理程序集生成工具”，
//    /// 可以创建一个控制台项目，并在 Program.Main() 中 直接调用 EntityProxyGenProgram.Run(args) 即可。
//    /// </summary>
//    public static class EntityProxyGenProgram
//    {

//        private static string s_dllDirectory;

//        /// <summary>
//        /// 启动入口
//        /// </summary>
//        /// <param name="args"></param>
//        public static void Run(string[] args)
//        {
//            if( args.Length < 1 ) {
//                Console.WriteLine("没有指定实体程序集文件名或者包含实体程序集的路径。");
//                return;
//            }

//            //Console.WriteLine("调试消息：请在附加进程后，按ENTER继续，");
//            //Console.ReadLine();

//            string[] commandArgs = (from x in System.Environment.GetCommandLineArgs()
//                                    let a = "\"" + x + "\""
//                                    select a).ToArray();

//            Console.WriteLine("Execute: " + string.Join(" ", commandArgs));


//            // 读取命令行参数，并计算绝对路径
//            bool isFile = false;
//            string path = Path.GetFullPath(Path.Combine(System.Environment.CurrentDirectory, args[0]));

//            if( RetryFile.Exists(path) ) {
//                if( path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) == false ) {
//                    Console.WriteLine("命令参数必须是一个程序集的DLL文件。");
//                    return;
//                }

//                s_dllDirectory = Path.GetDirectoryName(path);
//                isFile = true;
//            }
//            else if( Directory.Exists(path) ) {
//                s_dllDirectory = path;
//            }
//            else {
//                Console.WriteLine($"指定的参数 {args[0]} 既不是文件也不是目录。");
//                return;
//            }


//            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;


//            // 生成代理类程序集
//            try {
//                if( isFile )
//                    Execute(path);
//                else
//                    ExecuteBatch(path);
//            }
//            catch( Exception ex ) {
//                Console.WriteLine(ex.ToString());
//            }
//        }

//        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
//        {
//            if( s_dllDirectory == null )
//                return null;


//            string name = args.Name.Split(',')[0] + ".dll";
//            string filePath = Path.Combine(s_dllDirectory, name);

//            if( RetryFile.Exists(filePath) == false )
//                return null;

//            return Assembly.LoadFrom(filePath);
//        }



//        private static void Execute(string dllFile, Assembly asm = null)
//        {
//            string currentPath = Path.GetDirectoryName(dllFile);

//            string newName = Path.GetFileNameWithoutExtension(dllFile) + ".EntityProxy.dll";
//            string outFile = Path.Combine(currentPath, newName);

//            RetryFile.Delete(outFile);

//            if( asm == null )
//                asm = Assembly.LoadFrom(dllFile);

//            Type[] entityTypes = ProxyBuilder.GetAssemblyEntityTypes(asm);
//            if( entityTypes == null || entityTypes.Length == 0 )
//                return;

//            ProxyBuilder.Compile(entityTypes, outFile);

//            Console.WriteLine("成功生成实体代理程序集：" + outFile);
//        }


//        private static void ExecuteBatch(string directory)
//        {
//            string[] files = Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);

//            foreach( string file in files ) {
//                // 排除一些不可能的程序集
//                if( file.EndsWith(".EntityProxy.dll", StringComparison.OrdinalIgnoreCase) )
//                    continue;
//                if( file.StartsWith("System.", StringComparison.OrdinalIgnoreCase) )
//                    continue;
//                if( file.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) )
//                    continue;


//                Assembly asm = null;
//                try {
//                    asm = Assembly.LoadFrom(file);
//                }
//                catch { // 有些DLL可能就不是.NET程序集，所以是存在加载异常的，这里就直接忽略。
//                }

//                if( asm == null )
//                    continue;

//                Execute(file, asm);
//            }

//        }

//    }
//}
