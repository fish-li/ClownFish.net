using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Data.SqlClient
{
    /// <summary>
    /// 表示一个SQLSERVER数据表字段的描述信息
    /// </summary>
	public class Field : Entity
    {
        /// <summary>
        /// 字段名
        /// </summary>
		public virtual string Name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
		public virtual string DataType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
		public virtual int Length { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual int scale { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual int Precision { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual bool Identity { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual bool Nullable { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual bool Computed { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual bool IsPersisted { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual string DefaultValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual string Formular { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual int SeedValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
		public virtual int IncrementValue { get; set; }

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
