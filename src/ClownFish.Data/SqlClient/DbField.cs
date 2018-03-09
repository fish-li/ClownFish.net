using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Data.SqlClient
{
    /// <summary>
    /// 表示一个SQLSERVER数据表字段的描述信息
    /// </summary>
	public class DbField
    {
        /// <summary>
        /// 字段名
        /// </summary>
		public string Name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
		public string DataType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
		public int Length { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public int scale { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public int Precision { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public bool Identity { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public bool Nullable { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public bool Computed { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public bool IsPersisted { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string DefaultValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public string Formular { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public int SeedValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public int IncrementValue { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }




}
