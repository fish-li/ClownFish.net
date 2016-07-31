using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;

namespace ClownFish.Data
{
	/// <summary>
	/// ParaNameBuilder的工厂类
	/// </summary>
	public class ParaNameBuilderFactory
	{
		internal static LazyObject<ParaNameBuilderFactory> Factory = new LazyObject<ParaNameBuilderFactory>(true);

		/// <summary>
		/// 根据指定的DbCommand创建匹配的ParaNameBuilder，
		/// 框架直接三种DbCommand类型：SqlCommand, OdbcCommand, OleDbCommand
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public ParaNameBuilder GetBuilder(DbCommand command)
		{
			if( command == null )
				throw new ArgumentNullException("command");

			// SQLSERVER最常用，所以优先判断
			if( command is System.Data.SqlClient.SqlCommand )
				return SqlParaNameBuilder.Instance;


			if( command is System.Data.Odbc.OdbcCommand
				|| command is System.Data.OleDb.OleDbCommand
				)
				return OleDbParaNameBuilder.Instance;

			// 尝试获取框架外部的ParaNameBuilder实现
			ParaNameBuilder builder = TryGetOtherBuilder(command);

			// 默认就返回与SQLSERVER兼容的ParaNameBuilder，因为有不少数据库驱动也是支持的
			return builder ?? SqlParaNameBuilder.Instance;
		}

		/// <summary>
		/// 创建其它数据库类型匹配的ParaNameBuilder，
		/// 例如要支持Oralce，它的参数名要求以冒号开头。
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public virtual ParaNameBuilder TryGetOtherBuilder(DbCommand command)
		{
			return null;

			// 说明：如果要重写这个方法，需要4个步骤：

			// 1、实现一个ParaNameBuilder的继承类型，
			//    可参考（ParaNameBuilder.cs）被注释掉OracleParaNameBuilder

			// 2、创建一个ParaNameBuilderFactory的继承类

			// 3、重写TryGetOtherBuilder方法，返回步骤1的新类型实例，可参考上面的GetBuilder方法

			// 4、在程序初始化时调用 ClownFish.Base.TypeExtend.ExtenderManager.RegisterExtendType方法，
			//    调用参数就是第2步的类型
		}

	}
}
