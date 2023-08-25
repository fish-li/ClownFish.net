namespace ClownFish.Data;

/// <summary>
/// 数据实体的基类
/// </summary>
[Serializable]
public abstract partial class Entity
{

    /// <summary>
    /// 获取实体对应的数据库表名。
    /// </summary>
    /// <returns></returns>
    internal string GetTableName()
    {
        Type entityType = (this is IEntityProxy) ? this.GetType().BaseType : this.GetType();
        return entityType.GetDbTableName();
    }

    /// <summary>
    /// 获取实体的自增列。
    /// 如果实体没有定义自增列，就返回NULL
    /// </summary>
    /// <returns></returns>
    internal ColumnInfo GetIdentity()
    {
        Type entityType = (this is IEntityProxy) ? this.GetType().BaseType : this.GetType();

        EntityDescription description = EntityDescriptionCache.Get(entityType);

        return description.GetIdentity();
    }


    internal ColumnInfo GetPrimaryKey()
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



