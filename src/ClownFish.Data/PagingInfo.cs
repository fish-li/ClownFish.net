using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 基本的分页信息。
	/// </summary>
	public class PagingInfo
	{
		/// <summary>
		/// 分页序号，从0开始计数
		/// </summary>
		public int PageIndex { get; set; }
		/// <summary>
		/// 分页大小
		/// </summary>
		public int PageSize { get; set; }
		/// <summary>
		/// 从相关查询中获取到的符合条件的总记录数
		/// </summary>
		public int TotalRows { get; set; }


		/// <summary>
		/// 计算总页数
		/// </summary>
		/// <returns>总页数</returns>
		public int CalcPageCount()
		{
			if( this.PageSize == 0 || this.TotalRows == 0 )
				return 0;

			return (int)Math.Ceiling((double)this.TotalRows / (double)this.PageSize);
		}
	}
}
