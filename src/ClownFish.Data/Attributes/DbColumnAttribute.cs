using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 定义数据列的描述信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class DbColumnAttribute : Attribute
	{
		/// <summary>
		/// 数据库字段名（相对于C#属性来说就是别名）
		/// </summary>
		public string Alias { get; set; }

		/// <summary>
		/// 是否不从数据库加载
		/// </summary>
		public bool Ignore { get; set; }


        /// <summary>
        /// 是否主键（用于UPDATE操作生成WHERE条件）
        /// </summary>
        public bool PrimaryKey { get; set; }



	}
}
