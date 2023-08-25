namespace ClownFish.Log.Writers;

internal static class WriterFactory
{
    /// <summary>
    /// 日志【数据类型】和【写入器】的映射表
    /// </summary>
    private static List<DataTypeWriterMap> s_typeWriterMappings;

    internal static void Init(List<DataTypeWriterMap> list)
    {
        s_typeWriterMappings = list;
    }

    /// <summary>
    /// 判断指定的数据类型是否已配置到支持列表
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static bool IsSupport(Type dataType)
    {
        return s_typeWriterMappings.Exists(x => x.DataType == dataType);
    }


    /// <summary>
    /// 获取指定类型的日志序列化实例（从缓存中获取）
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static ILogWriter[] GetWriters(Type dataType)
    {
        if( LogConfig.Instance.Enable == false )
            return null;

        return s_typeWriterMappings.FirstOrDefault(x => x.DataType == dataType)?.Instances;
    }






}
