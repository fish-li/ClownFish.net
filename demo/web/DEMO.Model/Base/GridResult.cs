using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DEMO.Model
{
	/// <summary>
	///GridResultJson 的摘要说明
	/// </summary>
	public class GridResult<T>
	{
		public int total { get; private set; }

		public IEnumerable<T> rows { get; private set; }

		public GridResult(IEnumerable<T> list)
		{
			this.rows = list;
			this.total = rows.Count();
		}

		public GridResult(IEnumerable<T> list, int recCount)
		{
			this.rows = list;
			this.total = recCount;
		}

	}
}
