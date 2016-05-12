using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClownFish;

namespace DEMO.Model
{
	public class ProductSearchInfo : PagingInfo
	{
		public string SearchWord { get; set; }
		public int CategoryId { get; set; }
	}
}
