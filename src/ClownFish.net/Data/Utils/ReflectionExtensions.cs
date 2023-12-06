namespace ClownFish.Data;

internal static class ReflectionExtensions
{
    public static string ToTypeString(this Type t)
    {
        return
            // 获取一些常用类型的简写名称，免得生成像 System.Int32 这样长名称
            TypeList.GetTypeName(t)

            // 获取完整的名称
            ?? GetTypeString0(t);
    }


    internal static string GetTypeString0(this Type t)
    {
        if( t.IsArray ) {
            Type elementType = t.GetElementType();
            return elementType.ToTypeString() + "[]";
        }

        if( t.IsGenericType ) {

            Type define = t.GetGenericTypeDefinition();

            string defineString = define.FullName;
            int p = defineString.IndexOf('`');

            StringBuilder sb = StringBuilderPool.Get();
            try {
                sb.Append(defineString.Substring(0, p)).Append('<');

                Type[] paras = t.GetGenericArguments();
                foreach( Type a in paras )
                    sb.Append(ToTypeString(a)).Append(", ");

                sb.Remove(sb.Length - 2, 2);
                sb.Append('>');
                return sb.ToString().Replace("System.Collections.Generic.", "");
            }
            finally {
                StringBuilderPool.Return(sb);
            }
        }

        return t.FullName.Replace('+', '.');
    }

    public static bool IsAnonymousType(this Type testType)
    {
        return testType.Name.StartsWith("<>f__AnonymousType", StringComparison.Ordinal);
    }


    public static bool IsIndexerProperty(this PropertyInfo propertyInfo)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException(nameof(propertyInfo));

        // 其实索引器的名称应该是 Item ，不过，不能直接判断它。
        // 因为：如果没有索引器，那么可以定义名为 Item 的属性。

        if( propertyInfo.CanWrite ) {
            MethodInfo method = propertyInfo.GetSetMethod(true);
            // 如果是普通属性，应该只有一个参数: value
            return method.GetParameters().Length > 1;
        }

        if( propertyInfo.CanRead ) {
            MethodInfo method = propertyInfo.GetGetMethod(true);
            // 如果是普通属性，它应该是没有输入参数的。
            return method.GetParameters().Length > 0;
        }

        return false;
    }


    public static bool IsVirtual(this PropertyInfo propertyInfo)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException(nameof(propertyInfo));

        if( propertyInfo.SetMethod != null )
            return propertyInfo.SetMethod.IsVirtual;

        else if( propertyInfo.GetMethod != null )
            return propertyInfo.GetMethod.IsVirtual;

        // 应当不会到这里！
        return false;
    }
}
