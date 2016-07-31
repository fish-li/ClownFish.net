using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.UnitTest.Models
{
	[Serializable]
	[DbEntity(Alias = "Products")]
	public class Product : Entity
	{
		[DbColumn(PrimaryKey =true)]
		public virtual int ProductID { get; set; }
		public virtual string ProductName { get; set; }
		public virtual int CategoryID { get; set; }
		public virtual string Unit { get; set; }
		public virtual decimal UnitPrice { get; set; }
		public virtual string Remark { get; set; }
		public virtual int Quantity { get; set; }
	}

}
