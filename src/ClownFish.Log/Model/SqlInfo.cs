using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClownFish.Base;
using ClownFish.Base.Xml;
using ClownFish.Log;


namespace ClownFish.Log.Model
{
    /// <summary>
    /// 表示一组用于HTTP传输的 【名称/值】 对。
    /// </summary>
    [Serializable]
    public sealed class DbParam
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        [XmlAttribute]
        public string DbType { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }

        
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", this.Name, this.Value);
        }
    }


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
		public List<DbParam> Parameters { get; set; }


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

			info.Parameters = new List<DbParam>(command.Parameters.Count);

			for( int i = 0; i < command.Parameters.Count; i++ ) {
				if( i < MaxParameterCount ) {
					DbParameter parameter = command.Parameters[i];

                    DbParam pv = new DbParam();
					pv.Name = parameter.ParameterName;
                    pv.DbType = parameter.DbType.ToString();

					if( parameter.Value == null || parameter.Value == DBNull.Value )
                        pv.Value = "NULL";
					else
                        pv.Value = parameter.Value.ToString().SubstringN(MaxParaValueLen);
                    
                    info.Parameters.Add(pv);
				}
				else {
                    DbParam pv = new DbParam();
					pv.Name = "#####";
                    pv.DbType = System.Data.DbType.String.ToString();
                    pv.Value = "参数太多，已被截断...，参数数量：" + command.Parameters.Count.ToString();
					info.Parameters.Add(pv);
					break;
				}
			}
			return info;
		}


	}
}
