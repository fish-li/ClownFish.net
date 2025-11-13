using System.Linq.Expressions;

namespace ClownFish.Data.Linq;

/// <summary>
/// 解析WHERE条件
/// </summary>
internal class WhereParase : ExpressionVisitor
{
    private readonly DbContext _dbContext;
    private readonly CPQuery _query;

    internal WhereParase(DbContext dbContext, CPQuery query)
    {
        _dbContext = dbContext;
        _query = query;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _query.AppendSql("(");
        Visit(node.Left);
        _query.AppendSql(GetOperator(node.NodeType));

        object value;
        if( TryGetExpressionValue(node.Right, out value) ) {
            _query.AddQueryParameter(new QueryParameter(value));
        }
        else {
            Visit(node.Right);
            //throw new NotSupportedException("不支持的表达式，比较操作的右边不是常量。");  // 注意：不能启动这行代码！
        }

        _query.AppendSql(")");

        return node;
    }


    private bool TryGetExpressionValue(Expression node, out object value)
    {
        if( node is ConstantExpression node1 ) {
            value = node1.Value;
            return true;
        }
        else if( node is MemberExpression node2 ) {
            value = GetMemberExpressionValue(node2);
            return true;
        }
        else if( node is UnaryExpression node3 ) {
            if( node3.NodeType == ExpressionType.Convert ) {

                return TryGetExpressionValue(node3.Operand, out value);
            }
        }
        else if( node is MethodCallExpression node4 ) {
            if( node4.Method.Name == "op_Implicit" ) {
                TryGetExpressionValue(node4.Arguments[0], out value);
                return true;
            }
            //else {     // 不要取消这段注释，否则会出现新问题
            //    value = GetMethodCallExpressionValue(node4);
            //    return true;
            //}
        }

        value = null;
        return false;
    }

#if NETCOREAPP
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2026: FastGetValue")]
#endif
    private object GetMemberExpressionValue(MemberExpression node)
    {
        object instance = null;

        if( node.Expression != null ) {
            if( node.Expression is ConstantExpression )
                instance = (node.Expression as ConstantExpression).Value;

            else if( node.Expression is MemberExpression )
                instance = GetMemberExpressionValue(node.Expression as MemberExpression);

            else
                throw new NotSupportedException("表达式不是一个直接值");
        }
        // else 表达式是一个静态成员的访问


        if( node.Member is FieldInfo ) {
            FieldInfo field = node.Member as FieldInfo;
            return field.FastGetValue(instance);
        }
        else {
            PropertyInfo prop = node.Member as PropertyInfo;
            return prop.FastGetValue(instance);
        }
    }

    //private object GetMethodCallExpressionValue(MethodCallExpression node)
    //{
    //    object instance = null;

    //    if( node.Method.IsStatic == false ) {
    //        TryGetExpressionValue(node.Object, out instance);
    //    }

    //    List<object> arguments = new List<object>(node.Arguments.Count);
    //    foreach( Expression argument in node.Arguments ) {
    //        TryGetExpressionValue(argument, out object xx);
    //        arguments.Add(xx);
    //    }

    //    return node.Method.Invoke(instance, arguments.ToArray());
    //}

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if( node.Operand is LambdaExpression )
            Visit(node.Operand);

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        string fieldName = _dbContext.GetObjectFullName(node.Member.GetDbFieldName());
        _query.AppendSql(fieldName);

        return node;
    }


    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if( node.Method.Name == "StartsWith" && node.Method.DeclaringType == typeof(string) ) {
            return Like(node, string.Empty, "%");
            // EndsWith 对应的 like '%xxx' 没有任何意义，就不实现了
        }
        else if( node.Method.Name == "Contains" ) {
            if( node.Method.DeclaringType == typeof(System.Linq.Enumerable)
#if NETCOREAPP  // C# 14 会将 Array.Contains 翻译成调用下面的方法，同时还会做隐式转换
                // https://learn.microsoft.com/zh-cn/dotnet/core/compatibility/core-libraries/10.0/csharp-overload-resolution
                || node.Method.DeclaringType == typeof(System.MemoryExtensions) 
#endif
                ) {
                return In(node);
            }
            else if( node.Method.DeclaringType == typeof(string) ) {
                return Like(node, "%", "%");
            }
        }

        throw new NotSupportedException("不支持的表达式，当前操作方法：" + node.Method.Name);
        //return base.VisitMethodCall(node);
    }

    private Expression Like(MethodCallExpression node, string x1, string x2)
    {
        object value;
        if( TryGetExpressionValue(node.Arguments[0], out value) == false )
            throw new NotSupportedException("不支持的表达式，StartsWith的参数必须是常量");

        _query.AppendSql("(");
        Visit(node.Object);

        _query.AppendSql(" like ");

        _query.AddQueryParameter(new QueryParameter(x1 + value.ToString() + x2));
        _query.AppendSql(")");

        return node;
    }

    private Expression In(MethodCallExpression node)
    {
        object value;
        if( TryGetExpressionValue(node.Arguments[0], out value) == false )
            throw new NotSupportedException("不支持的表达式，Contains的参数必须是常量");

        if( false == (value is ICollection collection) ) {
            throw new NotSupportedException("不支持的表达式，Contains的参数必须是常量-2");
        }

        _query.AppendSql("(");
        Visit(node.Arguments[1]);

        _query.AppendSql(" IN (");
        _query.AppendArrayParameter(collection);
        _query.AppendSql("))");

        return node;
    }

    private string GetOperator(ExpressionType expressiontype)
    {
        switch( expressiontype ) {
            case ExpressionType.And:
                return " AND ";
            case ExpressionType.AndAlso:
                return " AND ";
            case ExpressionType.Or:
                return " OR ";
            case ExpressionType.OrElse:
                return " OR ";
            case ExpressionType.Equal:
                return " = ";
            case ExpressionType.NotEqual:
                return " != ";
            case ExpressionType.LessThan:
                return " < ";
            case ExpressionType.LessThanOrEqual:
                return " <= ";
            case ExpressionType.GreaterThan:
                return " > ";
            case ExpressionType.GreaterThanOrEqual:
                return " >= ";
            default:
                throw new NotSupportedException("不支持的比较运算符: " + expressiontype.ToString());
        }
    }


}
