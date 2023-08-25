using System.Reflection.Emit;

namespace ClownFish.Base.Reflection;

internal delegate object CtorDelegate();

internal delegate object MethodDelegate(object target, object[] args);

internal delegate object GetValueDelegate(object target);

internal delegate void SetValueDelegate(object target, object arg);


internal static class DynamicMethodFactory
{
    public static CtorDelegate CreateConstructor(ConstructorInfo constructor)
    {
        if( constructor == null )
            throw new ArgumentNullException("constructor");
        if( constructor.GetParameters().Length > 0 )
            throw new NotSupportedException("不支持有参数的构造函数。");

        DynamicMethod dm = new DynamicMethod(
            "ctor",
            constructor.DeclaringType,
            Type.EmptyTypes,
            true);

        ILGenerator il = dm.GetILGenerator();
        il.Emit(OpCodes.Nop);
        il.Emit(OpCodes.Newobj, constructor);
        il.Emit(OpCodes.Ret);

        return (CtorDelegate)dm.CreateDelegate(typeof(CtorDelegate));
    }


    public static MethodDelegate CreateMethod(MethodInfo method)
    {

        DynamicMethod dynamicMethod = new DynamicMethod("DynamicMethod", typeof(object),
            new[] { typeof(object), typeof(object[]) },
            typeof(DynamicMethodFactory), true);

        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

        ParameterInfo[] parameters = method.GetParameters();

        Type[] paramTypes = new Type[parameters.Length];

        // copies the parameter types to an array
        for( int i = 0; i < paramTypes.Length; i++ ) {
            if( parameters[i].ParameterType.IsByRef )
                paramTypes[i] = parameters[i].ParameterType.GetElementType();
            else
                paramTypes[i] = parameters[i].ParameterType;
        }

        LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

        // generates a local variable for each parameter
        for( int i = 0; i < paramTypes.Length; i++ ) {
            locals[i] = ilGenerator.DeclareLocal(paramTypes[i], true);
        }

        // creates code to copy the parameters to the local variables
        for( int i = 0; i < paramTypes.Length; i++ ) {
            ilGenerator.Emit(OpCodes.Ldarg_1);
            EmitFastInt(ilGenerator, i);
            ilGenerator.Emit(OpCodes.Ldelem_Ref);
            EmitCastToReference(ilGenerator, paramTypes[i]);
            ilGenerator.Emit(OpCodes.Stloc, locals[i]);
        }

        if( !method.IsStatic ) {
            // loads the object into the stack
            ilGenerator.Emit(OpCodes.Ldarg_0);
        }

        // loads the parameters copied to the local variables into the stack
        for( int i = 0; i < paramTypes.Length; i++ ) {
            if( parameters[i].ParameterType.IsByRef )
                ilGenerator.Emit(OpCodes.Ldloca_S, locals[i]);
            else
                ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
        }

        // calls the method
        if( !method.IsStatic ) {
            ilGenerator.EmitCall(OpCodes.Callvirt, method, null);
        }
        else {
            ilGenerator.EmitCall(OpCodes.Call, method, null);
        }

        // creates code for handling the return value
        if( method.ReturnType == typeof(void) ) {
            ilGenerator.Emit(OpCodes.Ldnull);
        }
        else {
            EmitBoxIfNeeded(ilGenerator, method.ReturnType);
        }

        // iterates through the parameters updating the parameters passed by ref
        for( int i = 0; i < paramTypes.Length; i++ ) {
            if( parameters[i].ParameterType.IsByRef ) {
                ilGenerator.Emit(OpCodes.Ldarg_1);
                EmitFastInt(ilGenerator, i);
                ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                if( locals[i].LocalType.IsValueType )
                    ilGenerator.Emit(OpCodes.Box, locals[i].LocalType);
                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }
        }

        // returns the value to the caller
        ilGenerator.Emit(OpCodes.Ret);

        // converts the DynamicMethod to a MethodDelegate delegate to call to the method
        MethodDelegate invoker = (MethodDelegate)dynamicMethod.CreateDelegate(typeof(MethodDelegate));

        return invoker;
    }

    /// <summary>Emits code to save an integer to the evaluation stack.</summary>
    /// <param name="ilGenerator">The MSIL generator.</param>
    /// <param name="value">The value to push.</param>
    private static void EmitFastInt(ILGenerator ilGenerator, int value)
    {
        ilGenerator.Emit(OpCodes.Ldc_I4, value);
    }

    /// <summary>Boxes a type if needed.</summary>
    /// <param name="ilGenerator">The MSIL generator.</param>
    /// <param name="type">The type.</param>
    private static void EmitBoxIfNeeded(ILGenerator ilGenerator, System.Type type)
    {
        if( type.IsValueType ) {
            ilGenerator.Emit(OpCodes.Box, type);
        }
    }

    public static GetValueDelegate CreatePropertyGetter(PropertyInfo property)
    {
        if( property == null )
            throw new ArgumentNullException("property");

        if( property.CanRead == false )
            throw new NotSupportedException("属性不支持读操作。");

        if( property.GetIndexParameters().Length > 0 )
            throw new NotSupportedException("不支持构造索引器属性的委托。");

        MethodInfo getMethod = property.GetGetMethod(true);

        DynamicMethod dm = new DynamicMethod("PropertyGetter", typeof(object),
            new Type[] { typeof(object) },
            property.DeclaringType, true);

        ILGenerator il = dm.GetILGenerator();

        if( !getMethod.IsStatic ) {
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, getMethod, null);
        }
        else
            il.EmitCall(OpCodes.Call, getMethod, null);

        if( property.PropertyType.IsValueType )
            il.Emit(OpCodes.Box, property.PropertyType);

        il.Emit(OpCodes.Ret);

        return (GetValueDelegate)dm.CreateDelegate(typeof(GetValueDelegate));
    }

    public static SetValueDelegate CreatePropertySetter(PropertyInfo property)
    {
        if( property == null )
            throw new ArgumentNullException("property");

        if( !property.CanWrite )
            throw new NotSupportedException("属性不支持写操作。");

        if( property.GetIndexParameters().Length > 0 )
            throw new NotSupportedException("不支持构造索引器属性的委托。");

        MethodInfo setMethod = property.GetSetMethod(true);

        DynamicMethod dm = new DynamicMethod("PropertySetter", null,
            new Type[] { typeof(object), typeof(object) },
            property.DeclaringType, true);

        ILGenerator il = dm.GetILGenerator();

        if( !setMethod.IsStatic ) {
            il.Emit(OpCodes.Ldarg_0);
        }
        il.Emit(OpCodes.Ldarg_1);

        EmitCastToReference(il, property.PropertyType);
        if( !setMethod.IsStatic && !property.DeclaringType.IsValueType ) {
            il.EmitCall(OpCodes.Callvirt, setMethod, null);
        }
        else
            il.EmitCall(OpCodes.Call, setMethod, null);

        il.Emit(OpCodes.Ret);

        return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
    }

    public static GetValueDelegate CreateFieldGetter(FieldInfo field)
    {
        if( field == null )
            throw new ArgumentNullException("field");

        DynamicMethod dm = new DynamicMethod("FieldGetter", typeof(object),
            new Type[] { typeof(object) },
            field.DeclaringType, true);

        ILGenerator il = dm.GetILGenerator();

        if( !field.IsStatic ) {
            il.Emit(OpCodes.Ldarg_0);

            EmitCastToReference(il, field.DeclaringType);  //to handle struct object

            il.Emit(OpCodes.Ldfld, field);
        }
        else
            il.Emit(OpCodes.Ldsfld, field);

        if( field.FieldType.IsValueType )
            il.Emit(OpCodes.Box, field.FieldType);

        il.Emit(OpCodes.Ret);

        return (GetValueDelegate)dm.CreateDelegate(typeof(GetValueDelegate));
    }

    public static SetValueDelegate CreateFieldSetter(FieldInfo field)
    {
        if( field == null )
            throw new ArgumentNullException("field");

        DynamicMethod dm = new DynamicMethod("FieldSetter", null,
            new Type[] { typeof(object), typeof(object) },
            field.DeclaringType, true);

        ILGenerator il = dm.GetILGenerator();

        if( !field.IsStatic ) {
            il.Emit(OpCodes.Ldarg_0);
        }
        il.Emit(OpCodes.Ldarg_1);

        EmitCastToReference(il, field.FieldType);

        if( !field.IsStatic )
            il.Emit(OpCodes.Stfld, field);
        else
            il.Emit(OpCodes.Stsfld, field);
        il.Emit(OpCodes.Ret);

        return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
    }

    private static void EmitCastToReference(ILGenerator il, Type type)
    {
        if( type.IsValueType )
            il.Emit(OpCodes.Unbox_Any, type);
        else
            il.Emit(OpCodes.Castclass, type);
    }
}


/// <summary>
/// 优化反射性能的工具类
/// </summary>
public static class EmitExtensions
{
    private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
    private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));
    private static readonly Hashtable s_methodDict = Hashtable.Synchronized(new Hashtable(10240));

    /// <summary>
    /// 用优化的方式快速读取PropertyInfo
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static object FastGetValue2(this PropertyInfo propertyInfo, object obj)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException("propertyInfo");

        GetValueDelegate getter = (GetValueDelegate)s_getterDict[propertyInfo];
        if( getter == null ) {
            getter = DynamicMethodFactory.CreatePropertyGetter(propertyInfo);
            s_getterDict[propertyInfo] = getter;
        }

        return getter(obj);
    }

    /// <summary>
    /// 用优化的方式快速写PropertyInfo
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void FastSetValue2(this PropertyInfo propertyInfo, object obj, object value)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException("propertyInfo");

        SetValueDelegate setter = (SetValueDelegate)s_setterDict[propertyInfo];
        if( setter == null ) {
            setter = DynamicMethodFactory.CreatePropertySetter(propertyInfo);
            s_setterDict[propertyInfo] = setter;
        }

        setter(obj, value);
    }


    /// <summary>
    /// 用优化的方式快速调用一个方法
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="obj"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static object FastInvoke2(this MethodInfo methodInfo, object obj, params object[] parameters)
    {
        if( methodInfo == null )
            throw new ArgumentNullException("methodInfo");

        MethodDelegate invoker = (MethodDelegate)s_methodDict[methodInfo];
        if( invoker == null ) {
            invoker = DynamicMethodFactory.CreateMethod(methodInfo);
            s_methodDict[methodInfo] = invoker;
        }

        return invoker(obj, parameters);
    }

}


