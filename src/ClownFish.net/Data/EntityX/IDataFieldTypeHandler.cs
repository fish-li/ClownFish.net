namespace ClownFish.Data;

/// <summary>
/// 定义数据库字段类型与实体字段类型映射转换的处理接口
/// </summary>
public interface IDataFieldTypeHandler
{
    /// <summary>
    /// 从DbDataReader中加载一个字段
    /// </summary>
    /// <param name="reader">DbDataReader实例</param>
    /// <param name="index">字段序号</param>
    /// <param name="entityType">当前实体类型</param>
    /// <param name="propertyName">当前实体的属性名称</param>
    /// <returns></returns>
    object GetValue(DbDataReader reader, int index, Type entityType, string propertyName);

    /// <summary>
    /// 从DataRow中加载一个字段
    /// </summary>
    /// <param name="row">DataRow实例</param>
    /// <param name="index">字段序号</param>
    /// <param name="entityType">当前实体类型</param>
    /// <param name="propertyName">当前实体的属性名称</param>
    /// <returns></returns>
    object GetValue(DataRow row, int index, Type entityType, string propertyName);

    /// <summary>
    /// 为DbParameter指定参数值
    /// </summary>
    /// <param name="parameter">DbParameter实例，在这个方法中需要给它的Value属性赋值</param>
    /// <param name="value">调用代码传入的数据</param>
    void SetValue(DbParameter parameter, object value);
}


internal sealed class DefaultDataFieldTypeHandler : IDataFieldTypeHandler
{
    public static readonly DefaultDataFieldTypeHandler Instance = new DefaultDataFieldTypeHandler();

    public object GetValue(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        return reader.GetValue(index);
    }

    public object GetValue(DataRow row, int index, Type entityType, string propertyName)
    {
        return row[index];
    }

    public void SetValue(DbParameter parameter, object value)
    {
        parameter.Value = value;
    }
}


/// <summary>
/// IDataFieldTypeHandler注册工厂类
/// </summary>
public static class DataFieldTypeHandlerFactory
{
    private static readonly TSafeDictionary<Type, IDataFieldTypeHandler> s_dict = new TSafeDictionary<Type, IDataFieldTypeHandler>();
    private static bool s_isEmpty = true;

    /// <summary>
    /// 注册IDataFieldTypeHandler
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="handler"></param>
    public static void Add(Type dataType, IDataFieldTypeHandler handler)
    {
        if( dataType == null )
            throw new ArgumentNullException(nameof(dataType));
        if( handler == null )
            throw new ArgumentNullException(nameof(handler));

        s_dict[dataType] = handler;
        s_isEmpty = false;
    }


    /// <summary>
    /// 返回参数匹配的已注册IDataFieldTypeHandler实例
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static IDataFieldTypeHandler Get(Type dataType)
    {
        // 其实绝大多数情况下，并不会有定制的 IDataFieldTypeHandler，所以没有必要查询字典表
        if( s_isEmpty )
            return DefaultDataFieldTypeHandler.Instance;

        IDataFieldTypeHandler handler = s_dict.TryGet(dataType);
        return handler ?? DefaultDataFieldTypeHandler.Instance;
    }
}
