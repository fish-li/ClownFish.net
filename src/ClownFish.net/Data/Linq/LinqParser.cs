using System.Linq.Expressions;

namespace ClownFish.Data.Linq;

internal class LinqParser
{
    #region 字段定义

    private readonly DbContext _dbContext;
    private readonly Type _entityType;
    private readonly Expression _expression;    
    private readonly string _tableName;

    private Type _selectEntityType;
    private List<PropertyInfo> _selectFields;

    // WHERE条件
    private CPQuery _query;

    // 排序表达式
    private StringBuilder _orders;

    // 可用于执行的命令动词
    private string _commandName;

    // 分页参数
    private int? _skip;
    private int? _take;

    #endregion

    internal LinqParser(DbContext dbContext, Type entityType, Expression expression)
    {
        _dbContext = dbContext;
        _entityType = entityType;
        _expression = expression;
        _tableName = dbContext.GetObjectFullName(_entityType.GetDbTableName());
    }


    #region 表达式解析


    public void Translator()
    {
        MethodCallExpression current = _expression as MethodCallExpression;
        if( current == null )
            throw new NotSupportedException("不支持的表达式，没有指定操作方法。");


        if( current.Method.DeclaringType == typeof(System.Linq.Queryable)
            || current.Method.DeclaringType == typeof(AsyncQueryExtensions)
            || current.Method.DeclaringType == typeof(QueryExtensions) ) {
            // 只支持部分由 .net framework 定义的标准查询操作符
            // 以及框架内部定义的异步方法
        }
        else {
            throw new NotSupportedException("不支持的表达式，操作方法不是标准操作符：" + current.Method.Name);
        }



        while( current != null ) {
            string methodName = current.Method.Name;

            switch( methodName ) {
                case "ToList":                  // 获取实例列表，其实这个枚举不会匹配到，因为它是默认操作
                case "ToSingle":                // 获取单个数据实体
                case "FirstOrDefault":          // 获取单个数据实体
                case "Any":                     // 判断是否存在匹配的数据库记录
                case "Count":                   // 获取匹配的数据库记录条数
                case "GetQuery":
                case "ToListAsync":             // 获取实例列表
                case "ToSingleAsync":           // 获取单个数据实体
                case "FirstOrDefaultAsync":     // 获取单个数据实体
                case "AnyAsync":                // 判断是否存在匹配的数据库记录
                case "CountAsync":              // 获取匹配的数据库记录条数
                    SetCommnad(methodName);
                        break;

                case "Where":
                    AppendWhereExpression(current.Arguments[1]);
                    break;

                case "OrderBy":
                case "OrderByDescending":
                case "ThenBy":
                case "ThenByDescending":
                    AppendOrderExpression(methodName, current.Arguments[1]);
                    break;

                case "Select":
                    ParseSelect(current);
                    break;

                case "Skip":
                    _skip = (int)(current.Arguments[1] as ConstantExpression).Value;
                    break;

                case "Take":
                    _take = (int)(current.Arguments[1] as ConstantExpression).Value;
                    break;

                default:
                    throw new NotSupportedException("不支持的表达式，当前操作方法：" + methodName);
            }

            // 判断下一层调用
            current = GetNextMethod(current);
        }
    }

    private void ParseSelect(MethodCallExpression current)
    {
        // 这里只分析第二个参数，因为第一个参数是一个集合，不涉及字段

        //{t => new Product() {ProductID = t.ProductID, ProductName = t.ProductName}}	
        UnaryExpression unaryExpression = current.Arguments[1] as UnaryExpression;

        //{new Product() {ProductID = t.ProductID, ProductName = t.ProductName}}
        LambdaExpression lamdba = unaryExpression.Operand as LambdaExpression;
        MemberInitExpression initExpression = lamdba.Body as MemberInitExpression;

        if( initExpression == null ) {
            if( lamdba.Body is NewExpression )
                throw new NotSupportedException("SELECT操作的类型不支持创建新类型。");
            else if( lamdba.Body is ParameterExpression )
                return;
            else
                throw new NotSupportedException("SELECT操作不支持表达式类型。");
        }

        _selectEntityType = initExpression.Type;
        _selectFields = new List<PropertyInfo>(initExpression.Bindings.Count);

        foreach( var b in initExpression.Bindings ) {
            PropertyInfo p = b.Member as PropertyInfo;
            if( p == null )
                throw new NotSupportedException("字段表达式只支持属性。");

            _selectFields.Add(p);
        }
    }


    private string ParseFields()
    {
        if( _selectEntityType != null && _selectEntityType != _entityType )
            throw new NotSupportedException("SELECT操作的类型不允许与Query方法中指定的类型不一致。");

        if( _selectFields == null || _selectFields.Count == 0 )
            return "*";


        string[] names = new string[_selectFields.Count];
        int index = 0;

        foreach( PropertyInfo p in _selectFields ) {
            names[index++] = _dbContext.GetObjectFullName(p.GetDbFieldName());
        }

        return string.Join(",", names);
    }


    #endregion



    #region 内部辅助方法

    private MethodCallExpression GetNextMethod(MethodCallExpression current)
    {
        Expression expression = current.Arguments[0];

        if( expression is MethodCallExpression )
            return expression as MethodCallExpression;
        else
            return null;
    }

    //private void TryGetEntityType(MethodCallExpression current)
    //{
    //    if( _entityType == null
    //        && current.Arguments[0] is ConstantExpression ) {

    //        Type t = (current.Arguments[0] as ConstantExpression).Type;

    //        if( t.IsGenericType && t.GetGenericTypeDefinition() == typeof(EntityQuery<>) )
    //            _entityType = t.GetGenericArguments()[0];
    //    }
    //}


    private void SetCommnad(string name)
    {
        if( _commandName == null )
            _commandName = name;
        else
            throw new NotSupportedException("不支持的表达式，因为包含了多个执行动词。");
    }

    private void AppendOrderExpression(string method, Expression exp)
    {
        try {
            var unaryExpression = exp as UnaryExpression;
            var operand = unaryExpression.Operand as LambdaExpression;
            var memberExpression = operand.Body as MemberExpression;
            var member = memberExpression.Member;

            string fieldName = _dbContext.GetObjectFullName(member.GetDbFieldName());
            string direct = method.IndexOf("Descending", StringComparison.Ordinal) > 0 ? " DESC" : string.Empty;

            if( _orders == null )
                _orders = new StringBuilder(fieldName + direct);

            else
                // 排序的调用次序是反的，所以要反过来拼接
                _orders.Insert(0, fieldName + direct + ",");
        }
        catch {
            throw new NotSupportedException("不支持的排序表达式。");
        }
    }


    private void AppendWhereExpression(Expression exp)
    {
        if( _query == null )
            _query = _dbContext.CreateCPQuery();
        else
            _query = _query + " AND ";

        WhereParase whereParse = new WhereParase(_dbContext, _query);
        whereParse.Visit(exp);
    }


    #endregion



    #region Execute Command

    public object ExecuteCommand()
    {
        if( _commandName == null ) {
            return ToList();
        }

        switch( _commandName ) {
            case "FirstOrDefault":
            case "ToSingle":
                return ToSingle();

            case "Any":
                return Any();

            case "Count":
                return Count();

            case "GetQuery":
                return GetToListQuery();

            default:
                throw new NotSupportedException("不支持的表达式，操作方法不支持：" + _commandName);
        }
    }


    public async Task<object> ExecuteCommandAsync()
    {
        switch( _commandName ) {
            case "ToListAsync":
                return await ToListAsync();

            case "FirstOrDefaultAsync":
            case "ToSingleAsync":
                return await ToSingleAsync();

            case "AnyAsync":
                return await AnyAsync();

            case "CountAsync":
                return await CountAsync();

            default:
                throw new NotSupportedException("不支持的表达式，操作方法不支持：" + _commandName);
        }
    }

    private CPQuery GetFirstOrDefaultQuery()
    {
        CPQuery query = _dbContext.CreateCPQuery()
                        + "SELECT " + ParseFields() + Environment.NewLine
                        + "FROM " + _tableName;

        if( _query != null )
            query = query + Environment.NewLine + "WHERE " + _query;

        return query;
    }
    private object ToSingle()
    {
        CPQuery query = GetFirstOrDefaultQuery();


        // 由于泛型结束，只能反射调用了
        MethodInfo method = query.GetType().GetMethod("ToSingle", BindingFlags.Instance | BindingFlags.Public);
        method = method.MakeGenericMethod(_entityType);
        return method.FastInvoke(query, null);
    }

    private async Task<object> ToSingleAsync()
    {
        CPQuery query = GetFirstOrDefaultQuery();


        // 由于泛型约束的限制，只能反射调用了
        MethodInfo method = query.GetType().GetMethod("ToSingleAsync", BindingFlags.Instance | BindingFlags.Public);
        method = method.MakeGenericMethod(_entityType);

        //return (Task<object>)method.FastInvoke(query, null);
        // 上面代码不能正常运行，会出现异常：不能将 Task<XXX> 转换成  Task<object>

        Task task = (Task)method.FastInvoke(query, null);
        await task;
        return task.GetType().GetProperty("Result").GetValue(task);
    }


    private CPQuery GetAnyQuery()
    {
        CPQuery query = _dbContext.CreateCPQuery()
                        + "SELECT 1 WHERE EXISTS ( SELECT 1" + Environment.NewLine
                        + "FROM " + _tableName;

        if( _query != null )
            query = query + Environment.NewLine + "WHERE " + _query;

        query = query + ")";
        return query;
    }

    private bool Any()
    {
        CPQuery query = GetAnyQuery();

        return query.ExecuteScalar<int>() == 1;
    }

    private async Task<bool> AnyAsync()
    {
        CPQuery query = GetAnyQuery();

        return await query.ExecuteScalarAsync<int>() == 1;
    }

    private CPQuery GetCountQuery()
    {
        CPQuery query = _dbContext.CreateCPQuery()
                        + "SELECT count(*)" + Environment.NewLine
                        + "FROM " + _tableName;

        if( _query != null )
            query = query + Environment.NewLine + "WHERE " + _query;

        return query;
    }
    private int Count()
    {
        CPQuery query = GetCountQuery();

        return query.ExecuteScalar<int>();
    }

    private async Task<int> CountAsync()
    {
        CPQuery query = GetCountQuery();

        return await query.ExecuteScalarAsync<int>();
    }


    private CPQuery GetToListQuery()
    {
        CPQuery query = _dbContext.CreateCPQuery()
                        + "SELECT " + ParseFields() + Environment.NewLine
                        + "FROM " + _tableName;

        if( _query != null )
            query = query + Environment.NewLine + "WHERE " + _query;

        if( _orders != null )
            query = query + Environment.NewLine + "ORDER BY " + _orders.ToString();

        if( _take.HasValue ) {
            query = _dbContext.ClientProvider.SetPagedQuery(query, _skip.GetValueOrDefault(), _take.Value);
        }

        return query;
    }

    private object ToList()
    {
        CPQuery query = GetToListQuery();


        // 由于泛型结束，只能反射调用了
        MethodInfo method = query.GetType().GetMethod("ToList", BindingFlags.Instance | BindingFlags.Public);
        method = method.MakeGenericMethod(_entityType);
        return method.FastInvoke(query, null);
    }

    private async Task<object> ToListAsync()
    {
        CPQuery query = GetToListQuery();


        // 由于泛型结束，只能反射调用了
        MethodInfo method = query.GetType().GetMethod("ToListAsync", BindingFlags.Instance | BindingFlags.Public);
        method = method.MakeGenericMethod(_entityType);


        Task task = (Task)method.FastInvoke(query, null);
        await task;
        return task.GetType().GetProperty("Result").GetValue(task);
    }

    #endregion

}
