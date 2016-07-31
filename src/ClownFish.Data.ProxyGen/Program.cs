using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.CodeDom;


// 命令行格式：ClownFish.Data.ProxyGen.exe "E:\ProjectFiles\my-github\ClownFish.data\test\ClownFish.Data.UnitTest\bin\ClownFish.Data.UnitTest.dll"

// 命令行参数是一个包含实体的程序集文件，建议采用完整路径，
// 程序执行后，将生成一个 xxx.EntityProxy.dll 文件（与实体程序集相同的目录），如果文件已存在将会更新。


namespace ClownFish.Data.ProxyGen
{
	class Program
	{
		static void Main(string[] args)
		{
			if( args.Length < 1 ) {
				Console.WriteLine("没有指定实体程序集文件名。");
				return;
			}



			// 读取命令行参数
			string entityDllFile = args[0];
			if( entityDllFile.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) == false ) {
				Console.WriteLine("命令参数必须是一个程序集的DLL文件。");
				return;
			}

			// 生成代理类程序集
			try {
				Execute(entityDllFile);

				Console.WriteLine("ClownFish.Data.ProxyGen.exe execute succes.");
			}
			catch( Exception ex ) {
				Console.WriteLine(ex.ToString());
			}
		}



		private static void Execute(string entityDllFile)
		{
			string currentPath = Path.GetDirectoryName(entityDllFile);
			if( string.IsNullOrEmpty(currentPath) )
				currentPath = AppDomain.CurrentDomain.BaseDirectory;


			string newName = Path.GetFileNameWithoutExtension(entityDllFile) + ".EntityProxy.dll";
			string outFile = Path.Combine(currentPath, newName);

			if( File.Exists(outFile) )
				File.Delete(outFile);

			// 设置当前路径
			Environment.CurrentDirectory = currentPath;

			Assembly asm = Assembly.LoadFile(entityDllFile);
			Type[] entityTypes = ProxyBuilder.GetAssemblyEntityTypes(asm);

			ProxyBuilder.Compile(entityTypes, outFile);

		}
	}
}
