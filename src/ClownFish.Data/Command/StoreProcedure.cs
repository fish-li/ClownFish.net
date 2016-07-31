using System;
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
	/// 表示一个存储过程命令
	/// </summary>
	public sealed class StoreProcedure : BaseCommand
	{
		internal StoreProcedure(DbContext context) : base(context)
		{
		}


		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="spName">存储过程名称</param>
		/// <param name="parameters">DbParameter参数数组</param>
		/// <returns>StoreProcedure实例</returns>
		internal void Init(string spName, DbParameter[] parameters)
		{
			if( string.IsNullOrEmpty(spName) )
				throw new ArgumentNullException("spName");

			// 有些存储过程是没有参数的，所以必须允许为 null ，所以不要删除下面被注释的代码。
			//if( parameters == null )
			//    throw new ArgumentNullException("parameters");

			_command.CommandText = spName;
			_command.CommandType = CommandType.StoredProcedure;
			
			if( parameters != null )
				foreach( DbParameter p in parameters )
					_command.Parameters.Add(p);
		}

		internal void Init(string spName, object argsObject)
		{
			DbParameter[] parameters = GetParameters(argsObject);

			this.Init(spName, parameters);
		}

		/// <summary>
		/// 创建StoreProcedure对象的实例
		/// </summary>
		/// <param name="spName">存储过程名称</param>
		/// <returns>StoreProcedure对象实例</returns>
		public static StoreProcedure Create(string spName)
		{
			StoreProcedure sp = new StoreProcedure(ConnectionScope.GetDefaultDbConext());
			sp.Init(spName, null);
			return sp;
		}

		/// <summary>
		/// 创建StoreProcedure对象的实例
		/// </summary>
		/// <param name="spName">存储过程名称</param>
		/// <param name="argsObject">匿名对象参数</param>
		/// <returns>StoreProcedure对象实例</returns>
		public static StoreProcedure Create(string spName, object argsObject)
		{
			StoreProcedure sp = new StoreProcedure(ConnectionScope.GetDefaultDbConext());
			sp.Init(spName, argsObject);
			return sp;
		}

		/// <summary>
		/// 创建StoreProcedure对象的实例
		/// </summary>
		/// <param name="spName">存储过程名称</param>
		/// <param name="parameters">匿名对象</param>
		/// <returns>StoreProcedure对象实例</returns>
		public static StoreProcedure Create(string spName, DbParameter[] parameters)
		{
			StoreProcedure sp = new StoreProcedure(ConnectionScope.GetDefaultDbConext());
			sp.Init(spName, parameters);
			return sp;
		}

	

		private DbParameter[] GetParameters(object parameterObject)
		{
			if( parameterObject == null )
				return null;

			PropertyInfo[] properties = parameterObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			DbParameter[] parameters = new DbParameter[properties.Length];
			int index = 0;

			foreach( PropertyInfo property in properties ) {
				object value = property.FastGetValue(parameterObject);
				DbParameter parameter = null;

				if( value == null || value == DBNull.Value ) {
					parameter = _command.CreateParameter();
					parameter.ParameterName = ParaNameBuilder.GetParaName(property.Name);
					parameter.Value = DBNull.Value;
				}
				if( value is DbParameter ) {
					// 允许包含DbParameter，用于构造输出参数
					parameter = value as DbParameter;

					if( string.IsNullOrEmpty( parameter.ParameterName) )
						parameter.ParameterName = ParaNameBuilder.GetParaName(property.Name);
				}
				else {
					parameter = _command.CreateParameter();
					parameter.ParameterName = ParaNameBuilder.GetParaName(property.Name);
					parameter.Value = value;
				}

				parameters[index] = parameter;
				index++;
			}

			return parameters;
		}



	}
}
