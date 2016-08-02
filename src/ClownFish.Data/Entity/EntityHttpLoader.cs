using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.Reflection;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Data
{
	internal static class EntityHttpLoader
	{
		private static Type s_builderType;
		private static MethodInfo s_method;

		private static readonly object s_lock = new object();
		private static bool s_inited = false;

		private static void Init()
		{
			if( s_inited == false ) {
				lock( s_lock ) {
					if( s_inited == false ) {

						s_builderType = Type.GetType("ClownFish.Web.Serializer.ModelBuilder, ClownFish.Web");
						if( s_builderType == null )
							throw new InvalidOperationException("Type ClownFish.Web.Serializer.ModelBuilder NOT FOUND!");

						s_method = s_builderType.GetMethod("FillObjectFromHttp",
							 BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy,
							 null, new Type[] { typeof(HttpContext), typeof(object), typeof(ParameterInfo) }, null);

						if( s_builderType == null )
							throw new InvalidOperationException("Method ClownFish.Web.Serializer.ModelBuilder.FillModelFromHttp NOT FOUND!");

						s_inited = true;
					}
				}
			}
		}


		public static Entity LoadFromHttp(HttpContext context, ParameterInfo p)
		{
			// 这个方法原本就是给 ClownFish.Web 调用的，
			// 所以这里直接反射 ClownFish.Web 来调用它的实体填充逻辑。

			if( context == null )
				throw new ArgumentNullException(nameof(context));
			if( p == null )
				throw new ArgumentNullException(nameof(p));

			Init();

			Entity resultObject = null;
			Entity entityObject = p.ParameterType.FastNew() as Entity;

			Type proxyType = EntityProxyFactory.GetProxy(p.ParameterType);
			if( proxyType != null ) {
				IEntityProxy proxy = proxyType.FastNew() as IEntityProxy;
				proxy.Init(entityObject);
				resultObject = proxy as Entity;
			}
			else
				resultObject = entityObject;

			// 为了不直接依赖于 ClownFish.Web 项目，这里就采用反射方式处理
			object builder = ObjectFactory.New(s_builderType);
			s_method.FastInvoke(builder, context, resultObject, p);

			return resultObject;
		}
	}
}
