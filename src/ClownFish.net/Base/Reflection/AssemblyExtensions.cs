namespace ClownFish.Base;

/// <summary>
/// 扩展方法工具类
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// 等同于调用 Assembly实例的GetCustomAttributes()，只是在缺少依赖程序集时能指出当前程序集的名称。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static T[] GetAttributes<T>(this Assembly assembly) where T : Attribute
    {
        // 获取程序集的Attribute使用场景较少，就不使用缓存版本了

        if( assembly == null )
            throw new ArgumentNullException("assembly");

        try {
            TestHelper.TryThrowException();
            return (T[])assembly.GetCustomAttributes(typeof(T), true);
        }
        catch( FileNotFoundException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( FileLoadException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( TypeLoadException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
    }


    /// <summary>
    /// 等同于调用 Assembly实例的GetExportedTypes()，只是在缺少依赖程序集时能指出当前程序集的名称。
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static Type[] GetPublicTypes(this Assembly assembly)
    {
        if( assembly == null )
            throw new ArgumentNullException("assembly");

        try {
            TestHelper.TryThrowException();
            return assembly.GetExportedTypes();
        }
        catch( FileNotFoundException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( FileLoadException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( TypeLoadException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( ReflectionTypeLoadException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( NotSupportedException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
    }



    /// <summary>
    /// 等同于调用 Assembly实例的GetTypes()，只是在缺少依赖程序集时能指出当前程序集的名称。
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static Type[] GetAllTypes(this Assembly assembly)
    {
        if( assembly == null )
            throw new ArgumentNullException("assembly");

        try {
            TestHelper.TryThrowException();
            return assembly.GetTypes();
        }
        catch( FileNotFoundException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( FileLoadException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( TypeLoadException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
        catch( ReflectionTypeLoadException ex ) {
            throw new InvalidOperationException(
                        "反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
        }
    }



    /// <summary>
    /// 读取一个程序集内的文本资源文件
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string ReadResAsText(this Assembly assembly, string filename)
    {
        if( assembly == null )
            throw new ArgumentNullException(nameof(assembly));
        if( filename.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filename));

        using( Stream stream = assembly.GetManifestResourceStream(filename) ) {
            using( StreamReader reader = new StreamReader(stream) ) {
                return reader.ReadToEnd();
            }
        }
    }


    /// <summary>
    /// 读取一个程序集内的二进制资源文件
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static byte[] ReadResAsBytes(this Assembly assembly, string filename)
    {
        if( assembly == null )
            throw new ArgumentNullException(nameof(assembly));
        if( filename.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filename));

        using( Stream stream = assembly.GetManifestResourceStream(filename) ) {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }

}
