namespace ClownFish.MQ.Messages;

#if NETCOREAPP


/// <summary>
/// 扩展工具类
/// </summary>
public static class BufferExtension
{
    private static readonly ConstructorInfo s_ctor = null;

#if NETCOREAPP
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2026: Assembly.GetType")]
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2075: type.GetConstructor")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(System.Net.Http.ReadOnlyMemoryContent))]  // 确保 ReadOnlyMemoryStream 不被裁剪
#endif
    static BufferExtension()
    {
        // ReadOnlyMemoryStream 这个类型在 .NET BCL 中一直存在，只是没有公开，
        // 它的代码量比较小，把它COPY出来也是可以的，但是会降低当前项目的 “单元测试代码覆盖率”，所以就采用反射的方式来使用

        Type type = typeof(System.Net.Http.HttpClientHandler).Assembly.GetType("System.IO.ReadOnlyMemoryStream", true, false);
        s_ctor = type.GetConstructor(new Type[] { typeof(ReadOnlyMemory<byte>) });
    }

    /// <summary>
    /// 根据 ReadOnlyMemory 实例创建一个只读流，且不提供异步操作
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static Stream AsReadonlyStream(this ReadOnlyMemory<byte> buffer)
    {
        return (Stream)s_ctor.Invoke(new object[] { buffer });
    }
}



#endif
