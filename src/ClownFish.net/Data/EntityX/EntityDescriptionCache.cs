namespace ClownFish.Data;

internal static class EntityDescriptionCache
{
    private static readonly Hashtable s_typeInfoDict = Hashtable.Synchronized(new Hashtable(2048));

    public static EntityDescription Get(Type entityType)
    {
        // 先尝试从缓存中获取
        EntityDescription description = s_typeInfoDict[entityType.FullName] as EntityDescription;
        if( description == null ) {

            // 创建类型的描述对象
            description = Create(entityType);

            // 添加到缓存字典
            s_typeInfoDict[entityType.FullName] = description;
        }

        return description;
    }

    private static bool IsSupportType(Type dataType)
    {
        // 注意：这里的类型要和 TypeList 中的类型列表保持一致！

        // IsPrimitive 不包含 IntPtr, UIntPtr
        //return dataType.IsPrimitive  // Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.
        //    || dataType == TypeList._string
        //    || dataType == TypeList._Guid
        //    || dataType == TypeList._DateTime
        //    || dataType == TypeList._TimeSpan
        //    || dataType == TypeList._decimal
        //    || dataType == TypeList._byteArray
        //    || dataType.IsEnum;

        return dataType.IsEnum || (TypeList.GetTypeName(dataType) != null);
    }

    private static EntityDescription Create(Type entityType)
    {
        // 获取所有类型的属性定义（注意：不处理Field）
        PropertyInfo[] properties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        Dictionary<string, ColumnInfo> dict
            = new Dictionary<string, ColumnInfo>(properties.Length, StringComparer.OrdinalIgnoreCase);

        int index = -1;
        foreach( PropertyInfo prop in properties ) {
            // 为了方便排查问题，属性的序号以出现的次序为准，
            // 如果有属性被忽略了，那么序号也累加（对于变更状态数组来说，就是浪费对应的元素）
            index++;

            if( prop.CanWrite == false )     // 不能写的属性根本不能赋值，只能排除！
                continue;

            if( prop.IsIndexerProperty() ) // 排除索引器属性
                continue;

            // 获取属性的数据库定义信息（可能为 null ）
            DbColumnAttribute attr = prop.GetMyAttribute<DbColumnAttribute>();

            if( attr != null && attr.Ignore )
                continue;

            Type dataType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;


            // 排除不支持的数据类型（有可能是嵌套实体）
            // 注意：如果某个实体属性是“自定义类型”，那么需要 [DbColumn(....)]
            if( attr == null && IsSupportType(dataType) == false )
                continue;

            ColumnInfo info = new ColumnInfo(prop, attr) { DataType = dataType, Index = index };

            dict[info.DbName] = info;
        }

        return new EntityDescription(dict, properties.Length, entityType);
    }

}
