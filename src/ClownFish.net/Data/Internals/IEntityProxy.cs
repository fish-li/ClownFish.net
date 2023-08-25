namespace ClownFish.Data.Internals;

/// <summary>
/// 定义实体代理应该实现的接口。仅供框架内部使用（不考虑升级兼容问题）！
/// </summary>
public interface IEntityProxy
{
    /// <summary>
    /// 设置代理要包装的实体对象
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="entity"></param>
    void Init(DbContext dbContext, Entity entity);

    /// <summary>
    /// 获取与代理对象关联的DbContext实例
    /// </summary>
    DbContext DbContext { get; }

    /// <summary>
    /// 获取与代理对象关联的原实体对象
    /// </summary>
    Entity InnerEntity { get; }


    /// <summary>
    /// 获取哪些字段发生了改变，返回对应的字段名称列表
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<string> GetChangeNames();

    /// <summary>
    /// 获取哪些字段发生了改变，返回对应的字段值列表
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<object> GetChangeValues();

    /// <summary>
    /// 获取实体的主键信息：字段名，字段值
    /// </summary>
    /// <returns></returns>
    FieldNvObject GetRowKey();

    /// <summary>
    /// 清除代理实体中所有属性的修改标记
    /// </summary>
    void ClearChangeFlags();

}


/// <summary>
/// 
/// </summary>
public sealed class FieldNvObject
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    public object Value { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public FieldNvObject(string name, object value)
    {
        this.Name = name;
        this.Value = value;
    }
}