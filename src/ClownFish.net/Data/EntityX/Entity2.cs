namespace ClownFish.Data;

// 主要定义一些实体代理的相关方法

public partial class Entity
{

    /// <summary>
    /// 根据当前实体创建代理对象，然后可执行数据库更新操作，
    /// 代理对象将监视属性的赋值过程，当给代理对象的属性赋值后，对应的字段会标记为更新状态。
    /// </summary>
    /// <param name="context">DbContext实例</param>
    /// <returns>与实体相关的代理对象</returns>
    internal IEntityProxy CreateProxy(DbContext context)
    {
        if( context == null )
            throw new ArgumentNullException(nameof(context));

        if( this is IEntityProxy )
            throw new InvalidOperationException("当前对象已经是实体代理！");


        Type entityType = this.GetType();
        Type proxyType = EntityProxyFactory.GetProxy(entityType);

        if( proxyType == null )
            throw new NotImplementedException($"实体类型 {entityType.FullName} 并没有注册匹配的代理类型。");


        IEntityProxy proxy = proxyType.FastNew() as IEntityProxy;
        proxy.Init(context, this);

        return proxy;
    }



    #region 基于代理实体的 INSERT

    internal CPQuery GetInsertQuery0()
    {
        IEntityProxy proxy = this as IEntityProxy;

        IReadOnlyList<string> names = proxy.GetChangeNames();
        IReadOnlyList<object> values = proxy.GetChangeValues();
        if( names.Count == 0 )
            return null;        // 没有设置任何属性，应该不会发生吧？


        CPQuery query = proxy.DbContext.CreateCPQuery();
        //query = query
        //        + "INSERT INTO " + GetTableName() + "("
        //        + string.Join(",", names.ToArray())
        //        + ") VALUES (";

        query.AppendSql("INSERT INTO ")
            .AppendSql(proxy.DbContext.GetObjectFullName(GetTableName()))
            .AppendSql("(");

        for( int i = 0; i < names.Count; i++ ) {
            query.AppendSql(proxy.DbContext.GetObjectFullName(names[i]));

            if( i < names.Count - 1 )
                query.AppendSql(",");
        }
        query.AppendSql(") \r\nVALUES (");




        for( int i = 0; i < values.Count; i++ ) {
            object value = values[i];
            if( value == null )
                query = query + "NULL";
            else
                query = query + new QueryParameter(value);

            if( i < values.Count - 1 )
                query.AppendSql(",");
        }

        query.AppendSql(")");

        return query;
    }

    internal CPQuery GetInsertQueryCommand()
    {
        IEntityProxy proxy = this as IEntityProxy;
        if( proxy == null )
            throw new InvalidOperationException("只能在实体代理对象上调用Insert方法。");


        CPQuery insert = GetInsertQuery0();
        if( insert == null )
            return null;

        proxy.ClearChangeFlags();  // 清除修改标记，防止多次调用

        return insert;
    }

    /// <summary>
    /// 根据当前【代理实体】所有已赋值的属性，生成INSERT语句，并执行数据库插入操作，
    /// 注意：此方法只能在实体的代理对象上调用。
    /// </summary>
    /// <returns>数据库操作过程中影响的行数</returns>
    public int Insert()
    {
        CPQuery query = GetInsertQueryCommand();
        if( query == null )
            return -1;

        return query.ExecuteNonQuery();
    }


    /// <summary>
    /// 根据当前【代理实体】所有已赋值的属性，生成INSERT语句，并执行数据库插入操作，
    /// 注意：此方法只能在实体的代理对象上调用。
    /// </summary>
    /// <returns>数据库操作过程中影响的行数</returns>
    public async Task<int> InsertAsync()
    {
        CPQuery query = GetInsertQueryCommand();
        if( query == null )
            return -1;

        return await query.ExecuteNonQueryAsync();
    }



    #endregion


    #region 基于代理实体的 DELETE

    internal CPQuery GetWhereQuery0()
    {
        IEntityProxy proxy = this as IEntityProxy;

        IReadOnlyList<string> names = proxy.GetChangeNames();
        IReadOnlyList<object> values = proxy.GetChangeValues();
        if( names.Count == 0 )
            return null;        // 没有设置任何属性，应该不会发生吧？

        CPQuery query = proxy.DbContext.CreateCPQuery() + " \r\nWHERE ";

        for( int i = 0; i < values.Count; i++ ) {
            string name = names[i];
            object value = values[i];

            if( i > 0 )
                query = query + " AND ";

            if( value == null )
                query = query + " " + proxy.DbContext.GetObjectFullName(name) + "=NULL";
            else
                query = query + " " + proxy.DbContext.GetObjectFullName(name) + "=" + new QueryParameter(value);
        }

        return query;
    }


    internal CPQuery GetDeleteQueryCommand()
    {
        IEntityProxy proxy = this as IEntityProxy;
        if( proxy == null )
            throw new InvalidOperationException("只能在实体代理对象上调用Delete方法。");


        CPQuery where = GetWhereQuery0();
        if( where == null )
            return null;      // 不允许没有WHERE条件的删除，如果确实需要，请手工写SQL

        CPQuery query = proxy.DbContext.CreateCPQuery()
                        + "DELETE FROM " + proxy.DbContext.GetObjectFullName(GetTableName()) + where;

        proxy.ClearChangeFlags();  // 清除修改标记，防止多次调用

        return query;
    }

    /// <summary>
    /// 根据当前【代理实体】所有已赋值的属性，生成DELETE查询条件，并执行数据库插入操作，
    /// 注意：此方法只能在实体的代理对象上调用。
    /// </summary>
    /// <returns>数据库操作过程中影响的行数</returns>
    public int Delete()
    {
        CPQuery query = GetDeleteQueryCommand();
        if( query == null )
            return -1;

        return query.ExecuteNonQuery();
    }

    /// <summary>
    /// 根据当前【代理实体】所有已赋值的属性，生成DELETE查询条件，并执行数据库插入操作，
    /// 注意：此方法只能在实体的代理对象上调用。
    /// </summary>
    /// <returns>数据库操作过程中影响的行数</returns>
    public async Task<int> DeleteAsync()
    {
        CPQuery query = GetDeleteQueryCommand();
        if( query == null )
            return -1;

        return await query.ExecuteNonQueryAsync();
    }


    #endregion



    #region 基于代理实体的 UPDATE


    internal CPQuery GetUpdateQuery0(FieldNvObject rowKey)
    {
        IEntityProxy proxy = this as IEntityProxy;

        IReadOnlyList<string> names = proxy.GetChangeNames();
        IReadOnlyList<object> values = proxy.GetChangeValues();
        if( names.Count == 0 )
            return null;        // 没有设置任何属性，应该不会发生吧？

        int keyIndex = -1;      // 标记主键字段在数组的哪个位置
        if( rowKey != null )
            keyIndex = ((List<string>)names).FindIndex(rowKey.Name);

        if( names.Count == 1 && keyIndex == 0 )
            return null;        // 如果仅仅只设置了主键字段，这样的更新是无意义的

        int forcount = values.Count;

        if( keyIndex == forcount - 1 )      // 主键出现在最后面
            forcount--;

        CPQuery query = proxy.DbContext.CreateCPQuery()
                        + "UPDATE " + proxy.DbContext.GetObjectFullName(GetTableName()) + " SET ";

        for( int i = 0; i < forcount; i++ ) {
            if( i == keyIndex )     // 忽略主键字段
                continue;

            string name = names[i];
            object value = values[i];

            if( value == null )
                query = query + " " + proxy.DbContext.GetObjectFullName(name) + "=NULL";
            else
                query = query + " " + proxy.DbContext.GetObjectFullName(name) + "=" + new QueryParameter(value);

            if( i < forcount - 1 )
                query.AppendSql(",");       // 注意，这个逗号的拼接，有可能主键出现在所有字段的最后。
        }

        return query;

    }


    internal CPQuery GetUpdateQueryCommand()
    {
        IEntityProxy proxy = this as IEntityProxy;
        if( proxy == null )
            throw new InvalidOperationException("只能在实体代理对象上调用Update方法。");

        // 获取数据实体对象的主键值，如果数据实体没有指定主键，将会抛出一个异常
        FieldNvObject rowKey = proxy.GetRowKey();

        CPQuery update = GetUpdateQuery0(rowKey);
        if( update == null )
            return null;

        CPQuery query = update + " \r\nWHERE " + proxy.DbContext.GetObjectFullName(rowKey.Name) + " = " + new QueryParameter(rowKey.Value);

        proxy.ClearChangeFlags();  // 清除修改标记，防止多次调用

        return query;
    }



    /// <summary>
    /// 根据当前【代理实体】所有已赋值的属性，生成UPDATE操作语句（WHERE条件由主键生成[DbColumn(PrimaryKey=true)]），并执行数据库插入操作，
    /// 注意：此方法只能在实体的代理对象上调用。
    /// </summary>
    /// <returns>数据库操作过程中影响的行数</returns>
    public int Update()
    {
        CPQuery query = GetUpdateQueryCommand();
        if( query == null )
            return -1;

        return query.ExecuteNonQuery();
    }


    /// <summary>
    /// 根据当前【代理实体】所有已赋值的属性，生成UPDATE操作语句（WHERE条件由主键生成[DbColumn(PrimaryKey=true)]），并执行数据库插入操作，
    /// 注意：此方法只能在实体的代理对象上调用。
    /// </summary>
    /// <returns>数据库操作过程中影响的行数</returns>
    public async Task<int> UpdateAsync()
    {
        CPQuery query = GetUpdateQueryCommand();
        if( query == null )
            return -1;

        return await query.ExecuteNonQueryAsync();
    }



    #endregion



}
