namespace ClownFish.Data;

/// <summary>
/// 安全简单的拼接SQL的工具类
/// </summary>
public sealed class CPQuery : BaseCommand
{
    #region 数据成员定义

    // 因为允许在CPQuery嵌套使用，所以为了避免产生了同名的动态参数名称，
    // 所以用静态变量来累加序号，防止序号重复，产生相同的参数名称。
    private static int s_index;


    // 构造CPQuery时指定的初始SQL语句
    private string _initSql = null;

    // 用于动态拼接SQL必需要的2个变量，如果没有动态拼接SQL的过程，下面2个变量不会被赋值
    private StringBuilder _sqlBuilder = null;
    private bool _sqlChanged = false;

    /// <summary>
    /// 获取当前CPQuery内部的DbCommand对象，
    /// 当前重写属性会更新CommandText属性
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2100")]
    public override DbCommand Command {
        get {
            if( _sqlChanged ) {
                _command.CommandText = _sqlBuilder.ToString();
                _sqlChanged = false;
            }

            if( _initSql != null && _command.CommandText.IsNullOrEmpty() )
                _command.CommandText = _initSql;

            return _command;
        }
    }

 

    #endregion

    #region 构造函数

    internal CPQuery(DbContext context) : base(context)
    {
    }

    internal void Init(string parameterizedSQL)
    {
        _initSql = parameterizedSQL ?? string.Empty;
    }

    internal void Init(string parameterizedSQL, object argsObject)
    {
        _initSql = parameterizedSQL ?? string.Empty;


        if( argsObject == null )
            return;

        PropertyInfo[] properties = argsObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach( PropertyInfo property in properties ) {
            object value = property.FastGetValue(argsObject);
            SetParameter(property.Name, value);
        }
    }


    internal void Init(string parameterizedSQL, Hashtable dictionary)
    {
        _initSql = parameterizedSQL ?? string.Empty;


        if( dictionary == null || dictionary.Count == 0 )
            return;


        foreach( DictionaryEntry entry in dictionary ) {
            SetParameter(entry.Key.ToString(), entry.Value);
        }
    }


    internal void Init(string parameterizedSQL, IDictionary<string, object> dictionary)
    {
        _initSql = parameterizedSQL ?? string.Empty;


        if( dictionary == null || dictionary.Count == 0 )
            return;

        foreach( KeyValuePair<string, object> kvp in dictionary ) {
            SetParameter(kvp.Key, kvp.Value);
        }
    }

    internal void Init(string parameterizedSQL, DbParameter[] parameters)
    {
        _initSql = parameterizedSQL ?? string.Empty;

        if( parameters == null || parameters.Length == 0 )
            return;

        foreach( var p in parameters )
            _command.Parameters.Add(p);
    }


    /// <summary>
    /// 根据指定的参数化SQL语句，创建CPQuery对象实例
    /// </summary>
    /// <param name="parameterizedSQL">参数化SQL语句</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery Create(string parameterizedSQL = null)
    {
        //if( string.IsNullOrEmpty(parameterizedSQL) )
        //	throw new ArgumentNullException("parameterizedSQL");

        CPQuery query = new CPQuery(ConnectionScope.GetCurrentDbConext());
        query.Init(parameterizedSQL);
        return query;
    }

    /// <summary>
    /// 根据指定的参数化SQL语句，匿名对象参数，创建CPQuery对象实例
    /// </summary>
    /// <param name="parameterizedSQL">参数化SQL语句</param>
    /// <param name="argsObject">匿名对象，每个属性对应一个命令参数</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery Create(string parameterizedSQL, object argsObject = null)
    {
        if( string.IsNullOrEmpty(parameterizedSQL) )
            throw new ArgumentNullException(nameof(parameterizedSQL));

        CPQuery query = new CPQuery(ConnectionScope.GetCurrentDbConext());
        query.Init(parameterizedSQL, argsObject);
        return query;
    }

    /// <summary>
    /// 通过参数化SQL、哈希表的方式,创建CPQuery对象实例
    /// </summary>
    /// <param name="parameterizedSQL">参数化的SQL字符串</param>
    /// <param name="dictionary">哈希表</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery Create(string parameterizedSQL, Hashtable dictionary)
    {
        if( string.IsNullOrEmpty(parameterizedSQL) )
            throw new ArgumentNullException(nameof(parameterizedSQL));

        CPQuery query = new CPQuery(ConnectionScope.GetCurrentDbConext());
        query.Init(parameterizedSQL, dictionary);
        return query;
    }

    /// <summary>
    /// 通过参数化SQL、字典表的方式,创建CPQuery对象实例
    /// </summary>
    /// <param name="parameterizedSQL">参数化的SQL字符串</param>
    /// <param name="dictionary">哈希表</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery Create(string parameterizedSQL, IDictionary<string, object> dictionary)
    {
        if( string.IsNullOrEmpty(parameterizedSQL) )
            throw new ArgumentNullException(nameof(parameterizedSQL));

        CPQuery query = new CPQuery(ConnectionScope.GetCurrentDbConext());
        query.Init(parameterizedSQL, dictionary);
        return query;
    }

    /// <summary>
    /// 通过参数化SQL、SqlParameter数组的方式，创建CPQuery实例
    /// </summary>
    /// <param name="parameterizedSQL">参数化的SQL字符串</param>
    /// <param name="parameters">SqlParameter参数数组</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery Create(string parameterizedSQL, params DbParameter[] parameters)
    {
        if( string.IsNullOrEmpty(parameterizedSQL) )
            throw new ArgumentNullException(nameof(parameterizedSQL));

        CPQuery query = new CPQuery(ConnectionScope.GetCurrentDbConext());
        query.Init(parameterizedSQL, parameters);
        return query;
    }

    #endregion

    #region 基础操作


    internal CPQuery AppendSql(string s)
    {
        if( string.IsNullOrEmpty(s) )
            return this;

        if( _sqlBuilder == null ) {
            InitSqlBuilder();
        }
        _sqlBuilder.Append(s);
        _sqlChanged = true;

        return this;
    }

    private void InitSqlBuilder()
    {
        if( _sqlBuilder == null ) {
            _sqlBuilder = new StringBuilder(1024);
            _sqlBuilder.Append(_initSql);
            _initSql = null;
        }
    }

    private static uint GetNextParamIndex()
    {
        int current = Interlocked.Increment(ref s_index);

        // 如果得到的序号小于 0 ，就接着 int.MaxValue 继续排号
        // int.MaxValue =  2147483647
        // int.MinValue = -2147483648

        // if current >= 0 && current <= int.MaxValue
        //    => return current ( 0 --> int.MaxValue )

        // Interlocked.Increment(int.MaxValue) => int.MinValue

        // if current == int.MinValue
        //    => uint.MaxValue -2147483648 + 1 = 2147483648 (int.MaxValue + 1)

        // if current == -1
        //    => uint.MaxValue -1 + 1 = 4294967295 (uint.MaxValue)

        uint index = current >= 0 ? (uint)current : (uint)(uint.MaxValue + current + 1);
        return index;
    }

    internal void AddQueryParameter(QueryParameter p)
    {
        // 诸如: int[], long[], guid[], string[] 之类的参数
        if( IsArrayValue(p.Value?.GetType()) ) {
            AppendArrayParameter((ICollection)p.Value);
            return;
        }

        // 普通的 【标量】参数
        string name = "p" + GetNextParamIndex().ToString();

        // SQL语句中拼入参数占位符
        InitSqlBuilder();
        _sqlBuilder.Append(this.Context.ClientProvider.GetParamterPlaceholder(name, this.Context));
        _sqlChanged = true;

        // 参数集合中添加命令参数
        AddParameter(name, p.Value);
    }


    internal static bool IsArrayValue(Type type)
    {
        if( type == null )
            return false;

        // 注意：这里不能简单地用 ICollection 来判断，否则 byte[] 会被误判。

        return typeof(IEnumerable<int>).IsAssignableFrom(type)
                || typeof(IEnumerable<long>).IsAssignableFrom(type)
                || typeof(IEnumerable<string>).IsAssignableFrom(type)
            || typeof(IEnumerable<Guid>).IsAssignableFrom(type);
    }

    private void SetParameter(string name, object value)
    {
        if( value == null || value == DBNull.Value )
            this.AddParameter(name, DBNull.Value);

        else if( value is DbParameter dbParameter )
            SetDbParameter(name, dbParameter);

        else if( value is CPQuery query )
            SetQueryParameter(name, query);

        else if( value is SqlFragment fragment )
            SetFragmentParameter(name, fragment);

        else if( IsArrayValue(value.GetType()) )
            SetInArrayParameters(name, (ICollection)value);

        else
            AddParameter(name, value);
    }

    private void AddParameter(string name, object value)
    {
        DbParameter parameter = _command.CreateParameter();
        parameter.ParameterName = this.Context.ClientProvider.GetParamterName(name, this.Context);
        SetParameterValue(parameter, value);
        _command.Parameters.Add(parameter);
    }

    internal static void SetParameterValue(DbParameter parameter, object value)
    {
        if( value == null || DBNull.Value.Equals(value) ) {
            parameter.Value = DBNull.Value;
            return;
        }

        if( value.GetType().IsEnum ) {      // 枚举值强制转成整数来存储
            parameter.Value = (int)value;
            return;
        }

        DataFieldTypeHandlerFactory.Get(value.GetType()).SetValue(parameter, value);
    }

    //internal static object GetParameterValue(object value)   // TODO: 这个方法要重构，允许用户扩展
    //{
    //    if( value == null || DBNull.Value.Equals(value) )
    //        return DBNull.Value;

    //    if( value.GetType().IsEnum )      // 枚举值强制转成整数来存储
    //        return (int)value;

    //    //if( value is TimeSpan time ) {
    //    //    // TimeSpan映射到数据库存储时的兼容性不太好。虽然大部分数据库支持 time 类型，但有些数据库根本就不支持，
    //    //    // 例如：SQLSERVER2005就不支持，达梦虽然有time类型，但是映射到.NET是DateTime类型

    //    //    // 为了解决兼容性问题，可以选择将 TimeSpan 转换成 long 或者 string，
    //    //    // 前者的精度较好（可以保留毫秒），但是跨语言的兼容性较很差，后者对跨语言的支持比较好，但是格式定义又会是个新问题（要不要保留毫秒？）

    //    //    // ClownFish强制要求行为，TimeSpan类型按long来存储
    //    //    return time.Ticks;

    //    //    // 2023-02-15 最终决定：不再对这个类型做强制转换，由调用者来选择！
    //    //}

    //    return value;
    //}


    private void SetFragmentParameter(string name, SqlFragment fragment)
    {
        string placeholder = "{" + name + "}";

        // 替换占位符
        InitSqlBuilder();
        _sqlBuilder.Replace(placeholder, fragment.Value);
        _sqlChanged = true;
    }

    private void SetDbParameter(string name, DbParameter parameter)
    {
        if( string.IsNullOrEmpty(parameter.ParameterName) )
            parameter.ParameterName = this.Context.ClientProvider.GetParamterName(name, this.Context);
        // else 不检查参数的名称是否匹配，由调用者保证。

        _command.Parameters.Add(parameter);
    }

    private void SetQueryParameter(string name, CPQuery query)
    {
        string placeholder = "{" + name + "}";

        // 替换占位符
        InitSqlBuilder();
        _sqlBuilder.Replace(placeholder, query.ToString());
        _sqlChanged = true;

        // 添加命令参数
        query.MoveParameters(_command);
    }

    /// <summary>
    /// 设置 IN 参数，参数名称格式要求：{parameterName}
    /// 例如：select * from t1 where rid in ( {parameterName} )
    /// </summary>
    /// <param name="name"></param>
    /// <param name="collection"></param>
    private void SetInArrayParameters(string name, ICollection collection)
    {
        //if( collection == null || collection.Count == 0 )
        //	throw new ArgumentNullException("collection");

        InitSqlBuilder();

        StringBuilder sb = StringBuilderPool.Get();
        try {

            // 优先检查 int[], Guid[] 类型，并转成SQL语句中的一部分
            // 因为这些强类型的数据本身是安全的，不存在注入，就不转换成命令参数。
            ArrayToString(collection, sb);

            if( sb.Length == 0 ) {  // 如果不是 int[], Guid[] ，就转换成命令参数

                foreach( object obj in collection ) {
                    string paraName = "x" + GetNextParamIndex().ToString();
                    this.AddParameter(paraName, obj);

                    if( sb.Length > 0 )
                        sb.Append(',');

                    sb.Append(this.Context.ClientProvider.GetParamterPlaceholder(paraName, this.Context));
                }
            }

            if( sb.Length == 0 ) {
                sb.Append("NULL");
                _sqlChanged = true;
            }


            if( name == null ) {     // 用于LINQ查询中的IN场景
                _sqlBuilder.Append(sb.ToString());
                _sqlChanged = true;
            }

            else {                  // 用于替换占位符场景
                string placeholder = "{" + name + "}";
                _sqlBuilder.Replace(placeholder, sb.ToString());
                _sqlChanged = true;
            }
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    /// <summary>
    /// 供LINQ查询中的IN操作，用于拼接一个 IN 数组（或者集合）
    /// </summary>
    /// <param name="collection"></param>
    internal void AppendArrayParameter(ICollection collection)
    {
        SetInArrayParameters(null, collection);
    }


    /// <summary>
    /// 尝试将 int[], Guid[] 变成 SQL语句中的一部分
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="sb"></param>
    internal static void ArrayToString(ICollection collection, StringBuilder sb)
    {
        // int[], List<int> 就直接生成到SQL语句中
        if( collection is IEnumerable<int> or IEnumerable<long>) {

            foreach( object obj in collection ) {
                if( sb.Length > 0 )
                    sb.Append(',');
                sb.Append(obj);
            }
        }
        // Guid[], List<Guid> 就直接生成到SQL语句中
        else if( collection is IEnumerable<Guid>) {

            foreach( object obj in collection ) {
                if( sb.Length > 0 )
                    sb.Append(',');
                sb.Append('\'').Append(obj).Append('\'');
            }
        }
    }

    /// <summary>
    /// 将当前所有的命令参数转移到指定的DbCommand中
    /// </summary>
    /// <param name="command"></param>
    internal void MoveParameters(DbCommand command)
    {
        // 将参数复制出来
        DbParameter[] parameters = _command.Parameters.Cast<DbParameter>().ToArray();

        // 断开参数与当前命令的关系
        _command.Parameters.Clear();

        // 将所有参数添加到新的命令中
        command.Parameters.AddRange(parameters);
    }


    /// <summary>
    /// 返回CPQuery中包含的SQL语句
    /// </summary>
    /// <returns>SQL语句</returns>
    public override string ToString()
    {
        if( _sqlBuilder != null )
            return _sqlBuilder.ToString();
        else
            return this._initSql ?? "(NULL)";
    }


    /// <summary>
    /// 获取要执行的SQL及命令参数
    /// </summary>
    /// <returns></returns>
    public string ToAllText()
    {
        return this.Command.ToLoggingText();
    }

    #endregion

    #region 拼接操作

    /// <summary>
    /// 添加SQL语句片段
    /// </summary>
    /// <param name="query">CPQuery对象实例</param>
    /// <param name="s">SQL语句片段</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery operator +(CPQuery query, string s)
    {
        query.AppendSql(s);
        return query;
    }


    /// <summary>
    /// 添加SQL语句片段
    /// </summary>
    /// <param name="query">CPQuery对象实例</param>
    /// <param name="s">SQL语句片段</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery operator +(CPQuery query, StringBuilder s)
    {
        query.AppendSql(s.ToString());
        return query;
    }


    /// <summary>
    /// 将字符串拼接到CPQuery对象
    /// </summary>
    /// <param name="query">CPQuery对象实例</param>
    /// <param name="s">SqlText对象</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery operator +(CPQuery query, SqlFragment s)
    {
        query.AppendSql(s.Value);
        return query;
    }

    /// <summary>
    /// 将QueryParameter实例拼接到CPQuery对象
    /// </summary>
    /// <param name="query">CPQuery对象实例</param>
    /// <param name="query2">QueryParameter对象实例</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery operator +(CPQuery query, CPQuery query2)
    {
        query.AppendSql(query2.ToString());

        // 复制命令参数
        query2.MoveParameters(query._command);

        return query;
    }



    /// <summary>
    /// 将QueryParameter实例拼接到CPQuery对象
    /// </summary>
    /// <param name="query">CPQuery对象实例</param>
    /// <param name="p">QueryParameter对象实例</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery operator +(CPQuery query, QueryParameter p)
    {
        query.AddQueryParameter(p);
        return query;
    }


    /// <summary>
    /// 将SqlParameter实例拼接到CPQuery对象
    /// </summary>
    /// <param name="query">CPQuery对象实例</param>
    /// <param name="p">SqlParameter对象实例</param>
    /// <returns>CPQuery对象实例</returns>
    public static CPQuery operator +(CPQuery query, DbParameter p)
    {
        query.InitSqlBuilder();
        query._sqlBuilder.Append(p.ParameterName);
        query._sqlChanged = true;

        query._command.Parameters.Add(p);
        return query;
    }

    #endregion

}
