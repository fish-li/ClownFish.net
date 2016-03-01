using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Xml;
using ClownFish.Log;


namespace ClownFish.Log.Model
{
	/// <summary>
	/// 表示SQL执行相关的消息数据结构
	/// </summary>
	public class SqlInfo
	{
		private static int MaxSqlTextLen = 1024 * 1024;

		private static int MaxParameterCount = 64;

		private static int MaxParaValueLen = 128;


		/// <summary>
		/// SQL 文本
		/// </summary>
		public XmlCdata SqlText { get; set; }

		/// <summary>
		/// SQL是否包含在事务中
		/// </summary>
		public bool InTranscation { get; set; }


		/// <summary>
		/// 命令参数列表（可能为 NULL）
		/// </summary>
		public List<NameValue> Parameters { get; set; }


		/// <summary>
		/// 根据DbCommand创建并填充SqlInfo对象
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static SqlInfo Create(DbCommand command)
		{
			if( command == null )
				return null;

			SqlInfo info = new SqlInfo();
			info.SqlText = command.CommandText.SubstringN(MaxSqlTextLen);

			if( command.Parameters == null || command.Parameters.Count == 0 )
				return info;

			info.Parameters = new List<NameValue>(command.Parameters.Count);

			for( int i = 0; i < command.Parameters.Count; i++ ) {
				if( i < MaxParameterCount ) {
					DbParameter parameter = command.Parameters[i];

					NameValue nv = new NameValue();
					nv.Name = parameter.ParameterName;
					if( parameter.Value != null )
						nv.Value = parameter.Value.ToString().SubstringN(MaxParaValueLen);
					else
						nv.Value = "NULL";

	
					info.Parameters.Add(nv);
				}
				else {
					NameValue nv = new NameValue();
					nv.Name = "#####";
					nv.Value = "参数太多，已被截断...，参数数量：" + command.Parameters.Count.ToString();
					info.Parameters.Add(nv);
					break;
				}
			}
			return info;
		}


	}
}
