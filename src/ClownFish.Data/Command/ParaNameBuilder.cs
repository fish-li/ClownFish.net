using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 构造参数名的基类
	/// </summary>
	public abstract class ParaNameBuilder
	{
		/// <summary>
		/// 根据指定的名称返回与数据库类型匹配的命令参数名称
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public abstract string GetParaName(string name);

		/// <summary>
		/// 根据指定的名称返回与数据库类型匹配的SQL语句中的参数名称
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public abstract string GetPlaceholder(string name);
	}

	/// <summary>
	/// ParaNameBuilder的SQLSERVER实现类（也可用于MySQL, Sqlite）
	/// </summary>
	public sealed class SqlParaNameBuilder : ParaNameBuilder
	{
		internal static ParaNameBuilder Instance = new SqlParaNameBuilder();

		/// <summary>
		/// 根据指定的名称返回与数据库类型匹配的命令参数名称
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string GetParaName(string name)
		{
			return "@" + name;
		}

		/// <summary>
		/// 根据指定的名称返回与数据库类型匹配的SQL语句中的参数名称
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string GetPlaceholder(string name)
		{
			return "@" + name;
		}
	}

	/// <summary>
	/// ParaNameBuilder的OleDb实现类
	/// </summary>
	public sealed class OleDbParaNameBuilder : ParaNameBuilder
	{
		internal static ParaNameBuilder Instance = new OleDbParaNameBuilder();

		/// <summary>
		/// 根据指定的名称返回与数据库类型匹配的命令参数名称
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string GetParaName(string name)
		{
			return name;
		}

		/// <summary>
		/// 根据指定的名称返回与数据库类型匹配的SQL语句中的参数名称
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string GetPlaceholder(string name)
		{
			return "?";
		}
	}

	//public sealed class OracleParaNameBuilder : ParaNameBuilder
	//{
	//	public override string GetParaName(string name)
	//	{
	//		return ":" + name;
	//	}

	//	public override string GetPlaceholder(string name)
	//	{
	//		return ":" + name;
	//	}
	//}
}
