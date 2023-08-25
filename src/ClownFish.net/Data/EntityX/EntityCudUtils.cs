namespace ClownFish.Data;

internal static class EntityCudUtils
{
    private static readonly Hashtable s_insertSqlDict = Hashtable.Synchronized(new Hashtable(2048));

    internal static void CheckArgs<T>(T entity, DbContext dbContext)
    {
        if( entity == null )
            throw new ArgumentNullException(nameof(entity));
        if( entity is IEntityProxy )
            throw new NotSupportedException("当前方法仅支持在实体对象上调用，不支持实体代理类型调用。");

        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));
    }


    internal static string GetInsertSQL<T>(T entity, DbContext dbContext) where T : Entity, new()
    {
        CheckArgs(entity, dbContext);

        // 不同数据库类型的SQL可能不一样，所以KEY要包含连接类型
        string key = typeof(T).FullName + "##" + dbContext.Connection.GetType().FullName;

        string insertSQL = s_insertSqlDict[key] as string;
        if( insertSQL == null ) {

            StringBuilder fieldBuilder = StringBuilderPool.Get();
            StringBuilder valueBuilder = StringBuilderPool.Get();

            try {
                EntityDescription description = EntityDescriptionCache.Get(typeof(T));

                foreach( var x in description.GetInsertColumns() ) {

                    if( fieldBuilder.Length > 0 ) {
                        fieldBuilder.Append(", ");
                        valueBuilder.Append(", ");
                    }

                    fieldBuilder.Append(dbContext.GetObjectFullName(x.DbName));
                    valueBuilder.Append(dbContext.ClientProvider.GetParamterPlaceholder(x.PropertyInfo.Name, dbContext));
                }

                string tableName = dbContext.GetObjectFullName(description.TableName);
                insertSQL = $"insert into {tableName} ( {fieldBuilder} ) values ( {valueBuilder} )";
                s_insertSqlDict[key] = insertSQL;
            }
            finally {
                StringBuilderPool.Return(fieldBuilder);
                StringBuilderPool.Return(valueBuilder);
            }
        }
        return insertSQL;
    }

    internal static long ExecuteInsert(CPQuery query, bool getNewId, object entity)
    {
        if( getNewId == false )
            return query.ExecuteNonQuery();

        query = query.Context.ClientProvider.GetNewIdQuery(query, entity);
        return query.ExecuteScalar<long>();


        //if( query.Context.DatabaseType == DatabaseType.SQLSERVER ) {
        //    query = query + "; select SCOPE_IDENTITY();";
        //    return query.ExecuteScalar<long>();
        //}
        //else if( query.Context.DatabaseType == DatabaseType.MySQL ) {
        //    query = query + "; select LAST_INSERT_ID();";
        //    return query.ExecuteScalar<long>();
        //}
        //else if( query.Context.DatabaseType == DatabaseType.PostgreSQL ) {
        //    query = query + "; select lastval();";
        //    return query.ExecuteScalar<long>();
        //}
        //else {
        //    throw new NotSupportedException("不支持从此类型数据库中获取自增ID");
        //}
    }


    internal static async Task<long> ExecuteInsertAsync(CPQuery query, bool getNewId, object entity)
    {
        if( getNewId == false )
            return await query.ExecuteNonQueryAsync();

        query = query.Context.ClientProvider.GetNewIdQuery(query, entity);
        return await query.ExecuteScalarAsync<long>();


        //if( query.Context.DatabaseType == DatabaseType.SQLSERVER ) {
        //    query = query + "; select SCOPE_IDENTITY();";
        //    return await query.ExecuteScalarAsync<long>();
        //}
        //else if( query.Context.DatabaseType == DatabaseType.MySQL ) {
        //    query = query + "; select LAST_INSERT_ID();";
        //    return await query.ExecuteScalarAsync<long>();
        //}
        //else if( query.Context.DatabaseType == DatabaseType.PostgreSQL ) {
        //    query = query + "; select lastval();";
        //    return await query.ExecuteScalarAsync<long>();
        //}
        //else {
        //    throw new NotSupportedException("不支持从此类型数据库中获取自增ID");
        //}
    }


    internal static CPQuery GetInsertQuery<T>(T entity, DbContext dbContext) where T : Entity, new()
    {
        T proxy = CreateInsertProxy(entity, dbContext);
        return proxy.GetInsertQueryCommand();
    }

    internal static T CreateInsertProxy<T>(T entity, DbContext dbContext) where T : Entity, new()
    {
        CheckArgs(entity, dbContext);

        // 创建一个代理实体
        T proxy = dbContext.Entity.CreateProxy<T>();

        EntityDescription description = EntityDescriptionCache.Get(typeof(T));
        foreach( var x in description.GetInsertColumns() ) {

            // 从当前实体读取各个属性，然后赋值给代理实体，引发属性修改，得到哪些字段被指定，然后生成INSERT语句
            // 这其实是一种很低效的做法，优点就是使用简单！
            object value = x.PropertyInfo.FastGetValue(entity);

            if( value != null )
                x.PropertyInfo.FastSetValue(proxy, value);
        }

        return proxy;
    }


    internal static CPQuery GetUpdateQuery<T>(T entity, DbContext dbContext) where T : Entity, new()
    {
        T proxy = CreateUpdateProxy(entity, dbContext);
        return proxy.GetUpdateQueryCommand();
    }

    internal static T CreateUpdateProxy<T>(T entity, DbContext dbContext) where T : Entity, new()
    {
        CheckArgs(entity, dbContext);


        T proxy = (T)dbContext.Entity.CreateProxy<T>();

        EntityDescription description = EntityDescriptionCache.Get(typeof(T));

        foreach( var x in description.GetInsertColumns(false) ) {
            object value = x.PropertyInfo.FastGetValue(entity);

            if( value != null )
                x.PropertyInfo.FastSetValue(proxy, value);
        }

        return proxy;
    }


    internal static T CreateDeleteProxy<T>(DbContext dbContext, object key) where T : Entity, new()
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));
        if( key == null )
            throw new ArgumentNullException(nameof(key));


        T proxy = (T)dbContext.Entity.CreateProxy<T>();

        SetKeyValue(proxy, key);

        return proxy;
    }


    internal static void SetKeyValue(Entity entity, object key)
    {
        ColumnInfo column = entity.GetPrimaryKey();

        if( column == null )
            throw new InvalidOperationException($"实体类型 {entity.GetType().Name} 没有标记主键信息。");

        column.PropertyInfo.FastSetValue(entity, key);
    }



}
