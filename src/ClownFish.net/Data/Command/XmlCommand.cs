using ClownFish.Data.Xml;

namespace ClownFish.Data;

/// <summary>
/// 一种将SQL语句配置在XML中数据库操作命令
/// </summary>
public sealed class XmlCommand : BaseCommand
{
    private XmlCommandItem _item;

    // 记录 IN 参数的序号，供SetInArrayParameter方法使用
    // 由于 XmlCommand 不支持嵌套，所以就用实例字段来累加（与CPQuery不同）
    private int _paramIndex = 1;

    #region 构造函数

    internal XmlCommand(DbContext context) : base(context)
    {
    }


    /// <summary>
    /// 创建一个XmlCommand对象实例。
    /// </summary>
    /// <param name="name">命令名字</param>
    /// <param name="argsObject">匿名对象表示的参数</param>
    /// <returns></returns>
    public static XmlCommand Create(string name, object argsObject = null)
    {
        XmlCommand command = new XmlCommand(ConnectionScope.GetCurrentDbConext());
        command.Init(name, argsObject);
        return command;
    }

    /// <summary>
    /// 创建一个XmlCommand对象实例。
    /// </summary>
    /// <param name="name">命令名字</param>
    /// <param name="dictionary">要传递的参数字典</param>
    /// <returns></returns>
    public static XmlCommand Create(string name, Hashtable dictionary)
    {
        // 说明：保留这个重载，而不是用匿名对象来代替是因为：
        // 匿名对象是只读的，需要一次性构造，而Dictionary可以在不同的代码段中分开构造。
        // 所以，不要删除这个重载方法。

        XmlCommand command = new XmlCommand(ConnectionScope.GetCurrentDbConext());
        command.Init(name, dictionary);
        return command;
    }

    /// <summary>
    /// 创建一个XmlCommand对象实例。
    /// </summary>
    /// <param name="name">命令名字</param>
    /// <param name="dictionary">要传递的参数字典</param>
    /// <returns></returns>
    public static XmlCommand Create(string name, IDictionary<string, object> dictionary)
    {
        // 说明：保留这个重载，而不是用匿名对象来代替是因为：
        // 匿名对象是只读的，需要一次性构造，而Dictionary可以在不同的代码段中分开构造。
        // 所以，不要删除这个重载方法。

        XmlCommand command = new XmlCommand(ConnectionScope.GetCurrentDbConext());
        command.Init(name, dictionary);
        return command;
    }

    internal void Init(string name, object argsObject)
    {
        SetCommand(name);

        if( argsObject == null )
            return;

        PropertyInfo[] properties = argsObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        // 为每个DbParameter赋值。
        foreach( PropertyInfo pInfo in properties ) {
            object value = pInfo.FastGetValue(argsObject);
            SetParameter(pInfo.Name, value);
        }
    }

    internal void Init(string name, Hashtable dictionary)
    {
        SetCommand(name);

        if( dictionary == null || dictionary.Count == 0 )
            return;

        foreach( DictionaryEntry entry in dictionary )
            SetParameter(entry.Key.ToString(), entry.Value);
    }

    internal void Init(string name, IDictionary<string, object> dictionary)
    {
        SetCommand(name);

        if( dictionary == null || dictionary.Count == 0 )
            return;

        foreach( KeyValuePair<string, object> kvp in dictionary )
            SetParameter(kvp.Key, kvp.Value);
    }

    #endregion


    /// <summary>
    /// 根据指定的命令参数名称获取XmlCommand已定义的命令参数
    /// </summary>
    /// <param name="name">XML配置文件中定义的参数名称，包含参数前缀</param>
    /// <returns></returns>
    public DbParameter GetParameter(string name)
    {
        foreach( DbParameter p in _command.Parameters ) {
            if( p.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase) )
                return p;
        }

        return null;
    }

    /// <summary>
    /// 根据指定的name/value，给命令参数赋值，或者处理【占位符参数】
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    private void SetParameter(string name, object value)
    {
        // 根据属性名称，查找参数集合有没有对应的参数
        //string pname = this.Context.ClientProvider.GetParamterName(name, this.Context);
        string pname = "@" + name;  // 为了让 XmlCommand 能通用（支持多种数据库），参数名称固定采用 @name 的格式

        DbParameter p = GetParameter(pname);
        if( p != null ) {
            // 注意：在调用 XmlCommand 时，匿名对象的值不能是DbParameter，
            // 因为XmlCommand已经定义过命令参数，调用时只需要赋值即可（与CPQuery不同）
            CPQuery.SetParameterValue(p, value);
            return;
        }


        // 如果参数集合没有匹配的参数，就尝试从命令文本中查找 {name} ，并做替换处理。
        string placeholder = "{" + name + "}";
        int j = _command.CommandText.IndexOf(placeholder, StringComparison.OrdinalIgnoreCase);

        if( j > 0 ) {
            // 处理【占位符参数】操作，有3种场景：
            // 1、字符串替换，例如：表名占符，允许一个XmlCommand操作多个表
            // 2、IN 子句替换
            // 3、查询条件替换，用CPQuery构造一个子查询或者查询片段

            if( value is CPQuery )
                SetQueryParameter(placeholder, (CPQuery)value);

            else if( CPQuery.IsArrayValue(value?.GetType()) )
                SetInArrayParameter(placeholder, (ICollection)value);

            else
                SetReplaceParameter(placeholder, (value?.ToString() ?? "NULL"));
        }

        // 2018-01-04 注释下面的抛异常代码，原因：
        // 存在一种场景：实体中包含的某个属性是数据表的自增列，此时在XmlCommand是没有定义的，
        //              当使用实体做为参数时，就会运行到这里，但是这种情况又是正常的。
        //else
        //	// 如果没有匹配的命令参数，也找不到匹配的【占位符参数】，就抛出异常告之调用者
        //	throw new ArgumentException(string.Format(
        //						"传入的参数对象中，属性 {0} 没有在MXL定义对应的参数名。", name));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
    private void SetReplaceParameter(string placeholder, string text)
    {
        // ###################  注意  ###########################

        // 这里不做任何编码处理，需要调用者保证没有SQL注入问题，
        // 例如只替换固定的表名，而不是使用前端浏览器提交的数据

        // ###################  注意  ###########################

        _command.CommandText = _command.CommandText.Replace(placeholder, text);
    }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
    private void SetInArrayParameter(string placeholder, ICollection collection)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            // 优先检查 int[], Guid[] 类型，并转成SQL语句中的一部分
            // 因为这些强类型的数据本身是安全的，不存在注入，就不转换成命令参数。
            CPQuery.ArrayToString(collection, sb);

            if( sb.Length == 0 ) {  // 如果不是 int[], Guid[] ，就转换成命令参数

                foreach( object obj in collection ) {
                    string name = "x" + (_paramIndex++).ToString();

                    DbParameter parameter = _command.CreateParameter();
                    parameter.ParameterName = this.Context.ClientProvider.GetParamterName(name, this.Context);
                    parameter.Value = obj;
                    _command.Parameters.Add(parameter);

                    if( sb.Length > 0 )
                        sb.Append(',');

                    sb.Append(this.Context.ClientProvider.GetParamterPlaceholder(name, this.Context));
                }
            }

            if( sb.Length == 0 )
                sb.Append("NULL");

            _command.CommandText = _command.CommandText.Replace(placeholder, sb.ToString());
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
    private void SetQueryParameter(string placeholder, CPQuery query)
    {
        // 替换占位符
        _command.CommandText = _command.CommandText.Replace(placeholder, query.ToString());

        // 添加命令参数
        query.MoveParameters(_command);
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
    private void SetCommand(string name)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException("name");

        XmlCommandItem item = XmlCommandManager.Instance.GetCommand(name);
        if( item == null )
            throw new ArgumentOutOfRangeException("name",
                            string.Format("指定的XmlCommand名称 {0} 不存在。", name));

        // 填充命令对象
        _command.CommandText = item.CommandText;
        _command.CommandType = item.CommandType;

        if( item.Timeout > 0 )
            _command.CommandTimeout = item.Timeout;

        FillParameters(item);
    }

    /// <summary>
    /// 将XmlCommand中定义的参数填充到命令中
    /// </summary>
    /// <param name="item"></param>
    private void FillParameters(XmlCommandItem item)
    {
        foreach( XmlCmdParameter cp in item.Parameters ) {
            DbParameter parameter = _command.CreateParameter();

            // 参数名需要和XmlCommand中的名字匹配，所以这里不做处理（不加前缀）
            parameter.ParameterName = cp.Name;
            parameter.DbType = cp.Type;
            parameter.Direction = cp.Direction;
            parameter.Size = cp.Size;
            //parameter.Value = DBNull.Value;		// 先给默认值
            _command.Parameters.Add(parameter);
        }

        _item = item;       // 记录从哪个 XmlCommandItem 对象构造出来的
    }


    /// <summary>
    /// 开始执行数据库操作前要处理的额外操作
    /// </summary>
    protected override void BeforeExecute(DbCommand command)
    {
        base.BeforeExecute(command);

        if( string.IsNullOrEmpty(_item.Database) == false )
            _context.ChangeDatabase(_item.Database);

    }




}
