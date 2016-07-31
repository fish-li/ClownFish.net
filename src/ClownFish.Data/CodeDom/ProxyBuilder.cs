using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;

namespace ClownFish.Data.CodeDom
{
    /// <summary>
    /// 用于生成并编译实体的代理类的工具类
    /// </summary>
	public static class ProxyBuilder
	{
		private static bool s_inited = false;

		/// <summary>
		/// 自动搜索当前程序加载的所有实体类型，并为它们编译生成代理类型及注册。
		/// 
		/// 注意：
		/// 1、搜索实体时只搜索用EntityAssemblyAttribute标记的程序集！
		/// 2、这个方法应该在初始化时被调用一次（再次调用会引发异常）
		/// 3、当前方法采用同步单线程方式运行，如果实体过多的大型项目，建议使用工具提前生成代理类型。
		/// </summary>
		/// <param name="useAttrFilter">
		/// 仅搜索标记为EntityAssemblyAttribute的程序集，
		/// 设置为true将会加快搜索速度，
		/// 但是要求包含实体类型的程序集用 [assembly: ClownFish.Data.EntityAssembly] 标记出来</param>
		/// <returns>返回处理了哪些实体类型（通常情况下不需要接收返回值，除非需要排错）。</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static List<Type> CompileAllEntityProxy(bool useAttrFilter = false)
		{
			if( s_inited )
				throw new InvalidOperationException("CompileAllEntityProxy方法不允许重复调用。");

			// 获取所有已生成的实体代理类型编译结果
			List<EntityCompileResult> existCompileResult = SearchExistEntityCompileResult();
			RegisterCompileResult(existCompileResult);


			// 搜索所有需要编译的实体类型
			List<Type> listTypes = SearchAllEntityTypes(existCompileResult, useAttrFilter);


			// 编译代理类型，不生成DLL文件
			List<EntityCompileResult> compileResult = Compile(listTypes.ToArray(), null);
			RegisterCompileResult(compileResult);

			s_inited = true;
			return listTypes;
		}

		/// <summary>
		/// 加载所有已存在的实体代理对应的实体类型。
		/// 说明：已经存在的代理类型是用工具提前生成好的。
		/// </summary>
		/// <returns></returns>
		private static List<EntityCompileResult> SearchExistEntityCompileResult()
		{
			// 工具生成的程序集一定会使用EntityProxyAssemblyAttribute，所以用它做过滤
			List<Assembly> proxyAsmList = ClownFish.Base.Reflection.ReflectionExtensions.GetAssemblyList<EntityProxyAssemblyAttribute>();

			List<EntityCompileResult> list = new List<EntityCompileResult>(1024);
			
			foreach( Assembly asm in proxyAsmList ) {
				foreach(Type t in asm.GetPublicTypes() ) {

					// 工具会为每个实体类型生成二个辅助类型：XxxxxProxy, XxxxxLoader
					// XxxxxLoader，实体加载器类型，用于从数据结果中加载实体列表
					// XxxxxProxy，实体代理类型，用于跟踪实体数据成员的修改情况，并用于生成INSERT/UPDATE/DELETE语句
					// 如果实体是密封类型，将不会生成实体代理类，但是实体加载器类型一定会生成
					// 所以，在查找时，先找【实体加载器类型】，并根据EntityAdditionAttribute来查找配对的【实体代理类型】

					// 【实体加载器类型】的样例代码请参考 \test\ClownFish.Data.UnitTest\AutoCode1.cs
					if( t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(BaseDataLoader<>)) {

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
				entityAsmList = ClownFish.Base.Reflection.ReflectionExtensions.GetAssemblyList<EntityAssemblyAttribute>();
			}
			else {
				// 先获取进程加载的所有程序集
				Assembly[] allAsm = Base.Framework.RunTimeEnvironment.GetLoadAssemblies(true);

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
            if (asm == null)
                throw new ArgumentNullException(nameof(asm));

			List<Type> list = new List<Type>();

			foreach(Type t in asm.GetPublicTypes() ) {
				if( t.IsSubclassOf(TypeList._Entity) ) {

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
		/// <param name="dllFilePath"></param>
		/// <returns></returns>
		internal static List<EntityCompileResult> Compile(Type[] entityTypes, string dllFilePath)
        {
            if (entityTypes == null)
                throw new ArgumentNullException(nameof(entityTypes));

			if( entityTypes.Length == 0 )
				return new List<EntityCompileResult>(0);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine(EntityGenerator.UsingCodeBlock);

			List<EntityCompileResult> result = new List<EntityCompileResult>(entityTypes.Length);

            for (int i = 0; i < entityTypes.Length; i++) {
                EntityGenerator g = new EntityGenerator();
                string code = g.GetCode(entityTypes[i]);
                sb.AppendLine(code);

				EntityCompileResult cr = new EntityCompileResult();
				cr.ProxyName =  g.NameSpace + "." + g.ProxyClassName;
				cr.LoaderName =  g.NameSpace + "." + g.DataLoaderClassName;
				result.Add(cr);
			}

			string tempOutPath = ConfigurationManager.AppSettings["ClownFish.Data:ProxyBuilder.TempOutPath"];

            // 编译有可能会抛出异常，这里不处理。
            Assembly asm = CompilerHelper.CompileCode(sb.ToString(), dllFilePath, tempOutPath);
            
            foreach(var cr in result) {
				cr.ProxyType = asm.GetType(cr.ProxyName);
				cr.LoaderType = asm.GetType(cr.LoaderName);
				cr.LoaderInstnace = cr.LoaderType.FastNew();       // 创建加载器的实例，供后面注册使用
			}

            return result;
        }

        
    }
}
