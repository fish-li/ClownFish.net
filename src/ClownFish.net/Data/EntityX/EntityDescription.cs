namespace ClownFish.Data;

internal sealed class EntityDescription
{
    public EntityDescription(Dictionary<string, ColumnInfo> dict, int propertyCount, Type entityType)
    {
        this.MemberDict = dict;
        this.PropertyCount = propertyCount;

        DbEntityAttribute entityAttribute = entityType.GetMyAttribute<DbEntityAttribute>();

        this.Attr = entityAttribute;
        this.TableName = (entityAttribute != null && entityAttribute.Alias.IsNullOrEmpty() == false) ? entityAttribute.Alias : entityType.Name;
        this.EntityType = entityType;
    }

    /// <summary>
    /// 实体的成员字典，仅包含可以处理的属性。
    /// 例如：可读写，基础数据类型，非属性索引器。
    /// </summary>
    public Dictionary<string, ColumnInfo> MemberDict { get; private set; }

    /// <summary>
    /// 实体的属性数量，可能 大于 MemberDict.Count，因为有些属性可能被忽略！
    /// 这个属性目前用于控制【变量状态数组】的长度
    /// </summary>
    public int PropertyCount { get; private set; }

    public DbEntityAttribute Attr { get; private set; }

    /// <summary>
    /// 实体表名
    /// </summary>
    public string TableName { get; private set; }

    public Type EntityType { get; private set; }



    private ColumnInfo _identityColumn;
    private ColumnInfo _primaryKeyColumn;

    public ColumnInfo GetIdentity()
    {
        if( _identityColumn == null ) {
            try {
                _identityColumn = this.MemberDict.Where(x => x.Value.Attr != null && x.Value.Attr.Identity).SingleOrDefault().Value;
            }
            catch( InvalidOperationException ex ) {
                string message = $"实体类型 {this.EntityType.FullName} 定义了多个 [DbColumn(Identity=true)]";
                throw new NotSupportedException(message, ex);
            }
        }

        return _identityColumn;
    }

    public ColumnInfo GetPrimaryKey()
    {
        if( _primaryKeyColumn == null ) {
            try {
                _primaryKeyColumn = this.MemberDict.Where(x => x.Value.Attr != null && x.Value.Attr.PrimaryKey).SingleOrDefault().Value;
            }
            catch( InvalidOperationException ex ) {
                string message = $"实体类型 {this.EntityType.FullName} 定义了多个 [DbColumn(PrimaryKey=true)]";
                throw new NotSupportedException(message, ex);
            }
        }

        return _primaryKeyColumn;
    }


    public IEnumerable<ColumnInfo> GetInsertColumns(bool excludeIdentity = true)
    {
        foreach( var x in this.MemberDict ) {
            DbColumnAttribute columnAttribute = x.Value.Attr;

            // 忽略不能写入的列
            if( columnAttribute != null ) {
                if( columnAttribute.ReadOnly || columnAttribute.Ignore )
                    continue;

                if( excludeIdentity && columnAttribute.Identity )
                    continue;
            }

            yield return x.Value;
        }
    }


    //public ColumnInfo GetPropertyDefine(string propertyName)
    //{
    //    foreach( var x in this.MemberDict ) {
    //        if( x.Value.PropertyInfo.Name == propertyName )
    //            return x.Value;
    //    }
    //    return null;
    //}


}
