namespace ClownFish.Data;

/// <summary>
/// 数据实体的基类
/// </summary>
[Serializable]
public abstract partial class Entity
{
    private string _targetTableName;


    /// <summary>
    /// 设置当前实体在执行数据库操作时，要使用的数据表名称。
    /// 通常场景下不需要调用当前方法，【仅当】一个实体类型对应多个数据表时才需要调用。
    /// 注意：此方法仅允许在实体代理上调用，否则会出现异常！
    /// </summary>
    /// <param name="tablename"></param>
    public void SetDbTableName(string tablename)
    {
        if( (this is IEntityProxy) == false )
            throw new InvalidOperationException("只允许在实体代理上调用当前方法！");

        _targetTableName = tablename;
    }

    /// <summary>
    /// 获取实体对应的数据库表名。
    /// </summary>
    /// <returns></returns>
    public string GetTableName()
    {
        if( _targetTableName.HasValue() )
            return _targetTableName;

        Type entityType = (this is IEntityProxy) ? this.GetType().BaseType : this.GetType();
        return entityType.GetDbTableName();
    }

    /// <summary>
    /// 获取实体的自增列。
    /// 如果实体没有定义自增列，就返回NULL
    /// </summary>
    /// <returns></returns>
    public ColumnInfo GetIdentity()
    {
        Type entityType = (this is IEntityProxy) ? this.GetType().BaseType : this.GetType();

        EntityDescription description = EntityDescriptionCache.Get(entityType);

        return description.GetIdentity();
    }


    /// <summary>
    /// 获取实体的主键列
    /// </summary>
    /// <returns></returns>
    public ColumnInfo GetPrimaryKey()
    {
        Type entityType = (this is IEntityProxy) ? this.GetType().BaseType : this.GetType();

        EntityDescription description = EntityDescriptionCache.Get(entityType);

        return description.GetPrimaryKey();
    }

    /// <summary>
    /// 加载实体类型中各属性使用[DbColumn(DefaultValue=xxx)]定义的默认值
    /// </summary>
    public void LoadDefaultValues()
    {
        Type entityType = (this is IEntityProxy) ? this.GetType().BaseType : this.GetType();

        EntityDescription description = EntityDescriptionCache.Get(entityType);

        foreach( var x in description.MemberDict ) {

            object defaultValue = x.Value.Attr?.DefaultValue;

            if( defaultValue != null ) {

                if( defaultValue.GetType() == x.Value.PropertyInfo.PropertyType )
                    x.Value.PropertyInfo.FastSetValue(this, defaultValue);
                else {
                    object value2 = Convert.ChangeType(defaultValue, x.Value.PropertyInfo.PropertyType);
                    x.Value.PropertyInfo.FastSetValue(this, value2);
                }
            }
        }
    }




}



