using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;


namespace ClownFish.Data
{
	/// <summary>
	/// 安全简单的拼接SQL的工具类
	/// </summary>
	public sealed class CPQuery : BaseCommand
	{
		#region 数据成员定义

		// 因为允许在CPQuery嵌套使用，所以为了避免产生了同名的动态参数名称，
		// 所以用静态变量来累加序号，防止序号重复，产生相同的参数名称。
		private static int s_index;

		private StringBuilder _sqlBuidler = new StringBuilder(1024);

        internal StringBuilder SqlBuilder
        {
            get { return _sqlBuidler; }
        }

		/// <summary>
		/// 获取当前CPQuery内部的DbCommand对象，
		/// 当前重写属性会更新CommandText属性
		/// </summary>
		public override DbCommand Command
		{
			get
			{
				_command.CommandText = _sqlBuidler.ToString();
				return _command;
			}
		}

		///// <summary>
		///// 只返回DbCommand实例，不更新CommandText属性
		///// </summary>
		//internal DbCommand DbCommandInstance => _command;


		#endregion

		#region 构造函数

		internal CPQuery(DbContext context) : base(context)
		{
		}

		internal void Init(string parameterizedSQL)
		{
			this.AppendSql(parameterizedSQL);
		}

		internal void Init(string parameterizedSQL, object argsObject)
		{
			this.AppendSql(parameterizedSQL);


			if( argsObject == null )
				return;

			PropertyInfo[] properties = argsObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach( PropertyInfo property in properties ) {
				object value = property.FastGetValue(argsObject);
				SetParameter(property.Name, value);
			}
		}

		internal void Init(string parameterizedSQL, Dictionary<string, object> dictionary)
		{
			this.AppendSql(parameterizedSQL);


			if( dictionary == null || dictionary.Count == 0 )
				return;

			foreach( KeyValuePair<string, object> kvp in dictionary )
				SetParameter(kvp.Key, kvp.Value);
		}

		internal void Init(string parameterizedSQL, DbParameter[] parameters)
		{
			this.AppendSql(parameterizedSQL);


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

			CPQuery query = new CPQuery(ConnectionScope.GetDefaultDbConext());
			query.Init(parameterizedSQL);
			return query;
		}
		
		/// <summary>
		/// 根据指定的参数化SQL语句，匿名对象参数，创建CPQuery对象实例
		/// </summary>
		/// <param name="parameterizedSQL">参数化SQL语句</param>
		/// <param name="argsObject">匿名对象，每个属性对应一个命令参数</param>
		/// <returns>CPQuery对象实例</returns>
		public static CPQuery Create(string parameterizedSQL, object argsObject)
		{
			if( string.IsNullOrEmpty(parameterizedSQL) )
				throw new ArgumentNullException("parameterizedSQL");

			CPQuery query = new CPQuery(ConnectionScope.GetDefaultDbConext());
			query.Init(parameterizedSQL, argsObject);
			return query;
		}

		/// <summary>
		/// 通过参数化SQL、哈希表的方式,创建CPQuery对象实例
		/// </summary>
		/// <param name="parameterizedSQL">参数化的SQL字符串</param>
		/// <param name="dictionary">哈希表</param>
		/// <returns>CPQuery对象实例</returns>
		public static CPQuery Create(string parameterizedSQL, Dictionary<string, object> dictionary)
		{
			if( string.IsNullOrEmpty(parameterizedSQL) )
				throw new ArgumentNullException("parameterizedSQL");

			CPQuery query = new CPQuery(ConnectionScope.GetDefaultDbConext());
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
				throw new ArgumentNullException("parameterizedSQL");

			CPQuery query = new CPQuery(ConnectionScope.GetDefaultDbConext());
			query.Init(parameterizedSQL, parameters);
			return query;
		}

		#endregion

		#region 基础操作


		internal void AppendSql(string s)
		{
			if( string.IsNullOrEmpty(s) )
				return;

			_sqlBuidler.Append(s);
		}

		private uint GetNextParamIndex()
		{
			int current = System.Threading.Interlocked.Increment(ref s_index);

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
			string name = "p" + GetNextParamIndex().ToString();

			// SQL语句中拼入参数占位符
			_sqlBuidler.Append(ParaNameBuilder.GetPlaceholder(name));

			// 参数集合中添加命令参数
			AddParameter(name, p.Value);
		}

		private void AddParameter(string name, object value)
		{
			DbParameter parameter = _command.CreateParameter();
			parameter.ParameterName = ParaNameBuilder.GetParaName(name);
			parameter.Value = value ?? DBNull.Value;
			_command.Parameters.Add(parameter);
		}
		

		private void SetParameter(string name, object value)
		{
			if( value == null || value == DBNull.Value )
				this.AddParameter(name, DBNull.Value);

			else if( value is ICollection )
				SetInArrayParameters(name, (ICollection)value);

			else if( value is DbParameter ) 
				SetDbParameter(name, (DbParameter)value);
			
			else if( value is CPQuery )
				SetQueryParameter(name, (CPQuery)value);

			else if( value is SqlFragment )
				SetFragmentParameter(name, (SqlFragment)value);

			else
				AddParameter(name, value);
		}

		private void SetFragmentParameter(string name, SqlFragment fragment)
		{
			string placeholder = "{" + name + "}";

			// 替换占位符
			_sqlBuidler.Replace(placeholder, fragment.Value);
		}

		private void SetDbParameter(string name, DbParameter parameter)
		{
			if( string.IsNullOrEmpty(parameter.ParameterName) )
				parameter.ParameterName = ParaNameBuilder.GetParaName(name);
			// else 不检查参数的名称是否匹配，由调用者保证。

			_command.Parameters.Add(parameter);
		}

		private void SetQueryParameter(string name, CPQuery query)
		{
			string placeholder = "{" + name + "}";

			// 替换占位符
			_sqlBuidler.Replace(placeholder, query.ToString());

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

			StringBuilder sb = new StringBuilder(128);

			// 优先检查 int[], Guid[] 类型，并转成SQL语句中的一部分
			// 因为这些强类型的数据本身是安全的，不存在注入，就不转换成命令参数。
			ArrayToString(collection, sb);

			if( sb.Length == 0 ) {  // 如果不是 int[], Guid[] ，就转换成命令参数

				foreach( object obj in collection ) {
					string paraName = "x" + GetNextParamIndex().ToString();
					this.AddParameter(paraName, obj);

					if( sb.Length > 0 )
						sb.Append(',');

					sb.Append(ParaNameBuilder.GetPlaceholder(paraName));
				}
			}

			if( sb.Length == 0 )
				sb.Append("NULL");


			if( name == null )		// 用于LINQ查询中的IN场景
				_sqlBuidler.Append(sb.ToString());

			else {                  // 用于替换占位符场景
				string placeholder = "{" + name + "}";
				_sqlBuidler.Replace(placeholder, sb.ToString());
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
			if( typeof(IEnumerable<int>).IsAssignableFrom(collection.GetType()) ) {
				foreach( object obj in collection ) {
					if( sb.Length > 0 )
						sb.Append(',');
					sb.Append(obj.ToString());
				}
			}
			// Guid[], List<Guid> 就直接生成到SQL语句中
			else if( typeof(IEnumerable<Guid>).IsAssignableFrom(collection.GetType()) ) {
				foreach( object obj in collection ) {
					if( sb.Length > 0 )
						sb.Append(',');
					sb.Append("'").Append(obj.ToString()).Append("'");
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
		/// 返回CPQuery中生成的SQL语句
		/// </summary>
		/// <returns>SQL语句</returns>
		public override string ToString()
		{
			return _sqlBuidler.ToString();
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
			query._sqlBuidler.Append(query.ParaNameBuilder.GetPlaceholder(p.ParameterName));
			query._command.Parameters.Add(p);
			return query;
		}

		#endregion

	}

	/// <summary>
	/// 表示一个SQL参数对象
	/// </summary>
	public sealed class QueryParameter
	{
		private object _val;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="val">要包装的参数值</param>
		public QueryParameter(object val)
		{
			this._val = val;
		}

		/// <summary>
		/// 参数值
		/// </summary>
		public object Value
		{
			get { return this._val; }
		}


		// 注意：string 不能以隐式方式转QueryParameter，
		// 因为：CPQuery重载了与string的 + 运算符，行为是拼接SQL语句，而非做为参数处理。

		/// <summary>
		/// 将string【显式】转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static explicit operator QueryParameter(string value)
		{
			return new QueryParameter(value);
		}


		#region 各种数据类型到QueryParameter的隐式转换

		/// <summary>
		/// 将DBNull隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(DBNull value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将bool隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(bool value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将char隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(char value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将sbyte隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(sbyte value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将byte隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(byte value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将int隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(int value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将uint隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(uint value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将long隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(long value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将ulong隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(ulong value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将short隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(short value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将ushort隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(ushort value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将float隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(float value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将double隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(double value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将decimal隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(decimal value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将Guid隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(Guid value)
		{
			return new QueryParameter(value);
		}
		
		/// <summary>
		/// 将DateTime隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(DateTime value)
		{
			return new QueryParameter(value);
		}

		/// <summary>
		/// 将byte隐式转换为QueryParameter
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>QueryParameter实例</returns>
		public static implicit operator QueryParameter(byte[] value)
		{
			return new QueryParameter(value);
		}

		#endregion

	}


	/// <summary>
	/// 提供CPQuery扩展方法的工具类
	/// </summary>
	public static class CPQueryExtensions
	{
		/// <summary>
		/// 将指定的字符串（T-SQL的片段）转成CPQuery对象
		/// </summary>
		/// <param name="sql">T-SQL的片段的字符串</param>
		/// <returns>包含T-SQL的片段的CPQuery对象</returns>
		public static CPQuery AsCPQuery(this string sql)
		{
			return CPQuery.Create(sql);
		}




		/// <summary>
		/// 将string转换成QueryParameter对象
		/// </summary>
		/// <param name="value">要转换成QueryParameter的原对象</param>
		/// <returns>转换后的QueryParameter对象</returns>
		public static QueryParameter AsQueryParameter(this string value)
		{
			return new QueryParameter(value);
		}


		/// <summary>
		/// SqlFragment
		/// </summary>
		/// <param name="text">T-SQL的片段的字符串</param>
		/// <returns></returns>
		public static SqlFragment AsSql(this string text)
		{
			return new SqlFragment(text);
		}

	}
}
