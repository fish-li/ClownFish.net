using System.CodeDom;
using Microsoft.CSharp;


namespace ClownFish.Base;

/// <summary>
/// 包含一些与类型相关的扩展方法
/// </summary>
public static class TypeExtensionsCF
{
    /// <summary>
    /// 判断指定的类型是不是常见的值类型，范围：DateTime, TimeSpan，Guid，decimal， Enum
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsSimpleValueType(this Type t)
    {
        return t == typeof(DateTime)
            || t == typeof(TimeSpan)
            || t == typeof(Guid)
            || t == typeof(decimal)
            || t.IsEnum;
    }


    /// <summary>
    /// 得到一个实际的类型（排除Nullable类型的影响）。比如：int? 最后将得到int
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetRealType(this Type type)
    {
        if( type.IsGenericType )
            return Nullable.GetUnderlyingType(type) ?? type;
        else
            return type;
    }

    /// <summary>
    /// 判断指定的类型是不是可空类型
    /// </summary>
    /// <param name="nullableType"></param>
    /// <returns></returns>
    public static bool IsNullableType(this Type nullableType)
    {
        if( nullableType.IsGenericType
            && nullableType.IsGenericTypeDefinition == false
            && nullableType.GetGenericTypeDefinition() == typeof(Nullable<>) )
            return true;

        return false;
    }

    /// <summary>
    /// 判断指定的类型是不是 可空的枚举 类型
    /// </summary>
    /// <param name="nullableType"></param>
    /// <returns></returns>
    public static bool IsNullableEnum(this Type nullableType)
    {
        if( nullableType.IsGenericType
            && nullableType.IsGenericTypeDefinition == false
            && nullableType.GetGenericTypeDefinition() == typeof(Nullable<>)
            && Nullable.GetUnderlyingType(nullableType).IsEnum
            )
            return true;

        return false;
    }



    /// <summary>
    /// 判断二个类型是不是兼容（可转换）的
    /// </summary>
    /// <param name="t">要测试的类型</param>
    /// <param name="convertToType">需要转换的类型（基类或者接口类型）</param>
    /// <returns></returns>
    public static bool IsCompatible(this Type t, Type convertToType)
    {
        if( t == convertToType )
            return true;

        if( convertToType.IsInterface )
            return convertToType.IsAssignableFrom(t);
        else
            return t.IsSubclassOf(convertToType);
    }


    /// <summary>
    /// 获取一个类型的完整的名称，例如："Nebula.Security.Auth.WebUserInfo, Nebula.net"，返回的结果可用于调用Type.GetType(...)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static string GetFullName(this Type type)
    {
        if( type == null )
            throw new ArgumentNullException(nameof(type));

        return $"{type.FullName}, {type.Assembly.GetName().Name}";
    }


    /// <summary>
    /// 获取类型的C#代码文本
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static string GetTypeCodeText(this Type type)
    {
        if( type == null )
            throw new ArgumentNullException(nameof(type));

        using( CSharpCodeProvider csharpProvider = new CSharpCodeProvider() ) {
            CodeTypeReference typeReference = new CodeTypeReference(type);
            return csharpProvider.GetTypeOutput(typeReference);
        }
    }


    /// <summary>
    /// 获取一个封闭泛型的类型参数
    /// </summary>
    /// <param name="type">一个具体的封装泛型类型</param>
    /// <param name="baseTypeDefinition">泛型定义</param>
    /// <returns>泛型的类型参数</returns>
    public static Type GetArgumentType(this Type type, Type baseTypeDefinition)
    {
        if( type == null )
            throw new ArgumentNullException("type");

        if( baseTypeDefinition == null )
            throw new ArgumentNullException("baseTypeDefinition");

        if( baseTypeDefinition.IsGenericTypeDefinition == false )
            throw new ArgumentException("参数必须是一个泛型定义。", "baseTypeDefinition");


        if( type.IsGenericType && type.GetGenericTypeDefinition() == baseTypeDefinition )
            return type.GetGenericArguments()[0];


        return null;
    }

    /// <summary>
    /// 判断类型是否存在公开无参的构造方法，等效于泛型约束中的 new()
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool CanNew(this Type type)
    {
        if( type == null )
            throw new ArgumentNullException(nameof(type));

        if( type.IsAbstract )
            return false;

        ConstructorInfo ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
        return ctor != null;
    }


    /// <summary>
    /// 获取类型的实例方法信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MethodInfo GetInstanceMethod(this Type type, string name)
    {
        if( type == null )
            throw new ArgumentNullException(nameof(type));
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }


    /// <summary>
    /// 获取类型的实例方法信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MethodInfo GetStaticMethod(this Type type, string name)
    {
        if( type == null )
            throw new ArgumentNullException(nameof(type));
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }



}
