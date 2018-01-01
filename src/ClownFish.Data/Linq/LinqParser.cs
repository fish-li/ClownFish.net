using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;

namespace ClownFish.Data.Linq
{
	internal class LinqParser
	{
		#region 字段定义

		internal DbContext Context { get; set; }
		internal bool WithNoLock { get; set; }

		private Expression _expression;

		private Type _entityType;
		private string _tableName;

		private Type _selectEntityType;
		private List<PropertyInfo> _selectFields;

		// WHERE条件
		private CPQuery _query;

		// 排序表达式
		private StringBuilder _orders;

		// 可用于执行的命令动词
		private string _commandName;

		// 分页参数
		//private int? _skip;
		//private int? _take;

		#endregion



		#region 表达式解析

		public object Translator(Expression expression)
		{
			if( expression == null )
				throw new ArgumentNullException("expression");

			_expression = expression;

			// 分析表达式
			DecomposeExpression(expression);


			ObtainTableName();			// 从拆分的结果中获得要访问的数据库表名
			return ExecuteCommand();	// 执行数据库操作
		}

		public async Task<object> TranslatorAsync(Expression expression)
		{
			if( expression == null )
				throw new ArgumentNullException("expression");

			_expression = expression;

			// 分析表达式
			DecomposeExpression(expression);


			ObtainTableName();          // 从拆分的结果中获得要访问的数据库表名
			return await ExecuteCommandAsync();    // 执行数据库操作
		}



		private void DecomposeExpression(Expression expression)
		{
			MethodCallExpression current = expression as MethodCallExpression;
			if( current == null )
				throw new NotSupportedException("不支持的表达式，没有指定操作方法。");


			if( current.Method.DeclaringType == typeof(System.Linq.Queryable) 
				|| current.Method.DeclaringType == typeof(AsyncQueryExtensions) ) {
				// 只支持部分由 .net framework 定义的标准查询操作符
				// 以及框架内部定义的异步方法
			}
			else {
				throw new NotSupportedException("不支持的表达式，操作方法不是标准操作符：" + current.Method.Name);
			}
			


			while( current != null ) {

				if( current.Method.Name == "FirstOrDefault"         // 获取单个数据实体
					|| current.Method.Name == "Any"                 // 判断是否存在匹配的数据库记录
					|| current.Method.Name == "Count"               // 获取匹配的数据库记录条数

					|| current.Method.Name == "FirstOrDefaultAsync"      // 获取单个数据实体
					|| current.Method.Name == "AnyAsync"                 // 判断是否存在匹配的数据库记录
					|| current.Method.Name == "CountAsync"               // 获取匹配的数据库记录条数
					|| current.Method.Name == "ToListAsync"               // 获取匹配的数据库记录条数
					) {
					SetCommnad(current.Method.Name);
				}

				//else if( current.Method.Name == "Skip" ) {
				//	_skip = (int)(current.Arguments[1] as ConstantExpression).Value;
				//}
				//else if( current.Method.Name == "Take" ) {
				//	_take = (int)(current.Arguments[1] as ConstantExpression).Value;
				//}

				else if( current.Method.Name == "Where" ) {
					TryGetEntityType(current);
					AppendWhereExpression(current.Arguments[1]);
				}

				else if( current.Method.Name == "OrderBy"
					|| current.Method.Name == "OrderByDescending"
					|| current.Method.Name == "ThenBy"
					|| current.Method.Name == "ThenByDescending"
					) {
					AppendOrderExpression(current.Method.Name, current.Arguments[1]);
				}

				else if( current.Method.Name == "Select" ) {
					TryGetEntityType(current);
					ParseSelect(current);
				}

				else
					throw new NotSupportedException("不支持的表达式，当前操作方法：" + current.Method.Name);

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
			if( _selectEntityType != null && _selectEntityType != _entityType)
				throw new NotSupportedException("SELECT操作的类型不允许与Query方法中指定的类型不一致。");

			if( _selectFields == null || _selectFields.Count == 0 )
				return "*";


			string[] names = new string[_selectFields.Count];
			int index = 0;

			foreach( PropertyInfo p in _selectFields ) {
				names[index++] = p.GetDbFieldName();
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

		private void TryGetEntityType(MethodCallExpression current)
		{
			if( _entityType == null
				&& current.Arguments[0] is ConstantExpression ) {

				Type t = (current.Arguments[0] as ConstantExpression).Type;

				if( t.IsGenericType && t.GetGenericTypeDefinition() == typeof(EntityQuery<>) )
					_entityType = t.GetGenericArguments()[0];
			}
		}

		private void ObtainTableName()
		{
			if( _entityType == null ) 
				throw new InvalidOperationException("不能从表达式中解析到实体对应的数据表。");			
			else 
				_tableName = _entityType.GetDbTableName() + (this.WithNoLock ? " WITH(NOLOCK)" : string.Empty);
		}

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

				string fieldName = member.GetDbFieldName();
				string direct = method.IndexOf("Descending") > 0 ? " DESC" : string.Empty;

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
				_query = this.Context.CreateCPQuery();
			else
				_query = _query + " AND ";

			WhereParase whereParse = new WhereParase(_query);
			whereParse.Visit(exp);
		}


		#endregion



		#region Execute Command

		private object ExecuteCommand()
		{
			if( _commandName == null ) {
				return ToList();
			}

			switch( _commandName ) {
				case "FirstOrDefault":
					return FirstOrDefault();

				case "Any":
					return Any();

				case "Count":
					return Count();

				default:
					throw new NotSupportedException("不支持的表达式，操作方法不支持：" + _commandName);
			}
		}


		private async Task<object> ExecuteCommandAsync()
		{
			switch( _commandName ) {
				case "ToListAsync":
					return await ToListAsync();

				case "FirstOrDefaultAsync":
					return await FirstOrDefaultAsync();

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
			CPQuery query = this.Context.CreateCPQuery()
							+ "SELECT " + ParseFields() + Environment.NewLine
							+ "FROM " + _tableName;

			if( _query != null )
				query = query + Environment.NewLine + "WHERE " + _query;

			return query;
		}
		private object FirstOrDefault()
		{
			CPQuery query = GetFirstOrDefaultQuery();


			// 由于泛型结束，只能反射调用了
			MethodInfo method = query.GetType().GetMethod("ToSingle", BindingFlags.Instance | BindingFlags.Public);
			method = method.MakeGenericMethod(_entityType);
			return method.FastInvoke(query, null);
		}

		private async Task<object> FirstOrDefaultAsync()
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
			CPQuery query = this.Context.CreateCPQuery()
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
			CPQuery query = this.Context.CreateCPQuery()
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
			CPQuery query = this.Context.CreateCPQuery()
							+ "SELECT " + ParseFields() + Environment.NewLine
							+ "FROM " + _tableName;

			if( _query != null )
				query = query + Environment.NewLine + "WHERE " + _query;

			if( _orders != null )
				query = query + Environment.NewLine + "ORDER BY " + _orders.ToString();

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
}
