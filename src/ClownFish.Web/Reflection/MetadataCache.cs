using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ClownFish.Web.Reflection;
using ClownFish.Base.TypeExtend;
using ClownFish.Base.Reflection;

namespace ClownFish.Web.Reflection
{
	// 说明：
	// 1. 对于PageController类型，这里不缓存，只缓存所包含的Action
	// 2. 对于ServiceController，在初始化时，只查找出Controller，包含的Action采用延迟加载方式

	// 原因：
	// 1. 页面请求URL是在Action中指定的，因此，只能先找到所有能处理页面请求的Action
	// 2. Service调用时，可以从URL中解析出要调用的类名与方法名，因此可以在调用时再去查找（延迟加载）。


	internal class MetadataCache
	{
		#region 缓存变量定义


		// 用于从类型查找Action的反射标记，后续改进计划：未来版本将不再支持 静态方法。
		internal static BindingFlags ActionFindBindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public;

		//----------------------------------------------------------------------------


		// PageController的Action字典（初始化时填充）
		public Dictionary<string, ActionDescription> PageActionDict { get; private set; }

		// 正则表达式匹配URL的数组（初始化时填充）
		public RegexActionDescription[] PageUrlRegexArray { get; private set; }

		//----------------------------------------------------------------------------

		// Service字典表（初始化时填充）
		public Dictionary<string, ControllerDescription> ServiceFullNameDict { get; private set; }


		// 保存ServiceController的Action字典（边用边填充）
		public Hashtable ServiceActionTable { get; private set; }


		//----------------------------------------------------------------------------

		// 命名空间映射关系的字典（初始化时填充）
		public Dictionary<string, NamespaceMapAttribute> NamespaceMapDict { get; private set; }


		#endregion


		#region Init

		private volatile bool _inited;
		private readonly object _lock = new object();

		public void Init()
		{
			if( _inited == false ) {
				lock( _lock ) {
					if( _inited == false ) {

						ControllerRecognizer recognizer = ObjectFactory.New<ControllerRecognizer>();
						List<Assembly> actionAssemblyList = recognizer.GetControllerAssembly();

						BuildRestNamespaceDict(actionAssemblyList);

						InitControllers(actionAssemblyList);

						ServiceActionTable = Hashtable.Synchronized(new Hashtable(4096, StringComparer.OrdinalIgnoreCase));
						_inited = true;
					}
				}
			}
		}


		/// <summary>
		/// 构建命名空间映射表（从命名空间全名到别名的映射关系）
		/// </summary>
		/// <param name="actionAssemblyList"></param>
		private void BuildRestNamespaceDict(List<Assembly> actionAssemblyList)
		{
			var namespaceMapDict = new Dictionary<string, NamespaceMapAttribute>(128, StringComparer.OrdinalIgnoreCase);

			foreach( Assembly assembly in actionAssemblyList ) {

				NamespaceMapAttribute[] namespaceAttrs = (NamespaceMapAttribute[])assembly.GetCustomAttributes(typeof(NamespaceMapAttribute), true);

				foreach( NamespaceMapAttribute attr in namespaceAttrs )
					try {
						namespaceMapDict.Add(attr.ShortName, attr);
					}
					catch( ArgumentException ) {
						throw new InvalidProgramException(string.Format("指定的NamespaceMapAttribute无效，因为ShortName存在重复值 {0} ，当前程序集 {1}",
							attr.ShortName,
							assembly.FullName));
					}
			}

			NamespaceMapDict = namespaceMapDict;
		}

		private void InitControllers(List<Assembly> actionAssemblyList)
		{
			List<ControllerDescription> serviceControllerList = new List<ControllerDescription>(1024);
			List<ControllerDescription> pageControllerList = new List<ControllerDescription>(1024);

			ControllerRecognizer recognizer = ObjectFactory.New<ControllerRecognizer>();

			foreach( Assembly assembly in actionAssemblyList ) {

				foreach( Type t in assembly.GetPublicTypes() ) {
					if( t.IsClass == false || (t.IsAbstract && t.IsSealed ==false ) /* 抽象类不能实例化 */ )
						continue;

					if( recognizer.IsPageController(t) )
						pageControllerList.Add(new ControllerDescription(t));

					else if( recognizer.IsServiceController(t) )
						serviceControllerList.Add(new ControllerDescription(t));
				}
			}


			BuildServiceActionDict(serviceControllerList);

			BuildPageActionDict(pageControllerList);
		}

		private void BuildServiceActionDict(List<ControllerDescription> serviceControllerList)
		{
			// 用于 Service 调用的Action信息则采用延迟加载的方式。
			// 所以这里只提取类型全名称形成字典

			ServiceFullNameDict = serviceControllerList.ToDictionary(x => x.ControllerType.FullName, StringComparer.OrdinalIgnoreCase);
		}


		private void BuildPageActionDict(List<ControllerDescription> pageControllerList)
		{
			// 由于 PageController 中的 Action 是采用URL匹配模式（不是采用 namespace / class / method 的解析模式），
			// 所以需要提前加载 PageController 中的所有Action方法

			// 判断逻辑：
			// 1、方法是公开的
			// 2、有 [PageUrl] 或者 [PageRegexUrl]
			// 3、[Action] 是可选的，因为上面二个Attribute已经很明确了，没有它们指定URL，有[Action]也没什么用

			PageActionDict = new Dictionary<string, ActionDescription>(4096, StringComparer.OrdinalIgnoreCase);

			List<RegexActionDescription> regexActions = new List<RegexActionDescription>();


			// 遍历所有 PageController 的所有公开方法
			foreach( ControllerDescription controller in pageControllerList ) {
				foreach( MethodInfo m in controller.ControllerType.GetMethods(ActionFindBindingFlags) ) {

					ActionDescription actionDescription = null;

					// 提取 PageUrlAttribute
					PageUrlAttribute[] pageUrlAttrs = m.GetMyAttributes<PageUrlAttribute>();
					foreach( PageUrlAttribute attr in pageUrlAttrs ) {
						if( string.IsNullOrEmpty(attr.Url) == false ) {

							if( actionDescription == null )
								actionDescription = CreatePageActionDescription(controller, m);

							PageActionDict.Add(attr.Url, actionDescription);
						}
					}


					// 提取 PageRegexUrlAttribute
					PageRegexUrlAttribute[] regexAttrs = m.GetMyAttributes<PageRegexUrlAttribute>();
					foreach( PageRegexUrlAttribute attr2 in regexAttrs ) {
						if( string.IsNullOrEmpty(attr2.Url) == false ) {

							if( actionDescription == null )
								actionDescription = CreatePageActionDescription(controller, m);

							Regex regex = attr2.GetRegex();
							regexActions.Add(new RegexActionDescription { Regex = regex, ActionDescription = actionDescription });
						}
					}

				}
			}

			PageUrlRegexArray = regexActions.ToArray();
		}


		private ActionDescription CreatePageActionDescription(ControllerDescription controller, MethodInfo m)
		{
			return new ActionDescription(m) { PageController = controller };
		}

		#endregion
	}
}
