using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
    internal static class ReflectionExtensions
    {
        public static string ToTypeString(this Type t)
        {
            return GetCommonestTypeName(t) ?? GetTypeString(t);
        }

        /// <summary>
        /// 获取一些常用类型的简写名称，免得生成像 System.Int32 这样长名称
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string GetCommonestTypeName(Type t)
        {
            if (t.IsGenericType) {
                if (t == TypeList._int_null)
                    return "int?";
                else if (t == TypeList._bool_null)
                    return "bool?";
                else if (t == TypeList._decimal_null)
                    return "decimal?";
                else if (t == TypeList._Guid_null)
                    return "Guid?";
                else if (t == TypeList._DateTime_null)
                    return "DateTime?";
                else if (t == TypeList._long_null)
                    return "long?";
                else if (t == TypeList._double_null)
                    return "double?";
				else if( t == TypeList._short_null )
					return "short?";
			}
            else {                
                if (t == TypeList._string)
                    return "string";

                else if (t == TypeList._int)
                    return "int";
                else if (t == TypeList._bool)
                    return "bool";
                else if (t == TypeList._decimal)
                    return "decimal";
                else if (t == TypeList._Guid)
                    return "Guid";
                else if (t == TypeList._DateTime)
                    return "DateTime";
                else if (t == TypeList._long)
                    return "long";
                else if (t == TypeList._double)
                    return "double";
				else if( t == TypeList._short )
					return "short";
				else if( t == TypeList._byteArray )
					return "byte[]";
			}


            // 如果不是常用类型，就返回 null
            return null;
        }


        private static string GetTypeString(this Type t)
        {
            if (t.IsGenericType == false || t.ContainsGenericParameters)
                return t.FullName.Replace('+', '.');

            Type define = t.GetGenericTypeDefinition();

            string defineString = define.FullName;
            int p = defineString.IndexOf('`');

            StringBuilder sb = new StringBuilder();
            sb.Append(defineString.Substring(0, p)).Append("<");

            Type[] paras = t.GetGenericArguments();
            foreach (Type a in paras)
                sb.Append(ToTypeString(a)).Append(",");

            sb.Remove(sb.Length - 1, 1);
            sb.Append(">");
            return sb.ToString();
        }

        public static bool IsAnonymousType(this Type testType)
        {
            return testType.Name.StartsWith("<>f__AnonymousType");
        }


        public static bool IsIndexerProperty(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            // 其实索引器的名称应该是 Item ，不过，不能直接判断它。
            // 因为：如果没有索引器，那么可以定义名为 Item 的属性。

            if (propertyInfo.CanWrite) {
                MethodInfo method = propertyInfo.GetSetMethod(true);
                // 如果是普通属性，应该只有一个参数: value
                return method.GetParameters().Length > 1;
            }

            if (propertyInfo.CanRead) {
                MethodInfo method = propertyInfo.GetGetMethod(true);
                // 如果是普通属性，它应该是没有输入参数的。
                return method.GetParameters().Length > 0;
            }                      

            return false;
        }


        public static bool IsVirtual(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            if (propertyInfo.SetMethod != null)
                return propertyInfo.SetMethod.IsVirtual;

            else if (propertyInfo.GetMethod != null)
                return propertyInfo.GetMethod.IsVirtual;

            // 应当不会到这里！
            return false;
        }
    }
}
